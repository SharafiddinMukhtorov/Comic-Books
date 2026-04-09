using ComicBooks.Application.Common.Interfaces;
using ComicBooks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Comic>       Comics       => Set<Comic>();
    public DbSet<Chapter>     Chapters     => Set<Chapter>();
    public DbSet<ChapterPage> ChapterPages => Set<ChapterPage>();
    public DbSet<Genre>       Genres       => Set<Genre>();
    public DbSet<Tag>         Tags         => Set<Tag>();
    public DbSet<ComicGenre>  ComicGenres  => Set<ComicGenre>();
    public DbSet<ComicTag>    ComicTags    => Set<ComicTag>();
    public DbSet<AppUser>     Users        => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<AppUser>(b =>
        {
            b.HasKey(u => u.Id);
            b.HasIndex(u => u.GoogleId).IsUnique();
            b.HasIndex(u => u.Email).IsUnique();
            b.HasQueryFilter(u => !u.IsDeleted);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<ComicBooks.Domain.Common.BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
