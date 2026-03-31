using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class Genre : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Slug { get; set; }

    // Navigation
    public ICollection<ComicGenre> ComicGenres { get; set; } = new List<ComicGenre>();
}
