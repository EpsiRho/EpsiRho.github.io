document.addEventListener('DOMContentLoaded', () => {
    const page = document.getElementById('pointer-fishing-page');
    const dialog = document.getElementById('pf-dialog');
    if (!page || !dialog || typeof dialog.showModal !== 'function') return;

    const title = document.getElementById('pf-dialog-title');
    const content = dialog.querySelector('.pf-dialog-content');
    const closeButton = dialog.querySelector('.pf-dialog-close');
    let lastTrigger = null;
    let borrowedVideo = null;
    const preparedVideos = new WeakSet();

    const shouldPlayVideo = video => !document.hidden &&
        (page.contains(video) || (dialog.open && dialog.contains(video)));

    const playVideo = video => {
        video.play().catch(error => {
            if (error.name !== 'AbortError') console.warn('Could not start preview video:', error);
        });
    };

    const prepareVideo = video => {
        video.muted = true;
        video.defaultMuted = true;
        video.playsInline = true;
        video.autoplay = true;
        video.controls = false;
        video.preload = 'auto';
        // Replay explicitly instead of relying on native WebM looping and seeking on mobile.
        video.loop = false;
        if (preparedVideos.has(video)) return;
        preparedVideos.add(video);
        video.addEventListener('ended', () => {
            video.currentTime = 0;
            if (shouldPlayVideo(video)) playVideo(video);
        });
        const resumeWhenReady = () => {
            if (video.paused && shouldPlayVideo(video)) playVideo(video);
        };
        video.addEventListener('canplay', resumeWhenReady);
        video.addEventListener('seeked', resumeWhenReady);
    };

    page.querySelectorAll('video').forEach(prepareVideo);

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

            const previewVideo = button.querySelector('video');
            const dialogVideo = content.querySelector(':scope > video');
            if (previewVideo && dialogVideo && previewVideo.src === dialogVideo.src) {
                // Reuse the working player instead of opening another media session from a template clone.
                const placeholder = document.createElement('span');
                placeholder.setAttribute('aria-hidden', 'true');
                placeholder.style.display = 'block';
                placeholder.style.height = `${previewVideo.getBoundingClientRect().height}px`;
                borrowedVideo = { video: previewVideo, placeholder, wasPlaying: !previewVideo.paused };
                previewVideo.replaceWith(placeholder);
                dialogVideo.replaceWith(previewVideo);
            }

            const videos = content.querySelectorAll('video');
            videos.forEach(prepareVideo);

            dialog.showModal();
            // Start while the opening tap still grants user activation on mobile browsers.
            videos.forEach(playVideo);
            closeButton.focus();
        });
    });

    closeButton.addEventListener('click', () => dialog.close());
    dialog.addEventListener('click', event => {
        if (event.target === dialog) dialog.close();
    });
    dialog.addEventListener('close', () => {
        if (borrowedVideo) {
            const { video, placeholder, wasPlaying } = borrowedVideo;
            placeholder.replaceWith(video);
            if (wasPlaying) playVideo(video);
            else video.pause();
            borrowedVideo = null;
        }
        content.querySelectorAll('video').forEach(video => video.pause());
        content.replaceChildren();
        lastTrigger?.focus();
        lastTrigger = null;
    });
});
