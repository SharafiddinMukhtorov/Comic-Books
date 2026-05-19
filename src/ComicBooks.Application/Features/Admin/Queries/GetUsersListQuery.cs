using ComicBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Features.Admin.Queries;

public class AdminUserDto
{
    public Guid     Id           { get; set; }
    public string   Email        { get; set; } = "";
    public string   Name         { get; set; } = "";
    public string?  Picture      { get; set; }
    public bool     HasGoogle    { get; set; }
    public bool     HasPassword  { get; set; }
    public bool     IsAdmin      { get; set; }
    public int      CoinBalance  { get; set; }
    public DateTime CreatedAt    { get; set; }
    public DateTime LastLogin    { get; set; }
}

public record GetUsersListQuery(int Take = 100) : IRequest<List<AdminUserDto>>;

public class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, List<AdminUserDto>>
{
    private readonly IApplicationDbContext _db;
    public GetUsersListQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<List<AdminUserDto>> Handle(GetUsersListQuery req, CancellationToken ct)
    {
        return await _db.Users
            .OrderByDescending(u => u.CreatedAt)
            .Take(req.Take)
            .Select(u => new AdminUserDto
            {
                Id          = u.Id,
                Email       = u.Email,
                Name        = string.IsNullOrEmpty(u.Name) ? u.Email : u.Name,
                Picture     = u.Picture,
                HasGoogle   = u.GoogleId != null && u.GoogleId != "",
                HasPassword = u.PasswordHash != null && u.PasswordHash != "",
                IsAdmin     = u.IsAdmin,
                CoinBalance = u.CoinBalance,
                CreatedAt   = u.CreatedAt,
                LastLogin   = u.LastLogin
            })
            .ToListAsync(ct);
    }
}
