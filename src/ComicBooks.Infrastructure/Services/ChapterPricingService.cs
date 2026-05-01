using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using ComicBooks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Infrastructure.Services;

public class ChapterPricingService : IChapterPricingService
{
    private readonly ApplicationDbContext _db;
    public ChapterPricingService(ApplicationDbContext db) => _db = db;

    public async Task<List<ChapterPriceItemDto>> GetChapterPricingByComicAsync(Guid comicId, CancellationToken cancellationToken = default)
    {
        var chapters = await _db.Chapters
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

        return chapters;
    }

    public async Task SaveChapterPricingAsync(List<ChapterPriceItemDto> items, CancellationToken cancellationToken = default)
    {
        var ids = items.Select(i => i.Id).ToList();
        var dbChapters = await _db.Chapters
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(cancellationToken);

        foreach (var dbCh in dbChapters)
        {
            var dto = items.FirstOrDefault(i => i.Id == dbCh.Id);
            if (dto is null) continue;
            dbCh.IsLocked  = dto.IsLocked;
            dbCh.CoinPrice = dto.IsLocked ? dto.CoinPrice : 0;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
