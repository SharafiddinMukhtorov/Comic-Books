window.themeColor = {
    set: function (color) {
        var meta = document.getElementById('theme-color-meta');
        if (!meta) {
            meta = document.createElement('meta');
            meta.name = 'theme-color';
            meta.id = 'theme-color-meta';
            document.head.appendChild(meta);
        }
        meta.setAttribute('content', color);
    }
};

// Theme persistance: localStorage orqali saqlash/o'qish
window.themeStorage = {
    get: function () {
        try { return localStorage.getItem('md-theme') || 'dark'; }
        catch { return 'dark'; }
    },
    set: function (value) {
        try { localStorage.setItem('md-theme', value); }
        catch { }
    }
};

// Scroll-aware navbar hide/show (mobile only, ≤768px)
(function () {
    var lastY = 0;
    var pendingY = 0;
    var rafId = null;

    function step() {
        rafId = null;
        var y = pendingY;
        if (window.innerWidth > 768) { lastY = y; return; }
        var nav = document.querySelector('header.mud-appbar');
        if (!nav) { lastY = y; return; }
        if (y < 60) {
            nav.classList.remove('nav-scroll-hidden');
        } else if (y > lastY + 8) {
            nav.classList.add('nav-scroll-hidden');
        } else if (y < lastY - 8) {
            nav.classList.remove('nav-scroll-hidden');
        }
        lastY = y;
    }

    function schedule(y) {
        pendingY = y;
        if (rafId === null) rafId = requestAnimationFrame(step);
    }

    // 1) Window/body scroll
    window.addEventListener('scroll', function () {
        schedule(window.scrollY);
    }, { passive: true });

    // 2) MudBlazor overflow element scroll (scroll doesn't bubble, use capture)
    document.addEventListener('scroll', function (e) {
        var t = e.target;
        if (t && t !== document && t !== document.body) {
            schedule(t.scrollTop || 0);
        }
    }, { passive: true, capture: true });
})();
