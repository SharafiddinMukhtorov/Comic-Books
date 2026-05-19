using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicBooks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChapterAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChapterId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CoinSpent = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterAccesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoinPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CoinAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    BonusCoins = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsPopular = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoinPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoinTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ChapterId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TelegramUsername = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoinTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    AlternativeTitles = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    CoverImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    BannerImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    Author = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Artist = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ReleaseYear = table.Column<int>(type: "INTEGER", nullable: true),
                    AverageRating = table.Column<double>(type: "REAL", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    BookmarkCount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFeatured = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPopular = table.Column<bool>(type: "INTEGER", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComicViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComicId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicViews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Picture = table.Column<string>(type: "TEXT", nullable: true),
                    GoogleId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CoinBalance = table.Column<int>(type: "INTEGER", nullable: false),
                    TelegramUsername = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComicId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChapterNumber = table.Column<double>(type: "REAL", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsLocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    CoinPrice = table.Column<int>(type: "INTEGER", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chapters_Comics_ComicId",
                        column: x => x.ComicId,
                        principalTable: "Comics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBookmarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComicId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
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
                name: "ComicGenres",
                columns: table => new
                {
                    ComicId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GenreId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicGenres", x => new { x.ComicId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_ComicGenres_Comics_ComicId",
                        column: x => x.ComicId,
                        principalTable: "Comics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComicTags",
                columns: table => new
                {
                    ComicId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicTags", x => new { x.ComicId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ComicTags_Comics_ComicId",
                        column: x => x.ComicId,
                        principalTable: "Comics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComicTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChapterPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChapterId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PageNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Width = table.Column<int>(type: "INTEGER", nullable: true),
                    Height = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterPages_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterAccesses_UserId_ChapterId",
                table: "ChapterAccesses",
                columns: new[] { "UserId", "ChapterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChapterPages_ChapterId_PageNumber",
                table: "ChapterPages",
                columns: new[] { "ChapterId", "PageNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_ComicId_ChapterNumber",
                table: "Chapters",
                columns: new[] { "ComicId", "ChapterNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComicGenres_GenreId",
                table: "ComicGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Comics_Slug",
                table: "Comics",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comics_Title",
                table: "Comics",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ComicTags_TagId",
                table: "ComicTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Slug",
                table: "Genres",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Slug",
                table: "Tags",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserBookmarks_ComicId",
                table: "UserBookmarks",
                column: "ComicId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChapterAccesses");

            migrationBuilder.DropTable(
                name: "ChapterPages");

            migrationBuilder.DropTable(
                name: "CoinPackages");

            migrationBuilder.DropTable(
                name: "CoinTransactions");

            migrationBuilder.DropTable(
                name: "ComicGenres");

            migrationBuilder.DropTable(
                name: "ComicTags");

            migrationBuilder.DropTable(
                name: "ComicViews");

            migrationBuilder.DropTable(
                name: "UserBookmarks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Comics");
        }
    }
}
