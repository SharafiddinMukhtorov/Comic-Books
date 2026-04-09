using ComicBooks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Comic>      Comics       { get; }
    DbSet<Chapter>    Chapters     { get; }
    DbSet<ChapterPage> ChapterPages { get; }
    DbSet<Genre>      Genres       { get; }
    DbSet<Tag>        Tags         { get; }
    DbSet<ComicGenre> ComicGenres  { get; }
    DbSet<ComicTag>   ComicTags    { get; }
    DbSet<AppUser>    Users        { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
