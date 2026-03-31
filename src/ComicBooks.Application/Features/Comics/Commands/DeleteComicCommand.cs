using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Comics.Commands;

public record DeleteComicCommand(Guid Id) : IRequest<bool>;

public class DeleteComicCommandHandler : IRequestHandler<DeleteComicCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteComicCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteComicCommand request, CancellationToken cancellationToken)
    {
        var comic = await _context.Comics
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

        if (comic is null) return false;

        comic.IsDeleted = true;
        comic.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
