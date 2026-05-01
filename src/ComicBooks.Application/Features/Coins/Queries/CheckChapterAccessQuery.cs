using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Coins.Queries;

public record CheckChapterAccessQuery(Guid UserId, Guid ChapterId) : IRequest<bool>;

public class CheckChapterAccessQueryHandler : IRequestHandler<CheckChapterAccessQuery, bool>
{
    private readonly IApplicationDbContext _db;
    public CheckChapterAccessQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<bool> Handle(CheckChapterAccessQuery request, CancellationToken cancellationToken)
    {
        var chapter = await _db.Chapters.FindAsync(new object[] { request.ChapterId }, cancellationToken);
        if (chapter is null) return false;
        if (!chapter.IsLocked || chapter.CoinPrice <= 0) return true;

        return await _db.ChapterAccesses
            .AnyAsync(a => a.UserId == request.UserId && a.ChapterId == request.ChapterId && !a.IsDeleted,
                      cancellationToken);
    }
}
