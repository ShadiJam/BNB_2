using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BNB_2.Migrations
{
    public partial class visitorclasstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "VisitorId",
                table: "Messages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_VisitorId",
                table: "Messages",
                column: "VisitorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Visitors_VisitorId",
                table: "Messages",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Visitors_VisitorId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_VisitorId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Visitors");
        }
    }
}
