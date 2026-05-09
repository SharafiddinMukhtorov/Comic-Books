using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicBooks.Infrastructure.Migrations
{
    public partial class AddBookmarksAndViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBookmarks",
                columns: table => new
                {
                    Id         = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId  = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComicId    = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt  = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt  = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted  = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBookmarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBookmarks_Comics_ComicId",
                        column: x => x.ComicId,
                        principalTable: "Comics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicViews",
                columns: table => new
                {
                    Id         = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId  = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComicId    = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedAt   = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt  = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt  = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted  = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicViews", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBookmarks_SessionId_ComicId",
                table: "UserBookmarks",
                columns: new[] { "SessionId", "ComicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBookmarks_ComicId",
                table: "UserBookmarks",
                column: "ComicId");

            migrationBuilder.CreateIndex(
                name: "IX_ComicViews_SessionId_ComicId_ViewedAt",
                table: "ComicViews",
                columns: new[] { "SessionId", "ComicId", "ViewedAt" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserBookmarks");
            migrationBuilder.DropTable(name: "ComicViews");
        }
    }
}
