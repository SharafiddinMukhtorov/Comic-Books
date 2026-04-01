using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Genres.Commands;

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
        var genre = await _context.Genres
            .FirstOrDefaultAsync(g => g.Id == request.Id && !g.IsDeleted, cancellationToken);

        if (genre is null) return false;

        genre.IsDeleted = true;
        genre.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
