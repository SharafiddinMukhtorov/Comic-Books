using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Genres.Queries;

public record GetAllGenresQuery : IRequest<List<GenreDto>>;

public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, List<GenreDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllGenresQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<GenreDto>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
    {
        return await _context.Genres
            .Where(g => !g.IsDeleted)
            .Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Slug = g.Slug,
                ComicCount = g.ComicGenres.Count(cg => !cg.Comic.IsDeleted)
            })
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);
    }
}
