using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using ComicBooks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Infrastructure.Services;

public class ChapterPricingService : IChapterPricingService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    public ChapterPricingService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<List<ChapterPriceItemDto>> GetChapterPricingByComicAsync(Guid comicId, CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        return await db.Chapters
            .AsNoTracking()
            .Where(c => c.ComicId == comicId && !c.IsDeleted)
            .OrderBy(c => c.ChapterNumber)
            .Select(c => new ChapterPriceItemDto
            {
                Id            = c.Id,
                ChapterNumber = c.ChapterNumber,
                Title         = c.Title,
                IsLocked      = c.IsLocked,
                CoinPrice     = c.CoinPrice
            })
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChapterPricingAsync(List<ChapterPriceItemDto> items, CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var ids = items.Select(i => i.Id).ToList();
        var dbChapters = await db.Chapters
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(cancellationToken);

        foreach (var dbCh in dbChapters)
        {
            var dto = items.FirstOrDefault(i => i.Id == dbCh.Id);
            if (dto is null) continue;
            dbCh.IsLocked = dto.IsLocked;
            // Qulflangan bob narxi 0 bo'lsa hamma bepul o'qiy oladi — eng kami 1 coin.
            dbCh.CoinPrice = dto.IsLocked ? Math.Max(dto.CoinPrice, 1) : 0;
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
