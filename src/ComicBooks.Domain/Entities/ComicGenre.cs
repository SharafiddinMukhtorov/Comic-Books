namespace ComicBooks.Domain.Entities;

public class ComicGenre
{
    public Guid ComicId { get; set; }
    public Guid GenreId { get; set; }

    // Navigation
    public Comic Comic { get; set; } = null!;
    public Genre Genre { get; set; } = null!;
}
