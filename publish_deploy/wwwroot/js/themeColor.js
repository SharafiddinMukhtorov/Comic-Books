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

// Navbar scroll-hide olib tashlandi — navbar doim tepada qotib turadi
