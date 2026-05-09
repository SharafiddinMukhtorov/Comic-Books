using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class ComicView : BaseEntity
{
    public Guid SessionId { get; set; }
    public Guid ComicId { get; set; }
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
}
