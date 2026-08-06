using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class VideoFavorite : BaseEntity
{
    public Guid VideoId  { get; set; }
    public Guid ViewerId { get; set; }
}
