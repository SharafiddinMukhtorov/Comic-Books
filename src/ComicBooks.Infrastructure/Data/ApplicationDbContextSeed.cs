using ComicBooks.Domain.Entities;
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
                await context.Genres.AddRangeAsync(new List<Genre>
                {
                    new() { Name = "Action",        Slug = "action" },
                    new() { Name = "Adventure",     Slug = "adventure" },
                    new() { Name = "Comedy",        Slug = "comedy" },
                    new() { Name = "Drama",         Slug = "drama" },
                    new() { Name = "Fantasy",       Slug = "fantasy" },
                    new() { Name = "Horror",        Slug = "horror" },
                    new() { Name = "Mystery",       Slug = "mystery" },
                    new() { Name = "Romance",       Slug = "romance" },
                    new() { Name = "Sci-Fi",        Slug = "sci-fi" },
                    new() { Name = "Supernatural",  Slug = "supernatural" },
                    new() { Name = "Thriller",      Slug = "thriller" },
                    new() { Name = "Martial Arts",  Slug = "martial-arts" },
                    new() { Name = "Isekai",        Slug = "isekai" },
                    new() { Name = "Psychological", Slug = "psychological" },
                    new() { Name = "School Life",   Slug = "school-life" },
                    new() { Name = "Murim",         Slug = "murim" },
                });
                await context.SaveChangesAsync();
            }

            if (!await context.Tags.AnyAsync())
            {
                await context.Tags.AddRangeAsync(new List<Tag>
                {
                    new() { Name = "Strong MC",     Slug = "strong-mc" },
                    new() { Name = "Overpowered",   Slug = "overpowered" },
                    new() { Name = "Reincarnation", Slug = "reincarnation" },
                    new() { Name = "System",        Slug = "system" },
                    new() { Name = "Dungeon",       Slug = "dungeon" },
                    new() { Name = "Regression",    Slug = "regression" },
                    new() { Name = "Cultivation",   Slug = "cultivation" },
                });
                await context.SaveChangesAsync();
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Seeding error");
            throw;
        }
    }

}
