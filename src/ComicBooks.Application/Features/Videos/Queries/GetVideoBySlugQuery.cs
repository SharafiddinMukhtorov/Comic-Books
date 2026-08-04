using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Videos.Queries;

public record GetVideoBySlugQuery(string Slug) : IRequest<VideoDto?>;

public class GetVideoBySlugQueryHandler : IRequestHandler<GetVideoBySlugQuery, VideoDto?>
{
    private readonly IApplicationDbContext _context;

    public GetVideoBySlugQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VideoDto?> Handle(GetVideoBySlugQuery request, CancellationToken cancellationToken)
    {
        var video = await _context.Videos
            .Include(v => v.VideoGenres).ThenInclude(vg => vg.Genre)
            .Include(v => v.Episodes.Where(e => !e.IsDeleted))
            .Include(v => v.CastMembers)
            .Include(v => v.LinkedComic)
            .Where(v => !v.IsDeleted && v.Slug == request.Slug)
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
                VideoUrl480p = v.VideoUrl480p,
                VideoUrl720p = v.VideoUrl720p,
                VideoUrl1080p = v.VideoUrl1080p,
                IsFeatured = v.IsFeatured,
                Slug = v.Slug,
                CreatedAt = v.CreatedAt,
                Genres = v.VideoGenres.Select(vg => vg.Genre.Name).ToList(),
                SeasonCount = v.Episodes.Select(e => e.SeasonNumber).Distinct().Count(),
                EpisodeCount = v.Episodes.Count,
                Episodes = v.Episodes
                    .OrderBy(e => e.SeasonNumber).ThenBy(e => e.EpisodeNumber)
                    .Select(e => new VideoEpisodeDto
                    {
                        Id = e.Id,
                        VideoId = e.VideoId,
                        SeasonNumber = e.SeasonNumber,
                        EpisodeNumber = e.EpisodeNumber,
                        Title = e.Title,
                        VideoUrl = e.VideoUrl,
                        VideoUrl480p = e.VideoUrl480p,
                        VideoUrl720p = e.VideoUrl720p,
                        VideoUrl1080p = e.VideoUrl1080p,
                        ThumbnailUrl = e.ThumbnailUrl,
                        DurationMinutes = e.DurationMinutes,
                        ViewCount = e.ViewCount
                    }).ToList(),
                CastMembers = v.CastMembers
                    .OrderBy(cm => cm.SortOrder)
                    .Select(cm => new VideoCastMemberDto
                    {
                        Id = cm.Id,
                        Name = cm.Name,
                        PhotoUrl = cm.PhotoUrl,
                        Role = cm.Role,
                        SortOrder = cm.SortOrder
                    }).ToList(),
                LinkedComic = v.LinkedComic == null ? null : new LinkedComicSummaryDto
                {
                    Id = v.LinkedComic.Id,
                    Title = v.LinkedComic.Title,
                    CoverImageUrl = v.LinkedComic.CoverImageUrl,
                    Slug = v.LinkedComic.Slug
                }
            })
            .FirstOrDefaultAsync(cancellationToken);

        return video;
    }
}
