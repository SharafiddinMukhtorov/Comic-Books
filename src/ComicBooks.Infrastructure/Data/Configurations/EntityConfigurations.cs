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
        builder.Property(p => p.ImageUrl).IsRequired().HasMaxLength(1000);
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
        builder.HasIndex(g => g.Slug).IsUnique();
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
        builder.HasIndex(t => t.Slug).IsUnique();
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
