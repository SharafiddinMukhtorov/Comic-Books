using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicBooks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoQualityUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoUrl1080p",
                table: "Videos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl480p",
                table: "Videos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl720p",
                table: "Videos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl1080p",
                table: "VideoEpisodes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl480p",
                table: "VideoEpisodes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl720p",
                table: "VideoEpisodes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl1080p",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "VideoUrl480p",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "VideoUrl720p",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "VideoUrl1080p",
                table: "VideoEpisodes");

            migrationBuilder.DropColumn(
                name: "VideoUrl480p",
                table: "VideoEpisodes");

            migrationBuilder.DropColumn(
                name: "VideoUrl720p",
                table: "VideoEpisodes");
        }
    }
}
