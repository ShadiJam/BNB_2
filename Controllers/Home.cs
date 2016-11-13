using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

public class LoginVM 
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [DataTypeAttribute(DataType.Password)]
    public string Password { get; set; }
}


[Route("/")]
[Authorize]
public class HomeController : Controller
{
    private DB db;
    private IRepository<BNB> bnbs;
    private IRepository<Message> messages;
    private IRepository<Visitor> visitors;
    private IAuthService auth;
   
    public HomeController(DB db, IRepository<BNB> bnbs, IRepository<Message> messages, IRepository<Visitor> visitors, IAuthService auth){
        this.db = db;
        this.bnbs = bnbs;
        this.messages = messages;
        this.visitors = visitors;
        this.auth = auth;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Root()
    {
      return View("Index", db.BNBs.ToList());
    }

    [HttpGet("bnb/new")]
    public IActionResult CreateBNB() => View("CreateBNB");

    [HttpPost("bnb/new")]
    [ValidateAntiForgeryToken]
    public IActionResult CreateBNB([FromForm] BNB bnb)
    {
        if(!ModelState.IsValid)
            return View("CreateBNB", bnb);

        db.BNBs.Add(bnb);
        db.SaveChanges();
        return Redirect("/");
    }

    [HttpGet("bnb/{id}")]
    public async Task<IActionResult> BNB(int id)
    {
        BNB item = bnbs.Read(id);
        if(item == null) return NotFound();
        return View("BNB", item);
    }

    [HttpPost("bnb/{id}/messages")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMessage([FromForm] Message m, int id)
    {
        m.BNB = null;
        string name = (await auth.GetUser(HttpContext))?.Email ?? "NOT PROVIDED";
        m.Visitor = new Visitor { Name = name };

        TryValidateModel(m);

        if(ModelState.IsValid){
            db.Messages.Add(m);
            db.SaveChanges();
        }
        return Redirect($"/bnb.{id}");
    }

    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login() => View("Login");

    [HttpPost("login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromForm] LoginVM user)
    {
        string result = await auth.Login(user.Email, user.Password);
        if(result == null) {
            return Redirect("/");
        }
        ModelState.AddModelError("", result);
        return View("Login", user);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromForm] LoginVM user)
    {
        var errors = await auth.Register(user.Email, user.Password);
        if((errors ?? new List<string>()).Count() == 0) {
            return Redirect("/");
        } else {
            foreach(var e in errors)
                ModelState.AddModelError("", e);

            return View("Login", user);
        }
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await auth.Logout();
        return Redirect("/");
    }
}


    // [HttpGet("sql/cards")] // ?sql=....
    // public IActionResult SqlCards([FromQuery]string sql) => Ok(cards.FromSql(sql));

    // [HttpGet("sql/lists")] // ?sql=....
    // public IActionResult SqlLists([FromQuery]string sql) => Ok(lists.FromSql(sql));

    // [HttpGet("sql/boards")] // ?sql=....
    // public IActionResult SqlBoards([FromQuery]string sql) => Ok(boards.FromSql(sql));
}