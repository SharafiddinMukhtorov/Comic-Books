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

            if (!await context.Videos.AnyAsync())
            {
                var genreMap = await context.Genres.ToDictionaryAsync(g => g.Name, g => g.Id);
                Guid GenreId(string name) => genreMap.TryGetValue(name, out var id) ? id : genreMap.Values.First();

                // Video 1 — Film (bog'lanmagan)
                var movie = new Video
                {
                    Title = "Katta Quyon",
                    OriginalTitle = "Big Buck Bunny",
                    Description = "Katta va mehribon quyon o'rmonda o'ziga zulm qiluvchi uch bezorini saboq berish uchun reja tuzadi.",
                    PosterImageUrl = "https://picsum.photos/seed/bigbuckbunny/500/700",
                    BannerImageUrl = "https://picsum.photos/seed/bigbuckbunny-banner/1600/700",
                    Type = VideoType.Movie,
                    Status = VideoStatus.Completed,
                    Rating = ContentRating.Everyone,
                    Language = "O'zbek tilida",
                    Country = "AQSH",
                    ReleaseYear = 2008,
                    DurationMinutes = 10,
                    ImdbRating = 7.2,
                    VideoUrl = "https://interactive-examples.mdn.mozilla.net/media/cc0-videos/flower.mp4",
                    IsFeatured = true,
                    Slug = "katta-quyon-" + Guid.NewGuid().ToString("N")[..6]
                };
                movie.VideoGenres.Add(new VideoGenre { VideoId = movie.Id, GenreId = GenreId("Comedy") });
                movie.VideoGenres.Add(new VideoGenre { VideoId = movie.Id, GenreId = GenreId("Adventure") });
                movie.CastMembers.Add(new VideoCastMember { VideoId = movie.Id, Name = "Sacha Goedegebure", Role = CastRole.Director, SortOrder = 0, PhotoUrl = "https://picsum.photos/seed/sacha/200/200" });
                movie.CastMembers.Add(new VideoCastMember { VideoId = movie.Id, Name = "Big Buck Bunny", Role = CastRole.Actor, SortOrder = 1, PhotoUrl = "https://picsum.photos/seed/bunny/200/200" });

                // Video 2 — Serial (2 fasl x 3 qism)
                var series = new Video
                {
                    Title = "Sarguzasht Maktabi",
                    Description = "Yosh sarguzashtchilar guruhi sirli maktabda kutilmagan voqealarga duch keladi.",
                    PosterImageUrl = "https://picsum.photos/seed/sarguzashtmaktabi/500/700",
                    BannerImageUrl = "https://picsum.photos/seed/sarguzashtmaktabi-banner/1600/700",
                    Type = VideoType.Series,
                    Status = VideoStatus.Ongoing,
                    Rating = ContentRating.Teen,
                    Language = "O'zbek tilida",
                    Country = "Yaponiya",
                    ReleaseYear = 2023,
                    ImdbRating = 8.1,
                    IsFeatured = true,
                    Slug = "sarguzasht-maktabi-" + Guid.NewGuid().ToString("N")[..6]
                };
                series.VideoGenres.Add(new VideoGenre { VideoId = series.Id, GenreId = GenreId("Fantasy") });
                series.VideoGenres.Add(new VideoGenre { VideoId = series.Id, GenreId = GenreId("School Life") });
                series.CastMembers.Add(new VideoCastMember { VideoId = series.Id, Name = "Aziz Karimov", Role = CastRole.Director, SortOrder = 0, PhotoUrl = "https://picsum.photos/seed/aziz/200/200" });
                series.CastMembers.Add(new VideoCastMember { VideoId = series.Id, Name = "Malika Yusupova", Role = CastRole.Actor, SortOrder = 1, PhotoUrl = "https://picsum.photos/seed/malika/200/200" });

                var sampleClips = new[]
                {
                    "https://interactive-examples.mdn.mozilla.net/media/cc0-videos/flower.mp4",
                    "https://interactive-examples.mdn.mozilla.net/media/cc0-videos/flower.mp4",
                    "https://interactive-examples.mdn.mozilla.net/media/cc0-videos/flower.mp4"
                };
                for (int season = 1; season <= 2; season++)
                {
                    for (int ep = 1; ep <= 3; ep++)
                    {
                        series.Episodes.Add(new VideoEpisode
                        {
                            VideoId = series.Id,
                            SeasonNumber = season,
                            EpisodeNumber = ep,
                            Title = $"{season}-fasl, {ep}-qism",
                            VideoUrl = sampleClips[(ep - 1) % sampleClips.Length],
                            ThumbnailUrl = $"https://picsum.photos/seed/sarguzashtmaktabi-s{season}e{ep}/300/450",
                            PublishedAt = DateTime.UtcNow
                        });
                    }
                }

                // Video 3 — mavjud komikka bog'langan (agar bazada komik bo'lsa)
                var linkedComic = await context.Comics.OrderByDescending(c => c.ViewCount).FirstOrDefaultAsync();
                if (linkedComic is null)
                {
                    logger.LogInformation("Video seed: bazada hali komik yo'q, 3-video hech qanday kitobga bog'lanmadi.");
                }

                var adaptation = new Video
                {
                    Title = linkedComic is not null ? $"{linkedComic.Title}: Film" : "Sirli Sayohat",
                    Description = "Mashhur asar asosida suratga olingan film.",
                    PosterImageUrl = linkedComic?.CoverImageUrl ?? "https://picsum.photos/seed/sirlisayohat/500/700",
                    BannerImageUrl = linkedComic?.BannerImageUrl ?? "https://picsum.photos/seed/sirlisayohat-banner/1600/700",
                    Type = VideoType.Movie,
                    Status = VideoStatus.Completed,
                    Rating = ContentRating.Teen,
                    Language = "O'zbek tilida",
                    Country = "Janubiy Koreya",
                    ReleaseYear = 2024,
                    DurationMinutes = 118,
                    ImdbRating = 6.9,
                    VideoUrl = "https://interactive-examples.mdn.mozilla.net/media/cc0-videos/flower.mp4",
                    IsFeatured = false,
                    Slug = "ekranizatsiya-" + Guid.NewGuid().ToString("N")[..6],
                    LinkedComicId = linkedComic?.Id
                };
                adaptation.VideoGenres.Add(new VideoGenre { VideoId = adaptation.Id, GenreId = GenreId("Drama") });
                adaptation.VideoGenres.Add(new VideoGenre { VideoId = adaptation.Id, GenreId = GenreId("Fantasy") });
                adaptation.CastMembers.Add(new VideoCastMember { VideoId = adaptation.Id, Name = "Jasur Rahimov", Role = CastRole.Director, SortOrder = 0, PhotoUrl = "https://picsum.photos/seed/jasur/200/200" });
                adaptation.CastMembers.Add(new VideoCastMember { VideoId = adaptation.Id, Name = "Nodira Aliyeva", Role = CastRole.Actor, SortOrder = 1, PhotoUrl = "https://picsum.photos/seed/nodira/200/200" });

                await context.Videos.AddRangeAsync(new List<Video> { movie, series, adaptation });
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
