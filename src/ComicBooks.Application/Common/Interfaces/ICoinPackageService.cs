using ComicBooks.Application.Common.Mappings;

namespace ComicBooks.Application.Common.Interfaces;

public interface ICoinPackageService
{
    Task<List<CoinPackageDto>> GetAllAsync(CancellationToken ct = default);
    Task<CoinPackageDto> CreateAsync(CoinPackageDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(CoinPackageDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
