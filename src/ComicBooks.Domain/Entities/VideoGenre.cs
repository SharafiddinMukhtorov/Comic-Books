namespace ComicBooks.Domain.Entities;

public class VideoGenre
{
    public Guid VideoId { get; set; }
    public Guid GenreId { get; set; }

    // Navigation
    public Video Video { get; set; } = null!;
    public Genre Genre { get; set; } = null!;
}
