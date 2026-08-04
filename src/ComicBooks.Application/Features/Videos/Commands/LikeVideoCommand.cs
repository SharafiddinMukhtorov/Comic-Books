using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Videos.Commands;

public record LikeVideoCommand(Guid VideoId) : IRequest<int>;

public class LikeVideoCommandHandler : IRequestHandler<LikeVideoCommand, int>
{
    private readonly IApplicationDbContext _context;

    public LikeVideoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<int> Handle(LikeVideoCommand request, CancellationToken cancellationToken)
    {
        await _context.Videos
            .Where(v => v.Id == request.VideoId)
            .ExecuteUpdateAsync(s => s.SetProperty(v => v.LikeCount, v => v.LikeCount + 1), cancellationToken);

        return await _context.Videos
            .Where(v => v.Id == request.VideoId)
            .Select(v => v.LikeCount)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

public record DislikeVideoCommand(Guid VideoId) : IRequest<int>;

public class DislikeVideoCommandHandler : IRequestHandler<DislikeVideoCommand, int>
{
    private readonly IApplicationDbContext _context;

    public DislikeVideoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<int> Handle(DislikeVideoCommand request, CancellationToken cancellationToken)
    {
        await _context.Videos
            .Where(v => v.Id == request.VideoId)
            .ExecuteUpdateAsync(s => s.SetProperty(v => v.DislikeCount, v => v.DislikeCount + 1), cancellationToken);

        return await _context.Videos
            .Where(v => v.Id == request.VideoId)
            .Select(v => v.DislikeCount)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
