using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class ChapterPage : BaseEntity
{
    public Guid ChapterId { get; set; }
    public int PageNumber { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int? Width { get; set; }
    public int? Height { get; set; }

    // Navigation
    public Chapter Chapter { get; set; } = null!;
}
