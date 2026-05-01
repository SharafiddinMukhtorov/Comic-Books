namespace ComicBooks.Application.Common.Interfaces;

public interface ICoinService
{
    Task<int> GetBalanceAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> HasAccessAsync(Guid userId, Guid chapterId, CancellationToken cancellationToken = default);
    Task<(bool Success, string Message)> SpendCoinsAsync(Guid userId, Guid chapterId, CancellationToken cancellationToken = default);
    Task<bool> AddCoinsAsync(Guid userId, int amount, string description, string? telegramUsername = null, CancellationToken cancellationToken = default);
}
