using ComicBooks.Domain.Entities;
using ComicBooks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Web.Services;

public class CoinService
{
    private readonly ApplicationDbContext _db;

    public CoinService(ApplicationDbContext db) => _db = db;

    // Foydalanuvchi balansini olish
    public async Task<int> GetBalance(Guid userId)
    {
        var user = await _db.Users.FindAsync(userId);
        return user?.CoinBalance ?? 0;
    }

    // Admin: foydalanuvchiga coin qo'shish
    public async Task<bool> AddCoins(Guid userId, int amount, string description, string? telegramUsername = null)
    {
        var user = await _db.Users.FindAsync(userId);
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

        await _db.SaveChangesAsync();
        return true;
    }

    // Bob uchun coin sarflash
    public async Task<(bool Success, string Message)> SpendCoins(Guid userId, Guid chapterId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return (false, "Foydalanuvchi topilmadi");

        var chapter = await _db.Chapters.FindAsync(chapterId);
        if (chapter is null) return (false, "Bob topilmadi");

        if (!chapter.IsLocked || chapter.CoinPrice <= 0)
            return (true, "Bepul bob");

        // Allaqachon sotib olinganmi?
        bool alreadyOwned = await _db.ChapterAccesses
            .AnyAsync(a => a.UserId == userId && a.ChapterId == chapterId && !a.IsDeleted);
        if (alreadyOwned) return (true, "Allaqachon sotib olingan");

        // Balans yetarlimi?
        if (user.CoinBalance < chapter.CoinPrice)
            return (false, $"Yetarli coin yo'q. Kerak: {chapter.CoinPrice}, Sizda: {user.CoinBalance}");

        // Coin yeching
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

        await _db.SaveChangesAsync();
        return (true, "Muvaffaqiyatli");
    }

    // Foydalanuvchi ushbu bobga kirish huquqiga egami?
    public async Task<bool> HasAccess(Guid userId, Guid chapterId)
    {
        var chapter = await _db.Chapters.FindAsync(chapterId);
        if (chapter is null) return false;
        if (!chapter.IsLocked || chapter.CoinPrice <= 0) return true;

        return await _db.ChapterAccesses
            .AnyAsync(a => a.UserId == userId && a.ChapterId == chapterId && !a.IsDeleted);
    }

    // Foydalanuvchi tranzaksiya tarixi
    public async Task<List<CoinTransaction>> GetHistory(Guid userId, int take = 20)
    {
        return await _db.CoinTransactions
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .Take(take)
            .ToListAsync();
    }
}
