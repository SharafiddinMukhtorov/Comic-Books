using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class AppUser : BaseEntity
{
    public string  Email            { get; set; } = "";
    public string  Name             { get; set; } = "";
    public string? Picture          { get; set; }
    public string  GoogleId         { get; set; } = "";
    public string? PasswordHash     { get; set; }   // null bo'lsa — faqat Google orqali kiradi
    public bool    IsAdmin          { get; set; } = false;
    public DateTime LastLogin       { get; set; } = DateTime.UtcNow;
    public int     CoinBalance      { get; set; } = 0;
    public string? TelegramUsername { get; set; }
}
