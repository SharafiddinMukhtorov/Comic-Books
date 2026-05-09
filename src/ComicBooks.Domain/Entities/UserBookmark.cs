using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class UserBookmark : BaseEntity
{
    public Guid SessionId { get; set; }
    public Guid ComicId { get; set; }
    public Comic? Comic { get; set; }
}
