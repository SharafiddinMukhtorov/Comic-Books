using MudBlazor;

namespace ComicBooks.Web.Services;

public class ThemeService
{
    private bool _isDarkMode = true;

    public bool IsDarkMode => _isDarkMode;
    public event Action? OnThemeChanged;

    public MudTheme ComicTheme => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#e94560",
            Secondary = "#0f3460",
            Background = "#f5f5f5",
            Surface = "#ffffff",
            AppbarBackground = "#ffffff",
            AppbarText = "#1a1a2e",
            DrawerBackground = "#ffffff",
            DrawerText = "#1a1a2e",
            TextPrimary = "#1a1a2e",
            TextSecondary = "#555555",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#e94560",
            Secondary = "#0f3460",
            Background = "#0d0d1a",
            Surface = "#1a1a2e",
            AppbarBackground = "#16213e",
            AppbarText = "#e0e0e0",
            DrawerBackground = "#16213e",
            DrawerText = "#e0e0e0",
            TextPrimary = "#e0e0e0",
            TextSecondary = "#a0a0a0",
            ActionDefault = "#e94560",
        }
    };

    public void ToggleTheme()
    {
        _isDarkMode = !_isDarkMode;
        OnThemeChanged?.Invoke();
    }
}
