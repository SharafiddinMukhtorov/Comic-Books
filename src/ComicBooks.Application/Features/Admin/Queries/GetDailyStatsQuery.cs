using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Admin.Queries;

public class DailyPoint
{
    public DateTime Day   { get; set; }
    public int      Count { get; set; }
}

public class DailyStatsDto
{
    public List<DailyPoint> Views         { get; set; } = new();
    public List<DailyPoint> Registrations { get; set; } = new();
    public List<DailyPoint> Purchases     { get; set; } = new();
}

public record GetDailyStatsQuery(int Days = 7) : IRequest<DailyStatsDto>;

public class GetDailyStatsQueryHandler : IRequestHandler<GetDailyStatsQuery, DailyStatsDto>
{
    private readonly IApplicationDbContext _db;
    public GetDailyStatsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<DailyStatsDto> Handle(GetDailyStatsQuery req, CancellationToken ct)
    {
        var since = DateTime.UtcNow.Date.AddDays(-(req.Days - 1));

        // Views - har kunda
        var viewsRaw = await _db.ComicViews
            .Where(v => v.ViewedAt >= since)
            .Select(v => v.ViewedAt)
            .ToListAsync(ct);

        // Registrations - har kunda
        var regsRaw = await _db.Users
            .Where(u => u.CreatedAt >= since)
            .Select(u => u.CreatedAt)
            .ToListAsync(ct);

        // Purchases (sotuvlar) - har kunda
        var purchRaw = await _db.CoinTransactions
            .Where(t => t.CreatedAt >= since
                     && t.ChapterId != null
                     && (int)t.Type == 1)  // CoinTransactionType.Spend
            .Select(t => t.CreatedAt)
            .ToListAsync(ct);

        // Bo'sh kunlar uchun ham 0 qaytarish
        var result = new DailyStatsDto();
        for (int i = 0; i < req.Days; i++)
        {
            var day = since.AddDays(i);
            var next = day.AddDays(1);
            result.Views.Add(new DailyPoint
            {
                Day   = day,
                Count = viewsRaw.Count(v => v >= day && v < next)
            });
            result.Registrations.Add(new DailyPoint
            {
                Day   = day,
                Count = regsRaw.Count(r => r >= day && r < next)
            });
            result.Purchases.Add(new DailyPoint
            {
                Day   = day,
                Count = purchRaw.Count(p => p >= day && p < next)
            });
        }
        return result;
    }
}
