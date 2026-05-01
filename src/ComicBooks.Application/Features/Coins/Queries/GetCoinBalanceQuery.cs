using ComicBooks.Application.Common.Interfaces;
using MediatR;

namespace ComicBooks.Application.Features.Coins.Queries;

public record GetCoinBalanceQuery(Guid UserId) : IRequest<int>;

public class GetCoinBalanceQueryHandler : IRequestHandler<GetCoinBalanceQuery, int>
{
    private readonly ICoinService _coinService;
    public GetCoinBalanceQueryHandler(ICoinService coinService) => _coinService = coinService;

    public Task<int> Handle(GetCoinBalanceQuery request, CancellationToken cancellationToken)
        => _coinService.GetBalanceAsync(request.UserId, cancellationToken);
}
