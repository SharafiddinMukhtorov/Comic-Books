using ComicBooks.Domain.Entities;

namespace ComicBooks.Application.Common.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Google OAuth orqali kelgan foydalanuvchini topadi yoki yaratadi.
    /// </summary>
    Task<AppUser> FindOrCreateUserAsync(
        string googleId,
        string email,
        string name,
        string? picture,
        CancellationToken ct = default);

    /// <summary>
    /// Email + parol bilan ro'yxatdan o'tkazish. Email band bo'lsa null qaytadi.
    /// </summary>
    Task<AppUser?> RegisterWithPasswordAsync(
        string email,
        string password,
        string name,
        CancellationToken ct = default);

    /// <summary>
    /// Email + parol bilan kirish. Topilmasa yoki parol noto'g'ri bo'lsa null.
    /// </summary>
    Task<AppUser?> LoginWithPasswordAsync(
        string email,
        string password,
        CancellationToken ct = default);

    /// <summary>
    /// Parolni o'zgartirish. Hozirgi parol bo'lmasa (Google'dan kirgan bo'lsa) currentPassword null bo'ladi.
    /// </summary>
    Task<bool> ChangePasswordAsync(
        Guid userId,
        string? currentPassword,
        string newPassword,
        CancellationToken ct = default);
}
