using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public enum CoinTransactionType
{
    Purchase,   // Admin qo'shdi (Telegram orqali sotib olingan)
    Spend,      // Bob uchun sarflandi
    Refund,     // Qaytarildi
    Bonus       // Sovg'a
}

public class CoinTransaction : BaseEntity
{
    public Guid   UserId      { get; set; }
    public int    Amount      { get; set; }   // + qo'shildi, - yechildi
    public CoinTransactionType Type { get; set; }
    public string? Description { get; set; }
    public Guid?  ChapterId   { get; set; }   // Qaysi bob uchun sarflandi
    public string? TelegramUsername { get; set; } // Telegram orqali kim oldi
}
