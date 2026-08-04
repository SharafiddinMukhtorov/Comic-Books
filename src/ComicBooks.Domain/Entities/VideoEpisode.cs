using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class VideoEpisode : BaseEntity
{
    public Guid VideoId { get; set; }
    public int SeasonNumber { get; set; } = 1;   // "fasl"
    public int EpisodeNumber { get; set; }        // "qism"
    public string? Title { get; set; }
    public string VideoUrl { get; set; } = string.Empty;   // "Avto" — asosiy/standart sifat manzili
    public string? VideoUrl480p { get; set; }
    public string? VideoUrl720p { get; set; }
    public string? VideoUrl1080p { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int? DurationMinutes { get; set; }
    public int ViewCount { get; set; } = 0;
    public DateTime? PublishedAt { get; set; }

    // Navigation
    public Video Video { get; set; } = null!;
}
