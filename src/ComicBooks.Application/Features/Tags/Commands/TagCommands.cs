using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Tags.Commands;

public record CreateTagCommand(string Name) : IRequest<Guid>;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateTagCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = new Tag
        {
            Name = request.Name,
            Slug = request.Name.ToLowerInvariant().Replace(" ", "-")
        };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);
        return tag.Id;
    }
}

public record DeleteTagCommand(Guid Id) : IRequest<bool>;

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteTagCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

        if (tag is null) return false;

        tag.IsDeleted = true;
        tag.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
