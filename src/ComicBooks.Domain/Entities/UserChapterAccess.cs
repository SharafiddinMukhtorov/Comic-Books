using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class UserChapterAccess : BaseEntity
{
    public Guid UserId    { get; set; }
    public Guid ChapterId { get; set; }
    public int  CoinSpent { get; set; }
}
