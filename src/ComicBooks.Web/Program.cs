using Microsoft.EntityFrameworkCore;
using ComicBooks.Application;
using ComicBooks.Infrastructure;
using ComicBooks.Infrastructure.Data;
using ComicBooks.Web.Components;
using ComicBooks.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Core services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMudServices();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Scoped Blazor services
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<BookmarkService>();
builder.Services.AddScoped<UserSessionService>();

// Cookie + Google OAuth
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme          = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath  = "/login";
    options.LogoutPath = "/logout";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
})
.AddGoogle(options =>
{
    options.ClientId     = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    options.CallbackPath = "/signin-google";
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.SaveTokens = true;
});

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Seed DB
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var log = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await ApplicationDbContextSeed.SeedAsync(ctx, log);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

// Google OAuth callback — saves user to DB, sets session
app.MapGet("/auth/google", (HttpContext ctx) =>
    Results.Challenge(
        new AuthenticationProperties { RedirectUri = "/auth/callback" },
        new[] { GoogleDefaults.AuthenticationScheme }));

app.MapGet("/auth/callback", async (HttpContext ctx,
    ApplicationDbContext db,
    ILogger<Program> log) =>
{
    var result = await ctx.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    if (!result.Succeeded) return Results.Redirect("/login?error=1");

    var claims     = result.Principal.Claims.ToList();
    var googleId   = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
    var email      = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
    var name       = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value ?? "";
    var picture    = claims.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;

    // Upsert user in DB
    var user = await db.Users
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(u => u.GoogleId == googleId);

    if (user is null)
    {
        user = new ComicBooks.Domain.Entities.AppUser
        {
            GoogleId  = googleId,
            Email     = email,
            Name      = name,
            Picture   = picture,
            LastLogin = DateTime.UtcNow
        };
        db.Users.Add(user);
    }
    else
    {
        user.Email     = email;
        user.Name      = name;
        user.Picture   = picture ?? user.Picture;
        user.LastLogin = DateTime.UtcNow;
        user.IsDeleted = false;
    }
    await db.SaveChangesAsync();

    log.LogInformation("User logged in: {Email}", email);
    return Results.Redirect("/");
});

app.MapGet("/auth/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
