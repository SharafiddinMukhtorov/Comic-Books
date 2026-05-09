using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Bookmarks.Queries;

public record GetUserBookmarksQuery(Guid SessionId) : IRequest<List<Guid>>;

public class GetUserBookmarksQueryHandler : IRequestHandler<GetUserBookmarksQuery, List<Guid>>
{
    private readonly IApplicationDbContext _context;

    public GetUserBookmarksQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<List<Guid>> Handle(GetUserBookmarksQuery request, CancellationToken cancellationToken)
        => await _context.UserBookmarks
            .Where(b => b.SessionId == request.SessionId && !b.IsDeleted)
            .Select(b => b.ComicId)
            .ToListAsync(cancellationToken);
}
