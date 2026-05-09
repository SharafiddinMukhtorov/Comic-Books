using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Bookmarks.Commands;

public record ToggleBookmarkCommand(Guid SessionId, Guid ComicId) : IRequest<bool>;

public class ToggleBookmarkCommandHandler : IRequestHandler<ToggleBookmarkCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ToggleBookmarkCommandHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<bool> Handle(ToggleBookmarkCommand request, CancellationToken cancellationToken)
    {
        var existing = await _context.UserBookmarks
            .Where(b => b.SessionId == request.SessionId && b.ComicId == request.ComicId && !b.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing is not null)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            await _context.Comics
                .Where(c => c.Id == request.ComicId)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.BookmarkCount, c => c.BookmarkCount > 0 ? c.BookmarkCount - 1 : 0), cancellationToken);

            return false;
        }

        _context.UserBookmarks.Add(new UserBookmark
        {
            SessionId = request.SessionId,
            ComicId   = request.ComicId,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Comics
            .Where(c => c.Id == request.ComicId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.BookmarkCount, c => c.BookmarkCount + 1), cancellationToken);

        return true;
    }
}
