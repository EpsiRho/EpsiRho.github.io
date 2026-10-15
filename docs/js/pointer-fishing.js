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

            dialog.showModal();
            closeButton.focus();
        });
    });

    closeButton.addEventListener('click', () => dialog.close());
    dialog.addEventListener('click', event => {
        if (event.target === dialog) dialog.close();
    });
    dialog.addEventListener('close', () => {
        content.replaceChildren();
        lastTrigger?.focus();
        lastTrigger = null;
    });
});
