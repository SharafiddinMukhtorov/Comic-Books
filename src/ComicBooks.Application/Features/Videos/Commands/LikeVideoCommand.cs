using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Videos.Commands;

public record VideoReactionResult(int LikeCount, int DislikeCount, bool? MyReaction);

// IsLike = true → like, false → dislike.
// Xuddi shu tugma qayta bosilsa reaksiya bekor qilinadi, boshqasi bosilsa almashadi.
public record SetVideoReactionCommand(Guid VideoId, Guid ViewerId, bool IsLike) : IRequest<VideoReactionResult>;

public class SetVideoReactionCommandHandler : IRequestHandler<SetVideoReactionCommand, VideoReactionResult>
{
    private readonly IApplicationDbContext _context;
    public SetVideoReactionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<VideoReactionResult> Handle(SetVideoReactionCommand request, CancellationToken ct)
    {
        if (request.ViewerId == Guid.Empty)
            return await CountsAsync(request.VideoId, null, ct);

        var existing = await _context.VideoReactions
            .FirstOrDefaultAsync(r => r.VideoId == request.VideoId && r.ViewerId == request.ViewerId, ct);

        bool? mine;
        if (existing is null)
        {
            _context.VideoReactions.Add(new VideoReaction
            {
                VideoId  = request.VideoId,
                ViewerId = request.ViewerId,
                IsLike   = request.IsLike
            });
            mine = request.IsLike;
        }
        else if (!existing.IsDeleted && existing.IsLike == request.IsLike)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            mine = null;
        }
        else
        {
            existing.IsDeleted = false;
            existing.IsLike    = request.IsLike;
            existing.UpdatedAt = DateTime.UtcNow;
            mine = request.IsLike;
        }

        await _context.SaveChangesAsync(ct);
        return await CountsAsync(request.VideoId, mine, ct);
    }

    // Sanoqlar reaksiyalar jadvalidan qayta hisoblanadi — ustma-ust bosish sanoqni oshirmaydi
    private async Task<VideoReactionResult> CountsAsync(Guid videoId, bool? mine, CancellationToken ct)
    {
        var likes = await _context.VideoReactions
            .CountAsync(r => r.VideoId == videoId && r.IsLike && !r.IsDeleted, ct);
        var dislikes = await _context.VideoReactions
            .CountAsync(r => r.VideoId == videoId && !r.IsLike && !r.IsDeleted, ct);

        await _context.Videos
            .Where(v => v.Id == videoId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(v => v.LikeCount, likes)
                .SetProperty(v => v.DislikeCount, dislikes), ct);

        return new VideoReactionResult(likes, dislikes, mine);
    }
}

public record ToggleVideoFavoriteCommand(Guid VideoId, Guid ViewerId) : IRequest<bool>;

public class ToggleVideoFavoriteCommandHandler : IRequestHandler<ToggleVideoFavoriteCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public ToggleVideoFavoriteCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(ToggleVideoFavoriteCommand request, CancellationToken ct)
    {
        if (request.ViewerId == Guid.Empty) return false;

        var existing = await _context.VideoFavorites
            .FirstOrDefaultAsync(f => f.VideoId == request.VideoId && f.ViewerId == request.ViewerId, ct);

        bool result;
        if (existing is null)
        {
            _context.VideoFavorites.Add(new VideoFavorite
            {
                VideoId  = request.VideoId,
                ViewerId = request.ViewerId
            });
            result = true;
        }
        else
        {
            existing.IsDeleted = !existing.IsDeleted;
            existing.UpdatedAt = DateTime.UtcNow;
            result = !existing.IsDeleted;
        }

        await _context.SaveChangesAsync(ct);
        return result;
    }
}
