using ComicBooks.Application.Common.Mappings;

namespace ComicBooks.Application.Common.Interfaces;

public interface IChapterPricingService
{
    Task<List<ChapterPriceItemDto>> GetChapterPricingByComicAsync(Guid comicId, CancellationToken cancellationToken = default);
    Task SaveChapterPricingAsync(List<ChapterPriceItemDto> items, CancellationToken cancellationToken = default);
}
