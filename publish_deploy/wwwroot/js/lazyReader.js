window.lazyReader = (function () {
    var _obs    = null;
    var _loaded = new Map(); // url → url : Blazor re-render'dan keyin tiklash uchun

    function loadImage(wrap) {
        var img = wrap.querySelector('img[data-lazy]');
        if (!img) return;

        var src = img.getAttribute('data-lazy');
        img.removeAttribute('data-lazy');

        // Yuklangan URL ni saqlaymiz
        _loaded.set(src, src);

        img.onload = function () {
            img.classList.add('lr-loaded');
            wrap.classList.add('lr-done');
        };
        img.onerror = function () {
            wrap.classList.add('lr-done');
        };
        img.src = src;
    }

    // Blazor re-render'dan so'ng, allaqachon yuklangan rasmlarni tiklaydi
    function restoreLoaded() {
        document.querySelectorAll('.lr-wrap img[data-lazy]').forEach(function (img) {
            var url = img.getAttribute('data-lazy');
            if (_loaded.has(url)) {
                img.removeAttribute('data-lazy');
                img.src = _loaded.get(url);
                img.classList.add('lr-loaded');
                var wrap = img.closest('.lr-wrap');
                if (wrap) wrap.classList.add('lr-done');
            }
        });
    }

    return {
        init: function () {
            // Avval tiklash — Blazor src/class atributlarini tozalab tashlagan bo'lishi mumkin
            restoreLoaded();

            if (_obs) { _obs.disconnect(); _obs = null; }

            var items = document.querySelectorAll('.lr-wrap:not(.lr-done)');
            if (!items.length) return;

            _obs = new IntersectionObserver(function (entries) {
                entries.forEach(function (e) {
                    if (e.isIntersecting) {
                        loadImage(e.target);
                        _obs.unobserve(e.target);
                    }
                });
            }, {
                rootMargin: '500px 0px',
                threshold: 0
            });

            items.forEach(function (el) { _obs.observe(el); });
        },

        dispose: function () {
            if (_obs) { _obs.disconnect(); _obs = null; }
            _loaded.clear();
        }
    };
})();
