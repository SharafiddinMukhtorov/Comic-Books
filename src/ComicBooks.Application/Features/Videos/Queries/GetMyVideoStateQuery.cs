using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Videos.Queries;

// Foydalanuvchining shu videodagi holati: like/dislike qo'yganmi va sevimlilarga qo'shganmi
public record MyVideoStateDto(bool? Reaction, bool IsFavorite);

public record GetMyVideoStateQuery(Guid VideoId, Guid ViewerId) : IRequest<MyVideoStateDto>;

public class GetMyVideoStateQueryHandler : IRequestHandler<GetMyVideoStateQuery, MyVideoStateDto>
{
    private readonly IApplicationDbContext _context;
    public GetMyVideoStateQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<MyVideoStateDto> Handle(GetMyVideoStateQuery request, CancellationToken ct)
    {
        if (request.ViewerId == Guid.Empty) return new MyVideoStateDto(null, false);

        var reaction = await _context.VideoReactions
            .AsNoTracking()
            .Where(r => r.VideoId == request.VideoId && r.ViewerId == request.ViewerId && !r.IsDeleted)
            .Select(r => (bool?)r.IsLike)
            .FirstOrDefaultAsync(ct);

        var isFavorite = await _context.VideoFavorites
            .AsNoTracking()
            .AnyAsync(f => f.VideoId == request.VideoId && f.ViewerId == request.ViewerId && !f.IsDeleted, ct);

        return new MyVideoStateDto(reaction, isFavorite);
    }
}
