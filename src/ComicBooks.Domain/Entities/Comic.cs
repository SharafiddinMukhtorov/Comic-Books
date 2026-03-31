using ComicBooks.Domain.Common;
using ComicBooks.Domain.Enums;

namespace ComicBooks.Domain.Entities;

public class Comic : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? AlternativeTitles { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? BannerImageUrl { get; set; }
    public ComicStatus Status { get; set; } = ComicStatus.Ongoing;
    public ComicType Type { get; set; } = ComicType.Manga;
    public ContentRating Rating { get; set; } = ContentRating.Teen;
    public string? Author { get; set; }
    public string? Artist { get; set; }
    public int? ReleaseYear { get; set; }
    public double AverageRating { get; set; } = 0;
    public int ViewCount { get; set; } = 0;
    public int BookmarkCount { get; set; } = 0;
    public bool IsFeatured { get; set; } = false;
    public bool IsPopular { get; set; } = false;
    public string? Slug { get; set; }

    // Navigation
    public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    public ICollection<ComicGenre> ComicGenres { get; set; } = new List<ComicGenre>();
    public ICollection<ComicTag> ComicTags { get; set; } = new List<ComicTag>();
}
