using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class Chapter : BaseEntity
{
    public Guid ComicId { get; set; }
    public double ChapterNumber { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int ViewCount { get; set; } = 0;
    public bool IsLocked  { get; set; }
    public int  CoinPrice { get; set; } = 0;  // 0 = bepul
    public DateTime? PublishedAt { get; set; }
    public string? Slug { get; set; }

    // Navigation
    public Comic Comic { get; set; } = null!;
    public ICollection<ChapterPage> Pages { get; set; } = new List<ChapterPage>();
}
