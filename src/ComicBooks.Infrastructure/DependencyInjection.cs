using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Infrastructure.Data;
using ComicBooks.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ComicBooks.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Blazor Server'da scoped servis butun circuit (brauzer tabi ochiq turgan vaqt)
        // davomida yashaydi. Shu sababli DbContext factory orqali yaratiladi — coin kabi
        // muhim amallar har safar yangi, qisqa umrli context bilan ishlaydi.
        services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            if (connectionString != null && connectionString.Contains(".db"))
                options.UseSqlite(connectionString);
            else
                options.UseSqlServer(connectionString ?? "Data Source=comicbooks.db");
        });

        services.AddScoped<ApplicationDbContext>(provider =>
            provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<ICoinService, CoinService>();
        services.AddScoped<IChapterPricingService, ChapterPricingService>();
        services.AddScoped<ICoinPackageService, CoinPackageService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
