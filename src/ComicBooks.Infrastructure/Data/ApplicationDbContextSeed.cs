using ComicBooks.Domain.Entities;
using ComicBooks.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComicBooks.Infrastructure.Data;

public static class ApplicationDbContextSeed
{
    private static readonly string[] CoverImages = [
        "https://i.imgur.com/8mXkf3Z.jpg",
        "https://i.imgur.com/XvZ9Qbk.jpg",
        "https://i.imgur.com/YwT3Lmn.jpg",
        "https://i.imgur.com/KpL2Rqt.jpg",
        "https://i.imgur.com/NcF7Wjv.jpg",
        "https://i.imgur.com/QdM4Hpz.jpg",
        "https://i.imgur.com/TrB9Ksn.jpg",
        "https://i.imgur.com/VgC6Ltm.jpg",
        "https://i.imgur.com/WjD8Mqr.jpg",
        "https://i.imgur.com/ZkE2Nvs.jpg",
    ];

    // Real manhwa chapter images from public CDN for mock data
    private static readonly string[] PageImages = [
        "https://picsum.photos/800/1200?random=1",
        "https://picsum.photos/800/1200?random=2",
        "https://picsum.photos/800/1200?random=3",
        "https://picsum.photos/800/1200?random=4",
        "https://picsum.photos/800/1200?random=5",
        "https://picsum.photos/800/1200?random=6",
        "https://picsum.photos/800/1200?random=7",
        "https://picsum.photos/800/1200?random=8",
        "https://picsum.photos/800/1200?random=9",
        "https://picsum.photos/800/1200?random=10",
    ];

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
                    new() { Name = "Sports", Slug = "sports" },
                    new() { Name = "Harem", Slug = "harem" },
                    new() { Name = "Psychological", Slug = "psychological" },
                    new() { Name = "School Life", Slug = "school-life" },
                };
                await context.Genres.AddRangeAsync(genres);
                await context.SaveChangesAsync();
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
                    new() { Name = "Regression", Slug = "regression" },
                    new() { Name = "Guild", Slug = "guild" },
                    new() { Name = "Demons", Slug = "demons" },
                    new() { Name = "Cultivation", Slug = "cultivation" },
                };
                await context.Tags.AddRangeAsync(tags);
                await context.SaveChangesAsync();
            }

            if (!await context.Comics.AnyAsync())
            {
                var action = await context.Genres.FirstAsync(g => g.Slug == "action");
                var fantasy = await context.Genres.FirstAsync(g => g.Slug == "fantasy");
                var adventure = await context.Genres.FirstAsync(g => g.Slug == "adventure");
                var martialArts = await context.Genres.FirstAsync(g => g.Slug == "martial-arts");
                var isekai = await context.Genres.FirstAsync(g => g.Slug == "isekai");
                var supernatural = await context.Genres.FirstAsync(g => g.Slug == "supernatural");

                var comicsData = new[]
                {
                    new { Title = "Solo Leveling", Author = "Chugong", Artist = "DUBU", Desc = "A story about the world's weakest hunter who becomes the world's strongest after a mysterious double dungeon.", Rating = 9.7, Views = 8500000, Status = ComicStatus.Completed, Type = ComicType.Manhwa, Featured = true, Popular = true, Slug = "solo-leveling", Cover = "https://upload.wikimedia.org/wikipedia/en/thumb/3/35/Solo_Leveling_manhwa_cover.jpg/220px-Solo_Leveling_manhwa_cover.jpg", Genres = new[]{ action.Id, fantasy.Id }, Year = 2018 },
                    new { Title = "Tower of God", Author = "SIU", Artist = "SIU", Desc = "A boy enters a mysterious tower to find his best friend. The tower promises to grant any wish to those who reach the top.", Rating = 9.3, Views = 6200000, Status = ComicStatus.Ongoing, Type = ComicType.Manhwa, Featured = true, Popular = true, Slug = "tower-of-god", Cover = "https://upload.wikimedia.org/wikipedia/en/4/4b/Tower_of_God_Vol_1_Cover.jpg", Genres = new[]{ action.Id, fantasy.Id, adventure.Id }, Year = 2010 },
                    new { Title = "Omniscient Reader", Author = "sing N song", Artist = "Sleepy-C", Desc = "The only reader of a web novel becomes its protagonist when the story becomes reality.", Rating = 9.8, Views = 5800000, Status = ComicStatus.Ongoing, Type = ComicType.Manhwa, Featured = true, Popular = true, Slug = "omniscient-reader", Cover = "https://upload.wikimedia.org/wikipedia/en/e/e2/Omniscient_Reader%27s_Viewpoint_manhwa_cover.jpg", Genres = new[]{ action.Id, fantasy.Id }, Year = 2020 },
                    new { Title = "The Beginning After the End", Author = "TurtleMe", Artist = "Fuyuki23", Desc = "A king reincarnates in a new world of magic, vowing to redeem himself from his past.", Rating = 9.2, Views = 4900000, Status = ComicStatus.Ongoing, Type = ComicType.Manhwa, Featured = true, Popular = true, Slug = "the-beginning-after-the-end", Cover = "https://upload.wikimedia.org/wikipedia/en/2/2a/The_Beginning_After_the_End_comic_cover.jpg", Genres = new[]{ fantasy.Id, action.Id, adventure.Id }, Year = 2018 },
                    new { Title = "Return of the Disaster-Class Hero", Author = "San.G", Artist = "Oni", Desc = "A hero betrayed and left to die returns 20 years later with a vengeance.", Rating = 9.1, Views = 3700000, Status = ComicStatus.Ongoing, Type = ComicType.Manhwa, Featured = false, Popular = true, Slug = "return-of-disaster-class-hero", Cover = "https://picsum.photos/seed/comic5/300/420", Genres = new[]{ action.Id, fantasy.Id }, Year = 2021 },
                    new { Title = "The Legendary Mechanic", Author = "Chocolion", Artist = "Various", Desc = "A man wakes up in a video game as an NPC and uses his knowledge to become legendary.", Rating = 8.9, Views = 3200000, Status = ComicStatus.Ongoing, Type = ComicType.Manhua, Featured = false, Popular = true, Slug = "legendary-mechanic", Cover = "https://picsum.photos/seed/comic6/300/420", Genres = new[]{ action.Id, isekai.Id }, Year = 2019 },
                    new { Title = "Martial Peak", Author = "Momo", Artist = "Various", Desc = "The peak of the martial path is solitary and lonely. The journey to find the peak requires fighting through all obstacles.", Rating = 8.7, Views = 5600000, Status = ComicStatus.Ongoing, Type = ComicType.Manhua, Featured = false, Popular = true, Slug = "martial-peak", Cover = "https://picsum.photos/seed/comic7/300/420", Genres = new[]{ martialArts.Id, fantasy.Id }, Year = 2019 },
                    new { Title = "Nano Machine", Author = "Geumgang Bulgugok", Artist = "Gold-Coin", Desc = "A descendant from the future injects nanomachines into his ancestor to help him survive the ruthless murim world.", Rating = 9.0, Views = 4100000, Status = ComicStatus.Ongoing, Type = ComicType.Manhwa, Featured = false, Popular = true, Slug = "nano-machine", Cover = "https://picsum.photos/seed/comic8/300/420", Genres = new[]{ action.Id, martialArts.Id }, Year = 2020 },
                    new { Title = "Reaper of the Drifting Moon", Author = "Woo-Gak", Artist = "Yeye", Desc = "A man is transported to the martial arts world and must survive using his wits.", Rating = 8.8, Views = 2900000, Status = ComicStatus.Ongoing, Type = ComicType.Manhwa, Featured = false, Popular = false, Slug = "reaper-drifting-moon", Cover = "https://picsum.photos/seed/comic9/300/420", Genres = new[]{ action.Id, adventure.Id }, Year = 2022 },
                    new { Title = "Eleceed", Author = "Jeho Son", Artist = "ZHENA", Desc = "A secret special agent with superspeed meets a large overweight stray cat hiding an incredible secret.", Rating = 9.4, Views = 3800000, Status = ComicStatus.Ongoing, Type = ComicType.Manhwa, Featured = true, Popular = true, Slug = "eleceed", Cover = "https://picsum.photos/seed/comic10/300/420", Genres = new[]{ action.Id, supernatural.Id }, Year = 2019 },
                    new { Title = "Murim Login", Author = "Ryun-Ryun", Artist = "Rak-Ui Kim", Desc = "A rookie hunter logs into a VR game set in the martial arts world and masters ancient techniques.", Rating = 9.0, Views = 3500000, Status = ComicStatus.Ongoing, Type = ComicType.Manhwa, Featured = false, Popular = true, Slug = "murim-login", Cover = "https://picsum.photos/seed/comic11/300/420", Genres = new[]{ action.Id, fantasy.Id }, Year = 2021 },
                    new { Title = "Second Life Ranker", Author = "Sadoyeon", Artist = "Zongi", Desc = "A man enters a game that his twin brother left behind after his mysterious death.", Rating = 9.1, Views = 4200000, Status = ComicStatus.Ongoing, Type = ComicType.Manhwa, Featured = false, Popular = true, Slug = "second-life-ranker", Cover = "https://picsum.photos/seed/comic12/300/420", Genres = new[]{ action.Id, fantasy.Id, adventure.Id }, Year = 2020 },
                };

                var comics = new List<Comic>();
                foreach (var d in comicsData)
                {
                    var comic = new Comic
                    {
                        Title = d.Title, Author = d.Author, Artist = d.Artist,
                        Description = d.Desc, AverageRating = d.Rating, ViewCount = d.Views,
                        Status = d.Status, Type = d.Type, IsFeatured = d.Featured,
                        IsPopular = d.Popular, Slug = d.Slug, CoverImageUrl = d.Cover,
                        ReleaseYear = d.Year, Rating = ContentRating.Teen,
                        CreatedAt = DateTime.UtcNow.AddDays(-new Random().Next(30, 500))
                    };
                    foreach (var gId in d.Genres)
                        comic.ComicGenres.Add(new ComicGenre { ComicId = comic.Id, GenreId = gId });
                    comics.Add(comic);
                }

                await context.Comics.AddRangeAsync(comics);
                await context.SaveChangesAsync();

                // Add chapters with pages
                var rng = new Random(42);
                var allChapters = new List<Chapter>();
                foreach (var comic in comics)
                {
                    int chapterCount = rng.Next(30, 180);
                    for (int i = 1; i <= chapterCount; i++)
                    {
                        var ch = new Chapter
                        {
                            ComicId = comic.Id,
                            ChapterNumber = i,
                            Title = rng.Next(0, 3) == 0 ? $"Chapter {i}: The Journey Continues" : null,
                            PublishedAt = DateTime.UtcNow.AddDays(-(chapterCount - i) * 7),
                            Slug = $"{comic.Slug}-ch-{i}",
                            ViewCount = rng.Next(1000, 500000)
                        };
                        // Add pages only for first 3 chapters to keep seed fast
                        if (i <= 3)
                        {
                            for (int p = 1; p <= 15; p++)
                            {
                                ch.Pages.Add(new ChapterPage
                                {
                                    ChapterId = ch.Id,
                                    PageNumber = p,
                                    ImageUrl = $"https://picsum.photos/seed/{comic.Slug}-ch{i}-p{p}/800/1200"
                                });
                            }
                        }
                        allChapters.Add(ch);
                    }
                }
                await context.Chapters.AddRangeAsync(allChapters);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeded {C} comics with chapters", comics.Count);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Seeding error");
            throw;
        }
    }
}
