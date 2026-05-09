using Microsoft.JSInterop;

namespace ComicBooks.Web.Services;

public class SessionService
{
    private readonly IJSRuntime _js;
    private Guid? _cachedId;

    public SessionService(IJSRuntime js) => _js = js;

    public async ValueTask<Guid> GetSessionIdAsync()
    {
        if (_cachedId.HasValue) return _cachedId.Value;

        try
        {
            var stored = await _js.InvokeAsync<string?>("localStorage.getItem", "mdunyo_sid");
            if (stored is not null && Guid.TryParse(stored, out var existing))
            {
                _cachedId = existing;
                return existing;
            }
        }
        catch { }

        var newId = Guid.NewGuid();
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", "mdunyo_sid", newId.ToString());
        }
        catch { }

        _cachedId = newId;
        return newId;
    }
}
