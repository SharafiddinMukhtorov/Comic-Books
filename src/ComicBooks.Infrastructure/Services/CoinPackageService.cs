using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using ComicBooks.Domain.Entities;
using ComicBooks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Infrastructure.Services;

public class CoinPackageService : ICoinPackageService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    public CoinPackageService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<List<CoinPackageDto>> GetAllAsync(CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);

        return await db.CoinPackages
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.SortOrder).ThenBy(p => p.CoinAmount)
            .Select(p => new CoinPackageDto
            {
                Id = p.Id, Name = p.Name, CoinAmount = p.CoinAmount,
                BonusCoins = p.BonusCoins, Price = p.Price,
                IsPopular = p.IsPopular, SortOrder = p.SortOrder
            })
            .ToListAsync(ct);
    }

    public async Task<CoinPackageDto> CreateAsync(CoinPackageDto dto, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);

        // Faqat bitta mashhur bo'lishi mumkin
        if (dto.IsPopular)
            await ClearPopularFlagAsync(db, null, ct);

        var entity = new CoinPackage
        {
            Name = dto.Name, CoinAmount = dto.CoinAmount, BonusCoins = dto.BonusCoins,
            Price = dto.Price, IsPopular = dto.IsPopular, SortOrder = dto.SortOrder
        };
        db.CoinPackages.Add(entity);
        await db.SaveChangesAsync(ct);
        dto.Id = entity.Id;
        return dto;
    }

    public async Task<bool> UpdateAsync(CoinPackageDto dto, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);

        var entity = await db.CoinPackages.FirstOrDefaultAsync(p => p.Id == dto.Id, ct);
        if (entity is null) return false;

        // Faqat bitta mashhur — boshqalardan olib tashlaymiz
        if (dto.IsPopular)
            await ClearPopularFlagAsync(db, dto.Id, ct);

        entity.Name = dto.Name; entity.CoinAmount = dto.CoinAmount;
        entity.BonusCoins = dto.BonusCoins; entity.Price = dto.Price;
        entity.IsPopular = dto.IsPopular; entity.SortOrder = dto.SortOrder;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);

        var entity = await db.CoinPackages.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (entity is null) return false;
        entity.IsDeleted = true;
        await db.SaveChangesAsync(ct);
        return true;
    }

    // Boshqa barcha paketlardan IsPopular = false qilamiz (except exceptId)
    private static async Task ClearPopularFlagAsync(ApplicationDbContext db, Guid? exceptId, CancellationToken ct)
    {
        var others = await db.CoinPackages
            .Where(p => !p.IsDeleted && p.IsPopular && (exceptId == null || p.Id != exceptId))
            .ToListAsync(ct);
        foreach (var p in others) p.IsPopular = false;
        // SaveChanges caller tomonidan chaqiriladi
    }
}
