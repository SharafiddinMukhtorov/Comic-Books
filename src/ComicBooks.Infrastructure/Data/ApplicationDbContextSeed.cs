using ComicBooks.Domain.Entities;
using ComicBooks.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComicBooks.Infrastructure.Data;

public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
    {
        try
        {
            await context.Database.MigrateAsync();

            if (!await context.Genres.AnyAsync())
            {
                var genres = new List<Genre>
                {
                    new() { Name = "Action", Slug = "action" },
                    new() { Name = "Adventure", Slug = "adventure" },
                    new() { Name = "Comedy", Slug = "comedy" },
                    new() { Name = "Drama", Slug = "drama" },
                    new() { Name = "Fantasy", Slug = "fantasy" },
                    new() { Name = "Horror", Slug = "horror" },
                    new() { Name = "Mystery", Slug = "mystery" },
                    new() { Name = "Romance", Slug = "romance" },
                    new() { Name = "Sci-Fi", Slug = "sci-fi" },
                    new() { Name = "Slice of Life", Slug = "slice-of-life" },
                    new() { Name = "Supernatural", Slug = "supernatural" },
                    new() { Name = "Thriller", Slug = "thriller" },
                    new() { Name = "Martial Arts", Slug = "martial-arts" },
                    new() { Name = "Isekai", Slug = "isekai" },
                    new() { Name = "Sports", Slug = "sports" }
                };
                await context.Genres.AddRangeAsync(genres);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} genres", genres.Count);
            }

            if (!await context.Tags.AnyAsync())
            {
                var tags = new List<Tag>
                {
                    new() { Name = "Strong MC", Slug = "strong-mc" },
                    new() { Name = "Magic", Slug = "magic" },
                    new() { Name = "Overpowered", Slug = "overpowered" },
                    new() { Name = "Reincarnation", Slug = "reincarnation" },
                    new() { Name = "System", Slug = "system" },
                    new() { Name = "Dungeon", Slug = "dungeon" },
                    new() { Name = "School Life", Slug = "school-life" },
                    new() { Name = "Harem", Slug = "harem" },
                    new() { Name = "Guild", Slug = "guild" },
                    new() { Name = "Demons", Slug = "demons" }
                };
                await context.Tags.AddRangeAsync(tags);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} tags", tags.Count);
            }

            if (!await context.Comics.AnyAsync())
            {
                var actionGenre = await context.Genres.FirstAsync(g => g.Slug == "action");
                var fantasyGenre = await context.Genres.FirstAsync(g => g.Slug == "fantasy");
                var isekaiGenre = await context.Genres.FirstAsync(g => g.Slug == "isekai");

                var comics = new List<Comic>
                {
                    new()
                    {
                        Title = "Solo Leveling",
                        Description = "A story about the world's weakest hunter who becomes the world's strongest.",
                        Status = ComicStatus.Completed,
                        Type = ComicType.Manhwa,
                        Rating = ContentRating.Teen,
                        Author = "Chugong",
                        Artist = "DUBU",
                        ReleaseYear = 2018,
                        AverageRating = 4.9,
                        ViewCount = 1500000,
                        IsFeatured = true,
                        IsPopular = true,
                        Slug = "solo-leveling-abc123",
                        CoverImageUrl = "https://placehold.co/300x420/1a1a2e/e94560?text=Solo+Leveling"
                    },
                    new()
                    {
                        Title = "Tower of God",
                        Description = "A boy enters a mysterious tower in search of his best friend.",
                        Status = ComicStatus.Ongoing,
                        Type = ComicType.Manhwa,
                        Rating = ContentRating.Teen,
                        Author = "SIU",
                        ReleaseYear = 2010,
                        AverageRating = 4.7,
                        ViewCount = 900000,
                        IsFeatured = true,
                        IsPopular = true,
                        Slug = "tower-of-god-def456",
                        CoverImageUrl = "https://placehold.co/300x420/0f3460/16213e?text=Tower+of+God"
                    },
                    new()
                    {
                        Title = "Omniscient Reader",
                        Description = "A reader of a web novel becomes the protagonist of the story he read for years.",
                        Status = ComicStatus.Ongoing,
                        Type = ComicType.Manhwa,
                        Rating = ContentRating.Teen,
                        Author = "sing N song",
                        ReleaseYear = 2020,
                        AverageRating = 4.8,
                        ViewCount = 750000,
                        IsFeatured = false,
                        IsPopular = true,
                        Slug = "omniscient-reader-ghi789",
                        CoverImageUrl = "https://placehold.co/300x420/533483/e8d5b7?text=Omniscient+Reader"
                    }
                };

                await context.Comics.AddRangeAsync(comics);
                await context.SaveChangesAsync();

                // Add genres to comics
                var comicGenres = new List<ComicGenre>
                {
                    new() { ComicId = comics[0].Id, GenreId = actionGenre.Id },
                    new() { ComicId = comics[0].Id, GenreId = fantasyGenre.Id },
                    new() { ComicId = comics[1].Id, GenreId = actionGenre.Id },
                    new() { ComicId = comics[1].Id, GenreId = fantasyGenre.Id },
                    new() { ComicId = comics[2].Id, GenreId = actionGenre.Id },
                    new() { ComicId = comics[2].Id, GenreId = isekaiGenre.Id },
                };
                await context.ComicGenres.AddRangeAsync(comicGenres);

                // Add sample chapters
                var chapters = new List<Chapter>();
                for (int i = 1; i <= 5; i++)
                {
                    chapters.Add(new Chapter
                    {
                        ComicId = comics[0].Id,
                        ChapterNumber = i,
                        Title = $"Chapter {i}",
                        PublishedAt = DateTime.UtcNow.AddDays(-i * 7),
                        Slug = $"solo-leveling-ch-{i}",
                        ViewCount = (6 - i) * 10000
                    });
                }
                await context.Chapters.AddRangeAsync(chapters);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeded {Count} comics with chapters", comics.Count);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}
