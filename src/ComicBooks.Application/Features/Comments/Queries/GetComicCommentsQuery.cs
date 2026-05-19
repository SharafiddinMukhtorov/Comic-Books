using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Comments.Queries;

public record GetComicCommentsQuery(Guid ComicId) : IRequest<List<ChapterCommentDto>>;

public class GetComicCommentsQueryHandler
    : IRequestHandler<GetComicCommentsQuery, List<ChapterCommentDto>>
{
    private readonly IApplicationDbContext _db;
    public GetComicCommentsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<List<ChapterCommentDto>> Handle(
        GetComicCommentsQuery req, CancellationToken ct)
    {
        return await _db.ChapterComments
            .Where(c => c.ComicId == req.ComicId && c.ChapterId == null && !c.IsDeleted)
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
