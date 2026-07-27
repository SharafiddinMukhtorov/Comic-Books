using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Comics.Commands;

// ComicId = null bo'lsa — admin tavsiyasi butunlay o'chiriladi (hech kim pin qilinmagan bo'ladi)
public record SetAdminPickCommand(Guid? ComicId) : IRequest<bool>;

public class SetAdminPickCommandHandler : IRequestHandler<SetAdminPickCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public SetAdminPickCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(SetAdminPickCommand request, CancellationToken cancellationToken)
    {
        // Har doim faqat bitta komik pin qilingan bo'lishi kerak — avval hammasini tozalaymiz
        var picked = await _context.Comics
            .Where(c => !c.IsDeleted && c.IsAdminPick)
            .ToListAsync(cancellationToken);
        foreach (var c in picked)
            c.IsAdminPick = false;

        if (request.ComicId.HasValue)
        {
            var comic = await _context.Comics
                .FirstOrDefaultAsync(c => c.Id == request.ComicId.Value && !c.IsDeleted, cancellationToken);
            if (comic is null) return false;
            comic.IsAdminPick = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
