window.pdfProcessor = {
    processPages: async function (fileBytes, dotNetRef) {
        if (!window.pdfjsLib) {
            await new Promise((resolve, reject) => {
                const s = document.createElement('script');
                s.src = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/3.11.174/pdf.min.js';
                s.onload = resolve;
                s.onerror = reject;
                document.head.appendChild(s);
            });
            pdfjsLib.GlobalWorkerOptions.workerSrc =
                'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/3.11.174/pdf.worker.min.js';
        }

        const uint8 = new Uint8Array(fileBytes);
        const pdf   = await pdfjsLib.getDocument({ data: uint8 }).promise;
        const total = pdf.numPages;
        const urls  = [];

        for (let i = 1; i <= total; i++) {
            const page   = await pdf.getPage(i);
            const vp     = page.getViewport({ scale: 1.5 });
            const canvas = document.createElement('canvas');
            canvas.width  = vp.width;
            canvas.height = vp.height;
            const ctx = canvas.getContext('2d');
            await page.render({ canvasContext: ctx, viewport: vp }).promise;
            urls.push(canvas.toDataURL('image/jpeg', 0.88));
            await dotNetRef.invokeMethodAsync('OnPageRendered', i, total);
        }
        return urls;
    }
};
