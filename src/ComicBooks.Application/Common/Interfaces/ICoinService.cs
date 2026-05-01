using ComicBooks.Application.Common.Mappings;
using ComicBooks.Domain.Entities;

namespace ComicBooks.Application.Common.Interfaces;

public interface ICoinService
{
    Task<int> GetBalanceAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> HasAccessAsync(Guid userId, Guid chapterId, CancellationToken cancellationToken = default);
    Task<(bool Success, string Message)> SpendCoinsAsync(Guid userId, Guid chapterId, CancellationToken cancellationToken = default);
    Task<bool> AddCoinsAsync(Guid userId, int amount, string description, string? telegramUsername = null, CancellationToken cancellationToken = default);
    Task<AppUserDto?> FindUserAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<CoinTransactionDto>> GetRecentTransactionsAsync(int take = 50, CancellationToken cancellationToken = default);
}
