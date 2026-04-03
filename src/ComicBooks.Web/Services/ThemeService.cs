namespace ComicBooks.Web.Services;

public class ThemeService
{
    private bool _isDark = true;
    public bool IsDark => _isDark;
    public event Action? OnChange;

    public void Toggle()
    {
        _isDark = !_isDark;
        OnChange?.Invoke();
    }
}
