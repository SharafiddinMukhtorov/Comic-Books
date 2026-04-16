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
                    new() { Name = "Action",       Slug = "action" },
                    new() { Name = "Adventure",    Slug = "adventure" },
                    new() { Name = "Comedy",       Slug = "comedy" },
                    new() { Name = "Drama",        Slug = "drama" },
                    new() { Name = "Fantasy",      Slug = "fantasy" },
                    new() { Name = "Horror",       Slug = "horror" },
                    new() { Name = "Mystery",      Slug = "mystery" },
                    new() { Name = "Romance",      Slug = "romance" },
                    new() { Name = "Sci-Fi",       Slug = "sci-fi" },
                    new() { Name = "Supernatural", Slug = "supernatural" },
                    new() { Name = "Thriller",     Slug = "thriller" },
                    new() { Name = "Martial Arts", Slug = "martial-arts" },
                    new() { Name = "Isekai",       Slug = "isekai" },
                    new() { Name = "Psychological",Slug = "psychological" },
                    new() { Name = "School Life",  Slug = "school-life" },
                    new() { Name = "Murim",        Slug = "murim" },
                };
                await context.Genres.AddRangeAsync(genres);
                await context.SaveChangesAsync();
            }

            if (!await context.Tags.AnyAsync())
            {
                var tags = new List<Tag>
                {
                    new() { Name = "Strong MC",    Slug = "strong-mc" },
                    new() { Name = "Overpowered",  Slug = "overpowered" },
                    new() { Name = "Reincarnation",Slug = "reincarnation" },
                    new() { Name = "System",       Slug = "system" },
                    new() { Name = "Dungeon",      Slug = "dungeon" },
                    new() { Name = "Regression",   Slug = "regression" },
                    new() { Name = "Cultivation",  Slug = "cultivation" },
                };
                await context.Tags.AddRangeAsync(tags);
                await context.SaveChangesAsync();
            }

            if (!await context.Comics.AnyAsync())
            {
                var action      = await context.Genres.FirstAsync(g => g.Slug == "action");
                var fantasy     = await context.Genres.FirstAsync(g => g.Slug == "fantasy");
                var adventure   = await context.Genres.FirstAsync(g => g.Slug == "adventure");
                var martialArts = await context.Genres.FirstAsync(g => g.Slug == "martial-arts");
                var isekai      = await context.Genres.FirstAsync(g => g.Slug == "isekai");
                var supernatural= await context.Genres.FirstAsync(g => g.Slug == "supernatural");
                var murim       = await context.Genres.FirstAsync(g => g.Slug == "murim");
                var psychological= await context.Genres.FirstAsync(g => g.Slug == "psychological");

                // High-quality picsum covers per comic (seeded by slug = consistent)
                var comicsData = new[]
                {
                    // ── FEATURED ──────────────────────────────────────────────────────────
                    new { Title="Solo Leveling",              Author="Chugong",        Artist="DUBU",        Desc="Sung Jinwoo, the world's weakest hunter, discovers a mysterious quest log after nearly dying in a double dungeon. He alone can see these quests that allow him to grow without limits.",       Rating=9.7, Views=8500000, Status=ComicStatus.Completed, Type=ComicType.Manhwa, Featured=true,  Popular=true,  Slug="solo-leveling",           Cover="https://picsum.photos/seed/sololvl/300/450",   Genres=new[]{action.Id,fantasy.Id},            Year=2018 },
                    new { Title="Omniscient Reader's Viewpoint",Author="sing N song",   Artist="Sleepy-C",    Desc="The only reader of a web novel witnesses its story become reality. Armed with complete foreknowledge, Kim Dokja must survive a world transformed into a ruthless game.",                       Rating=9.8, Views=5800000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=true,  Popular=true,  Slug="omniscient-reader",        Cover="https://picsum.photos/seed/omnireader/300/450",Genres=new[]{action.Id,fantasy.Id,psychological.Id},Year=2020 },
                    new { Title="The Beginning After the End",Author="TurtleMe",        Artist="Fuyuki23",    Desc="King Grey reincarnates as Arthur Leywin in a magical world. Vowing to cherish what he lost, he rises again — but dark forces from his past follow him across lifetimes.",                   Rating=9.2, Views=4900000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=true,  Popular=true,  Slug="the-beginning-after-end",  Cover="https://picsum.photos/seed/tbate/300/450",    Genres=new[]{fantasy.Id,action.Id,adventure.Id},      Year=2018 },
                    new { Title="Tower of God",               Author="SIU",             Artist="SIU",         Desc="Twenty-Fifth Bam has lived his whole life beneath a tower. When his best friend Rachel enters it, he follows — and discovers the tower tests everything: power, ambition, and heart.",       Rating=9.3, Views=6200000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=true,  Popular=true,  Slug="tower-of-god",             Cover="https://picsum.photos/seed/towergod/300/450", Genres=new[]{action.Id,fantasy.Id,adventure.Id},      Year=2010 },
                    new { Title="Eleceed",                    Author="Jeho Son",        Artist="ZHENA",       Desc="Jiwoo Seo, a kind-hearted boy with superspeed, takes in a mysterious fat cat — who turns out to be a top-tier awakened warrior in hiding. Together they navigate a world of covert battles.", Rating=9.4, Views=3800000, Status=ComicStatus.Ongoing, Type=ComicType.Manhwa, Featured=true,  Popular=true,  Slug="eleceed",                  Cover="https://picsum.photos/seed/eleceed/300/450",  Genres=new[]{action.Id,supernatural.Id},              Year=2019 },
                    new { Title="Wandering Warrior of Wudang",Author="Kim Gyu-Sam",     Artist="Park Chan",   Desc="A boy raised in the sacred Wudang sect descends into the chaos of the martial world. His journey tests his training, his will, and his understanding of what it means to be a warrior.",    Rating=9.3, Views=3100000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=true,  Popular=true,  Slug="wandering-warrior-wudang", Cover="https://picsum.photos/seed/wudang/300/450",   Genres=new[]{martialArts.Id,murim.Id,action.Id},      Year=2022 },
                    new { Title="Standard of Reincarnation",  Author="Ip Gak",          Artist="Various",     Desc="A genius swordsman of the Zion bloodline reincarnates as the weakest member of the Adenka family. Using memories of a past life, he rewrites his destiny.",                                  Rating=9.1, Views=2700000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=true,  Popular=true,  Slug="standard-reincarnation",   Cover="https://picsum.photos/seed/stdreincarn/300/450",Genres=new[]{action.Id,fantasy.Id,isekai.Id},        Year=2023 },
                    new { Title="Eternally Regressing Knight", Author="Ro Ya",          Artist="Various",     Desc="A knight is cursed to reset and repeat each day without anyone remembering. He must unravel the curse alone, accumulating experience and power no one else can see.",                         Rating=9.9, Views=4600000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=true,  Popular=true,  Slug="eternally-regressing-knight",Cover="https://picsum.photos/seed/eteregress/300/450",Genres=new[]{action.Id,fantasy.Id},                  Year=2023 },
                    new { Title="Childhood Friend of the Zenith",Author="Chaeyun",      Artist="Zoontar",     Desc="A regression story where a man returns to his childhood, armed with the knowledge of a lifetime, to change his fate and protect those he once lost.",                                       Rating=9.2, Views=3400000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=true,  Popular=true,  Slug="childhood-friend-zenith",  Cover="https://picsum.photos/seed/cfzenith/300/450", Genres=new[]{action.Id,murim.Id,fantasy.Id},          Year=2022 },
                    new { Title="Reborn as a Heavenly Demon",  Author="Various",        Artist="Various",     Desc="A man reborn into the body of a demonic cult's young master must conceal his true identity while navigating the treacherous politics of the murim world.",                                   Rating=9.0, Views=2800000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=true,  Popular=false, Slug="reborn-heavenly-demon",    Cover="https://picsum.photos/seed/reborndemon/300/450",Genres=new[]{action.Id,murim.Id},                    Year=2022 },
                    new { Title="Swordmaster's Youngest Son",  Author="Sung San-Young", Artist="Hyun-Jin",    Desc="Jin Runcandel, the weakest of the legendary Runcandel family, is killed in disgrace — and reborn with memories intact. This time, he will become the greatest swordmaster.",                Rating=9.0, Views=3200000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=true,  Popular=true,  Slug="swordmasters-youngest-son",Cover="https://picsum.photos/seed/swordmaster/300/450",Genres=new[]{action.Id,fantasy.Id},                 Year=2021 },
                    new { Title="Infinite Mage",               Author="Juugin",         Artist="Lek",         Desc="Shirone, a commoner who loves magic, is discovered by a noble and given a chance to attend the mage academy. His unique insight challenges every magical convention.",                        Rating=9.1, Views=2500000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=true,  Popular=true,  Slug="infinite-mage",            Cover="https://picsum.photos/seed/infmage/300/450",  Genres=new[]{fantasy.Id,action.Id,adventure.Id},      Year=2021 },
                    // ── POPULAR / TRENDING ────────────────────────────────────────────────
                    new { Title="Return of the Disaster-Class Hero",Author="San.G",     Artist="Oni",         Desc="A hero betrayed and left to die in the monster realm returns 20 years later with overwhelming power — and a hunger for vengeance against those who condemned him.",                          Rating=9.1, Views=3700000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=false, Popular=true,  Slug="return-disaster-class-hero",Cover="https://picsum.photos/seed/disaster/300/450",  Genres=new[]{action.Id,fantasy.Id},                  Year=2021 },
                    new { Title="Revenge of the Iron-Blooded Sword Hound",Author="Sajoin", Artist="Suk",      Desc="Vikir, a loyal hunting dog of the empire, is betrayed and executed. Reborn as a child, he will exact cold-blooded revenge against the eight houses that destroyed him.",                   Rating=9.4, Views=4100000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=false, Popular=true,  Slug="iron-blooded-sword-hound", Cover="https://picsum.photos/seed/ironblood/300/450", Genres=new[]{action.Id,fantasy.Id},                  Year=2022 },
                    new { Title="Chronicles of the Demon Faction",Author="Ugak",        Artist="Various",     Desc="A demon faction elder reincarnates into the righteous sect. With knowledge of both worlds, he must carefully walk the line between light and darkness.",                                     Rating=9.2, Views=3600000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=false, Popular=true,  Slug="chronicles-demon-faction", Cover="https://picsum.photos/seed/demonfact/300/450",Genres=new[]{action.Id,murim.Id,fantasy.Id},          Year=2022 },
                    new { Title="The Novel's Extra",            Author="Jee Gab Song",  Artist="Various",     Desc="A writer wakes up inside his own unfinished novel as a background extra. He must survive using his authorial knowledge while the plot unfolds around him.",                                  Rating=9.0, Views=2900000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=false, Popular=true,  Slug="the-novels-extra",         Cover="https://picsum.photos/seed/novelsextra/300/450",Genres=new[]{action.Id,fantasy.Id,isekai.Id},        Year=2021 },
                    new { Title="Nano Machine",                 Author="Geumgang",       Artist="Gold-Coin",   Desc="A descendant from the future injects nanomachines into his ancestor Cheon Yeo-Woon to help him survive the deadly power struggles of the murim world.",                                   Rating=9.0, Views=4100000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=false, Popular=true,  Slug="nano-machine",             Cover="https://picsum.photos/seed/nanomach/300/450", Genres=new[]{action.Id,martialArts.Id,murim.Id},      Year=2020 },
                    new { Title="Martial Peak",                 Author="Momo",           Artist="Various",     Desc="Yang Kai discovers a black book that sets him on the solitary and arduous path of martial cultivation. The journey to the peak tests everything he has.",                                   Rating=8.7, Views=5600000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhua, Featured=false, Popular=true,  Slug="martial-peak",             Cover="https://picsum.photos/seed/martialpk/300/450",Genres=new[]{martialArts.Id,fantasy.Id},              Year=2019 },
                    new { Title="Second Life Ranker",           Author="Sadoyeon",       Artist="Zongi",       Desc="After his twin brother is betrayed and killed inside a brutal clock tower, Yeon-woo uses the diary left behind to conquer it himself — and avenge his brother.",                          Rating=9.1, Views=4200000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=false, Popular=true,  Slug="second-life-ranker",       Cover="https://picsum.photos/seed/secondlife/300/450",Genres=new[]{action.Id,fantasy.Id,adventure.Id},     Year=2020 },
                    new { Title="Ending Maker",                 Author="D-dart",         Artist="Various",     Desc="Two players transmigrate into their favorite game at its end stage. With complete knowledge of the plot, they try to avoid the game's catastrophic ending.",                               Rating=8.8, Views=2100000, Status=ComicStatus.Completed,  Type=ComicType.Manhwa, Featured=false, Popular=true,  Slug="ending-maker",             Cover="https://picsum.photos/seed/endingmkr/300/450",Genres=new[]{action.Id,fantasy.Id,isekai.Id},         Year=2020 },
                    new { Title="The Great Mage Returns After 4000 Years",Author="Kingofez",Artist="Various",  Desc="The greatest mage of all time is reincarnated after 4000 years of imprisonment as the weakest student at a magic academy. His return reshapes the world.",                                 Rating=9.0, Views=3100000, Status=ComicStatus.Ongoing,   Type=ComicType.Manhwa, Featured=false, Popular=true,  Slug="great-mage-returns",       Cover="https://picsum.photos/seed/greatmage/300/450",Genres=new[]{action.Id,fantasy.Id},                  Year=2020 },
                };

                var rng = new Random(42);
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
                        BannerImageUrl = d.Cover, // same image blurred as banner
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

                // Add chapters with pages
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
                        // Pages for first 5 chapters
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
                logger.LogInformation("Seeded {C} comics with chapters", comics.Count);
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
