using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace ComicBooks.Web.Auth;

/// <summary>
/// Blazor Server uchun cookie-based auth state provider.
/// RevalidatingServerAuthenticationStateProvider ishlatadi —
/// HttpContext null bo'lganida ham circuit auth state ni to'g'ri saqlaydi.
/// </summary>
public class RevalidatingAuthStateProvider : RevalidatingServerAuthenticationStateProvider
{
    public RevalidatingAuthStateProvider(ILoggerFactory loggerFactory)
        : base(loggerFactory) { }

    // Har 30 daqiqada session hali amal qilishini tekshiradi
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState,
        CancellationToken cancellationToken)
    {
        var isValid = authenticationState.User.Identity?.IsAuthenticated == true;
        return Task.FromResult(isValid);
    }
}
