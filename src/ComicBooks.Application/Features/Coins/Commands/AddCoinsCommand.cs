using ComicBooks.Application.Common.Interfaces;
using MediatR;

namespace ComicBooks.Application.Features.Coins.Commands;

public record AddCoinsCommand(Guid UserId, int Amount, string Description, string? TelegramUsername = null)
    : IRequest<bool>;

public class AddCoinsCommandHandler : IRequestHandler<AddCoinsCommand, bool>
{
    private readonly ICoinService _coinService;
    public AddCoinsCommandHandler(ICoinService coinService) => _coinService = coinService;

    public Task<bool> Handle(AddCoinsCommand request, CancellationToken cancellationToken)
        => _coinService.AddCoinsAsync(request.UserId, request.Amount, request.Description, request.TelegramUsername, cancellationToken);
}
