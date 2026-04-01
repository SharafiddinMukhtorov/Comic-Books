namespace ComicBooks.Web.Services;

public class BookmarkService
{
    private readonly HashSet<Guid> _bookmarks = new();
    public event Action? OnChanged;

    public bool IsBookmarked(Guid comicId) => _bookmarks.Contains(comicId);

    public void Toggle(Guid comicId)
    {
        if (_bookmarks.Contains(comicId)) _bookmarks.Remove(comicId);
        else _bookmarks.Add(comicId);
        OnChanged?.Invoke();
    }

    public IReadOnlySet<Guid> GetAll() => _bookmarks;
}
