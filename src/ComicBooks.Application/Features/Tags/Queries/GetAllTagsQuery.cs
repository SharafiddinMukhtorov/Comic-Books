using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Tags.Queries;

public record GetAllTagsQuery : IRequest<List<TagDto>>;

public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, List<TagDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllTagsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Tags
            .Where(t => !t.IsDeleted)
            .Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                ComicCount = t.ComicTags.Count(ct => !ct.Comic.IsDeleted)
            })
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }
}
