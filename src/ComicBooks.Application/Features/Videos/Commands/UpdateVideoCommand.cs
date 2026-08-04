using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using ComicBooks.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Videos.Commands;

public record UpdateVideoCommand(
    Guid Id,
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
) : IRequest<bool>;

public class UpdateVideoCommandValidator : AbstractValidator<UpdateVideoCommand>
{
    public UpdateVideoCommandValidator()
    {
        RuleFor(v => v.Title).NotEmpty().MaximumLength(500);
        RuleFor(v => v.Description).MaximumLength(5000);
        RuleFor(v => v.ImdbRating).InclusiveBetween(0, 10);
        RuleFor(v => v.VideoUrl).NotEmpty().WithMessage("Film uchun video havolasi kerak.").When(v => v.Type == VideoType.Movie);
        RuleFor(v => v.Episodes).NotEmpty().WithMessage("Serial uchun kamida bitta qism kerak.").When(v => v.Type == VideoType.Series);
    }
}

public class UpdateVideoCommandHandler : IRequestHandler<UpdateVideoCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateVideoCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await _context.Videos
            .Include(v => v.VideoGenres)
            .Include(v => v.Episodes)
            .Include(v => v.CastMembers)
            .FirstOrDefaultAsync(v => v.Id == request.Id && !v.IsDeleted, cancellationToken);

        if (video is null) return false;

        video.Title = request.Title;
        video.OriginalTitle = request.OriginalTitle;
        video.Description = request.Description;
        video.PosterImageUrl = request.PosterImageUrl;
        video.BannerImageUrl = request.BannerImageUrl;
        video.Type = request.Type;
        video.Status = request.Status;
        video.Rating = request.Rating;
        video.Language = request.Language;
        video.Country = request.Country;
        video.ReleaseYear = request.ReleaseYear;
        video.DurationMinutes = request.DurationMinutes;
        video.ImdbRating = request.ImdbRating;
        video.VideoUrl = request.VideoUrl;
        video.VideoUrl480p = request.VideoUrl480p;
        video.VideoUrl720p = request.VideoUrl720p;
        video.VideoUrl1080p = request.VideoUrl1080p;
        video.IsFeatured = request.IsFeatured;
        video.LinkedComicId = request.LinkedComicId;
        video.UpdatedAt = DateTime.UtcNow;

        // Janrlar/qismlar/rollar — to'liq almashtirish. Avval o'chirishni saqlab yuboramiz
        // (alohida SaveChanges), keyin qo'shamiz — aks holda bir xil (VideoId,SeasonNumber,
        // EpisodeNumber) qiymatlari bilan eski qator hali o'chmasdan turib yangisi qo'shilishi
        // yoki bitta DbContext'da eski trackланган qator bilan to'qnashish xavfi bor
        // (CreateChapterCommand'dagi "revive" logikasida ham shu sabab alohida saqlanadi).
        _context.VideoGenres.RemoveRange(video.VideoGenres);
        _context.VideoEpisodes.RemoveRange(video.Episodes);
        _context.VideoCastMembers.RemoveRange(video.CastMembers);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var genreId in request.GenreIds)
            _context.VideoGenres.Add(new VideoGenre { VideoId = video.Id, GenreId = genreId });

        foreach (var ep in request.Episodes)
        {
            _context.VideoEpisodes.Add(new VideoEpisode
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
            _context.VideoCastMembers.Add(new VideoCastMember
            {
                VideoId = video.Id,
                Name = cm.Name,
                PhotoUrl = cm.PhotoUrl,
                Role = cm.Role,
                SortOrder = cm.SortOrder
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
