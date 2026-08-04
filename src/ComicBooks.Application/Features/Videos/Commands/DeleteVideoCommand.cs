using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Videos.Commands;

public record DeleteVideoCommand(Guid Id) : IRequest<bool>;

public class DeleteVideoCommandHandler : IRequestHandler<DeleteVideoCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteVideoCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await _context.Videos
            .FirstOrDefaultAsync(v => v.Id == request.Id && !v.IsDeleted, cancellationToken);

        if (video is null) return false;

        video.IsDeleted = true;
        video.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
