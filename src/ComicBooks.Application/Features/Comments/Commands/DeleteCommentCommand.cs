using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Comments.Commands;

public record DeleteCommentCommand(Guid CommentId, Guid UserId, bool IsAdmin) : IRequest<bool>;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
{
    private readonly IApplicationDbContext _db;
    public DeleteCommentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteCommentCommand req, CancellationToken ct)
    {
        var c = await _db.ChapterComments.FirstOrDefaultAsync(x => x.Id == req.CommentId, ct);
        if (c is null) return false;
        // Faqat o'z komenti yoki admin
        if (c.UserId != req.UserId && !req.IsAdmin) return false;

        c.IsDeleted = true;
        c.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
