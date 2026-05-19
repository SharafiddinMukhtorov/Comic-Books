using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicBooks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentComicId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ChapterId",
                table: "ChapterComments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<Guid>(
                name: "ComicId",
                table: "ChapterComments",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComicId",
                table: "ChapterComments");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChapterId",
                table: "ChapterComments",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
