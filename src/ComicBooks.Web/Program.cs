using ComicBooks.Application;
using ComicBooks.Infrastructure;
using ComicBooks.Infrastructure.Data;
using ComicBooks.Web.Components;
using ComicBooks.Web.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMudServices();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<BookmarkService>();
// CoinService olib tashlandi — barcha logika MediatR Commands/Queries orqali ishlaydi

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var log = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await ctx.Database.MigrateAsync();

    // Ensure new tables exist (idempotent)
    await ctx.Database.ExecuteSqlRawAsync(@"
        IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserBookmarks')
        BEGIN
            CREATE TABLE UserBookmarks (
                Id          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
                SessionId   UNIQUEIDENTIFIER NOT NULL,
                ComicId     UNIQUEIDENTIFIER NOT NULL,
                CreatedAt   DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                UpdatedAt   DATETIME2 NULL,
                IsDeleted   BIT NOT NULL DEFAULT 0,
                CONSTRAINT FK_UserBookmarks_Comics_ComicId
                    FOREIGN KEY (ComicId) REFERENCES Comics(Id) ON DELETE CASCADE
            );
            CREATE UNIQUE INDEX IX_UserBookmarks_SessionId_ComicId ON UserBookmarks(SessionId, ComicId);
            CREATE INDEX IX_UserBookmarks_ComicId ON UserBookmarks(ComicId);
        END

        IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ComicViews')
        BEGIN
            CREATE TABLE ComicViews (
                Id          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
                SessionId   UNIQUEIDENTIFIER NOT NULL,
                ComicId     UNIQUEIDENTIFIER NOT NULL,
                ViewedAt    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                CreatedAt   DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                UpdatedAt   DATETIME2 NULL,
                IsDeleted   BIT NOT NULL DEFAULT 0
            );
            CREATE INDEX IX_ComicViews_SessionId_ComicId_ViewedAt ON ComicViews(SessionId, ComicId, ViewedAt);
        END
    ");

    await ApplicationDbContextSeed.SeedAsync(ctx, log);
}
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Error", createScopeForErrors: true); app.UseHsts(); }
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
