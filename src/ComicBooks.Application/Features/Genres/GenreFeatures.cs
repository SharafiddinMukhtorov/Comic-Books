using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Application.Common.Mappings;
using ComicBooks.Domain.Entities;
using FluentValidation;
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
public record CreateGenreCommand(string Name, string? Description) : IRequest<Guid>;

public class CreateGenreCommandValidator : AbstractValidator<CreateGenreCommand>
{
    public CreateGenreCommandValidator() => RuleFor(v => v.Name).NotEmpty().MaximumLength(100);
}

public class CreateGenreCommandHandler : IRequestHandler<CreateGenreCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateGenreCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = new Genre
        {
            Name = request.Name,
            Description = request.Description,
            Slug = request.Name.ToLowerInvariant().Replace(" ", "-")
        };
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync(cancellationToken);
        return genre.Id;
    }
}

public record DeleteGenreCommand(Guid Id) : IRequest<bool>;

public class DeleteGenreCommandHandler : IRequestHandler<DeleteGenreCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public DeleteGenreCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == request.Id && !g.IsDeleted, cancellationToken);
        if (genre is null) return false;
        genre.IsDeleted = true;
        genre.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

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

public record CreateTagCommand(string Name) : IRequest<Guid>;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateTagCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = new Tag { Name = request.Name, Slug = request.Name.ToLowerInvariant().Replace(" ", "-") };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);
        return tag.Id;
    }
}
