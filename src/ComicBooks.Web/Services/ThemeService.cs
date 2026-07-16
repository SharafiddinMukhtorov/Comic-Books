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

    /// <summary>localStorage dan o'qilgan qiymatni qo'yish uchun</summary>
    public void SetDark(bool dark)
    {
        _dark = dark;
        OnChange?.Invoke();
    }
}
