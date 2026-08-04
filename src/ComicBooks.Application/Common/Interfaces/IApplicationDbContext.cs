using ComicBooks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComicBooks.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Comic> Comics { get; }
    DbSet<Chapter> Chapters { get; }
    DbSet<ChapterPage> ChapterPages { get; }
    DbSet<Genre> Genres { get; }
    DbSet<Tag> Tags { get; }
    DbSet<ComicGenre> ComicGenres { get; }
    DbSet<ComicTag> ComicTags { get; }
    DbSet<AppUser> Users { get; }
    DbSet<CoinTransaction> CoinTransactions { get; }
    DbSet<UserChapterAccess> ChapterAccesses { get; }
    DbSet<CoinPackage> CoinPackages { get; }
    DbSet<UserBookmark> UserBookmarks { get; }
    DbSet<ComicView> ComicViews { get; }
    DbSet<ChapterComment> ChapterComments { get; }
    DbSet<Video> Videos { get; }
    DbSet<VideoEpisode> VideoEpisodes { get; }
    DbSet<VideoCastMember> VideoCastMembers { get; }
    DbSet<VideoGenre> VideoGenres { get; }
    DbSet<VideoView> VideoViews { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
