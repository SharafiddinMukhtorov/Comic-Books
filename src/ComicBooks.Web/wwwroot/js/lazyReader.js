window.lazyReader = (function () {
    var _obs = null;

    function loadImage(wrap) {
        var img = wrap.querySelector('img[data-lazy]');
        if (!img) return;

        var src = img.getAttribute('data-lazy');
        img.removeAttribute('data-lazy');

        img.onload = function () {
            img.classList.add('lr-loaded');
            wrap.classList.add('lr-done');
        };
        img.onerror = function () {
            wrap.classList.add('lr-done');
        };
        img.src = src;
    }

    return {
        init: function () {
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
                rootMargin: '500px 0px',   // 500px oldin yuklay boshlaydi
                threshold: 0
            });

            items.forEach(function (el) { _obs.observe(el); });
        },

        dispose: function () {
            if (_obs) { _obs.disconnect(); _obs = null; }
        }
    };
})();
