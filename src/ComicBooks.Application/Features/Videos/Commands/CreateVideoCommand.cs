using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using ComicBooks.Domain.Enums;
using FluentValidation;
using MediatR;

namespace ComicBooks.Application.Features.Videos.Commands;

public record VideoEpisodeItemDto(int SeasonNumber, int EpisodeNumber, string? Title, string VideoUrl, string? ThumbnailUrl, int? DurationMinutes, string? VideoUrl480p = null, string? VideoUrl720p = null, string? VideoUrl1080p = null);
public record VideoCastMemberItemDto(string Name, string? PhotoUrl, CastRole Role, int SortOrder);

public record CreateVideoCommand(
    string Title,
    string? OriginalTitle,
    string? Description,
    string? PosterImageUrl,
    string? BannerImageUrl,
    VideoType Type,
    VideoStatus Status,
    ContentRating Rating,
    string? Language,
    string? Country,
    int? ReleaseYear,
    int? DurationMinutes,
    double ImdbRating,
    string? VideoUrl,
    bool IsFeatured,
    List<Guid> GenreIds,
    List<VideoEpisodeItemDto> Episodes,
    List<VideoCastMemberItemDto> CastMembers,
    Guid? LinkedComicId,
    string? VideoUrl480p = null,
    string? VideoUrl720p = null,
    string? VideoUrl1080p = null
) : IRequest<Guid>;

public class CreateVideoCommandValidator : AbstractValidator<CreateVideoCommand>
{
    public CreateVideoCommandValidator()
    {
        RuleFor(v => v.Title).NotEmpty().MaximumLength(500);
        RuleFor(v => v.Description).MaximumLength(5000);
        RuleFor(v => v.ReleaseYear).InclusiveBetween(1900, DateTime.UtcNow.Year + 5).When(v => v.ReleaseYear.HasValue);
        RuleFor(v => v.ImdbRating).InclusiveBetween(0, 10);
        RuleFor(v => v.VideoUrl).NotEmpty().WithMessage("Film uchun video havolasi kerak.").When(v => v.Type == VideoType.Movie);
        RuleFor(v => v.Episodes).NotEmpty().WithMessage("Serial uchun kamida bitta qism kerak.").When(v => v.Type == VideoType.Series);
    }
}

public class CreateVideoCommandHandler : IRequestHandler<CreateVideoCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateVideoCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
    {
        var slug = GenerateSlug(request.Title);

        var video = new Video
        {
            Title = request.Title,
            OriginalTitle = request.OriginalTitle,
            Description = request.Description,
            PosterImageUrl = request.PosterImageUrl,
            BannerImageUrl = request.BannerImageUrl,
            Type = request.Type,
            Status = request.Status,
            Rating = request.Rating,
            Language = request.Language,
            Country = request.Country,
            ReleaseYear = request.ReleaseYear,
            DurationMinutes = request.DurationMinutes,
            ImdbRating = request.ImdbRating,
            VideoUrl = request.VideoUrl,
            VideoUrl480p = request.VideoUrl480p,
            VideoUrl720p = request.VideoUrl720p,
            VideoUrl1080p = request.VideoUrl1080p,
            IsFeatured = request.IsFeatured,
            Slug = slug,
            LinkedComicId = request.LinkedComicId
        };

        foreach (var genreId in request.GenreIds)
            video.VideoGenres.Add(new VideoGenre { VideoId = video.Id, GenreId = genreId });

        foreach (var ep in request.Episodes)
        {
            video.Episodes.Add(new VideoEpisode
            {
                VideoId = video.Id,
                SeasonNumber = ep.SeasonNumber,
                EpisodeNumber = ep.EpisodeNumber,
                Title = ep.Title,
                VideoUrl = ep.VideoUrl,
                VideoUrl480p = ep.VideoUrl480p,
                VideoUrl720p = ep.VideoUrl720p,
                VideoUrl1080p = ep.VideoUrl1080p,
                ThumbnailUrl = ep.ThumbnailUrl,
                DurationMinutes = ep.DurationMinutes,
                PublishedAt = DateTime.UtcNow
            });
        }

        foreach (var cm in request.CastMembers)
        {
            video.CastMembers.Add(new VideoCastMember
            {
                VideoId = video.Id,
                Name = cm.Name,
                PhotoUrl = cm.PhotoUrl,
                Role = cm.Role,
                SortOrder = cm.SortOrder
            });
        }

        _context.Videos.Add(video);
        await _context.SaveChangesAsync(cancellationToken);

        return video.Id;
    }

    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace(".", "")
            .Replace(",", "");

        return slug + "-" + Guid.NewGuid().ToString("N")[..6];
    }
}
