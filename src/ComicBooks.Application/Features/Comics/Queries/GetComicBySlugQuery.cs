using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Comics.Queries;

public record GetComicBySlugQuery(string Slug) : IRequest<ComicDto?>;

public class GetComicBySlugQueryHandler : IRequestHandler<GetComicBySlugQuery, ComicDto?>
{
    private readonly IApplicationDbContext _context;

    public GetComicBySlugQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ComicDto?> Handle(GetComicBySlugQuery request, CancellationToken cancellationToken)
    {
        var comic = await _context.Comics
            .Include(c => c.ComicGenres).ThenInclude(cg => cg.Genre)
            .Include(c => c.ComicTags).ThenInclude(ct => ct.Tag)
            .Include(c => c.Chapters.Where(ch => !ch.IsDeleted))
            .Where(c => !c.IsDeleted && c.Slug == request.Slug)
            .Select(c => new ComicDto
            {
                Id = c.Id,
                Title = c.Title,
                AlternativeTitles = c.AlternativeTitles,
                Description = c.Description,
                CoverImageUrl = c.CoverImageUrl,
                BannerImageUrl = c.BannerImageUrl,
                Status = c.Status,
                Type = c.Type,
                Rating = c.Rating,
                Author = c.Author,
                Artist = c.Artist,
                ReleaseYear = c.ReleaseYear,
                AverageRating = c.AverageRating,
                ViewCount = c.ViewCount,
                BookmarkCount = c.BookmarkCount,
                IsFeatured = c.IsFeatured,
                IsPopular = c.IsPopular,
                Slug = c.Slug,
                CreatedAt = c.CreatedAt,
                ChapterCount = c.Chapters.Count(ch => !ch.IsDeleted),
                LatestChapterNumber = c.Chapters.Where(ch => !ch.IsDeleted).OrderByDescending(ch => ch.ChapterNumber).Select(ch => (double?)ch.ChapterNumber).FirstOrDefault(),
                Genres = c.ComicGenres.Select(cg => cg.Genre.Name).ToList(),
                Tags = c.ComicTags.Select(ct => ct.Tag.Name).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return comic;
    }
}
