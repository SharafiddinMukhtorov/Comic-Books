using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace ComicBooks.Infrastructure.Migrations
{
    public partial class CoinSystem : Migration
    {
        protected override void Up(MigrationBuilder m)
        {
            // Add CoinPrice to Chapter
            m.AddColumn<int>("CoinPrice", "Chapters", nullable: false, defaultValue: 0);

            // Add CoinBalance + TelegramUsername to Users (if table exists)
            m.Sql(@"
                CREATE TABLE IF NOT EXISTS Users (
                    Id TEXT NOT NULL PRIMARY KEY,
                    Email TEXT NOT NULL DEFAULT '',
                    Name TEXT NOT NULL DEFAULT '',
                    Picture TEXT,
                    GoogleId TEXT NOT NULL DEFAULT '',
                    IsAdmin INTEGER NOT NULL DEFAULT 0,
                    LastLogin TEXT NOT NULL DEFAULT '',
                    CoinBalance INTEGER NOT NULL DEFAULT 0,
                    TelegramUsername TEXT,
                    CreatedAt TEXT NOT NULL DEFAULT '',
                    UpdatedAt TEXT,
                    IsDeleted INTEGER NOT NULL DEFAULT 0
                );
            ");
            m.Sql("ALTER TABLE Users ADD COLUMN IF NOT EXISTS CoinBalance INTEGER NOT NULL DEFAULT 0;");
            m.Sql("ALTER TABLE Users ADD COLUMN IF NOT EXISTS TelegramUsername TEXT;");

            // CoinTransactions table
            m.CreateTable("CoinTransactions", t => new
            {
                Id          = t.Column<Guid>("TEXT"),
                UserId      = t.Column<Guid>("TEXT"),
                Amount      = t.Column<int>("INTEGER"),
                Type        = t.Column<int>("INTEGER"),
                Description = t.Column<string>("TEXT", nullable: true),
                ChapterId   = t.Column<Guid>("TEXT", nullable: true),
                TelegramUsername = t.Column<string>("TEXT", nullable: true),
                CreatedAt   = t.Column<DateTime>("TEXT"),
                UpdatedAt   = t.Column<DateTime>("TEXT", nullable: true),
                IsDeleted   = t.Column<bool>("INTEGER"),
            }, constraints: t => t.PrimaryKey("PK_CoinTransactions", x => x.Id));

            m.CreateIndex("IX_CoinTransactions_UserId", "CoinTransactions", "UserId");

            // UserChapterAccess table
            m.CreateTable("UserChapterAccesses", t => new
            {
                Id        = t.Column<Guid>("TEXT"),
                UserId    = t.Column<Guid>("TEXT"),
                ChapterId = t.Column<Guid>("TEXT"),
                CoinSpent = t.Column<int>("INTEGER"),
                CreatedAt = t.Column<DateTime>("TEXT"),
                UpdatedAt = t.Column<DateTime>("TEXT", nullable: true),
                IsDeleted = t.Column<bool>("INTEGER"),
            }, constraints: t => t.PrimaryKey("PK_UserChapterAccesses", x => x.Id));

            m.CreateIndex("IX_UserChapterAccesses_UserId_ChapterId", "UserChapterAccesses",
                new[] { "UserId", "ChapterId" }, unique: true);
        }

        protected override void Down(MigrationBuilder m)
        {
            m.DropTable("CoinTransactions");
            m.DropTable("UserChapterAccesses");
            m.DropColumn("CoinPrice", "Chapters");
        }
    }
}
