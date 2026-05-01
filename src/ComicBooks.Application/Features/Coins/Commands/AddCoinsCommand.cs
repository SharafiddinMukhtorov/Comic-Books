using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using MediatR;

namespace ComicBooks.Application.Features.Coins.Commands;

public record AddCoinsCommand(Guid UserId, int Amount, string Description, string? TelegramUsername = null)
    : IRequest<bool>;

public class AddCoinsCommandHandler : IRequestHandler<AddCoinsCommand, bool>
{
    private readonly IApplicationDbContext _db;
    public AddCoinsCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<bool> Handle(AddCoinsCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user is null) return false;

        user.CoinBalance += request.Amount;

        _db.CoinTransactions.Add(new CoinTransaction
        {
            UserId           = request.UserId,
            Amount           = request.Amount,
            Type             = CoinTransactionType.Purchase,
            Description      = request.Description,
            TelegramUsername = request.TelegramUsername,
        });

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
