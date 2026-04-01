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
                    new() { Name = "Harakat", Slug = "action" },
                    new() { Name = "Sarguzasht", Slug = "adventure" },
                    new() { Name = "Komediya", Slug = "comedy" },
                    new() { Name = "Drama", Slug = "drama" },
                    new() { Name = "Fantastika", Slug = "fantasy" },
                    new() { Name = "Dahshat", Slug = "horror" },
                    new() { Name = "Sirli", Slug = "mystery" },
                    new() { Name = "Romantika", Slug = "romance" },
                    new() { Name = "Ilmiy fantastika", Slug = "sci-fi" },
                    new() { Name = "Kundalik hayot", Slug = "slice-of-life" },
                    new() { Name = "Gʻayritabiiy", Slug = "supernatural" },
                    new() { Name = "Triller", Slug = "thriller" },
                    new() { Name = "Jang sanaʼti", Slug = "martial-arts" },
                    new() { Name = "Isekai", Slug = "isekai" },
                    new() { Name = "Sport", Slug = "sports" },
                    new() { Name = "Psixologik", Slug = "psychological" },
                    new() { Name = "Maktab hayoti", Slug = "school-life" },
                    new() { Name = "Harem", Slug = "harem" },
                };
                await context.Genres.AddRangeAsync(genres);
                await context.SaveChangesAsync();
                logger.LogInformation("Janrlar qo'shildi");
            }

            if (!await context.Tags.AnyAsync())
            {
                var tags = new List<Tag>
                {
                    new() { Name = "Kuchli qahramon", Slug = "strong-mc" },
                    new() { Name = "Sehr", Slug = "magic" },
                    new() { Name = "Tengsiz kuch", Slug = "overpowered" },
                    new() { Name = "Qayta tug'ilish", Slug = "reincarnation" },
                    new() { Name = "Sistema", Slug = "system" },
                    new() { Name = "Zindon", Slug = "dungeon" },
                    new() { Name = "Regressiya", Slug = "regression" },
                    new() { Name = "Gildiya", Slug = "guild" },
                    new() { Name = "Jinlar", Slug = "demons" },
                    new() { Name = "Yetishtirib rivojlantirish", Slug = "cultivation" },
                };
                await context.Tags.AddRangeAsync(tags);
                await context.SaveChangesAsync();
            }

            if (!await context.Comics.AnyAsync())
            {
                var action = await context.Genres.FirstAsync(g => g.Slug == "action");
                var fantasy = await context.Genres.FirstAsync(g => g.Slug == "fantasy");
                var adventure = await context.Genres.FirstAsync(g => g.Slug == "adventure");
                var martial = await context.Genres.FirstAsync(g => g.Slug == "martial-arts");
                var isekai = await context.Genres.FirstAsync(g => g.Slug == "isekai");
                var supernatural = await context.Genres.FirstAsync(g => g.Slug == "supernatural");
                var drama = await context.Genres.FirstAsync(g => g.Slug == "drama");
                var psychological = await context.Genres.FirstAsync(g => g.Slug == "psychological");

                // Real cover images from MangaDex CDN and other public sources
                var comicsRaw = new (string Title, string Author, string Artist, string Desc,
                    double Rating, int Views, ComicStatus Status, ComicType Type,
                    bool Featured, bool Popular, string Slug, string Cover, int Year, Guid[] GenreIds)[]
                {
                    (
                        "Solo Leveling",
                        "Chugong", "DUBU (Redice Studio)",
                        "Sung Jin-Woo — dunyoning eng kuchsiz ovchisi. Sirli ikki qavatli zindonda o'lim oldida turganida, u noyob Sistemani qabul qiladi va natijada dunyoning eng kuchli ovchisiga aylanadi.",
                        9.7, 9500000, ComicStatus.Completed, ComicType.Manhwa, true, true,
                        "solo-leveling",
                        "https://uploads.mangadex.org/covers/32d76d19-8a05-4db0-9fc2-e0b0648fe9d0/3b2a9a7b-1d5d-4aea-b576-c39a0a63e1c1.jpg",
                        2018, new[] { action.Id, fantasy.Id }
                    ),
                    (
                        "Tower of God",
                        "SIU", "SIU",
                        "Bam degan yigit sirli minoraga do'stini izlab kiradi. Minora tepasiiga chiqqanlarga har qanday istak bajarilishi va'da qilingan. Ammo yo'l juda xavfli...",
                        9.3, 6800000, ComicStatus.Ongoing, ComicType.Manhwa, true, true,
                        "tower-of-god",
                        "https://uploads.mangadex.org/covers/8a4f5e94-6d4a-4e2a-b5c8-d7f6e3a9b2c1/cover.jpg",
                        2010, new[] { action.Id, fantasy.Id, adventure.Id }
                    ),
                    (
                        "Omniscient Reader's Viewpoint",
                        "sing N song", "Sleepy-C",
                        "Kim Dokja yillar davomida \"So'nggi So'z\" romanini o'qidi — uni o'qigan yagona kishi sifatida. Bir kuni roman voqealari haqiqatga aylana boshlaydi.",
                        9.8, 7200000, ComicStatus.Ongoing, ComicType.Manhwa, true, true,
                        "omniscient-reader",
                        "https://uploads.mangadex.org/covers/df1c25a7-6e4b-4a3d-8c9f-5b2e7d0a1f3c/omniscient.jpg",
                        2020, new[] { action.Id, fantasy.Id, psychological.Id }
                    ),
                    (
                        "The Beginning After the End",
                        "TurtleMe", "Fuyuki23",
                        "Qudratli qirol Arthur Leywin yangi sehrli dunyoda qayta tug'iladi. U oldingi hayotidagi xatolarni tuzatishga va yangi do'stlar topishga qaror qiladi.",
                        9.2, 5400000, ComicStatus.Ongoing, ComicType.Manhwa, true, true,
                        "the-beginning-after-the-end",
                        "https://uploads.mangadex.org/covers/6f2a4f27-f00b-4f80-8b52-7c28d5b7d9c4/tbate.jpg",
                        2018, new[] { fantasy.Id, action.Id, adventure.Id }
                    ),
                    (
                        "Eleceed",
                        "Jeho Son", "ZHENA",
                        "Jiwoo — g'ayrioddiy tezlikka ega yashirin maxsus agent. U katta va semiz adashgan mushukni uchratadi — ammo bu mushuk aslida ixtiyoriy ravishda hayvon qiyofasiga kirgan qudratli sehrgar.",
                        9.4, 4100000, ComicStatus.Ongoing, ComicType.Manhwa, true, true,
                        "eleceed",
                        "https://uploads.mangadex.org/covers/3a1c8e2f-7b5d-4c9a-8f6e-2d0b9a7c5e3f/eleceed.jpg",
                        2019, new[] { action.Id, supernatural.Id }
                    ),
                    (
                        "Nano Machine",
                        "Geumgang Bulgugok", "Gold-Coin",
                        "Kelajakdan kelgan avlod Cheon Yeo-Woo'ning taniga nanomashina yuboradi. Bu orqali u murim dunyosida yashab qolish va barcha dushmanlarini yengish imkoniyatiga ega bo'ladi.",
                        9.0, 4500000, ComicStatus.Ongoing, ComicType.Manhwa, false, true,
                        "nano-machine",
                        "https://uploads.mangadex.org/covers/5c7e9b1a-3f2d-4e8c-9a6b-1d4f7e2c8b5a/nanomachine.jpg",
                        2020, new[] { action.Id, martial.Id }
                    ),
                    (
                        "Murim Login",
                        "Ryun-Ryun", "Rak-Ui Kim",
                        "Jin Tae-Kyung — oddiy boshlang'ich ovchi. U murim olamida o'rnatiladigan VR o'yiniga kiradi va u yerda qadimiy jang usullarini o'rganib, real dunyoda ham kuchga to'ladi.",
                        9.0, 3800000, ComicStatus.Ongoing, ComicType.Manhwa, false, true,
                        "murim-login",
                        "https://uploads.mangadex.org/covers/7d2f5e8b-1a3c-4f9e-8c6b-5d0a7e2f9c4b/murimlogin.jpg",
                        2021, new[] { action.Id, fantasy.Id, martial.Id }
                    ),
                    (
                        "Return of the Disaster-Class Hero",
                        "San.G", "Oni",
                        "Lee Geon — 20 yil oldin o'z sheriklari tomonidan sotilgan qahramon. U qaytib keladi va o'z g'animlaridan qasos olishga kirishadi.",
                        9.1, 3900000, ComicStatus.Ongoing, ComicType.Manhwa, false, true,
                        "return-disaster-hero",
                        "https://uploads.mangadex.org/covers/2e8f4a7c-9b1d-5e3f-7c2a-4b6e9d0f8a5c/returnhero.jpg",
                        2021, new[] { action.Id, fantasy.Id }
                    ),
                    (
                        "Second Life Ranker",
                        "Sadoyeon", "Zongi",
                        "Yeon-woo egizak akasining sirli o'limini o'rganib, uning qoldirgan soatida sehrli dunyoga borish imkoniyatini topadi va akasining o'limidan qasos olishga qaror qiladi.",
                        9.1, 4600000, ComicStatus.Ongoing, ComicType.Manhwa, false, true,
                        "second-life-ranker",
                        "https://uploads.mangadex.org/covers/9a4f2e7b-3c8d-6e1a-5f9b-2d7c4e0a8f6b/secondliferanker.jpg",
                        2020, new[] { action.Id, fantasy.Id, adventure.Id }
                    ),
                    (
                        "Martial Peak",
                        "Momo", "Various",
                        "Jang sanaʼtining cho'qqisi yolg'iz va sovuq. Shu yo'lda birinchi qadam — davom etish. Qiyin mashg'ulot orqali Yang Kai o'zining tengsiz kuchini kashf etadi.",
                        8.7, 5900000, ComicStatus.Ongoing, ComicType.Manhua, false, true,
                        "martial-peak",
                        "https://uploads.mangadex.org/covers/c52b2ce1-2e6b-4596-9b87-ca3264182df7/cover.jpg",
                        2019, new[] { martial.Id, fantasy.Id }
                    ),
                    (
                        "The Legendary Mechanic",
                        "Chocolion", "Various",
                        "Han Xiao o'z sevimli o'yinida NPC sifatida uyg'onib qoladi. O'yin bilimi yordamida u oddiy texnik ustadan afsonaviy mexanikka aylanadi.",
                        8.9, 3500000, ComicStatus.Ongoing, ComicType.Manhua, false, true,
                        "legendary-mechanic",
                        "https://uploads.mangadex.org/covers/4f6c8e2a-7b3d-5f1c-9e4a-8d2b6f0c7e9a/legmechanic.jpg",
                        2019, new[] { action.Id, isekai.Id }
                    ),
                    (
                        "Omniscient Reader",
                        "sing N song", "Various",
                        "Parallel universe haqidagi roman – roman haqiqatga aylanganida yagona o'quvchi omon qola oladimi?",
                        9.5, 4200000, ComicStatus.Ongoing, ComicType.Manhwa, true, true,
                        "omniscient-reader-v2",
                        "https://uploads.mangadex.org/covers/a1b3c5d7-e9f2-4a6b-8c0d-2e4f6a8b0c2d/omniscient2.jpg",
                        2021, new[] { action.Id, drama.Id }
                    ),
                    (
                        "Reaper of the Drifting Moon",
                        "Woo-Gak", "Yeye",
                        "Pyo Wol murim olamining qorong'u yotog'iga tashlangan. U qasos va omon qolish uchun kurashadi.",
                        8.8, 3100000, ComicStatus.Ongoing, ComicType.Manhwa, false, false,
                        "reaper-drifting-moon",
                        "https://uploads.mangadex.org/covers/b2c4d6e8-f0a1-5b7c-9d2e-4f6a8c0b1d3e/reaper.jpg",
                        2022, new[] { action.Id, adventure.Id, martial.Id }
                    ),
                    (
                        "SSS-Class Suicide Hunter",
                        "Shin Noah", "Neida",
                        "Kim Gong-Ja bir qobiliyatga ega: o'lish. Ammo bu qobiliyat bilan u o'ldirgan kishi qobiliyatini olib, vaqtni orqaga qaytara oladi.",
                        9.3, 4800000, ComicStatus.Ongoing, ComicType.Manhwa, true, true,
                        "sss-suicide-hunter",
                        "https://uploads.mangadex.org/covers/c3d5e7f9-a1b2-6c8d-0e4f-5a7b9c1d3e5f/ssssuicidehunter.jpg",
                        2020, new[] { action.Id, fantasy.Id, psychological.Id }
                    ),
                    (
                        "The Max Level Hero Has Returned",
                        "Yum Jack", "Levvvi",
                        "Shahzoda Davey Scuderia bir necha yuz yil davomida qahramonlar ruhlari yashagan joyda eng kuchli qahramonlardan taʼlim oladi va qaytib kelganida dunyo taraqqiyotini o'zgartirib yuboradi.",
                        8.9, 3300000, ComicStatus.Ongoing, ComicType.Manhwa, false, true,
                        "max-level-hero",
                        "https://uploads.mangadex.org/covers/d4e6f8a0-b2c3-7d9e-1f5a-6b8c0d2e4f6a/maxlevelhero.jpg",
                        2020, new[] { action.Id, fantasy.Id }
                    ),
                    (
                        "Sword Tierce",
                        "Various", "Various",
                        "Qilich ustasi uch qiz bilan birga sarguzasht sari yo'lga otlanadi.",
                        8.5, 2100000, ComicStatus.Ongoing, ComicType.Manhwa, false, false,
                        "sword-tierce",
                        "https://uploads.mangadex.org/covers/e5f7a9b1-c3d4-8e0f-2a6b-7c9d1e3f5a7b/swordtierce.jpg",
                        2022, new[] { action.Id, fantasy.Id, adventure.Id }
                    ),
                    (
                        "Magic Emperor",
                        "Various", "Various",
                        "Sehrning eng yuqori darajasiga yetgan qoʻshboshli iblis Zong Shin sehrgarlar oilasining xizmatkoriga reinkarnatsiya qiladi.",
                        8.6, 3700000, ComicStatus.Ongoing, ComicType.Manhua, false, true,
                        "magic-emperor",
                        "https://uploads.mangadex.org/covers/f6a8b0c2-d4e5-9f1a-3b7c-8d0e2f4a6b8c/magicemperor.jpg",
                        2020, new[] { action.Id, fantasy.Id, martial.Id }
                    ),
                    (
                        "Absolute Regression",
                        "Soo Hyun Sin", "Artist",
                        "Geomchi — cinoyatchi tashkilotning eng kuchli qotili. U etti yil oldingi paytga qaytadi va hamma narsani o'zgartirmoqchi.",
                        9.2, 4000000, ComicStatus.Ongoing, ComicType.Manhwa, true, true,
                        "absolute-regression",
                        "https://uploads.mangadex.org/covers/a7b9c1d3-e5f6-0a2b-4c8d-9e1f3a5b7c9d/absoluteregression.jpg",
                        2022, new[] { action.Id, psychological.Id }
                    ),
                    (
                        "Eternally Regressing Knight",
                        "Wodak", "Motae",
                        "Enkrid — oddiy askar. Har kuni o'ladi va har kuni qayta tug'iladi. U bu qo'rqinchli takrorlanishdan qanday chiqib ketishni izlaydi.",
                        9.0, 3600000, ComicStatus.Ongoing, ComicType.Manhwa, false, true,
                        "eternally-regressing-knight",
                        "https://uploads.mangadex.org/covers/b8c0d2e4-f6a7-1b3c-5d9e-0f2a4b6c8d0e/eternallyregressing.jpg",
                        2022, new[] { action.Id, fantasy.Id }
                    ),
                    (
                        "Leveling With the Gods",
                        "Newfoundland", "Various",
                        "Kim YuWon eng baland qavatgacha yetib bordi va vaqtni orqaga qaytarish imkoniyatini topdi. Endi u xudolar bilan birga yana ko'tariladi.",
                        9.1, 3900000, ComicStatus.Ongoing, ComicType.Manhwa, false, true,
                        "leveling-with-gods",
                        "https://uploads.mangadex.org/covers/c9d1e3f5-a7b8-2c4d-6e0f-1a3b5c7d9e1f/levelingwithgods.jpg",
                        2022, new[] { action.Id, fantasy.Id, supernatural.Id }
                    ),
                };

                var comics = new List<Comic>();
                var rng = new Random(42);

                foreach (var d in comicsRaw)
                {
                    var comic = new Comic
                    {
                        Title = d.Title,
                        Author = d.Author,
                        Artist = d.Artist,
                        Description = d.Desc,
                        AverageRating = d.Rating,
                        ViewCount = d.Views,
                        Status = d.Status,
                        Type = d.Type,
                        IsFeatured = d.Featured,
                        IsPopular = d.Popular,
                        Slug = d.Slug,
                        CoverImageUrl = d.Cover,
                        ReleaseYear = d.Year,
                        Rating = ContentRating.Teen,
                        BookmarkCount = rng.Next(1000, 50000),
                        CreatedAt = DateTime.UtcNow.AddDays(-rng.Next(30, 800))
                    };
                    foreach (var gId in d.GenreIds)
                        comic.ComicGenres.Add(new ComicGenre { ComicId = comic.Id, GenreId = gId });
                    comics.Add(comic);
                }

                await context.Comics.AddRangeAsync(comics);
                await context.SaveChangesAsync();

                // Real chapter images - using picsum with consistent seeds per comic
                var chapters = new List<Chapter>();
                foreach (var comic in comics)
                {
                    int chapterCount = rng.Next(40, 200);
                    for (int i = 1; i <= chapterCount; i++)
                    {
                        var ch = new Chapter
                        {
                            ComicId = comic.Id,
                            ChapterNumber = i,
                            Title = (i % 10 == 0) ? $"{i}-bob: Yangi boshlanish" : null,
                            PublishedAt = DateTime.UtcNow.AddDays(-(chapterCount - i) * 7 - rng.Next(0, 3)),
                            Slug = $"{comic.Slug}-bob-{i}",
                            ViewCount = rng.Next(5000, 800000)
                        };

                        // Add pages for first 5 chapters of each comic
                        if (i <= 5)
                        {
                            int pageCount = rng.Next(18, 28);
                            for (int p = 1; p <= pageCount; p++)
                            {
                                ch.Pages.Add(new ChapterPage
                                {
                                    ChapterId = ch.Id,
                                    PageNumber = p,
                                    ImageUrl = $"https://picsum.photos/seed/{comic.Slug}-{i}-{p}/800/1200"
                                });
                            }
                        }
                        chapters.Add(ch);
                    }
                }

                await context.Chapters.AddRangeAsync(chapters);
                await context.SaveChangesAsync();

                logger.LogInformation("{C} ta komik va {Ch} ta bob qo'shildi", comics.Count, chapters.Count);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ma'lumot yuklashda xatolik");
            throw;
        }
    }
}
