using ComicBooks.Domain.Common;
using ComicBooks.Domain.Enums;

namespace ComicBooks.Domain.Entities;

public class VideoCastMember : BaseEntity
{
    public Guid VideoId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public CastRole Role { get; set; } = CastRole.Actor;
    public int SortOrder { get; set; } = 0;

    // Navigation
    public Video Video { get; set; } = null!;
}
