using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Coins.Commands;

public record SpendCoinsCommand(Guid UserId, Guid ChapterId) : IRequest<(bool Success, string Message)>;

public class SpendCoinsCommandHandler : IRequestHandler<SpendCoinsCommand, (bool Success, string Message)>
{
    private readonly IApplicationDbContext _db;
    public SpendCoinsCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<(bool Success, string Message)> Handle(SpendCoinsCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user is null) return (false, "Foydalanuvchi topilmadi");

        var chapter = await _db.Chapters.FindAsync(new object[] { request.ChapterId }, cancellationToken);
        if (chapter is null) return (false, "Bob topilmadi");

        if (!chapter.IsLocked || chapter.CoinPrice <= 0)
            return (true, "Bepul bob");

        bool alreadyOwned = await _db.ChapterAccesses
            .AnyAsync(a => a.UserId == request.UserId && a.ChapterId == request.ChapterId && !a.IsDeleted,
                      cancellationToken);
        if (alreadyOwned) return (true, "Allaqachon sotib olingan");

        if (user.CoinBalance < chapter.CoinPrice)
            return (false, $"Yetarli coin yo'q. Kerak: {chapter.CoinPrice}, Sizda: {user.CoinBalance}");

        user.CoinBalance -= chapter.CoinPrice;

        _db.CoinTransactions.Add(new CoinTransaction
        {
            UserId      = request.UserId,
            Amount      = -chapter.CoinPrice,
            Type        = CoinTransactionType.Spend,
            Description = $"Chapter {chapter.ChapterNumber} uchun",
            ChapterId   = request.ChapterId,
        });

        _db.ChapterAccesses.Add(new UserChapterAccess
        {
            UserId    = request.UserId,
            ChapterId = request.ChapterId,
            CoinSpent = chapter.CoinPrice,
        });

        await _db.SaveChangesAsync(cancellationToken);
        return (true, "Muvaffaqiyatli");
    }
}
