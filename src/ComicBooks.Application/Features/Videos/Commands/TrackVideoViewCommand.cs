using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Videos.Commands;

public record TrackVideoViewCommand(Guid SessionId, Guid VideoId) : IRequest<bool>;

public class TrackVideoViewCommandHandler : IRequestHandler<TrackVideoViewCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public TrackVideoViewCommandHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<bool> Handle(TrackVideoViewCommand request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;

        var alreadyViewed = await _context.VideoViews
            .AnyAsync(v => v.SessionId == request.SessionId
                        && v.VideoId   == request.VideoId
                        && v.ViewedAt  >= today
                        && !v.IsDeleted,
                      cancellationToken);

        if (alreadyViewed) return false;

        _context.VideoViews.Add(new VideoView
        {
            SessionId = request.SessionId,
            VideoId   = request.VideoId,
            ViewedAt  = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Videos
            .Where(v => v.Id == request.VideoId)
            .ExecuteUpdateAsync(s => s.SetProperty(v => v.ViewCount, v => v.ViewCount + 1), cancellationToken);

        return true;
    }
}
