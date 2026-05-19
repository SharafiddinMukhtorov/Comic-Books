using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicBooks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChapterComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PasswordHash va UploaderId allaqachon qo'shilgan (oldingi qo'lda).
            // Bu yerda ularni qaytadan qo'shmaymiz.

            migrationBuilder.CreateTable(
                name: "ChapterComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChapterId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LikeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterComments_UserId",
                table: "ChapterComments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChapterComments");
        }
    }
}
