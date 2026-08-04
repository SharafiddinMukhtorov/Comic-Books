using ComicBooks.Domain.Common;
using ComicBooks.Domain.Enums;

namespace ComicBooks.Domain.Entities;

public class Video : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? OriginalTitle { get; set; }
    public string? Description { get; set; }
    public string? PosterImageUrl { get; set; }
    public string? BannerImageUrl { get; set; }
    public VideoType Type { get; set; } = VideoType.Movie;
    public VideoStatus Status { get; set; } = VideoStatus.Ongoing;
    public ContentRating Rating { get; set; } = ContentRating.Teen;
    public string? Language { get; set; }
    public string? Country { get; set; }
    public int? ReleaseYear { get; set; }
    public int? DurationMinutes { get; set; }
    public double ImdbRating { get; set; } = 0;
    public int LikeCount { get; set; } = 0;
    public int DislikeCount { get; set; } = 0;
    public int ViewCount { get; set; } = 0;
    public string? VideoUrl { get; set; }   // "Avto" — asosiy/standart sifat manzili
    public string? VideoUrl480p { get; set; }
    public string? VideoUrl720p { get; set; }
    public string? VideoUrl1080p { get; set; }
    public bool IsFeatured { get; set; } = false;
    public string? Slug { get; set; }

    // Ixtiyoriy bog'lanish — shu videoning kitob (Comic) ekranizatsiyasi bo'lsa
    public Guid? LinkedComicId { get; set; }

    // Navigation
    public ICollection<VideoEpisode> Episodes { get; set; } = new List<VideoEpisode>();
    public ICollection<VideoCastMember> CastMembers { get; set; } = new List<VideoCastMember>();
    public ICollection<VideoGenre> VideoGenres { get; set; } = new List<VideoGenre>();
    public Comic? LinkedComic { get; set; }
}
