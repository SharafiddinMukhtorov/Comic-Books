using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ComicBooks.Application.Features.Comments.Commands;

public record AddComicCommentCommand(
    Guid    ComicId,
    Guid    UserId,
    string  Content
) : IRequest<Guid>;

public class AddComicCommentCommandValidator : AbstractValidator<AddComicCommentCommand>
{
    public AddComicCommentCommandValidator()
    {
        RuleFor(c => c.Content)
            .NotEmpty().MinimumLength(2).MaximumLength(1000);
        RuleFor(c => c.ComicId).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
    }
}

public class AddComicCommentCommandHandler : IRequestHandler<AddComicCommentCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    public AddComicCommentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Guid> Handle(AddComicCommentCommand req, CancellationToken ct)
    {
        var c = new ChapterComment
        {
            ComicId   = req.ComicId,
            ChapterId = null,
            UserId    = req.UserId,
            Content   = req.Content.Trim(),
            LikeCount = 0
        };
        _db.ChapterComments.Add(c);
        await _db.SaveChangesAsync(ct);
        return c.Id;
    }
}
