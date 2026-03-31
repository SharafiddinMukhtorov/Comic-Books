namespace ComicBooks.Domain.Entities;

public class ComicTag
{
    public Guid ComicId { get; set; }
    public Guid TagId { get; set; }

    // Navigation
    public Comic Comic { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
