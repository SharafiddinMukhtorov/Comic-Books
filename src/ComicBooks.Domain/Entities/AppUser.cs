using ComicBooks.Domain.Common;

namespace ComicBooks.Domain.Entities;

public class AppUser : BaseEntity
{
    public string Email       { get; set; } = "";
    public string Name        { get; set; } = "";
    public string? Picture    { get; set; }
    public string GoogleId    { get; set; } = "";
    public bool   IsAdmin     { get; set; } = false;
    public DateTime LastLogin { get; set; } = DateTime.UtcNow;
}
