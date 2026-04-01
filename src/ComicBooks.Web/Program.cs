using ComicBooks.Application;
using ComicBooks.Infrastructure;
using ComicBooks.Infrastructure.Data;
using ComicBooks.Web.Components;
using ComicBooks.Web.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMudServices();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<BookmarkService>();

var app = builder.Build();

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
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
