using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Chapters.Queries;

public record GetChaptersByComicQuery(Guid ComicId) : IRequest<List<ChapterDto>>;

public class GetChaptersByComicQueryHandler : IRequestHandler<GetChaptersByComicQuery, List<ChapterDto>>
{
    private readonly IApplicationDbContext _context;

    public GetChaptersByComicQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChapterDto>> Handle(GetChaptersByComicQuery request, CancellationToken cancellationToken)
    {
        return await _context.Chapters
            .Include(ch => ch.Pages)
            .Where(ch => ch.ComicId == request.ComicId && !ch.IsDeleted)
            .OrderByDescending(ch => ch.ChapterNumber)
            .Select(ch => new ChapterDto
            {
                Id = ch.Id,
                ComicId = ch.ComicId,
                ChapterNumber = ch.ChapterNumber,
                Title = ch.Title,
                ViewCount = ch.ViewCount,
                IsLocked = ch.IsLocked,
                PublishedAt = ch.PublishedAt,
                Slug = ch.Slug,
                CreatedAt = ch.CreatedAt,
                PageCount = ch.Pages.Count
            })
            .ToListAsync(cancellationToken);
    }
}

public record GetChapterWithPagesQuery(Guid ChapterId) : IRequest<(ChapterDto? Chapter, List<ChapterPageDto> Pages)>;

public class GetChapterWithPagesQueryHandler : IRequestHandler<GetChapterWithPagesQuery, (ChapterDto? Chapter, List<ChapterPageDto> Pages)>
{
    private readonly IApplicationDbContext _context;

    public GetChapterWithPagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(ChapterDto? Chapter, List<ChapterPageDto> Pages)> Handle(GetChapterWithPagesQuery request, CancellationToken cancellationToken)
    {
        var chapter = await _context.Chapters
            .Include(ch => ch.Pages)
            .Include(ch => ch.Comic)
            .Where(ch => ch.Id == request.ChapterId && !ch.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (chapter is null) return (null, new List<ChapterPageDto>());

        var chapterDto = new ChapterDto
        {
            Id = chapter.Id,
            ComicId = chapter.ComicId,
            ComicTitle = chapter.Comic.Title,
            ComicCoverUrl = chapter.Comic.CoverImageUrl,
            ChapterNumber = chapter.ChapterNumber,
            Title = chapter.Title,
            ViewCount = chapter.ViewCount,
            IsLocked = chapter.IsLocked,
            PublishedAt = chapter.PublishedAt,
            Slug = chapter.Slug,
            CreatedAt = chapter.CreatedAt,
            PageCount = chapter.Pages.Count
        };

        var pages = chapter.Pages
            .OrderBy(p => p.PageNumber)
            .Select(p => new ChapterPageDto
            {
                Id = p.Id,
                ChapterId = p.ChapterId,
                PageNumber = p.PageNumber,
                ImageUrl = p.ImageUrl,
                Width = p.Width,
                Height = p.Height
            })
            .ToList();

        return (chapterDto, pages);
    }
}
