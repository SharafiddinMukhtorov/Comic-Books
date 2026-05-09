using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Comics.Commands;

public record IncrementComicViewCommand(Guid ComicId) : IRequest;

public class IncrementComicViewCommandHandler : IRequestHandler<IncrementComicViewCommand>
{
    private readonly IApplicationDbContext _context;

    public IncrementComicViewCommandHandler(IApplicationDbContext context)
        => _context = context;

    public async Task Handle(IncrementComicViewCommand request, CancellationToken cancellationToken)
    {
        await _context.Comics
            .Where(c => c.Id == request.ComicId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.ViewCount, c => c.ViewCount + 1), cancellationToken);
    }
}
