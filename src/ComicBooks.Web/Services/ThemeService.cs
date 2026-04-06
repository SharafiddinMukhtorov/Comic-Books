namespace ComicBooks.Web.Services;

public class ThemeService
{
    private bool _dark = true;
    public bool IsDark => _dark;
    public event Action? OnChange;

    public void Toggle()
    {
        _dark = !_dark;
        OnChange?.Invoke();
    }
}
