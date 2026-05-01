using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using ComicBooks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Infrastructure.Services;

public class CoinService : ICoinService
{
    private readonly ApplicationDbContext _db;

    public CoinService(ApplicationDbContext db) => _db = db;

    public async Task<int> GetBalanceAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FindAsync(new object[] { userId }, cancellationToken);
        return user?.CoinBalance ?? 0;
    }

    public async Task<bool> HasAccessAsync(Guid userId, Guid chapterId, CancellationToken cancellationToken = default)
    {
        var chapter = await _db.Chapters.FindAsync(new object[] { chapterId }, cancellationToken);
        if (chapter is null) return false;
        if (!chapter.IsLocked || chapter.CoinPrice <= 0) return true;

        return await _db.ChapterAccesses
            .AnyAsync(a => a.UserId == userId && a.ChapterId == chapterId && !a.IsDeleted, cancellationToken);
    }

    public async Task<(bool Success, string Message)> SpendCoinsAsync(Guid userId, Guid chapterId, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user is null) return (false, "Foydalanuvchi topilmadi");

        var chapter = await _db.Chapters.FindAsync(new object[] { chapterId }, cancellationToken);
        if (chapter is null) return (false, "Bob topilmadi");

        if (!chapter.IsLocked || chapter.CoinPrice <= 0)
            return (true, "Bepul bob");

        bool alreadyOwned = await _db.ChapterAccesses
            .AnyAsync(a => a.UserId == userId && a.ChapterId == chapterId && !a.IsDeleted, cancellationToken);
        if (alreadyOwned) return (true, "Allaqachon sotib olingan");

        if (user.CoinBalance < chapter.CoinPrice)
            return (false, $"Yetarli coin yo'q. Kerak: {chapter.CoinPrice}, Sizda: {user.CoinBalance}");

        user.CoinBalance -= chapter.CoinPrice;

        _db.CoinTransactions.Add(new CoinTransaction
        {
            UserId      = userId,
            Amount      = -chapter.CoinPrice,
            Type        = CoinTransactionType.Spend,
            Description = $"Chapter {chapter.ChapterNumber} uchun",
            ChapterId   = chapterId,
        });

        _db.ChapterAccesses.Add(new UserChapterAccess
        {
            UserId    = userId,
            ChapterId = chapterId,
            CoinSpent = chapter.CoinPrice,
        });

        await _db.SaveChangesAsync(cancellationToken);
        return (true, "Muvaffaqiyatli");
    }

    public async Task<bool> AddCoinsAsync(Guid userId, int amount, string description, string? telegramUsername = null, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user is null) return false;

        user.CoinBalance += amount;

        _db.CoinTransactions.Add(new CoinTransaction
        {
            UserId           = userId,
            Amount           = amount,
            Type             = CoinTransactionType.Purchase,
            Description      = description,
            TelegramUsername = telegramUsername,
        });

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
