using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class VideoView : BaseEntity
{
    public Guid SessionId { get; set; }
    public Guid VideoId { get; set; }
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
}
