class VisualizerPlayer {
    constructor(frequencyVisualizer, cnvs, playBtn, timeline, timeStr, frameCount, currentTime, currentFrameHtml, autoPlay = true) {
        this.frequencyVisualizer = frequencyVisualizer;
        this.canvas = cnvs;
        this.playBtn = playBtn;
        this.timeline = timeline;
        this.timeStr = timeStr;
        this.frameCount = frameCount;
        this.currentTime = currentTime;
        this.currentFrameHtml = currentFrameHtml;
        this.ctx = this.canvas.getContext('2d');
        
        this.currentFrame = 0;
        this.isPlaying = false;
        this.startTime = 0;
        this.pauseTime = 0;
        this.animationId = null;
        
        
        this.setupCanvas();
        this.setupControls();
        this.render();
        this.updateTimeDisplay();

        if (autoPlay) {
            this.isPlaying = true;
            this.startTime = performance.now() - (this.pauseTime || 0);
            
            this.playBtn.textContent = 'Pause';
            this.playBtn.onclick = () => this.pause();
            this.animate();
        } else {
            this.isPlaying = false;
            this.playBtn.textContent = 'Play';
        }
    }

    setupCanvas() {
        this.canvas.style.width = '100%';
        this.canvas.style.height = '200px';
        this.resizeCanvas();
        
        // Handle window resize
        window.addEventListener('resize', () => {
            this.resizeCanvas();
            this.render(); // Re-render with new dimensions
        });
    }

    resizeCanvas() {
        
        // Get the actual container width (or use window width)
        const container = this.canvas.parentElement;
        const containerWidth = document.getElementById("post-page").clientWidth;
        
        // Use container width, but ensure minimum width for 250 bars
        const minWidth = 250; // At least 1px per bar
        const canvasWidth = Math.max(containerWidth, minWidth);
        
        let cheight = canvasWidth > 600 ? 200 : 100;

        // Set actual canvas size
        this.canvas.width = canvasWidth;
        this.canvas.height = cheight;
        
        this.canvas.width = canvasWidth;
        this.canvas.style.width = canvasWidth + 'px';
        this.canvas.style.height = cheight + 'px';
        
        // Store the logical canvas size for rendering
        this.canvasWidth = canvasWidth;
        this.canvasHeight = cheight;
    }

    setupControls() {
        // Update UI elements
        this.frameCount.textContent = this.frequencyVisualizer.header.totalFrames;
        const duration = this.frequencyVisualizer.header.totalFrames / this.frequencyVisualizer.header.frameRate;
        this.timeStr.textContent = this.formatTime(duration);
        this.timeline.max = this.frequencyVisualizer.header.totalFrames - 1;
        this.timeline.disabled = false;

        // Play button
        this.playBtn.onclick = () => this.play();
        
        // Timeline slider input
        this.timeline.oninput = (e) => {
            this.currentFrame = parseInt(e.target.value);
            
            // Update pause time if not playing
            if (!this.isPlaying) {
                const currentTime = this.currentFrame / this.frequencyVisualizer.header.frameRate;
                this.pauseTime = currentTime * 1000;
            }
            
            this.render();
            this.updateTimeDisplay();
        };

        // Timeline slider - make it draggable and responsive
        let wasPaused = false;
        
        // Start dragging - pause if playing
        this.timeline.addEventListener('mousedown', () => {
            wasPaused = !this.isPlaying;
            if (this.isPlaying) {
                this.pause();
            }
        });
        this.timeline.addEventListener('mouseup', () => {
            wasPaused = !this.isPlaying;
            if (!this.isPaused) {
                this.play();
            }
        });
        this.timeline.addEventListener('touchstart', () => {
            wasPaused = !this.isPlaying;
            if (this.isPlaying) {
                this.pause();
            }
        });
        this.timeline.addEventListener('touchsend', () => {
            wasPaused = !this.isPlaying;
            if (!this.isPaused) {
                this.play();
            }
        });
    }

    play() {
        if (this.isPlaying) return;
        
        this.isPlaying = true;
        this.startTime = performance.now() - (this.pauseTime || 0);
        
        this.playBtn.textContent = 'Pause';
        this.playBtn.onclick = () => this.pause();
        
        this.animate();
    }

    pause() {
        this.isPlaying = false;
        this.pauseTime = performance.now() - this.startTime;
        
        this.playBtn.textContent = 'Play';
        this.playBtn.onclick = () => this.play();
        if (this.animationId) {
            cancelAnimationFrame(this.animationId);
        }
    }

    animate() {
        if (!this.isPlaying) return;

        // Simple timer calculation
        const elapsed = performance.now() - this.startTime;
        const currentTime = elapsed / 1000; // seconds
        this.currentFrame = Math.floor(currentTime * this.frequencyVisualizer.header.frameRate);
        
        // Loop at end
        if (this.currentFrame >= this.frequencyVisualizer.header.totalFrames) {
            this.currentFrame = 0;
        
            this.isPlaying = false;
            this.pauseTime = 0;

            this.isPlaying = true;
            this.startTime = performance.now() - (this.pauseTime || 0);
            
        }
        
        this.render();
        this.updateTimeDisplay();
        
        // Update controls
        if (this.playBtn) {
            this.timeline.value = this.currentFrame;
        }
        
        this.animationId = requestAnimationFrame(() => this.animate());
    }

    render() {
        if (this.currentFrame >= this.frequencyVisualizer.header.totalFrames) {
            return;
        }

        const frame = this.frequencyVisualizer.frames[this.currentFrame];
        if (!frame) {
            console.warn('No frame data for frame:', this.currentFrame);
            return;
        }

        const ctx = this.ctx;
        
        // Use stored dimensions (responsive to container width)
        const canvasWidth = this.canvasWidth;
        const canvasHeight = this.canvasHeight;
        const centerY = canvasHeight / 2;
        
        // Clear canvas with dark background
        ctx.fillStyle = getComputedStyle(document.documentElement).getPropertyValue('--bg-card').trim();
        ctx.fillRect(0, 0, canvasWidth, canvasHeight);
        
        // Calculate bar dimensions based on current canvas width
        const numBands = frame.length;
        const barWidth = this.canvasWidth / numBands;  // Use the stored canvas width
        const maxBarHeight = (canvasHeight / 2);
        
        // Draw frequency bars
        for (let i = 0; i < numBands; i++) {
            let amplitude = frame[i];
            
            // Calculate bar height
            const barHeight = amplitude * maxBarHeight;
            const x = i * barWidth;

            const colorA = getComputedStyle(document.documentElement).getPropertyValue('--link-primary').trim();
            const colorB = getComputedStyle(document.documentElement).getPropertyValue('--link-hover').trim();
            let clr = this.lerpColor(colorA, colorB, amplitude);
            
            ctx.fillStyle = clr;
            
            // Draw bar extending up and down from center
            const barTop = centerY - barHeight;
            const barBottom = centerY + barHeight;
            const actualBarWidth = Math.max(1, barWidth + 1);
            
            ctx.fillRect(x, barTop, actualBarWidth, barBottom - barTop);
        }
    }

    lerpColor(colorA, colorB, t) {
        // Clamp t between 0 and 1
        t = Math.max(0, Math.min(1, t));

        // Convert hex to RGB
        const hexToRgb = hex => {
            let c = hex.replace('#', '');
            if (c.length === 3) c = c[0]+c[0]+c[1]+c[1]+c[2]+c[2];
            const num = parseInt(c, 16);
            return [(num >> 16) & 255, (num >> 8) & 255, num & 255];
        };

        // Convert RGB to hex
        const rgbToHex = ([r, g, b]) =>
            '#' +
            [r, g, b]
            .map(x => x.toString(16).padStart(2, '0'))
            .join('');

        const rgbA = hexToRgb(colorA);
        const rgbB = hexToRgb(colorB);

        // Lerp each component
        const rgbResult = rgbA.map((a, i) => Math.round(a + (rgbB[i] - a) * t));

        return rgbToHex(rgbResult);
    }

    updateTimeDisplay() {
        const currentTime = this.currentFrame / this.frequencyVisualizer.header.frameRate;
        if (this.currentTime) {
            this.currentTime.textContent = this.formatTime(currentTime);
        }
        if (this.currentFrameHtml) {
            this.currentFrameHtml.textContent = this.currentFrame;
        }
    }

    formatTime(seconds) {
        const mins = Math.floor(seconds / 60);
        const secs = Math.floor(seconds % 60);
        return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
    }
}

const demoDefinitions = [
    {
        src: "/fvz/Scatman John - Scatman's World.fvz",
        containerId: "visualizer",
        canvasId: "visualizerCanvas",
        playBtnId: "playBtn",
        timelineId: "timelineSlider",
        totalTimeId: "totalTime",
        totalFramesId: "totalFrames",
        currentTimeId: "currentTime",
        currentFrameId: "currentFrame"
    },
    {
        src: "/fvz/Caleb Belkin - for her..fvz",
        containerId: "visualizer2",
        canvasId: "visualizerCanvas2",
        playBtnId: "playBtn2",
        timelineId: "timelineSlider2",
        totalTimeId: "totalTime2",
        totalFramesId: "totalFrames2",
        currentTimeId: "currentTime2",
        currentFrameId: "currentFrame2"
    },
    {
        src: "/fvz/What Are You (Wow).fvz",
        containerId: "visualizer3",
        canvasId: "visualizerCanvas3",
        playBtnId: "playBtn3",
        timelineId: "timelineSlider3",
        totalTimeId: "totalTime3",
        totalFramesId: "totalFrames3",
        currentTimeId: "currentTime3",
        currentFrameId: "currentFrame3"
    }
];

const demoInstances = demoDefinitions
    .map(def => ({
        ...def,
        container: document.getElementById(def.containerId),
        canvas: document.getElementById(def.canvasId),
        playBtn: document.getElementById(def.playBtnId),
        timeline: document.getElementById(def.timelineId),
        totalTime: document.getElementById(def.totalTimeId),
        totalFrames: document.getElementById(def.totalFramesId),
        currentTime: document.getElementById(def.currentTimeId),
        currentFrame: document.getElementById(def.currentFrameId),
        player: null
    }));

demoInstances.forEach(setupLazyDemo);

function setupLazyDemo(demo) {
    const requiredElements = [demo.container, demo.canvas, demo.playBtn, demo.timeline, demo.totalTime, demo.totalFrames];
    if (requiredElements.some(el => !el)) {
        return;
    }

    let loadingPromise = null;

    const loadDemo = async (autoPlay = true) => {
        if (demo.player) {
            return demo.player;
        }
        if (loadingPromise) {
            return loadingPromise;
        }

        demo.playBtn.disabled = true;
        demo.playBtn.textContent = 'Loading...';

        loadingPromise = fetch(demo.src)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`Failed to load ${demo.src}: ${response.status} ${response.statusText}`);
                }
                return response.blob();
            })
            .then(blob => handleFile(blob, demo, autoPlay))
            .then(player => {
                demo.player = player;
                demo.playBtn.removeEventListener('click', initialClickHandler);
                demo.playBtn.disabled = false;
                return player;
            })
            .catch(error => {
                console.error('Failed to load FVZ demo', error);
                demo.playBtn.textContent = 'Retry';
                demo.playBtn.disabled = false;
                return Promise.reject(error);
            })
            .finally(() => {
                loadingPromise = null;
            });

        return loadingPromise;
    };

    const initialClickHandler = async (event) => {
        if (demo.player) {
            return;
        }
        event.preventDefault();
        event.stopPropagation();

        try {
            await loadDemo(true);
        } catch (_) {
            // Keep button enabled for manual retry
        }
    };

    demo.playBtn.addEventListener('click', initialClickHandler);

    if ('IntersectionObserver' in window && demo.container) {
        const observer = new IntersectionObserver((entries, obs) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    loadDemo(false)
                        .then(() => obs.unobserve(entry.target))
                        .catch(() => {
                            // Leave observer active for future attempts
                        });
                }
            });
        }, { rootMargin: '200px 0px', threshold: 0.1 });

        observer.observe(demo.container);
    }
}

async function handleFile(file, demo, autoPlay = true) {
    try {
        if (typeof fzstd === 'undefined') {
            throw new Error('Zstd library not loaded. Please ensure fzstd is included.');
        }

        const arrayBuffer = await file.arrayBuffer();
        const { decompress } = fzstd;
        const result = await AudioDecoder.readFile(arrayBuffer, decompress);

        const player = new VisualizerPlayer(
            result,
            demo.canvas,
            demo.playBtn,
            demo.timeline,
            demo.totalTime,
            demo.totalFrames,
            demo.currentTime,
            demo.currentFrame,
            autoPlay
        );

        demo.playBtn.disabled = false;
        return player;
    } catch (error) {
        console.error('Full error object:', error);
        demo.playBtn.disabled = false;
        demo.playBtn.textContent = 'Retry';
        throw error;
    }
}

function setRemoveOnClick(className) {
  const divs = document.querySelectorAll('span.' + className);
  divs.forEach(div => {
    div.onclick = function() {
      this.classList.remove(className);
    };
  });
}

setRemoveOnClick('songNameHidden');
