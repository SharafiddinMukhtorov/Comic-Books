using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }

    // Navigation
    public ICollection<ComicTag> ComicTags { get; set; } = new List<ComicTag>();
}
