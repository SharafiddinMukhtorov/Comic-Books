namespace ComicBooks.Web.Services;

/// <summary>
/// Scoped per circuit. Dark/Light mode toggle.
/// </summary>
public class ThemeService
{
    private bool _isDark = true;

    public bool IsDark
    {
        get => _isDark;
        set
        {
            if (_isDark == value) return;
            _isDark = value;
            OnChange?.Invoke();
        }
    }

    public event Action? OnChange;

    public void Toggle() => IsDark = !_isDark;
}
