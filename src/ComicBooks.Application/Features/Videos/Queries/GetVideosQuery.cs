using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using ComicBooks.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Videos.Queries;

public record GetVideosQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null,
    VideoType? Type = null,
    VideoStatus? Status = null,
    Guid? GenreId = null,
    bool? IsFeatured = null,
    string? SortBy = "createdAt",
    bool SortDescending = true
) : IRequest<PaginatedList<VideoDto>>;

public class GetVideosQueryHandler : IRequestHandler<GetVideosQuery, PaginatedList<VideoDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVideosQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<VideoDto>> Handle(GetVideosQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Videos
            .Include(v => v.VideoGenres).ThenInclude(vg => vg.Genre)
            .Include(v => v.Episodes)
            .Where(v => !v.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(v => v.Title.Contains(request.Search) || (v.OriginalTitle != null && v.OriginalTitle.Contains(request.Search)));

        if (request.Type.HasValue)
            query = query.Where(v => v.Type == request.Type);

        if (request.Status.HasValue)
            query = query.Where(v => v.Status == request.Status);

        if (request.GenreId.HasValue)
            query = query.Where(v => v.VideoGenres.Any(vg => vg.GenreId == request.GenreId));

        if (request.IsFeatured.HasValue)
            query = query.Where(v => v.IsFeatured == request.IsFeatured);

        query = request.SortBy?.ToLower() switch
        {
            "title" => request.SortDescending ? query.OrderByDescending(v => v.Title) : query.OrderBy(v => v.Title),
            "rating" => request.SortDescending ? query.OrderByDescending(v => v.ImdbRating) : query.OrderBy(v => v.ImdbRating),
            "views" => request.SortDescending ? query.OrderByDescending(v => v.ViewCount) : query.OrderBy(v => v.ViewCount),
            _ => request.SortDescending ? query.OrderByDescending(v => v.CreatedAt) : query.OrderBy(v => v.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(v => new VideoDto
            {
                Id = v.Id,
                Title = v.Title,
                OriginalTitle = v.OriginalTitle,
                Description = v.Description,
                PosterImageUrl = v.PosterImageUrl,
                BannerImageUrl = v.BannerImageUrl,
                Type = v.Type,
                Status = v.Status,
                Rating = v.Rating,
                Language = v.Language,
                Country = v.Country,
                ReleaseYear = v.ReleaseYear,
                DurationMinutes = v.DurationMinutes,
                ImdbRating = v.ImdbRating,
                LikeCount = v.LikeCount,
                DislikeCount = v.DislikeCount,
                ViewCount = v.ViewCount,
                VideoUrl = v.VideoUrl,
                IsFeatured = v.IsFeatured,
                Slug = v.Slug,
                CreatedAt = v.CreatedAt,
                Genres = v.VideoGenres.Select(vg => vg.Genre.Name).ToList(),
                SeasonCount = v.Episodes.Where(e => !e.IsDeleted).Select(e => e.SeasonNumber).Distinct().Count(),
                EpisodeCount = v.Episodes.Count(e => !e.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        return PaginatedList<VideoDto>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }
}
