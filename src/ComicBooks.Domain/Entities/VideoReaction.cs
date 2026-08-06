using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class VideoReaction : BaseEntity
{
    public Guid VideoId  { get; set; }
    // Tizimga kirgan bo'lsa — foydalanuvchi Id, aks holda brauzer sessiya Id
    public Guid ViewerId { get; set; }
    public bool IsLike   { get; set; }
}
