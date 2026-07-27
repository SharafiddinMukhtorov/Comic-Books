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
        // Foydalanuvchi baholashi endi komikning umumiy reytingiga (AverageRating) ta'sir qilmaydi —
        // reytingni faqat admin belgilaydi (AdminComicForm). Baholash tugmasi/UI o'zgarishsiz ishlaydi,
        // faqat hozirgi (admin qo'ygan) qiymatni qaytaradi — frontend shu qiymatni ko'rsatishda davom etadi.
        var comic = await _context.Comics
            .Where(c => c.Id == request.ComicId)
            .Select(c => c.AverageRating)
            .FirstOrDefaultAsync(cancellationToken);

        return comic;
    }
}
