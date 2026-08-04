using ComicBooks.Domain.Enums;

namespace ComicBooks.Application.Common.Mappings;

public class ComicDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? AlternativeTitles { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? BannerImageUrl { get; set; }
    public ComicStatus Status { get; set; }
    public ComicType Type { get; set; }
    public ContentRating Rating { get; set; }
    public string? Author { get; set; }
    public string? Artist { get; set; }
    public int? ReleaseYear { get; set; }
    public double AverageRating { get; set; }
    public int RatingCount { get; set; } = 0;
    public int ViewCount { get; set; }
    public int BookmarkCount { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPopular { get; set; }
    public bool IsAdminPick { get; set; }   // Admin tavsiyasi (faqat bitta komikda true bo'ladi)
    public string? Slug { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }   // oxirgi bob qo'shilgan sana (yoki yaratilgan sana)
    public int ChapterCount { get; set; }
    public double? LatestChapterNumber { get; set; }
    public bool LatestChapterLocked { get; set; }   // oxirgi bob qulflanganmi (premium)
    public double? SecondLatestChapterNumber { get; set; }   // oxirgidan oldingi bob raqami
    public DateTime? SecondLatestChapterDate { get; set; }   // oxirgidan oldingi bob qo'shilgan sana
    public List<string> Genres { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public LinkedVideoSummaryDto? LinkedVideo { get; set; }   // shu komikning ekranizatsiyasi (film/serial) bo'lsa
}

public class ChapterDto
{
    public Guid Id { get; set; }
    public Guid ComicId { get; set; }
    public string? ComicTitle { get; set; }
    public string? ComicSlug { get; set; }   // ← comic sahifasiga qaytish uchun
    public string? ComicCoverUrl { get; set; }
    public double ChapterNumber { get; set; }
    public string? Title { get; set; }
    public int ViewCount { get; set; }
    public bool IsLocked  { get; set; }
    public int  CoinPrice { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? Slug { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PageCount { get; set; }
}

public class ChapterPageDto
{
    public Guid Id { get; set; }
    public Guid ChapterId { get; set; }
    public int PageNumber { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int? Width { get; set; }
    public int? Height { get; set; }
}

public class GenreDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public int ComicCount { get; set; }
}

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public int ComicCount { get; set; }
}

public class AppUserDto
{
    public Guid   Id               { get; set; }
    public string Name             { get; set; } = "";
    public string Email            { get; set; } = "";
    public string? Picture         { get; set; }
    public int    CoinBalance      { get; set; }
    public string? TelegramUsername { get; set; }
    public bool   IsAdmin          { get; set; }
}

public class CoinTransactionDto
{
    public Guid   UserId           { get; set; }
    public string UserName         { get; set; } = "";
    public int    Amount           { get; set; }
    public string Type             { get; set; } = "";
    public string? Description     { get; set; }
    public string? TelegramUsername { get; set; }
    public DateTime CreatedAt      { get; set; }
    public string?  ComicTitle     { get; set; }   // Sarflangan bo'lsa — qaysi komik
    public double?  ChapterNumber  { get; set; }   // Sarflangan bo'lsa — qaysi bob
}

public class ChapterPriceItemDto
{
    public Guid    Id            { get; set; }
    public double  ChapterNumber { get; set; }
    public string? Title         { get; set; }
    public bool    IsLocked      { get; set; }
    public int     CoinPrice     { get; set; }
}

public class CoinPackageDto
{
    public Guid   Id         { get; set; }
    public string Name       { get; set; } = "";
    public int    CoinAmount { get; set; }
    public int    BonusCoins { get; set; }
    public string Price      { get; set; } = "";
    public bool   IsPopular  { get; set; }
    public int    SortOrder  { get; set; }
}

public class VideoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? OriginalTitle { get; set; }
    public string? Description { get; set; }
    public string? PosterImageUrl { get; set; }
    public string? BannerImageUrl { get; set; }
    public VideoType Type { get; set; }
    public VideoStatus Status { get; set; }
    public ContentRating Rating { get; set; }
    public string? Language { get; set; }
    public string? Country { get; set; }
    public int? ReleaseYear { get; set; }
    public int? DurationMinutes { get; set; }
    public double ImdbRating { get; set; }
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }
    public int ViewCount { get; set; }
    public string? VideoUrl { get; set; }
    public string? VideoUrl480p { get; set; }
    public string? VideoUrl720p { get; set; }
    public string? VideoUrl1080p { get; set; }
    public bool IsFeatured { get; set; }
    public string? Slug { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Genres { get; set; } = new();
    public int SeasonCount { get; set; }
    public int EpisodeCount { get; set; }
    // Faqat bitta video sahifasida (GetVideoBySlug/Id) to'ldiriladi — ro'yxatlarda bo'sh qoladi
    public List<VideoEpisodeDto> Episodes { get; set; } = new();
    public List<VideoCastMemberDto> CastMembers { get; set; } = new();
    public LinkedComicSummaryDto? LinkedComic { get; set; }
}

public class VideoEpisodeDto
{
    public Guid Id { get; set; }
    public Guid VideoId { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string? Title { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public string? VideoUrl480p { get; set; }
    public string? VideoUrl720p { get; set; }
    public string? VideoUrl1080p { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int? DurationMinutes { get; set; }
    public int ViewCount { get; set; }
}

public class VideoCastMemberDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public CastRole Role { get; set; }
    public int SortOrder { get; set; }
}

public class LinkedComicSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string? Slug { get; set; }
}

public class LinkedVideoSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PosterImageUrl { get; set; }
    public string? Slug { get; set; }
    public VideoType Type { get; set; }
}
