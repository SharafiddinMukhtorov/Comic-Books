using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Profile.Queries;

public class UserProfileDto
{
    public Guid     Id              { get; set; }
    public string   Email           { get; set; } = "";
    public string   Name            { get; set; } = "";
    public string?  Picture         { get; set; }
    public int      CoinBalance     { get; set; }
    public DateTime CreatedAt       { get; set; }
    public DateTime LastLogin       { get; set; }
    public bool     HasPassword     { get; set; }
    public bool     HasGoogle       { get; set; }
    public int      BookmarkCount   { get; set; }
    public int      PurchaseCount   { get; set; }
}

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileDto?>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto?>
{
    private readonly IApplicationDbContext _db;
    public GetUserProfileQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<UserProfileDto?> Handle(GetUserProfileQuery req, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == req.UserId, ct);
        if (user is null) return null;

        var bookmarkCount = await _db.UserBookmarks
            .Where(b => !b.IsDeleted)
            .CountAsync(ct);   // session-based, dummy 0 if not tracked per user

        var purchaseCount = await _db.ChapterAccesses
            .Where(a => a.UserId == user.Id)
            .CountAsync(ct);

        return new UserProfileDto
        {
            Id            = user.Id,
            Email         = user.Email,
            Name          = string.IsNullOrEmpty(user.Name) ? user.Email.Split('@')[0] : user.Name,
            Picture       = user.Picture,
            CoinBalance   = user.CoinBalance,
            CreatedAt     = user.CreatedAt,
            LastLogin     = user.LastLogin,
            HasPassword   = !string.IsNullOrEmpty(user.PasswordHash),
            HasGoogle     = !string.IsNullOrEmpty(user.GoogleId),
            BookmarkCount = bookmarkCount,
            PurchaseCount = purchaseCount
        };
    }
}
