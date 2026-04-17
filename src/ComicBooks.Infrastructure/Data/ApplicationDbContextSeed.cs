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
                };
                await context.Genres.AddRangeAsync(genres);
                await context.SaveChangesAsync();
            }

            if (!await context.Tags.AnyAsync())
            {
                var tags = new List<Tag>
                {
                    new() { Name = "Strong MC",     Slug = "strong-mc" },
                    new() { Name = "Overpowered",   Slug = "overpowered" },
                    new() { Name = "Reincarnation", Slug = "reincarnation" },
                    new() { Name = "System",        Slug = "system" },
                    new() { Name = "Dungeon",       Slug = "dungeon" },
                    new() { Name = "Regression",    Slug = "regression" },
                    new() { Name = "Cultivation",   Slug = "cultivation" },
                };
                await context.Tags.AddRangeAsync(tags);
                await context.SaveChangesAsync();
            }

            if (!await context.Comics.AnyAsync())
            {
                var action       = await context.Genres.FirstAsync(g => g.Slug == "action");
                var fantasy      = await context.Genres.FirstAsync(g => g.Slug == "fantasy");
                var adventure    = await context.Genres.FirstAsync(g => g.Slug == "adventure");
                var martialArts  = await context.Genres.FirstAsync(g => g.Slug == "martial-arts");
                var isekai       = await context.Genres.FirstAsync(g => g.Slug == "isekai");
                var supernatural = await context.Genres.FirstAsync(g => g.Slug == "supernatural");
                var murim        = await context.Genres.FirstAsync(g => g.Slug == "murim");
                var psychological= await context.Genres.FirstAsync(g => g.Slug == "psychological");
                var romance      = await context.Genres.FirstAsync(g => g.Slug == "romance");

                // Real manhwa/anime cover images from MyAnimeList CDN & other public sources
                var comicsData = new[]
                {
                    new {
                        Title   = "Solo Leveling",
                        Author  = "Chugong", Artist = "DUBU",
                        Desc    = "Sung Jinwoo, the world's weakest hunter, discovers a mysterious quest log after nearly dying in a double dungeon. He alone can see these quests — and through them, he grows without limit into the world's most powerful hunter.",
                        Rating  = 9.7, Views = 8500000,
                        Status  = ComicStatus.Completed, Type = ComicType.Manhwa,
                        Featured= true, Popular = true, Slug = "solo-leveling", Year = 2018,
                        Cover   = "https://cdn.myanimelist.net/images/manga/3/222295l.jpg",
                        Genres  = new[]{ action.Id, fantasy.Id }
                    },
                    new {
                        Title   = "Omniscient Reader's Viewpoint",
                        Author  = "sing N song", Artist = "Sleepy-C",
                        Desc    = "The only reader of a web novel witnesses its story become reality. Armed with complete foreknowledge, Kim Dokja must survive a world transformed into a ruthless game of scenarios.",
                        Rating  = 9.8, Views = 5800000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= true, Popular = true, Slug = "omniscient-reader", Year = 2020,
                        Cover   = "https://cdn.myanimelist.net/images/manga/2/234183l.jpg",
                        Genres  = new[]{ action.Id, fantasy.Id, psychological.Id }
                    },
                    new {
                        Title   = "Tower of God",
                        Author  = "SIU", Artist = "SIU",
                        Desc    = "Twenty-Fifth Bam has lived his whole life beneath a tower. When his best friend Rachel enters it, he follows — and discovers the tower tests everything: power, ambition, and heart.",
                        Rating  = 9.3, Views = 6200000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= true, Popular = true, Slug = "tower-of-god", Year = 2010,
                        Cover   = "https://cdn.myanimelist.net/images/manga/2/275576l.jpg",
                        Genres  = new[]{ action.Id, fantasy.Id, adventure.Id }
                    },
                    new {
                        Title   = "The Beginning After the End",
                        Author  = "TurtleMe", Artist = "Fuyuki23",
                        Desc    = "King Grey reincarnates as Arthur Leywin in a world of magic and monsters. Vowing to cherish what he lost, he rises again — but dark forces from his past life follow him across lifetimes.",
                        Rating  = 9.2, Views = 4900000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= true, Popular = true, Slug = "beginning-after-end", Year = 2018,
                        Cover   = "https://cdn.myanimelist.net/images/manga/1/267585l.jpg",
                        Genres  = new[]{ fantasy.Id, action.Id, adventure.Id }
                    },
                    new {
                        Title   = "Eleceed",
                        Author  = "Jeho Son", Artist = "ZHENA",
                        Desc    = "Jiwoo Seo, a kind-hearted boy with superspeed, takes in a mysterious fat cat — who turns out to be a top-tier awakened warrior in hiding. Together they navigate a covert world of superhuman battles.",
                        Rating  = 9.4, Views = 3800000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= true, Popular = true, Slug = "eleceed", Year = 2019,
                        Cover   = "https://cdn.myanimelist.net/images/manga/1/212713l.jpg",
                        Genres  = new[]{ action.Id, supernatural.Id }
                    },
                    new {
                        Title   = "Eternally Regressing Knight",
                        Author  = "Ro Ya", Artist = "Various",
                        Desc    = "A knight is cursed to reset and repeat each day without anyone remembering. He accumulates experience and power invisible to everyone — and slowly unravels the curse alone.",
                        Rating  = 9.9, Views = 4600000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= true, Popular = true, Slug = "eternally-regressing-knight", Year = 2023,
                        Cover   = "https://cdn.myanimelist.net/images/manga/2/291440l.jpg",
                        Genres  = new[]{ action.Id, fantasy.Id }
                    },
                    new {
                        Title   = "Childhood Friend of the Zenith",
                        Author  = "Chaeyun", Artist = "Zoontar",
                        Desc    = "A regression manhwa where a man returns to his childhood armed with the knowledge of a lifetime. This time he will protect the people he failed and change his tragic fate.",
                        Rating  = 9.2, Views = 3400000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= true, Popular = true, Slug = "childhood-friend-zenith", Year = 2022,
                        Cover   = "https://cdn.myanimelist.net/images/manga/1/278572l.jpg",
                        Genres  = new[]{ action.Id, murim.Id, fantasy.Id }
                    },
                    new {
                        Title   = "Swordmaster's Youngest Son",
                        Author  = "Sung San-Young", Artist = "Hyun-Jin",
                        Desc    = "Jin Runcandel, the weakest son of the legendary Runcandel family, is executed in disgrace — and reborn with his memories intact. This time, he will become the greatest swordmaster.",
                        Rating  = 9.0, Views = 3200000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= true, Popular = true, Slug = "swordmasters-youngest-son", Year = 2021,
                        Cover   = "https://cdn.myanimelist.net/images/manga/1/268082l.jpg",
                        Genres  = new[]{ action.Id, fantasy.Id }
                    },
                    new {
                        Title   = "Infinite Mage",
                        Author  = "Juugin", Artist = "Lek",
                        Desc    = "Shirone, a commoner who loves magic, is discovered and given a chance to attend a mage academy. His unique insight into the infinite challenges every magical convention known to man.",
                        Rating  = 9.1, Views = 2500000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= true, Popular = true, Slug = "infinite-mage", Year = 2021,
                        Cover   = "https://cdn.myanimelist.net/images/manga/3/268393l.jpg",
                        Genres  = new[]{ fantasy.Id, action.Id, adventure.Id }
                    },
                    new {
                        Title   = "Standard of Reincarnation",
                        Author  = "Ip Gak", Artist = "Various",
                        Desc    = "A genius swordsman reincarnates as the weakest member of the Adenka family. Using memories of a past life, he rewrites his destiny and rises to claim the glory he was denied.",
                        Rating  = 9.1, Views = 2700000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= true, Popular = true, Slug = "standard-reincarnation", Year = 2023,
                        Cover   = "https://cdn.myanimelist.net/images/manga/1/289350l.jpg",
                        Genres  = new[]{ action.Id, fantasy.Id, isekai.Id }
                    },
                    new {
                        Title   = "Wandering Warrior of Wudang",
                        Author  = "Kim Gyu-Sam", Artist = "Park Chan",
                        Desc    = "A boy raised in the sacred Wudang sect descends into the chaos of the martial world. His journey tests his training, his will, and his understanding of what it truly means to be a warrior.",
                        Rating  = 9.3, Views = 3100000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= true, Popular = true, Slug = "wandering-warrior-wudang", Year = 2022,
                        Cover   = "https://cdn.myanimelist.net/images/manga/1/295390l.jpg",
                        Genres  = new[]{ martialArts.Id, murim.Id, action.Id }
                    },
                    new {
                        Title   = "Reborn as a Heavenly Demon",
                        Author  = "Various", Artist = "Various",
                        Desc    = "A man reborn into the body of a demonic cult's young master must conceal his true identity while navigating treacherous murim politics and climbing to the top of the martial world.",
                        Rating  = 9.0, Views = 2800000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= true, Popular = false, Slug = "reborn-heavenly-demon", Year = 2022,
                        Cover   = "https://cdn.myanimelist.net/images/manga/2/274680l.jpg",
                        Genres  = new[]{ action.Id, murim.Id }
                    },
                    // ── TRENDING / POPULAR ───────────────────────────────────────────
                    new {
                        Title   = "Revenge of the Iron-Blooded Sword Hound",
                        Author  = "Sajoin", Artist = "Suk",
                        Desc    = "Vikir, a loyal hunting dog of the empire, is betrayed and executed. Reborn as a child, he will exact cold-blooded revenge against the eight houses that destroyed him.",
                        Rating  = 9.4, Views = 4100000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= false, Popular = true, Slug = "iron-blooded-sword-hound", Year = 2022,
                        Cover   = "https://cdn.myanimelist.net/images/manga/3/277553l.jpg",
                        Genres  = new[]{ action.Id, fantasy.Id }
                    },
                    new {
                        Title   = "Chronicles of the Demon Faction",
                        Author  = "Ugak", Artist = "Various",
                        Desc    = "A demon faction elder reincarnates into the righteous sect. With knowledge of both worlds he must carefully walk the line between light and darkness to reshape the entire martial world.",
                        Rating  = 9.2, Views = 3600000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= false, Popular = true, Slug = "chronicles-demon-faction", Year = 2022,
                        Cover   = "https://cdn.myanimelist.net/images/manga/1/285977l.jpg",
                        Genres  = new[]{ action.Id, murim.Id, fantasy.Id }
                    },
                    new {
                        Title   = "The Novel's Extra",
                        Author  = "Jee Gab Song", Artist = "Various",
                        Desc    = "A writer wakes up inside his own unfinished novel as a background extra. He must survive using his authorial knowledge while the plot — and its dangers — unfold around him.",
                        Rating  = 9.0, Views = 2900000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= false, Popular = true, Slug = "the-novels-extra", Year = 2021,
                        Cover   = "https://cdn.myanimelist.net/images/manga/3/264501l.jpg",
                        Genres  = new[]{ action.Id, fantasy.Id, isekai.Id }
                    },
                    new {
                        Title   = "Return of the Disaster-Class Hero",
                        Author  = "San.G", Artist = "Oni",
                        Desc    = "A hero betrayed and left to die in the monster realm returns 20 years later with overwhelming power and an unquenchable hunger for vengeance against those who condemned him.",
                        Rating  = 9.1, Views = 3700000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= false, Popular = true, Slug = "return-disaster-class-hero", Year = 2021,
                        Cover   = "https://cdn.myanimelist.net/images/manga/3/268474l.jpg",
                        Genres  = new[]{ action.Id, fantasy.Id }
                    },
                    new {
                        Title   = "Nano Machine",
                        Author  = "Geumgang Bulgugok", Artist = "Gold-Coin",
                        Desc    = "A descendant from the future injects nanomachines into his ancestor Cheon Yeo-Woon to help him survive the deadly power struggles of the murim world and rise to become its absolute master.",
                        Rating  = 9.0, Views = 4100000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= false, Popular = true, Slug = "nano-machine", Year = 2020,
                        Cover   = "https://cdn.myanimelist.net/images/manga/3/246050l.jpg",
                        Genres  = new[]{ action.Id, martialArts.Id, murim.Id }
                    },
                    new {
                        Title   = "Second Life Ranker",
                        Author  = "Sadoyeon", Artist = "Zongi",
                        Desc    = "After his twin brother is betrayed and killed inside a brutal tower, Yeon-woo uses the diary left behind to conquer it himself — and exact terrible vengeance on those responsible.",
                        Rating  = 9.1, Views = 4200000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= false, Popular = true, Slug = "second-life-ranker", Year = 2020,
                        Cover   = "https://cdn.myanimelist.net/images/manga/1/223032l.jpg",
                        Genres  = new[]{ action.Id, fantasy.Id, adventure.Id }
                    },
                    new {
                        Title   = "The Great Mage Returns After 4000 Years",
                        Author  = "Kingofez", Artist = "Various",
                        Desc    = "The greatest mage of all time is reincarnated after 4000 years of imprisonment as the weakest student at a magic academy. His return will reshape the balance of all power in the world.",
                        Rating  = 9.0, Views = 3100000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhwa,
                        Featured= false, Popular = true, Slug = "great-mage-returns", Year = 2020,
                        Cover   = "https://cdn.myanimelist.net/images/manga/1/247082l.jpg",
                        Genres  = new[]{ action.Id, fantasy.Id }
                    },
                    new {
                        Title   = "Martial Peak",
                        Author  = "Momo", Artist = "Various",
                        Desc    = "Yang Kai discovers a black book and sets off on the solitary and arduous path of martial cultivation. The journey to the peak of the martial path tests everything he has — and everything he is.",
                        Rating  = 8.7, Views = 5600000,
                        Status  = ComicStatus.Ongoing, Type = ComicType.Manhua,
                        Featured= false, Popular = true, Slug = "martial-peak", Year = 2019,
                        Cover   = "https://cdn.myanimelist.net/images/manga/2/201819l.jpg",
                        Genres  = new[]{ martialArts.Id, fantasy.Id }
                    },
                };

                var rng    = new Random(42);
                var comics = new List<Comic>();

                foreach (var d in comicsData)
                {
                    var comic = new Comic
                    {
                        Title          = d.Title,
                        Author         = d.Author,
                        Artist         = d.Artist,
                        Description    = d.Desc,
                        AverageRating  = d.Rating,
                        ViewCount      = d.Views,
                        Status         = d.Status,
                        Type           = d.Type,
                        IsFeatured     = d.Featured,
                        IsPopular      = d.Popular,
                        Slug           = d.Slug,
                        CoverImageUrl  = d.Cover,
                        BannerImageUrl = d.Cover,   // same image used as blur background
                        ReleaseYear    = d.Year,
                        Rating         = ContentRating.Teen,
                        CreatedAt      = DateTime.UtcNow.AddDays(-rng.Next(30, 600)),
                        BookmarkCount  = rng.Next(500, 80000),
                    };
                    foreach (var gId in d.Genres)
                        comic.ComicGenres.Add(new ComicGenre { ComicId = comic.Id, GenreId = gId });
                    comics.Add(comic);
                }

                await context.Comics.AddRangeAsync(comics);
                await context.SaveChangesAsync();

                // Chapters + pages
                var allChapters = new List<Chapter>();
                foreach (var comic in comics)
                {
                    int chapterCount = rng.Next(40, 180);
                    for (int i = 1; i <= chapterCount; i++)
                    {
                        var ch = new Chapter
                        {
                            ComicId       = comic.Id,
                            ChapterNumber = i,
                            Title         = rng.Next(0, 4) == 0 ? $"Chapter {i}: {Epithets[rng.Next(Epithets.Length)]}" : null,
                            PublishedAt   = DateTime.UtcNow.AddDays(-(chapterCount - i) * 7 - rng.Next(0, 3)),
                            Slug          = $"{comic.Slug}-ch-{i}",
                            ViewCount     = rng.Next(2000, 800000)
                        };
                        if (i <= 5)
                        {
                            for (int p = 1; p <= 18; p++)
                            {
                                ch.Pages.Add(new ChapterPage
                                {
                                    ChapterId  = ch.Id,
                                    PageNumber = p,
                                    ImageUrl   = $"https://picsum.photos/seed/{comic.Slug}-ch{i}-p{p}/800/1200"
                                });
                            }
                        }
                        allChapters.Add(ch);
                    }
                }

                await context.Chapters.AddRangeAsync(allChapters);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {C} comics", comics.Count);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Seeding error");
            throw;
        }
    }

    private static readonly string[] Epithets =
    [
        "The Awakening", "Dark Horizon", "Rising Storm", "Shattered Veil",
        "Blood Oath", "Eternal Night", "The Reckoning", "Forbidden Path",
        "Last Stand", "The Betrayal", "New Dawn", "Demon's Gate"
    ];
}
