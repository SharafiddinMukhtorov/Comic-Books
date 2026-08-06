using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Videos.Queries;

// Foydalanuvchi sevimlilarga qo'shgan videolar (oxirgi qo'shilgani birinchi)
public record GetMyFavoriteVideosQuery(Guid ViewerId, int Take = 20) : IRequest<List<VideoDto>>;

public class GetMyFavoriteVideosQueryHandler : IRequestHandler<GetMyFavoriteVideosQuery, List<VideoDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMyFavoriteVideosQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<VideoDto>> Handle(GetMyFavoriteVideosQuery request, CancellationToken ct)
    {
        if (request.ViewerId == Guid.Empty) return new List<VideoDto>();

        var favorites = await _context.VideoFavorites
            .AsNoTracking()
            .Where(f => f.ViewerId == request.ViewerId && !f.IsDeleted)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new { f.VideoId, f.CreatedAt })
            .Take(request.Take)
            .ToListAsync(ct);

        if (favorites.Count == 0) return new List<VideoDto>();

        var ids = favorites.Select(f => f.VideoId).ToList();

        var videos = await _context.Videos
            .AsNoTracking()
            .Where(v => ids.Contains(v.Id) && !v.IsDeleted)
            .Select(v => new VideoDto
            {
                Id = v.Id,
                Title = v.Title,
                OriginalTitle = v.OriginalTitle,
                PosterImageUrl = v.PosterImageUrl,
                BannerImageUrl = v.BannerImageUrl,
                Type = v.Type,
                Status = v.Status,
                Rating = v.Rating,
                ReleaseYear = v.ReleaseYear,
                DurationMinutes = v.DurationMinutes,
                ImdbRating = v.ImdbRating,
                ViewCount = v.ViewCount,
                IsFeatured = v.IsFeatured,
                Slug = v.Slug,
                CreatedAt = v.CreatedAt,
                Genres = v.VideoGenres.Select(vg => vg.Genre.Name).ToList(),
                SeasonCount = v.Episodes.Where(e => !e.IsDeleted).Select(e => e.SeasonNumber).Distinct().Count(),
                EpisodeCount = v.Episodes.Count(e => !e.IsDeleted)
            })
            .ToListAsync(ct);

        // Sevimlilarga qo'shilgan tartibda qaytaramiz
        var order = favorites.Select((f, i) => new { f.VideoId, i }).ToDictionary(x => x.VideoId, x => x.i);
        return videos.OrderBy(v => order.TryGetValue(v.Id, out var i) ? i : int.MaxValue).ToList();
    }
}
