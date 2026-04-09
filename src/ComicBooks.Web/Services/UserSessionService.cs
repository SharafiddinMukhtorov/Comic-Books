using ComicBooks.Domain.Entities;
using ComicBooks.Infrastructure.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Web.Services;

public class UserSessionService
{
    private readonly IHttpContextAccessor _http;
    private readonly ApplicationDbContext _db;
    private AppUser? _cachedUser;

    public UserSessionService(IHttpContextAccessor http, ApplicationDbContext db)
    {
        _http = http;
        _db   = db;
    }

    public bool IsAuthenticated => _http.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public string? Email => _http.HttpContext?.User?
        .FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

    public string? Name => _http.HttpContext?.User?
        .FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

    public string? Picture => _http.HttpContext?.User?
        .FindFirst("urn:google:picture")?.Value;

    public async Task<AppUser?> GetUserAsync()
    {
        if (_cachedUser != null) return _cachedUser;
        if (!IsAuthenticated || Email is null) return null;

        _cachedUser = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == Email);
        return _cachedUser;
    }

    public async Task<bool> IsAdminAsync()
    {
        var user = await GetUserAsync();
        return user?.IsAdmin == true;
    }
}
