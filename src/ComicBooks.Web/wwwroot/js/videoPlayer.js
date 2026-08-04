window.videoPlayer = (function () {
    var _players = {};

    function fmt(sec) {
        if (!isFinite(sec) || sec < 0) sec = 0;
        var h = Math.floor(sec / 3600);
        var m = Math.floor((sec % 3600) / 60);
        var s = Math.floor(sec % 60);
        var mm = (h > 0 && m < 10 ? '0' : '') + m;
        var ss = (s < 10 ? '0' : '') + s;
        return h > 0 ? (h + ':' + mm + ':' + ss) : (m + ':' + ss);
    }

    function q(container, sel) { return container.querySelector(sel); }

    return {
        init: function (containerId, src) {
            var root = document.getElementById(containerId);
            if (!root) return;
            if (_players[containerId]) this.destroy(containerId);

            var video      = q(root, 'video');
            var centerPlay = q(root, '[data-vp="centerplay"]');
            var spinner    = q(root, '[data-vp="spinner"]');
            var controls   = q(root, '[data-vp="controls"]');
            var playBtn    = q(root, '[data-vp="play"]');
            var icPlay     = playBtn ? playBtn.querySelector('.vp-ic-play') : null;
            var icPause    = playBtn ? playBtn.querySelector('.vp-ic-pause') : null;
            var seek       = q(root, '[data-vp="seek"]');
            var played     = q(root, '[data-vp="played"]');
            var buffered   = q(root, '[data-vp="buffered"]');
            var volBtn     = q(root, '[data-vp="volume-btn"]');
            var volume     = q(root, '[data-vp="volume"]');
            var timeLbl    = q(root, '[data-vp="time"]');
            var fullBtn    = q(root, '[data-vp="fullscreen"]');
            if (!video) return;

            if (src) video.src = src;

            var hideTimer = null;
            var scrubbing = false;

            function showControls() {
                controls.classList.add('vp-visible');
                root.classList.remove('vp-hide-cursor');
                clearTimeout(hideTimer);
                if (!video.paused && !scrubbing) {
                    hideTimer = setTimeout(function () {
                        controls.classList.remove('vp-visible');
                        root.classList.add('vp-hide-cursor');
                    }, 2800);
                }
            }

            function updatePlayIcon() {
                var playing = !video.paused && !video.ended;
                if (icPlay)  icPlay.style.display  = playing ? 'none' : '';
                if (icPause) icPause.style.display = playing ? '' : 'none';
                centerPlay.style.display = playing ? 'none' : 'flex';
            }

            function updateProgress() {
                if (!video.duration) return;
                var pct = (video.currentTime / video.duration) * 100;
                if (played) played.style.width = pct + '%';
                if (seek && !scrubbing) seek.value = (video.currentTime / video.duration) * 1000;
                if (timeLbl) timeLbl.textContent = fmt(video.currentTime) + ' / ' + fmt(video.duration);

                if (buffered && video.buffered.length > 0) {
                    var end = video.buffered.end(video.buffered.length - 1);
                    buffered.style.width = (end / video.duration * 100) + '%';
                }
            }

            var onPlay = function () { updatePlayIcon(); showControls(); };
            var onPause = function () { updatePlayIcon(); controls.classList.add('vp-visible'); clearTimeout(hideTimer); };
            var onWaiting = function () { spinner.classList.add('vp-visible'); };
            var onPlaying = function () { spinner.classList.remove('vp-visible'); };
            var onCanPlay = function () { spinner.classList.remove('vp-visible'); };

            video.addEventListener('timeupdate', updateProgress);
            video.addEventListener('loadedmetadata', updateProgress);
            video.addEventListener('progress', updateProgress);
            video.addEventListener('play', onPlay);
            video.addEventListener('pause', onPause);
            video.addEventListener('ended', onPause);
            video.addEventListener('waiting', onWaiting);
            video.addEventListener('playing', onPlaying);
            video.addEventListener('canplay', onCanPlay);

            function togglePlay() { video.paused ? video.play() : video.pause(); }

            if (centerPlay) centerPlay.onclick = togglePlay;
            if (playBtn) playBtn.onclick = togglePlay;
            video.onclick = togglePlay;

            if (seek) {
                seek.oninput = function () {
                    scrubbing = true;
                    if (video.duration) {
                        var t = (seek.value / 1000) * video.duration;
                        if (played) played.style.width = (seek.value / 10) + '%';
                        if (timeLbl) timeLbl.textContent = fmt(t) + ' / ' + fmt(video.duration);
                    }
                };
                seek.onchange = function () {
                    if (video.duration) video.currentTime = (seek.value / 1000) * video.duration;
                    scrubbing = false;
                    showControls();
                };
            }

            if (volume) volume.oninput = function () {
                video.volume = volume.value / 100;
                video.muted = video.volume === 0;
            };
            if (volBtn) volBtn.onclick = function () {
                video.muted = !video.muted;
                if (volume) volume.value = video.muted ? 0 : (video.volume * 100 || 100);
            };
            if (fullBtn) fullBtn.onclick = function () {
                if (root.requestFullscreen) root.requestFullscreen();
                else if (video.requestFullscreen) video.requestFullscreen();
            };

            root.addEventListener('mousemove', showControls);
            root.addEventListener('touchstart', showControls, { passive: true });
            controls.addEventListener('mousemove', function (e) { e.stopPropagation(); showControls(); });

            updatePlayIcon();
            showControls();

            _players[containerId] = {
                video: video,
                listeners: [
                    ['timeupdate', updateProgress], ['loadedmetadata', updateProgress], ['progress', updateProgress],
                    ['play', onPlay], ['pause', onPause], ['ended', onPause],
                    ['waiting', onWaiting], ['playing', onPlaying], ['canplay', onCanPlay]
                ],
                hideTimer: function () { return hideTimer; }
            };
        },

        play: function (containerId) {
            var p = _players[containerId];
            if (p) p.video.play();
        },

        setSrc: function (containerId, src) {
            var p = _players[containerId];
            if (!p) return;
            var wasPlaying = !p.video.paused;
            p.video.src = src;
            p.video.load();
            if (wasPlaying) p.video.play();
        },

        // Sifat almashtirish: joriy pozitsiya va ijro holatini saqlab, manbani almashtiradi
        switchQuality: function (containerId, src) {
            var p = _players[containerId];
            if (!p || !src) return;
            var video = p.video;
            var wasPlaying = !video.paused;
            var savedTime = video.currentTime;

            function onReady() {
                video.removeEventListener('loadedmetadata', onReady);
                video.currentTime = savedTime;
                if (wasPlaying) video.play();
            }
            video.addEventListener('loadedmetadata', onReady);

            video.src = src;
            video.load();
        },

        destroy: function (containerId) {
            var p = _players[containerId];
            if (!p) return;
            p.listeners.forEach(function (pair) { p.video.removeEventListener(pair[0], pair[1]); });
            try { p.video.pause(); } catch (e) {}
            delete _players[containerId];
        }
    };
})();
