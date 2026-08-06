using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using ComicBooks.Domain.Entities;
using ComicBooks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Infrastructure.Services;

public class CoinService : ICoinService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    public CoinService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<int> GetBalanceAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);
        return await db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.CoinBalance)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> HasAccessAsync(Guid userId, Guid chapterId, CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var chapter = await db.Chapters
            .AsNoTracking()
            .Where(c => c.Id == chapterId)
            .Select(c => new { c.IsLocked, c.CoinPrice })
            .FirstOrDefaultAsync(cancellationToken);

        if (chapter is null) return false;
        if (!chapter.IsLocked || chapter.CoinPrice <= 0) return true;

        return await db.ChapterAccesses
            .AsNoTracking()
            .AnyAsync(a => a.UserId == userId && a.ChapterId == chapterId && !a.IsDeleted, cancellationToken);
    }

    public async Task<(bool Success, string Message)> SpendCoinsAsync(Guid userId, Guid chapterId, CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var chapter = await db.Chapters
            .AsNoTracking()
            .Where(c => c.Id == chapterId)
            .Select(c => new { c.ChapterNumber, c.IsLocked, c.CoinPrice })
            .FirstOrDefaultAsync(cancellationToken);
        if (chapter is null) return (false, "Bob topilmadi");

        if (!chapter.IsLocked || chapter.CoinPrice <= 0) return (true, "Bepul bob");

        if (!await db.Users.AsNoTracking().AnyAsync(u => u.Id == userId, cancellationToken))
            return (false, "Foydalanuvchi topilmadi");

        if (await db.ChapterAccesses.AsNoTracking()
                .AnyAsync(a => a.UserId == userId && a.ChapterId == chapterId && !a.IsDeleted, cancellationToken))
            return (true, "Allaqachon sotib olingan");

        var price = chapter.CoinPrice;

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Balansni shart bilan atomar kamaytiramiz — parallel so'rovlarda ham
            // manfiyga tushmaydi va eski keshdan qayta yozilmaydi.
            var affected = await db.Users
                .Where(u => u.Id == userId && u.CoinBalance >= price)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.CoinBalance, u => u.CoinBalance - price),
                    cancellationToken);

            if (affected == 0)
            {
                await tx.RollbackAsync(cancellationToken);
                var balance = await db.Users.AsNoTracking()
                    .Where(u => u.Id == userId).Select(u => u.CoinBalance)
                    .FirstOrDefaultAsync(cancellationToken);
                return (false, $"Yetarli coin yo'q. Kerak: {price}, Sizda: {balance}");
            }

            db.CoinTransactions.Add(new CoinTransaction
            {
                UserId = userId, Amount = -price,
                Type = CoinTransactionType.Spend,
                Description = $"Chapter {chapter.ChapterNumber} uchun",
                ChapterId = chapterId,
            });
            db.ChapterAccesses.Add(new UserChapterAccess
            {
                UserId = userId, ChapterId = chapterId, CoinSpent = price,
            });
            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return (true, "Muvaffaqiyatli");
        }
        catch (DbUpdateException)
        {
            // Unique (UserId, ChapterId) — bir vaqtda ikki marta sotib olishga urinilgan.
            // Tranzaksiya qaytarilgani uchun coin yechilmay qoladi.
            await tx.RollbackAsync(cancellationToken);
            return (true, "Allaqachon sotib olingan");
        }
    }

    public async Task<bool> AddCoinsAsync(Guid userId, int amount, string description, string? telegramUsername = null, CancellationToken cancellationToken = default)
    {
        if (amount <= 0) return false;

        await using var db = await _factory.CreateDbContextAsync(cancellationToken);
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        var affected = await db.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.CoinBalance, u => u.CoinBalance + amount),
                cancellationToken);

        if (affected == 0)
        {
            await tx.RollbackAsync(cancellationToken);
            return false;
        }

        db.CoinTransactions.Add(new CoinTransaction
        {
            UserId = userId, Amount = amount,
            Type = CoinTransactionType.Purchase,
            Description = description,
            TelegramUsername = telegramUsername,
        });
        await db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RemoveCoinsAsync(Guid userId, int amount, string description, CancellationToken cancellationToken = default)
    {
        if (amount <= 0) return false;

        await using var db = await _factory.CreateDbContextAsync(cancellationToken);
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        var before = await db.Users.AsNoTracking()
            .Where(u => u.Id == userId).Select(u => (int?)u.CoinBalance)
            .FirstOrDefaultAsync(cancellationToken);

        if (before is null or <= 0)
        {
            await tx.RollbackAsync(cancellationToken);
            return false;
        }

        // Balansdan ko'p ayirmaymiz (manfiy bo'lib ketmasin)
        var toRemove = Math.Min(amount, before.Value);

        var affected = await db.Users
            .Where(u => u.Id == userId && u.CoinBalance >= toRemove)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.CoinBalance, u => u.CoinBalance - toRemove),
                cancellationToken);

        if (affected == 0)
        {
            await tx.RollbackAsync(cancellationToken);
            return false;
        }

        db.CoinTransactions.Add(new CoinTransaction
        {
            UserId = userId, Amount = -toRemove,
            Type = CoinTransactionType.Refund,
            Description = description,
        });
        await db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<AppUserDto?> FindUserAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var q = searchTerm.TrimStart('@').Trim().ToLower();
        return await db.Users
            .AsNoTracking()
            .Where(u => u.Email.ToLower().Contains(q) ||
                        (u.TelegramUsername != null && u.TelegramUsername.ToLower().Contains(q)) ||
                        u.Name.ToLower().Contains(q))
            .Select(u => new AppUserDto
            {
                Id = u.Id, Name = u.Name, Email = u.Email,
                Picture = u.Picture, CoinBalance = u.CoinBalance,
                TelegramUsername = u.TelegramUsername, IsAdmin = u.IsAdmin,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<CoinTransactionDto>> GetRecentTransactionsAsync(int take = 50, CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var txs = await db.CoinTransactions
            .AsNoTracking()
            .Where(t => !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

        var userIds = txs.Select(t => t.UserId).Distinct().ToList();
        var users = await db.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Name })
            .ToDictionaryAsync(u => u.Id, u => u.Name, cancellationToken);

        // Sarflangan bo'lsa — qaysi bob, qaysi komik ekanini hozirgi Chapter/Comic
        // ma'lumotidan ko'rsatamiz — nomi o'zgargan bo'lsa ham to'g'ri chiqadi.
        var chapterIds = txs.Where(t => t.ChapterId.HasValue).Select(t => t.ChapterId!.Value).Distinct().ToList();
        var chapterInfo = await db.Chapters
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(ch => chapterIds.Contains(ch.Id))
            .Select(ch => new { ch.Id, ch.ChapterNumber, ComicTitle = ch.Comic!.Title })
            .ToDictionaryAsync(ch => ch.Id, cancellationToken);

        return txs.Select(t => new CoinTransactionDto
        {
            UserId = t.UserId,
            UserName = users.TryGetValue(t.UserId, out var n) ? n : t.UserId.ToString()[..8],
            Amount = t.Amount,
            Type = t.Type.ToString(),
            Description = t.Description,
            TelegramUsername = t.TelegramUsername,
            CreatedAt = t.CreatedAt,
            ComicTitle = t.ChapterId.HasValue && chapterInfo.TryGetValue(t.ChapterId.Value, out var ci) ? ci.ComicTitle : null,
            ChapterNumber = t.ChapterId.HasValue && chapterInfo.TryGetValue(t.ChapterId.Value, out var ci2) ? ci2.ChapterNumber : null,
        }).ToList();
    }
}
