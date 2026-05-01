using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Chapters.Commands;

// Create Chapter
public record CreateChapterCommand(
    Guid ComicId,
    double ChapterNumber,
    string? Title,
    string? Description,
    bool IsLocked,
    DateTime? PublishedAt,
    List<CreateChapterPageDto> Pages
) : IRequest<Guid>;

public record CreateChapterPageDto(int PageNumber, string ImageUrl, int? Width, int? Height);

public class CreateChapterCommandValidator : AbstractValidator<CreateChapterCommand>
{
    public CreateChapterCommandValidator()
    {
        RuleFor(v => v.ComicId).NotEmpty();
        RuleFor(v => v.ChapterNumber).GreaterThan(0);
        RuleFor(v => v.Pages).NotEmpty().WithMessage("Chapter must have at least one page.");
    }
}

public class CreateChapterCommandHandler : IRequestHandler<CreateChapterCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateChapterCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateChapterCommand request, CancellationToken cancellationToken)
    {
        // Slug: comicId + chapterNumber, unique guaranteed
        var slug = $"ch-{request.ComicId:N[..8]}-{request.ChapterNumber}".Replace(".", "-");

        var chapter = new Chapter
        {
            ComicId       = request.ComicId,
            ChapterNumber = request.ChapterNumber,
            Title         = request.Title,
            Description   = request.Description,
            IsLocked      = request.IsLocked,
            PublishedAt   = request.PublishedAt ?? DateTime.UtcNow,
            Slug          = slug
        };

        foreach (var page in request.Pages)
        {
            chapter.Pages.Add(new ChapterPage
            {
                ChapterId  = chapter.Id,
                PageNumber = page.PageNumber,
                ImageUrl   = page.ImageUrl,
                Width      = page.Width,
                Height     = page.Height
            });
        }

        _context.Chapters.Add(chapter);
        await _context.SaveChangesAsync(cancellationToken);
        return chapter.Id;
    }
}

// Delete Chapter
public record DeleteChapterCommand(Guid Id) : IRequest<bool>;

public class DeleteChapterCommandHandler : IRequestHandler<DeleteChapterCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public DeleteChapterCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteChapterCommand request, CancellationToken cancellationToken)
    {
        var chapter = await _context.Chapters
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);
        if (chapter is null) return false;
        chapter.IsDeleted = true;
        chapter.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
