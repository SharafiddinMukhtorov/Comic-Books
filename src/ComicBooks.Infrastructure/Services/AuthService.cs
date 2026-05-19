using System.Security.Cryptography;
using System.Text;
using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using ComicBooks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;

    public AuthService(ApplicationDbContext db) => _db = db;

    // ── Google ────────────────────────────────────────────
    public async Task<AppUser> FindOrCreateUserAsync(
        string googleId,
        string email,
        string name,
        string? picture,
        CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId, ct);
        if (user is null)
        {
            user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
            if (user is null)
            {
                user = new AppUser
                {
                    GoogleId    = googleId,
                    Email       = email,
                    Name        = name,
                    Picture     = picture,
                    IsAdmin     = false,
                    CoinBalance = 0,
                    LastLogin   = DateTime.UtcNow
                };
                _db.Users.Add(user);
            }
            else
            {
                user.GoogleId = googleId;
                user.Picture  = picture ?? user.Picture;
            }
        }

        user.Name      = name;
        user.LastLogin = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(picture))
            user.Picture = picture;

        await _db.SaveChangesAsync(ct);
        return user;
    }

    // ── Password registration ─────────────────────────────
    public async Task<AppUser?> RegisterWithPasswordAsync(
        string email, string password, string name, CancellationToken ct = default)
    {
        email = email.Trim().ToLowerInvariant();
        var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (existing != null)
        {
            // Email mavjud — Google bilan kirgan bo'lsa, parol qo'shamiz
            if (string.IsNullOrEmpty(existing.PasswordHash))
            {
                existing.PasswordHash = HashPassword(password);
                if (string.IsNullOrEmpty(existing.Name)) existing.Name = name;
                existing.LastLogin = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);
                return existing;
            }
            return null; // email allaqachon ro'yxatdan o'tgan parol bilan
        }

        var user = new AppUser
        {
            Email        = email,
            Name         = string.IsNullOrWhiteSpace(name) ? email.Split('@')[0] : name,
            PasswordHash = HashPassword(password),
            IsAdmin      = false,
            CoinBalance  = 0,
            LastLogin    = DateTime.UtcNow,
            GoogleId     = ""
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return user;
    }

    // ── Password login ────────────────────────────────────
    public async Task<AppUser?> LoginWithPasswordAsync(
        string email, string password, CancellationToken ct = default)
    {
        email = email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user is null || string.IsNullOrEmpty(user.PasswordHash)) return null;

        if (!VerifyPassword(password, user.PasswordHash)) return null;

        user.LastLogin = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return user;
    }

    // ── Change password ───────────────────────────────────
    public async Task<bool> ChangePasswordAsync(
        Guid userId, string? currentPassword, string newPassword, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6) return false;

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null) return false;

        // Agar parol mavjud bo'lsa — eskini tekshirish kerak
        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            if (string.IsNullOrEmpty(currentPassword)) return false;
            if (!VerifyPassword(currentPassword, user.PasswordHash)) return false;
        }

        user.PasswordHash = HashPassword(newPassword);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // ── Helpers ───────────────────────────────────────────
    // PBKDF2 with SHA256, 100k iterations, 16-byte salt
    private const int SaltSize  = 16;
    private const int HashSize  = 32;
    private const int Iters     = 100_000;

    static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iters, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(HashSize);
        return $"v1${Iters}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    static bool VerifyPassword(string password, string stored)
    {
        try
        {
            var parts = stored.Split('$');
            if (parts.Length != 4 || parts[0] != "v1") return false;
            var iters = int.Parse(parts[1]);
            var salt  = Convert.FromBase64String(parts[2]);
            var hash  = Convert.FromBase64String(parts[3]);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iters, HashAlgorithmName.SHA256);
            var check = pbkdf2.GetBytes(hash.Length);
            return CryptographicOperations.FixedTimeEquals(check, hash);
        }
        catch { return false; }
    }
}
