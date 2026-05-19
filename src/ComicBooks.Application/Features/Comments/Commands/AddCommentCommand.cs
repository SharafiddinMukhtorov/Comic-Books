using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using FluentValidation;
using MediatR;

namespace ComicBooks.Application.Features.Comments.Commands;

public record AddCommentCommand(
    Guid    ChapterId,
    Guid    UserId,
    string  Content,
    Guid?   ParentId = null
) : IRequest<Guid>;

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(c => c.Content)
            .NotEmpty().WithMessage("Komentariya bo'sh bo'lmasligi kerak")
            .MinimumLength(2).WithMessage("Kamida 2 belgi")
            .MaximumLength(1000).WithMessage("Maksimal 1000 belgi");
        RuleFor(c => c.ChapterId).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
    }
}

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    public AddCommentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Guid> Handle(AddCommentCommand req, CancellationToken ct)
    {
        var c = new ChapterComment
        {
            ChapterId = req.ChapterId,
            UserId    = req.UserId,
            Content   = req.Content.Trim(),
            ParentId  = req.ParentId,
            LikeCount = 0
        };
        _db.ChapterComments.Add(c);
        await _db.SaveChangesAsync(ct);
        return c.Id;
    }
}
