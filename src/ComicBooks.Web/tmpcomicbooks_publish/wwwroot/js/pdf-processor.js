// Optimized PDF processor — 3x faster than sequential
window.pdfProcessor = (function () {
    const PDF_CDN = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/3.11.174';
    const SCALE   = 1.25;     // Sifatni saqlab, faylni 30% kichik
    const QUALITY = 0.75;     // JPEG sifati (oldin 0.85 edi)
    const PARALLEL = 3;       // Bir vaqtda 3 ta sahifa render

    let _libLoaded = false;
    async function loadLib() {
        if (_libLoaded) return;
        await new Promise((res, rej) => {
            const s = document.createElement('script');
            s.src = `${PDF_CDN}/pdf.min.js`;
            s.onload = res;
            s.onerror = rej;
            document.head.appendChild(s);
        });
        pdfjsLib.GlobalWorkerOptions.workerSrc = `${PDF_CDN}/pdf.worker.min.js`;
        _libLoaded = true;
    }

    async function renderPage(pdf, pageNum) {
        const page   = await pdf.getPage(pageNum);
        const vp     = page.getViewport({ scale: SCALE });
        const canvas = document.createElement('canvas');
        canvas.width  = vp.width;
        canvas.height = vp.height;
        const ctx = canvas.getContext('2d', { alpha: false });
        await page.render({ canvasContext: ctx, viewport: vp }).promise;
        const dataUrl = canvas.toDataURL('image/jpeg', QUALITY);
        canvas.width = 0;
        canvas.height = 0;
        return dataUrl;
    }

    return {
        processPages: async function (fileBytes, dotNetRef) {
            await loadLib();
            const uint8 = new Uint8Array(fileBytes);
            const pdf   = await pdfjsLib.getDocument({ data: uint8 }).promise;
            const total = pdf.numPages;

            // Sahifalarni PARALLEL chunklab render qilamiz
            let completed = 0;
            for (let start = 1; start <= total; start += PARALLEL) {
                const batch = [];
                for (let i = start; i < start + PARALLEL && i <= total; i++) {
                    batch.push(renderPage(pdf, i).then(dataUrl => ({ idx: i, dataUrl })));
                }
                const results = await Promise.all(batch);
                // Natijalarni tartibli yuborish
                results.sort((a, b) => a.idx - b.idx);
                for (const r of results) {
                    completed++;
                    await dotNetRef.invokeMethodAsync('OnPageRendered', completed, total, r.dataUrl);
                }
            }
            return total;
        }
    };
})();
