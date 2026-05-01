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
    public int ViewCount { get; set; }
    public int BookmarkCount { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPopular { get; set; }
    public string? Slug { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ChapterCount { get; set; }
    public double? LatestChapterNumber { get; set; }
    public List<string> Genres { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

public class ChapterDto
{
    public Guid Id { get; set; }
    public Guid ComicId { get; set; }
    public string? ComicTitle { get; set; }
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
}

public class ChapterPriceItemDto
{
    public Guid    Id            { get; set; }
    public double  ChapterNumber { get; set; }
    public string? Title         { get; set; }
    public bool    IsLocked      { get; set; }
    public int     CoinPrice     { get; set; }
}
