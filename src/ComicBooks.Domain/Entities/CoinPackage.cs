using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class CoinPackage : BaseEntity
{
    public string  Name        { get; set; } = "";   // "Starter", "Popular", "Best Value"
    public int     CoinAmount  { get; set; }          // Nechi coin
    public int     BonusCoins  { get; set; } = 0;     // Bonus coin
    public string  Price       { get; set; } = "";    // "20,000 UZS"
    public bool    IsPopular   { get; set; } = false;
    public int     SortOrder   { get; set; } = 0;
}
