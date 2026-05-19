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
