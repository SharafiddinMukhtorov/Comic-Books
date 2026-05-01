using ComicBooks.Application.Common.Interfaces;
using MediatR;

namespace ComicBooks.Application.Features.Coins.Commands;

public record SpendCoinsCommand(Guid UserId, Guid ChapterId) : IRequest<(bool Success, string Message)>;

public class SpendCoinsCommandHandler : IRequestHandler<SpendCoinsCommand, (bool Success, string Message)>
{
    private readonly ICoinService _coinService;
    public SpendCoinsCommandHandler(ICoinService coinService) => _coinService = coinService;

    public Task<(bool Success, string Message)> Handle(SpendCoinsCommand request, CancellationToken cancellationToken)
        => _coinService.SpendCoinsAsync(request.UserId, request.ChapterId, cancellationToken);
}
