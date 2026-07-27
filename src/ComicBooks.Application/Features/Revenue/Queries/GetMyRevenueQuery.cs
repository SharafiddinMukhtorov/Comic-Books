using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Revenue.Queries;

// 1 coin = 500 so'm (kelishilgan kurs)
public static class RevenueRates
{
    public const decimal CoinToSom    = 500m;
    public const decimal PlatformShare = 0.20m;   // 20% platform
    public const decimal UploaderShare = 0.80m;   // 80% yuklovchi
}

public class RevenueComicDto
{
    public Guid    ComicId        { get; set; }
    public string  Title          { get; set; } = "";
    public string? CoverImageUrl  { get; set; }
    public string? Slug           { get; set; }
    public int     ChapterCount   { get; set; }
    public int     TotalCoins     { get; set; }
    public int     TotalPurchases { get; set; }   // nechta sotuv
    public decimal TotalSom       => TotalCoins * RevenueRates.CoinToSom;
    public decimal PlatformSom    => TotalSom * RevenueRates.PlatformShare;
    public decimal UploaderSom    => TotalSom * RevenueRates.UploaderShare;
}

public class RevenueChapterDto
{
    public Guid    ChapterId      { get; set; }
    public double  ChapterNumber  { get; set; }
    public string? Title          { get; set; }
    public int     Coins          { get; set; }
    public int     Purchases      { get; set; }
}

public class RevenueResultDto
{
    public List<RevenueComicDto> Comics { get; set; } = new();
    public int     TotalCoins     { get; set; }
    public decimal TotalSom       { get; set; }
    public decimal PlatformSom    { get; set; }
    public decimal UploaderSom    { get; set; }
    public int     TotalPurchases { get; set; }
}

public record GetMyRevenueQuery(Guid UserId) : IRequest<RevenueResultDto>;

public class GetMyRevenueQueryHandler : IRequestHandler<GetMyRevenueQuery, RevenueResultDto>
{
    private readonly IApplicationDbContext _db;
    public GetMyRevenueQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<RevenueResultDto> Handle(GetMyRevenueQuery req, CancellationToken ct)
    {
        // Foydalanuvchi yuklagan komikslar — IgnoreQueryFilters: komik/bob keyinchalik
        // o'chirilsa ham, undan oldin ishlangan daromad statistikadan tushib qolmasin
        var myComics = await _db.Comics
            .IgnoreQueryFilters()
            .Where(c => c.UploaderId == req.UserId)
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.CoverImageUrl,
                c.Slug,
                ChapterCount = c.Chapters.Count
            })
            .ToListAsync(ct);

        var comicIds = myComics.Select(c => c.Id).ToList();

        // Mavjud chapters va ularning komikslari
        var chapterToComic = await _db.Chapters
            .IgnoreQueryFilters()
            .Where(ch => comicIds.Contains(ch.ComicId))
            .Select(ch => new { ch.Id, ch.ComicId, ch.ChapterNumber, ch.Title })
            .ToListAsync(ct);

        var chapterIds = chapterToComic.Select(c => c.Id).ToList();

        // Spend tranzaksiyalar (chapter sotib olish)
        var spends = await _db.CoinTransactions
            .Where(t => t.Type == CoinTransactionType.Spend
                     && t.ChapterId.HasValue
                     && chapterIds.Contains(t.ChapterId.Value))
            .Select(t => new { t.ChapterId, t.Amount })
            .ToListAsync(ct);

        // Per-comic aggregation
        var perComic = new List<RevenueComicDto>();
        foreach (var c in myComics)
        {
            var chapters = chapterToComic.Where(ch => ch.ComicId == c.Id).Select(ch => ch.Id).ToHashSet();
            var coinSpend = spends.Where(s => chapters.Contains(s.ChapterId!.Value));
            var totalCoins = coinSpend.Sum(s => Math.Abs(s.Amount));
            var purchases  = coinSpend.Count();

            perComic.Add(new RevenueComicDto
            {
                ComicId        = c.Id,
                Title          = c.Title,
                CoverImageUrl  = c.CoverImageUrl,
                Slug           = c.Slug,
                ChapterCount   = c.ChapterCount,
                TotalCoins     = totalCoins,
                TotalPurchases = purchases
            });
        }

        // Umumiy
        var totalCoinsAll = perComic.Sum(c => c.TotalCoins);
        var totalSomAll   = totalCoinsAll * RevenueRates.CoinToSom;

        return new RevenueResultDto
        {
            Comics         = perComic.OrderByDescending(c => c.TotalCoins).ToList(),
            TotalCoins     = totalCoinsAll,
            TotalSom       = totalSomAll,
            PlatformSom    = totalSomAll * RevenueRates.PlatformShare,
            UploaderSom    = totalSomAll * RevenueRates.UploaderShare,
            TotalPurchases = perComic.Sum(c => c.TotalPurchases)
        };
    }
}
