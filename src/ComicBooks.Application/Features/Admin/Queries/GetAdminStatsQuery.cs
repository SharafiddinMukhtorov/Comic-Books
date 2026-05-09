using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Admin.Queries;

public record GetAdminStatsQuery : IRequest<AdminStatsDto>;

public class AdminStatsDto
{
    public int TotalComics      { get; set; }
    public int TotalChapters    { get; set; }
    public int TotalGenres      { get; set; }
    public int TotalTags        { get; set; }
    public int TotalBookmarks   { get; set; }
    public int TotalViews       { get; set; }
    public int UniqueViewers    { get; set; }
    public List<ComicDto> TopViewed    { get; set; } = new();
    public List<ComicDto> TopBookmarked { get; set; } = new();
}

public class GetAdminStatsQueryHandler : IRequestHandler<GetAdminStatsQuery, AdminStatsDto>
{
    private readonly IApplicationDbContext _context;

    public GetAdminStatsQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<AdminStatsDto> Handle(GetAdminStatsQuery request, CancellationToken cancellationToken)
    {
        var totalComics   = await _context.Comics.CountAsync(c => !c.IsDeleted, cancellationToken);
        var totalChapters = await _context.Chapters.CountAsync(c => !c.IsDeleted, cancellationToken);
        var totalGenres   = await _context.Genres.CountAsync(g => !g.IsDeleted, cancellationToken);
        var totalTags     = await _context.Tags.CountAsync(t => !t.IsDeleted, cancellationToken);
        var totalBookmarks = await _context.UserBookmarks.CountAsync(b => !b.IsDeleted, cancellationToken);
        var totalViews    = await _context.ComicViews.CountAsync(v => !v.IsDeleted, cancellationToken);
        var uniqueViewers = await _context.ComicViews
            .Where(v => !v.IsDeleted)
            .Select(v => v.SessionId)
            .Distinct()
            .CountAsync(cancellationToken);

        var topViewed = await _context.Comics
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.ViewCount)
            .Take(5)
            .Select(c => new ComicDto
            {
                Id = c.Id, Title = c.Title, Slug = c.Slug,
                CoverImageUrl = c.CoverImageUrl, ViewCount = c.ViewCount,
                AverageRating = c.AverageRating, ChapterCount = c.Chapters.Count(ch => !ch.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        var topBookmarked = await _context.Comics
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.BookmarkCount)
            .Take(5)
            .Select(c => new ComicDto
            {
                Id = c.Id, Title = c.Title, Slug = c.Slug,
                CoverImageUrl = c.CoverImageUrl, BookmarkCount = c.BookmarkCount,
                AverageRating = c.AverageRating, ChapterCount = c.Chapters.Count(ch => !ch.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        return new AdminStatsDto
        {
            TotalComics      = totalComics,
            TotalChapters    = totalChapters,
            TotalGenres      = totalGenres,
            TotalTags        = totalTags,
            TotalBookmarks   = totalBookmarks,
            TotalViews       = totalViews,
            UniqueViewers    = uniqueViewers,
            TopViewed        = topViewed,
            TopBookmarked    = topBookmarked
        };
    }
}
