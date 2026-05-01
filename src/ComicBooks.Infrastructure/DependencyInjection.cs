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

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (connectionString != null && connectionString.Contains(".db"))
                options.UseSqlite(connectionString);
            else
                options.UseSqlServer(connectionString ?? "Data Source=comicbooks.db");
        });

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<ICoinService, CoinService>();
        services.AddScoped<IChapterPricingService, ChapterPricingService>();

        return services;
    }
}
