(function () {
    var active = null;

    document.addEventListener('pointerdown', function (e) {
        if (e.pointerType !== 'mouse' || e.button !== 0) return;   // faqat sichqoncha, chap tugma
        var el = e.target.closest('.drag-scroll');
        if (!el) return;
        active = { el: el, startX: e.clientX, startScrollLeft: el.scrollLeft, moved: false, pointerId: e.pointerId };
        el.classList.add('drag-scrolling');
        e.preventDefault();   // matn/rasm belgilanishining oldini oladi
    });

    document.addEventListener('pointermove', function (e) {
        if (!active || active.pointerId !== e.pointerId) return;
        var dx = e.clientX - active.startX;
        if (Math.abs(dx) > 3) active.moved = true;
        active.el.scrollLeft = active.startScrollLeft - dx;
    });

    function endDrag(e) {
        if (!active || (e.pointerId !== undefined && active.pointerId !== e.pointerId)) return;
        var el = active.el;
        el.classList.remove('drag-scrolling');
        if (active.moved) {
            // Sudralgandan keyingi tasodifiy click (link)ni bekor qilamiz
            var suppress = function (ce) { ce.preventDefault(); ce.stopPropagation(); el.removeEventListener('click', suppress, true); };
            el.addEventListener('click', suppress, true);
        }
        active = null;
    }
    document.addEventListener('pointerup', endDrag);
    document.addEventListener('pointercancel', endDrag);
    document.addEventListener('pointerleave', function (e) { if (active && e.target === active.el) endDrag(e); }, true);
})();
