document.addEventListener('DOMContentLoaded', () => {
    const page = document.getElementById('pointer-fishing-page');
    const dialog = document.getElementById('pf-dialog');
    if (!page || !dialog || typeof dialog.showModal !== 'function') return;

    const title = document.getElementById('pf-dialog-title');
    const content = dialog.querySelector('.pf-dialog-content');
    const closeButton = dialog.querySelector('.pf-dialog-close');
    let lastTrigger = null;

    page.querySelectorAll('[data-pf-preview]').forEach(button => {
        button.addEventListener('click', () => {
            const template = document.getElementById(button.dataset.pfPreview);
            if (!template) return;

            lastTrigger = button;
            title.textContent = button.dataset.pfTitle || 'Preview';
            content.replaceChildren(template.content.cloneNode(true));

            const image = content.querySelector(':scope > img');
            if (image) {
                const figure = document.createElement('figure');
                figure.className = 'pf-dialog-media';
                const caption = document.createElement('figcaption');
                const fullSizeLink = document.createElement('a');
                fullSizeLink.href = image.src;
                fullSizeLink.target = '_blank';
                fullSizeLink.rel = 'noopener';
                fullSizeLink.textContent = 'Open full-size image ↗';
                fullSizeLink.setAttribute('aria-label', 'Open full-size image in a new tab');
                image.replaceWith(figure);
                caption.append(fullSizeLink);
                figure.append(image, caption);
            }

            const videos = content.querySelectorAll('video');
            videos.forEach(video => {
                // Set playback properties explicitly; template clones may only retain the attributes.
                video.muted = true;
                video.defaultMuted = true;
                video.playsInline = true;
                video.loop = true;
                video.controls = false;
                video.preload = 'auto';
            });

            dialog.showModal();
            // Start while the opening tap still grants user activation on mobile browsers.
            videos.forEach(video => {
                video.load();
                video.play().catch(error => console.warn('Could not start preview video:', error));
            });
            closeButton.focus();
        });
    });

    closeButton.addEventListener('click', () => dialog.close());
    dialog.addEventListener('click', event => {
        if (event.target === dialog) dialog.close();
    });
    dialog.addEventListener('close', () => {
        content.querySelectorAll('video').forEach(video => video.pause());
        content.replaceChildren();
        lastTrigger?.focus();
        lastTrigger = null;
    });
});
