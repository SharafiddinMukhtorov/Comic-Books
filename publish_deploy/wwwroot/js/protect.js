(function () {
    'use strict';

    // ── 1. O'ng tugmani bloklash ─────────────────────────────────────
    document.addEventListener('contextmenu', function (e) {
        e.preventDefault();
        return false;
    });

    // ── 2. Rasm drag-and-drop bloklash ──────────────────────────────
    document.addEventListener('dragstart', function (e) {
        if (e.target && (e.target.tagName === 'IMG' || e.target.tagName === 'VIDEO')) {
            e.preventDefault();
            return false;
        }
    });

    // ── 3. Klaviatura himoya ─────────────────────────────────────────
    document.addEventListener('keydown', function (e) {
        var key = e.key ? e.key.toLowerCase() : '';
        var ctrl = e.ctrlKey || e.metaKey;

        // F12 — DevTools
        if (e.key === 'F12') { e.preventDefault(); return false; }

        // Ctrl+S — Saqlash
        // Ctrl+U — Sahifa manbasini ko'rish
        // Ctrl+P — Print
        if (ctrl && (key === 's' || key === 'u' || key === 'p')) {
            e.preventDefault(); return false;
        }

        // Ctrl+Shift+I / Ctrl+Shift+J / Ctrl+Shift+C — DevTools
        if (ctrl && e.shiftKey && (key === 'i' || key === 'j' || key === 'c')) {
            e.preventDefault(); return false;
        }
    });

    // ── 4. Tanlab ko'chirish (Copy) bloklash rasm elementlarida ─────
    document.addEventListener('copy', function (e) {
        var sel = window.getSelection();
        if (!sel || sel.toString().length === 0) return;
        // Faqat rasm yoki maxfiy content bo'lsa bloklash
        var node = sel.anchorNode && sel.anchorNode.parentElement;
        if (node && node.closest('.lr-wrap, .hero-cover-wrap, .trending-card-img-wrap, .dr-cover-wrap')) {
            e.preventDefault();
        }
    });

    // ── 5. DevTools ochilishini aniqlash (print bloklash) ────────────
    // Bu oddiy usul — devtools ochilsa sahifani tozalaydi (ixtiyoriy)
    // Hozircha faqat print bloklash yetarli

})();
