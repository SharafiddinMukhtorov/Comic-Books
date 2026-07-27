using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicBooks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddComicAdminPick : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdminPick",
                table: "Comics",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdminPick",
                table: "Comics");
        }
    }
}
