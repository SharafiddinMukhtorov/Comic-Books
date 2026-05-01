using ComicBooks.Application.Common.Interfaces;
using MediatR;

namespace ComicBooks.Application.Features.Coins.Queries;

public record CheckChapterAccessQuery(Guid UserId, Guid ChapterId) : IRequest<bool>;

public class CheckChapterAccessQueryHandler : IRequestHandler<CheckChapterAccessQuery, bool>
{
    private readonly ICoinService _coinService;
    public CheckChapterAccessQueryHandler(ICoinService coinService) => _coinService = coinService;

    public Task<bool> Handle(CheckChapterAccessQuery request, CancellationToken cancellationToken)
        => _coinService.HasAccessAsync(request.UserId, request.ChapterId, cancellationToken);
}
