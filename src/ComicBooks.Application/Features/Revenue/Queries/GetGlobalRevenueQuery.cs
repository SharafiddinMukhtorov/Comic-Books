using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Revenue.Queries;

// Platformaning umumiy daromadi (super admin uchun)
public record GetGlobalRevenueQuery : IRequest<RevenueResultDto>;

public class GetGlobalRevenueQueryHandler : IRequestHandler<GetGlobalRevenueQuery, RevenueResultDto>
{
    private readonly IApplicationDbContext _db;
    public GetGlobalRevenueQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<RevenueResultDto> Handle(GetGlobalRevenueQuery req, CancellationToken ct)
    {
        // Barcha SPEND tranzaksiyalar
        var spends = await _db.CoinTransactions
            .Where(t => t.Type == CoinTransactionType.Spend && t.ChapterId.HasValue)
            .Select(t => new { t.ChapterId, t.Amount })
            .ToListAsync(ct);

        var chapterIds = spends.Select(s => s.ChapterId!.Value).Distinct().ToList();

        // Chapter → ComicId map — IgnoreQueryFilters: o'chirilgan bob bo'lsa ham
        // undan oldin sotib olingan coin daromadga hisoblanishi davom etishi kerak
        var chapterMap = await _db.Chapters
            .IgnoreQueryFilters()
            .Where(ch => chapterIds.Contains(ch.Id))
            .Select(ch => new { ch.Id, ch.ComicId })
            .ToListAsync(ct);

        var comicIds = chapterMap.Select(c => c.ComicId).Distinct().ToList();

        // Comics ma'lumotlari — shu sababdan IgnoreQueryFilters
        var comicsList = await _db.Comics
            .IgnoreQueryFilters()
            .Where(c => comicIds.Contains(c.Id))
            .Select(c => new
            {
                c.Id, c.Title, c.CoverImageUrl, c.Slug,
                ChapterCount = c.Chapters.Count
            })
            .ToListAsync(ct);

        var perComic = new List<RevenueComicDto>();
        foreach (var c in comicsList)
        {
            var chapters = chapterMap.Where(cm => cm.ComicId == c.Id).Select(cm => cm.Id).ToHashSet();
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
