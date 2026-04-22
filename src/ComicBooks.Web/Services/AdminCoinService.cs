using ComicBooks.Domain.Entities;
using ComicBooks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Web.Services;

public class AdminCoinService
{
    private readonly ApplicationDbContext _db;
    public AdminCoinService(ApplicationDbContext db) => _db = db;

    public async Task<List<CoinTransaction>> GetRecentTransactions(int take = 50)
    {
        return await _db.CoinTransactions
            .Where(t => !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .Take(take)
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, string>> GetUserNames(IEnumerable<Guid> userIds)
    {
        return await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Name);
    }

    public async Task<AppUser?> FindUser(string query)
    {
        var q = query.TrimStart('@').Trim().ToLower();
        return await _db.Users.FirstOrDefaultAsync(u =>
            u.Email.ToLower().Contains(q) ||
            (u.TelegramUsername != null && u.TelegramUsername.ToLower().Contains(q)) ||
            u.Name.ToLower().Contains(q));
    }

    public async Task<AppUser?> RefreshUser(Guid id) =>
        await _db.Users.FindAsync(id);

    public async Task<List<ChapterPricingItem>> GetChapterPricing(Guid comicId)
    {
        return await _db.Chapters
            .Where(c => c.ComicId == comicId && !c.IsDeleted)
            .OrderBy(c => c.ChapterNumber)
            .Select(c => new ChapterPricingItem
            {
                Id            = c.Id,
                ChapterNumber = c.ChapterNumber,
                Title         = c.Title,
                IsLocked      = c.IsLocked,
                CoinPrice     = c.CoinPrice
            })
            .ToListAsync();
    }

    public async Task SaveChapterPricing(List<ChapterPricingItem> items)
    {
        var ids = items.Select(i => i.Id).ToList();
        var chapters = await _db.Chapters.Where(c => ids.Contains(c.Id)).ToListAsync();
        foreach (var ch in chapters)
        {
            var item = items.First(i => i.Id == ch.Id);
            ch.IsLocked  = item.IsLocked;
            ch.CoinPrice = item.IsLocked ? item.CoinPrice : 0;
        }
        await _db.SaveChangesAsync();
    }
}

public class ChapterPricingItem
{
    public Guid   Id            { get; set; }
    public double ChapterNumber { get; set; }
    public string? Title        { get; set; }
    public bool   IsLocked      { get; set; }
    public int    CoinPrice     { get; set; }
}
