using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Comics.Commands;

public record RateComicCommand(Guid ComicId, int Stars) : IRequest<double>;

public class RateComicCommandHandler : IRequestHandler<RateComicCommand, double>
{
    private readonly IApplicationDbContext _context;

    public RateComicCommandHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<double> Handle(RateComicCommand request, CancellationToken cancellationToken)
    {
        var comic = await _context.Comics
            .Where(c => c.Id == request.ComicId)
            .FirstOrDefaultAsync(cancellationToken);

        if (comic is null) return 0;

        var clampedStars = Math.Clamp(request.Stars, 1, 10);

        // Exponential moving average — DB column o'zgartirmay ishlaydi
        double newAvg = comic.AverageRating < 0.1
            ? clampedStars
            : Math.Round(comic.AverageRating * 0.92 + clampedStars * 0.08, 1);

        await _context.Comics
            .Where(c => c.Id == request.ComicId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.AverageRating, newAvg), cancellationToken);

        return newAvg;
    }
}
