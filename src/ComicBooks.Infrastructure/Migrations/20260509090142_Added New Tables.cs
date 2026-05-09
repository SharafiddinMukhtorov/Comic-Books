using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicBooks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1 FROM sys.indexes 
            WHERE name = 'IX_UserBookmarks_SessionId_ComicId' 
            AND object_id = OBJECT_ID('UserBookmarks')
        )
        DROP INDEX [IX_UserBookmarks_SessionId_ComicId] ON [UserBookmarks];
    ");

            migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1 FROM sys.indexes 
            WHERE name = 'IX_ComicViews_SessionId_ComicId_ViewedAt' 
            AND object_id = OBJECT_ID('ComicViews')
        )
        DROP INDEX [IX_ComicViews_SessionId_ComicId_ViewedAt] ON [ComicViews];
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserBookmarks_SessionId_ComicId",
                table: "UserBookmarks",
                columns: new[] { "SessionId", "ComicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComicViews_SessionId_ComicId_ViewedAt",
                table: "ComicViews",
                columns: new[] { "SessionId", "ComicId", "ViewedAt" });
        }
    }
}
