using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Comments.Queries;

public class ChapterCommentDto
{
    public Guid     Id          { get; set; }
    public Guid     UserId      { get; set; }
    public string   UserName    { get; set; } = "";
    public string?  UserPicture { get; set; }
    public string   Content     { get; set; } = "";
    public Guid?    ParentId    { get; set; }
    public int      LikeCount   { get; set; }
    public DateTime CreatedAt   { get; set; }
}

public record GetChapterCommentsQuery(Guid ChapterId) : IRequest<List<ChapterCommentDto>>;

public class GetChapterCommentsQueryHandler
    : IRequestHandler<GetChapterCommentsQuery, List<ChapterCommentDto>>
{
    private readonly IApplicationDbContext _db;
    public GetChapterCommentsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<List<ChapterCommentDto>> Handle(
        GetChapterCommentsQuery req, CancellationToken ct)
    {
        return await _db.ChapterComments
            .Where(c => c.ChapterId == req.ChapterId && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new ChapterCommentDto
            {
                Id          = c.Id,
                UserId      = c.UserId,
                UserName    = c.User != null ? c.User.Name : "Foydalanuvchi",
                UserPicture = c.User != null ? c.User.Picture : null,
                Content     = c.Content,
                ParentId    = c.ParentId,
                LikeCount   = c.LikeCount,
                CreatedAt   = c.CreatedAt
            })
            .Take(200)
            .ToListAsync(ct);
    }
}
