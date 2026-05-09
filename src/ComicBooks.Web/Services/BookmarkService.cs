using ComicBooks.Application.Features.Bookmarks.Commands;
using ComicBooks.Application.Features.Bookmarks.Queries;
using MediatR;

namespace ComicBooks.Web.Services;

public class BookmarkService
{
    private readonly IMediator _mediator;
    private readonly SessionService _session;
    private HashSet<Guid> _bookmarks = new();
    private bool _loaded;

    public event Action? OnChanged;

    public BookmarkService(IMediator mediator, SessionService session)
    {
        _mediator = mediator;
        _session  = session;
    }

    public async Task EnsureLoadedAsync()
    {
        if (_loaded) return;
        var sessionId = await _session.GetSessionIdAsync();
        var ids = await _mediator.Send(new GetUserBookmarksQuery(sessionId));
        _bookmarks = ids.ToHashSet();
        _loaded = true;
    }

    public bool IsBookmarked(Guid comicId) => _bookmarks.Contains(comicId);

    public async Task ToggleAsync(Guid comicId)
    {
        var sessionId = await _session.GetSessionIdAsync();
        var isNowBookmarked = await _mediator.Send(new ToggleBookmarkCommand(sessionId, comicId));

        if (isNowBookmarked) _bookmarks.Add(comicId);
        else _bookmarks.Remove(comicId);

        OnChanged?.Invoke();
    }

    public IReadOnlySet<Guid> GetAll() => _bookmarks;
}
