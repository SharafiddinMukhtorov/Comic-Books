using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class ChapterComment : BaseEntity
{
    public Guid?   ChapterId   { get; set; }    // null bo'lsa — comic-level koment
    public Guid?   ComicId     { get; set; }    // qaysi komikga (chapter null bo'lsa)
    public Guid    UserId      { get; set; }
    public string  Content     { get; set; } = "";
    public Guid?   ParentId    { get; set; }
    public int     LikeCount   { get; set; } = 0;

    public AppUser? User { get; set; }
}
