using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using ComicBooks.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Comics.Commands;

public record UpdateComicCommand(
    Guid Id,
    string Title,
    string? AlternativeTitles,
    string? Description,
    string? CoverImageUrl,
    string? BannerImageUrl,
    ComicStatus Status,
    ComicType Type,
    ContentRating Rating,
    string? Author,
    string? Artist,
    int? ReleaseYear,
    bool IsFeatured,
    bool IsPopular,
    List<Guid> GenreIds,
    List<Guid> TagIds
) : IRequest<bool>;

public class UpdateComicCommandValidator : AbstractValidator<UpdateComicCommand>
{
    public UpdateComicCommandValidator()
    {
        RuleFor(v => v.Title).NotEmpty().MaximumLength(500);
        RuleFor(v => v.Description).MaximumLength(5000);
    }
}

public class UpdateComicCommandHandler : IRequestHandler<UpdateComicCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateComicCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateComicCommand request, CancellationToken cancellationToken)
    {
        var comic = await _context.Comics
            .Include(c => c.ComicGenres)
            .Include(c => c.ComicTags)
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

        if (comic is null) return false;

        comic.Title = request.Title;
        comic.AlternativeTitles = request.AlternativeTitles;
        comic.Description = request.Description;
        comic.CoverImageUrl = request.CoverImageUrl;
        comic.BannerImageUrl = request.BannerImageUrl;
        comic.Status = request.Status;
        comic.Type = request.Type;
        comic.Rating = request.Rating;
        comic.Author = request.Author;
        comic.Artist = request.Artist;
        comic.ReleaseYear = request.ReleaseYear;
        comic.IsFeatured = request.IsFeatured;
        comic.IsPopular = request.IsPopular;
        comic.UpdatedAt = DateTime.UtcNow;

        // Update genres
        _context.ComicGenres.RemoveRange(comic.ComicGenres);
        foreach (var genreId in request.GenreIds)
            comic.ComicGenres.Add(new ComicGenre { ComicId = comic.Id, GenreId = genreId });

        // Update tags
        _context.ComicTags.RemoveRange(comic.ComicTags);
        foreach (var tagId in request.TagIds)
            comic.ComicTags.Add(new ComicTag { ComicId = comic.Id, TagId = tagId });

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
