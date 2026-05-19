using System.Security.Claims;
using ComicBooks.Application;
using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Infrastructure;
using ComicBooks.Infrastructure.Data;
using ComicBooks.Web.Auth;
using ComicBooks.Web.Components;
using ComicBooks.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Asosiy xizmatlar ───────────────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// ── Blazor ────────────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(o => o.MaximumReceiveMessageSize = 200 * 1024 * 1024);

// ── Custom xizmatlar ──────────────────────────────────────────────────────
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<BookmarkService>();

// ── Autentifikatsiya ──────────────────────────────────────────────────────
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme          = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath        = "/login";
    options.LogoutPath       = "/auth/logout";
    options.ExpireTimeSpan   = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly  = true;
    options.Cookie.SameSite  = SameSiteMode.Lax;
    options.Cookie.Name      = "mdunyo_auth";
})
.AddGoogle(options =>
{
    options.ClientId     = builder.Configuration["Google:ClientId"]     ?? "";
    options.ClientSecret = builder.Configuration["Google:ClientSecret"] ?? "";
    options.CallbackPath = "/signin-google";
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.Events.OnCreatingTicket = async ctx =>
    {
        // Google dan kelgan ma'lumotlar
        var googleId = ctx.Principal!.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        var email    = ctx.Principal.FindFirst(ClaimTypes.Email)?.Value            ?? "";
        var name     = ctx.Principal.FindFirst(ClaimTypes.Name)?.Value             ?? "";
        var picture  = ctx.User.TryGetProperty("picture", out var picProp)
                           ? picProp.GetString()
                           : null;

        // DB dan foydalanuvchini topish yoki yaratish
        var authService = ctx.HttpContext.RequestServices
                              .GetRequiredService<IAuthService>();
        var user = await authService.FindOrCreateUserAsync(googleId, email, name, picture);

        // Maxsus claim'lar qo'shish
        var identity = (ClaimsIdentity)ctx.Principal.Identity!;
        identity.AddClaim(new Claim("app_user_id",      user.Id.ToString()));
        identity.AddClaim(new Claim("app_is_admin",     user.IsAdmin.ToString().ToLower()));
        identity.AddClaim(new Claim("app_coin_balance", user.CoinBalance.ToString()));
        identity.AddClaim(new Claim("app_picture",      user.Picture ?? ""));
        identity.AddClaim(new Claim("app_name",         user.Name));
    };
});

builder.Services.AddAuthorization();

// ── Blazor uchun AuthState ─────────────────────────────────────────────────
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingAuthStateProvider>();
builder.Services.AddCascadingAuthenticationState();

// ──────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── DB migratsiya va seed ──────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var log = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await ctx.Database.MigrateAsync();
    await ApplicationDbContextSeed.SeedAsync(ctx, log);
}

// ── Middleware ────────────────────────────────────────────────────────────
// Nginx reverse proxy orqali HTTPS schemani to'g'ri aniqlash
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
};
forwardedOptions.KnownNetworks.Clear();
forwardedOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ── Persistent uploads serving (/uploads/* → external path) ────────────────
var uploadsPath = builder.Configuration["Uploads:Path"];
if (string.IsNullOrWhiteSpace(uploadsPath))
{
    uploadsPath = app.Environment.IsDevelopment()
        ? Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads")
        : "/var/www/comicbooks/uploads";
}
Directory.CreateDirectory(uploadsPath);
Directory.CreateDirectory(Path.Combine(uploadsPath, "covers"));
Directory.CreateDirectory(Path.Combine(uploadsPath, "banners"));
Directory.CreateDirectory(Path.Combine(uploadsPath, "chapters"));

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath  = "/uploads"
});

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();

// ── Auth endpointlar ──────────────────────────────────────────────────────
// Google bilan kirish
app.MapGet("/auth/google", () =>
    Results.Challenge(
        new AuthenticationProperties { RedirectUri = "/" },
        [GoogleDefaults.AuthenticationScheme]));

// Email + parol bilan ro'yxatdan o'tish
var authGroup = app.MapGroup("/auth").DisableAntiforgery();

authGroup.MapPost("/register", async (HttpContext ctx, IAuthService auth) =>
{
    var form  = await ctx.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var pass  = form["password"].ToString();
    var name  = form["name"].ToString();

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass))
        return Results.Redirect("/login?err=missing");
    if (pass.Length < 6) return Results.Redirect("/login?err=short");

    var user = await auth.RegisterWithPasswordAsync(email, pass, name ?? "");
    if (user is null) return Results.Redirect("/login?err=exists");

    await SignInCookie(ctx, user);
    return Results.Redirect("/");
});

// Email + parol bilan kirish
authGroup.MapPost("/login", async (HttpContext ctx, IAuthService auth) =>
{
    var form  = await ctx.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var pass  = form["password"].ToString();

    var user = await auth.LoginWithPasswordAsync(email, pass);
    if (user is null) return Results.Redirect("/login?err=invalid");

    await SignInCookie(ctx, user);
    return Results.Redirect("/");
});

// ── Image upload (admin only) ──────────────────────────────────────────────
var uploadGroup = app.MapGroup("/api/upload").DisableAntiforgery();
uploadGroup.MapPost("/{kind}", async (string kind, HttpContext ctx) =>
{
    if (!(ctx.User.Identity?.IsAuthenticated ?? false) ||
        ctx.User.FindFirst("app_is_admin")?.Value != "true")
        return Results.Forbid();

    var sub = kind switch { "cover" => "covers", "banner" => "banners", "chapter" => "chapters", _ => null };
    if (sub is null) return Results.BadRequest(new { error = "kind: cover|banner|chapter" });

    if (!ctx.Request.HasFormContentType) return Results.BadRequest(new { error = "multipart kerak" });
    var form = await ctx.Request.ReadFormAsync();
    var file = form.Files.GetFile("file");
    if (file is null || file.Length == 0) return Results.BadRequest(new { error = "fayl yo'q" });

    // 20 MB chegara
    if (file.Length > 20 * 1024 * 1024) return Results.BadRequest(new { error = "20MB dan katta" });

    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
    var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    if (!allowed.Contains(ext)) return Results.BadRequest(new { error = "Faqat jpg/png/webp/gif" });

    var name = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{ext}";
    var fullPath = Path.Combine(uploadsPath, sub, name);
    using (var fs = new FileStream(fullPath, FileMode.Create))
    {
        await file.CopyToAsync(fs);
    }

    return Results.Json(new { url = $"/uploads/{sub}/{name}" });
});

// ── Admin master login (hard-coded credentials, 1 kun) ─────────────────────
// SUPER ADMIN: hammasiga ruxsat (daromad, coin paketlari, foydalanuvchilar, ...)
//   Login: mdadmin
//   Parol: MdunyoAdm@2026!Sec
//
// REGULAR ADMIN: faqat kitob/bob qo'sha oladi
//   Login: mduploader
//   Parol: MdunyoUpl@2026!Pub
const string SUPER_USERNAME    = "mdadmin";
const string SUPER_PASSWORD    = "MdunyoAdm@2026!Sec";
const string REGULAR_USERNAME  = "mduploader";
const string REGULAR_PASSWORD  = "MdunyoUpl@2026!Pub";

var SUPER_USER_ID   = Guid.Parse("00000000-0000-0000-0000-000000000001");
var REGULAR_USER_ID = Guid.Parse("00000000-0000-0000-0000-000000000002");

authGroup.MapPost("/admin-login", async (HttpContext ctx) =>
{
    var form = await ctx.Request.ReadFormAsync();
    var u = form["username"].ToString();
    var p = form["password"].ToString();

    string role;
    Guid   userId;
    string displayName;

    if (u == SUPER_USERNAME && p == SUPER_PASSWORD)
    {
        role        = "super";
        userId      = SUPER_USER_ID;
        displayName = "Super Admin";
    }
    else if (u == REGULAR_USERNAME && p == REGULAR_PASSWORD)
    {
        role        = "regular";
        userId      = REGULAR_USER_ID;
        displayName = "Admin";
    }
    else
    {
        return Results.Redirect("/admin-login?err=invalid");
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, userId.ToString()),
        new(ClaimTypes.Name,           displayName),
        new("app_user_id",      userId.ToString()),
        new("app_name",         displayName),
        new("app_picture",      ""),
        new("app_is_admin",     "true"),
        new("app_coin_balance", "0"),
        new("admin_master",     "true"),
        new("admin_role",       role),     // "super" | "regular"
    };
    var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
        new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc   = DateTime.UtcNow.AddDays(1)
        });
    return Results.Redirect("/admin");
});

// Parolni o'zgartirish
authGroup.MapPost("/change-password", async (HttpContext ctx, IAuthService auth) =>
{
    if (!(ctx.User.Identity?.IsAuthenticated ?? false))
        return Results.Redirect("/login");

    var form = await ctx.Request.ReadFormAsync();
    var cur  = form["current"].ToString();
    var nw   = form["new"].ToString();
    if (!Guid.TryParse(ctx.User.FindFirst("app_user_id")?.Value, out var uid))
        return Results.Redirect("/profile?pwd=err");

    var ok = await auth.ChangePasswordAsync(uid, cur, nw);
    return Results.Redirect(ok ? "/profile?pwd=ok" : "/profile?pwd=err");
});

// Chiqish
app.MapGet("/auth/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

// Cookie sign-in helper
static async Task SignInCookie(HttpContext ctx, ComicBooks.Domain.Entities.AppUser user)
{
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email,          user.Email),
        new(ClaimTypes.Name,           user.Name),
        new("app_user_id",      user.Id.ToString()),
        new("app_name",         user.Name),
        new("app_picture",      user.Picture ?? ""),
        new("app_is_admin",     user.IsAdmin.ToString().ToLower()),
        new("app_coin_balance", user.CoinBalance.ToString()),
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
        new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTime.UtcNow.AddDays(30) });
}

// ── Blazor ────────────────────────────────────────────────────────────────
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
