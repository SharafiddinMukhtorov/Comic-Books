using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Comics.Commands;

public record ToggleFeaturedCommand(List<Guid> FeaturedIds) : IRequest<bool>;

public class ToggleFeaturedCommandHandler : IRequestHandler<ToggleFeaturedCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ToggleFeaturedCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ToggleFeaturedCommand request, CancellationToken cancellationToken)
    {
        var comics = await _context.Comics
            .Where(c => !c.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var comic in comics)
            comic.IsFeatured = request.FeaturedIds.Contains(comic.Id);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
