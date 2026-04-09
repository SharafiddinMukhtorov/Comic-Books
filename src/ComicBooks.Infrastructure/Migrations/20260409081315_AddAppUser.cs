using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicBooks.Infrastructure.Migrations
{
    public partial class AddAppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id        = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email     = table.Column<string>(type: "TEXT", nullable: false),
                    Name      = table.Column<string>(type: "TEXT", nullable: false),
                    Picture   = table.Column<string>(type: "TEXT", nullable: true),
                    GoogleId  = table.Column<string>(type: "TEXT", nullable: false),
                    IsAdmin   = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Users", x => x.Id));

            migrationBuilder.CreateIndex(name: "IX_Users_Email",    table: "Users", column: "Email",    unique: true);
            migrationBuilder.CreateIndex(name: "IX_Users_GoogleId", table: "Users", column: "GoogleId", unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
