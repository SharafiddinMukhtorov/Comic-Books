using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Comics.Commands;

public record TrackComicViewCommand(Guid SessionId, Guid ComicId) : IRequest<bool>;

public class TrackComicViewCommandHandler : IRequestHandler<TrackComicViewCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public TrackComicViewCommandHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<bool> Handle(TrackComicViewCommand request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;

        var alreadyViewed = await _context.ComicViews
            .AnyAsync(v => v.SessionId == request.SessionId
                        && v.ComicId   == request.ComicId
                        && v.ViewedAt  >= today
                        && !v.IsDeleted,
                      cancellationToken);

        if (alreadyViewed) return false;

        _context.ComicViews.Add(new ComicView
        {
            SessionId = request.SessionId,
            ComicId   = request.ComicId,
            ViewedAt  = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Comics
            .Where(c => c.Id == request.ComicId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.ViewCount, c => c.ViewCount + 1), cancellationToken);

        return true;
    }
}
