using ComicBooks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComicBooks.Infrastructure.Data.Configurations;

public class ComicConfiguration : IEntityTypeConfiguration<Comic>
{
    public void Configure(EntityTypeBuilder<Comic> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Title).IsRequired().HasMaxLength(500);
        builder.Property(c => c.Description).HasMaxLength(5000);
        builder.Property(c => c.Author).HasMaxLength(200);
        builder.Property(c => c.Artist).HasMaxLength(200);
        builder.Property(c => c.Slug).HasMaxLength(600);
        builder.HasIndex(c => c.Slug).IsUnique();
        builder.HasIndex(c => c.Title);
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Title).HasMaxLength(500);
        builder.Property(c => c.Slug).HasMaxLength(200);
        // Slug unique emas — bir xil chapter raqamlari boshqa komiklarda bo'lishi mumkin
        builder.HasIndex(c => new { c.ComicId, c.ChapterNumber }).IsUnique();
        builder.HasOne(c => c.Comic)
            .WithMany(c => c.Chapters)
            .HasForeignKey(c => c.ComicId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}

public class ChapterPageConfiguration : IEntityTypeConfiguration<ChapterPage>
{
    public void Configure(EntityTypeBuilder<ChapterPage> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.ImageUrl).IsRequired().HasMaxLength(2000);
        // PageNumber unique per chapter
        builder.HasIndex(p => new { p.ChapterId, p.PageNumber }).IsUnique();
        builder.HasOne(p => p.Chapter)
            .WithMany(c => c.Pages)
            .HasForeignKey(p => p.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Name).IsRequired().HasMaxLength(100);
        builder.Property(g => g.Slug).HasMaxLength(120);
        builder.HasIndex(g => g.Slug).IsUnique().HasFilter("[Slug] IS NOT NULL");
        builder.HasQueryFilter(g => !g.IsDeleted);
    }
}

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Slug).HasMaxLength(120);
        builder.HasIndex(t => t.Slug).IsUnique().HasFilter("[Slug] IS NOT NULL");
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}

public class ComicGenreConfiguration : IEntityTypeConfiguration<ComicGenre>
{
    public void Configure(EntityTypeBuilder<ComicGenre> builder)
    {
        builder.HasKey(cg => new { cg.ComicId, cg.GenreId });
        builder.HasOne(cg => cg.Comic).WithMany(c => c.ComicGenres).HasForeignKey(cg => cg.ComicId);
        builder.HasOne(cg => cg.Genre).WithMany(g => g.ComicGenres).HasForeignKey(cg => cg.GenreId);
    }
}

public class ComicTagConfiguration : IEntityTypeConfiguration<ComicTag>
{
    public void Configure(EntityTypeBuilder<ComicTag> builder)
    {
        builder.HasKey(ct => new { ct.ComicId, ct.TagId });
        builder.HasOne(ct => ct.Comic).WithMany(c => c.ComicTags).HasForeignKey(ct => ct.ComicId);
        builder.HasOne(ct => ct.Tag).WithMany(t => t.ComicTags).HasForeignKey(ct => ct.TagId);
    }
}

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(300);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(200);
        builder.Property(u => u.GoogleId).HasMaxLength(200);
        builder.HasIndex(u => u.Email).IsUnique();
    }
}

public class CoinTransactionConfiguration : IEntityTypeConfiguration<CoinTransaction>
{
    public void Configure(EntityTypeBuilder<CoinTransaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.TelegramUsername).HasMaxLength(100);
    }
}

public class UserChapterAccessConfiguration : IEntityTypeConfiguration<UserChapterAccess>
{
    public void Configure(EntityTypeBuilder<UserChapterAccess> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => new { a.UserId, a.ChapterId }).IsUnique();
    }
}

public class CoinPackageConfiguration : IEntityTypeConfiguration<CoinPackage>
{
    public void Configure(EntityTypeBuilder<CoinPackage> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Price).HasMaxLength(50);
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
