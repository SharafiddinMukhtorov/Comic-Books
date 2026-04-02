using MudBlazor;

namespace ComicBooks.Web.Services;

public class ThemeService
{
    private bool _isDarkMode = true;
    public bool IsDarkMode => _isDarkMode;

    // Use Action<bool> so subscribers get new value
    public event Action<bool>? ThemeChanged;

    public MudTheme Theme { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#7c3aed",
            Secondary = "#5b21b6",
            Background = "#f1f0f5",
            Surface = "#ffffff",
            AppbarBackground = "#ffffff",
            AppbarText = "#111827",
            DrawerBackground = "#f9f9fb",
            DrawerText = "#111827",
            TextPrimary = "#111827",
            TextSecondary = "#6b7280",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#7c3aed",
            Secondary = "#5b21b6",
            Background = "#0f0f1a",
            Surface = "#1a1a2e",
            AppbarBackground = "#16213e",
            AppbarText = "#e5e7eb",
            DrawerBackground = "#16213e",
            DrawerText = "#e5e7eb",
            TextPrimary = "#f9fafb",
            TextSecondary = "#9ca3af",
        }
    };

    public void Toggle()
    {
        _isDarkMode = !_isDarkMode;
        ThemeChanged?.Invoke(_isDarkMode);
    }
}
