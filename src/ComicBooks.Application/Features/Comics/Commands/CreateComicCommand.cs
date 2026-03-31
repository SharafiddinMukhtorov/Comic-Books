using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using ComicBooks.Domain.Enums;
using FluentValidation;
using MediatR;

namespace ComicBooks.Application.Features.Comics.Commands;

// Command
public record CreateComicCommand(
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
) : IRequest<Guid>;

// Validator
public class CreateComicCommandValidator : AbstractValidator<CreateComicCommand>
{
    public CreateComicCommandValidator()
    {
        RuleFor(v => v.Title).NotEmpty().MaximumLength(500);
        RuleFor(v => v.Description).MaximumLength(5000);
        RuleFor(v => v.ReleaseYear).InclusiveBetween(1900, DateTime.UtcNow.Year + 5).When(v => v.ReleaseYear.HasValue);
    }
}

// Handler
public class CreateComicCommandHandler : IRequestHandler<CreateComicCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateComicCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateComicCommand request, CancellationToken cancellationToken)
    {
        var slug = GenerateSlug(request.Title);

        var comic = new Comic
        {
            Title = request.Title,
            AlternativeTitles = request.AlternativeTitles,
            Description = request.Description,
            CoverImageUrl = request.CoverImageUrl,
            BannerImageUrl = request.BannerImageUrl,
            Status = request.Status,
            Type = request.Type,
            Rating = request.Rating,
            Author = request.Author,
            Artist = request.Artist,
            ReleaseYear = request.ReleaseYear,
            IsFeatured = request.IsFeatured,
            IsPopular = request.IsPopular,
            Slug = slug
        };

        foreach (var genreId in request.GenreIds)
            comic.ComicGenres.Add(new ComicGenre { ComicId = comic.Id, GenreId = genreId });

        foreach (var tagId in request.TagIds)
            comic.ComicTags.Add(new ComicTag { ComicId = comic.Id, TagId = tagId });

        _context.Comics.Add(comic);
        await _context.SaveChangesAsync(cancellationToken);

        return comic.Id;
    }

    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace(".", "")
            .Replace(",", "");

        return slug + "-" + Guid.NewGuid().ToString("N")[..6];
    }
}
