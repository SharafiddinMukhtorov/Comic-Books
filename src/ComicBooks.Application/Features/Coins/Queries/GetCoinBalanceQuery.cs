using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Coins.Queries;

public record GetCoinBalanceQuery(Guid UserId) : IRequest<int>;

public class GetCoinBalanceQueryHandler : IRequestHandler<GetCoinBalanceQuery, int>
{
    private readonly IApplicationDbContext _db;
    public GetCoinBalanceQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<int> Handle(GetCoinBalanceQuery request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        return user?.CoinBalance ?? 0;
    }
}
