using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using ComicBooks.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Comics.Queries;

// Query
public record GetComicsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    ComicStatus? Status = null,
    ComicType? Type = null,
    Guid? GenreId = null,
    bool? IsFeatured = null,
    bool? IsPopular = null,
    string? SortBy = "createdAt",
    bool SortDescending = true
) : IRequest<PaginatedList<ComicDto>>;

// Handler
public class GetComicsQueryHandler : IRequestHandler<GetComicsQuery, PaginatedList<ComicDto>>
{
    private readonly IApplicationDbContext _context;

    public GetComicsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<ComicDto>> Handle(GetComicsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Comics
            .Include(c => c.ComicGenres).ThenInclude(cg => cg.Genre)
            .Include(c => c.ComicTags).ThenInclude(ct => ct.Tag)
            .Include(c => c.Chapters)
            .Where(c => !c.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.Title.Contains(request.Search) || (c.Author != null && c.Author.Contains(request.Search)));

        if (request.Status.HasValue)
            query = query.Where(c => c.Status == request.Status);

        if (request.Type.HasValue)
            query = query.Where(c => c.Type == request.Type);

        if (request.GenreId.HasValue)
            query = query.Where(c => c.ComicGenres.Any(cg => cg.GenreId == request.GenreId));

        if (request.IsFeatured.HasValue)
            query = query.Where(c => c.IsFeatured == request.IsFeatured);

        if (request.IsPopular.HasValue)
            query = query.Where(c => c.IsPopular == request.IsPopular);

        query = request.SortBy?.ToLower() switch
        {
            "title" => request.SortDescending ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title),
            "rating" => request.SortDescending ? query.OrderByDescending(c => c.AverageRating) : query.OrderBy(c => c.AverageRating),
            "views" => request.SortDescending ? query.OrderByDescending(c => c.ViewCount) : query.OrderBy(c => c.ViewCount),
            _ => request.SortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .ToListAsync(cancellationToken);

        return PaginatedList<ComicDto>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }
}
