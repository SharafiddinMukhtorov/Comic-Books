using ComicBooks.Application;
using ComicBooks.Infrastructure;
using ComicBooks.Infrastructure.Data;
using ComicBooks.Web.Components;
using ComicBooks.Web.Services;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Clean Architecture layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// MudBlazor
builder.Services.AddMudServices();

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Theme service for dark/light mode
builder.Services.AddScoped<ThemeService>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await ApplicationDbContextSeed.SeedAsync(context, logger);
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
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
