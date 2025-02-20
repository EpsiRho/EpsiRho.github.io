function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}

fetch('/site/version')
.then(response => {
    if (!response.ok) {
        throw new Error('Network response was not ok');
    }
    return response.text();
})
.then(version => {
    const displayElements = document.getElementsByClassName('version-display');
    console.log(displayElements);
    for (let item of displayElements) {
        item.textContent = version;
    }
})
.catch(error => {
    console.error('There was a problem with the fetch operation:', error);
});

const root = document.documentElement;
const settingsButton = document.getElementById('page-settings');
const settingsModal = document.getElementById('settings-modal');
const settingsBg = document.getElementById('settings-bg');
const closeModal = document.getElementById('close-modal');
const themeLight = document.getElementById('theme-option-light');
const themeDark = document.getElementById('theme-option-dark');
const themeTransparent = document.getElementById('theme-option-transparent');
const themeOcean = document.getElementById('theme-option-ocean');
const themeDream = document.getElementById('theme-option-dream');
const themeFields = document.getElementById('theme-option-fields');
const themeSunset = document.getElementById('theme-option-sunset');
const themeMatrix = document.getElementById('theme-option-matrix');
const themeNotebook = document.getElementById('theme-option-notebook');

function setTheme(theme) {
    console.log(`Using ${theme} theme`)
    if (theme === 'dark') {
        /* 
            --bg-dark: #00000028;
            --bg-sidebar: #47474721;
            --bg-main: #1414143a;
            --bg-card: #2a2a2a;
            --text-primary: #ffffff;
            --text-secondary: #a0a0a0; 
        */

        /* 
            --bg-dark: #1a1a1a;
            --bg-card: #2a2a2a;
            --text-primary: #ffffff;
            --text-secondary: #a0a0a0; 
            --bg-code: #e6e6e6;
        */
        root.style.setProperty('--bg-sidebar', '#2a2a2a');
        root.style.setProperty('--bg-main', '#1a1a1a');
        root.style.setProperty('--bg-card', 'rgb(53, 53, 53)');
        root.style.setProperty('--bg-code', '#282c34');
        root.style.setProperty('--text-primary', '#ffffff');
        root.style.setProperty('--text-secondary', '#a0a0a0');

        root.style.setProperty('--accent', `#3383d2`);
        root.style.setProperty('--accent-secondary', 'rgba(7,195,252,1)');
        root.style.setProperty('--link-primary', '#3383d2');
        root.style.setProperty('--link-hover', 'rgba(7,195,252,1)');
        root.style.setProperty('--link-clicked', 'rgba(191,8,246,1)');

        themeLight.classList.remove('selected');
        themeDark.classList.add('selected');
        themeTransparent.classList.remove('selected');
        themeOcean.classList.remove('selected');
        themeDream.classList.remove('selected');
        themeFields.classList.remove('selected');
        themeSunset.classList.remove('selected');
        themeMatrix.classList.remove('selected');
        themeNotebook.classList.remove('selected');

        changeHighlightTheme('atom-one-dark');
    } else if (theme === 'light')  {
        root.style.setProperty('--bg-sidebar', 'rgb(216, 216, 216)');
        root.style.setProperty('--bg-main', 'rgb(228, 228, 228)');
        root.style.setProperty('--bg-card','rgb(199, 199, 199)');
        root.style.setProperty('--bg-code', '#e6e6e6');
        root.style.setProperty('--text-primary', '#000000');
        root.style.setProperty('--text-secondary', '#1a1a1a');

        root.style.setProperty('--accent', `#3383d2`);
        root.style.setProperty('--accent-secondary', 'rgba(7,195,252,1)');
        root.style.setProperty('--link-primary', '#3383d2');
        root.style.setProperty('--link-hover', 'rgba(7,195,252,1)');
        root.style.setProperty('--link-clicked', 'rgba(191,8,246,1)');

        themeLight.classList.add('selected');
        themeDark.classList.remove('selected');
        themeTransparent.classList.remove('selected');
        themeOcean.classList.remove('selected');
        themeDream.classList.remove('selected');
        themeFields.classList.remove('selected');
        themeSunset.classList.remove('selected');
        themeMatrix.classList.remove('selected');
        themeNotebook.classList.remove('selected');

        changeHighlightTheme('panda-syntax-light');
    } else if (theme === 'ocean')  {
        root.style.setProperty('--bg-sidebar', '#073642');
        root.style.setProperty('--bg-main', '#002b36');
        root.style.setProperty('--bg-card','#083c4a');
        root.style.setProperty('--bg-code', '#002451');
        root.style.setProperty('--text-primary', '#fdf6e3');
        root.style.setProperty('--text-secondary', '#93a1a1');
        
        root.style.setProperty('--accent', `#2eacf8`);
        root.style.setProperty('--accent-secondary', '#268bd2');
        root.style.setProperty('--link-primary', '#2eacf8');
        root.style.setProperty('--link-hover', '#d33682');
        root.style.setProperty('--link-clicked', '#28918c');

        themeLight.classList.remove('selected');
        themeDark.classList.remove('selected');
        themeTransparent.classList.remove('selected');
        themeOcean.classList.add('selected');
        themeDream.classList.remove('selected');
        themeFields.classList.remove('selected');
        themeSunset.classList.remove('selected');
        themeMatrix.classList.remove('selected');
        themeNotebook.classList.remove('selected');

        changeHighlightTheme('tomorrow-night-blue');
    } else if (theme === 'dream')  {
        root.style.setProperty('--bg-sidebar', '#2c2a3e');
        root.style.setProperty('--bg-main', '#2b2336');
        root.style.setProperty('--bg-card','#3b3f51');
        root.style.setProperty('--bg-code', '#2e3440');
        root.style.setProperty('--text-primary', '#f8f8f2');
        root.style.setProperty('--text-secondary', '#bd93f9');
        
        root.style.setProperty('--accent', `#ff79c6`);
        root.style.setProperty('--accent-secondary', '#8be9fd');
        root.style.setProperty('--link-primary', '#ff79c6');
        root.style.setProperty('--link-hover', '#8be9fd');
        root.style.setProperty('--link-clicked', '#441f5e');

        themeLight.classList.remove('selected');
        themeDark.classList.remove('selected');
        themeTransparent.classList.remove('selected');
        themeOcean.classList.remove('selected');
        themeDream.classList.add('selected');
        themeFields.classList.remove('selected');
        themeSunset.classList.remove('selected');
        themeMatrix.classList.remove('selected');
        themeNotebook.classList.remove('selected');

        changeHighlightTheme('nord');
    } else if (theme === 'fields')  {
        root.style.setProperty('--bg-sidebar', '#365142');
        root.style.setProperty('--bg-main', '#283c2e');
        root.style.setProperty('--bg-card','#2d543f');
        root.style.setProperty('--bg-code', '#2e3440');
        root.style.setProperty('--text-primary', '#c0f5dc');
        root.style.setProperty('--text-secondary', '#a5cab8');
        
        root.style.setProperty('--accent', `#2fc980`);
        root.style.setProperty('--accent-secondary', '#009201');
        root.style.setProperty('--link-primary', '#2fc980');
        root.style.setProperty('--link-hover', '#106225');
        root.style.setProperty('--link-clicked', '#2CA269');

        themeLight.classList.remove('selected');
        themeDark.classList.remove('selected');
        themeTransparent.classList.remove('selected');
        themeOcean.classList.remove('selected');
        themeDream.classList.remove('selected');
        themeFields.classList.add('selected');
        themeSunset.classList.remove('selected');
        themeMatrix.classList.remove('selected');
        themeNotebook.classList.remove('selected');

        changeHighlightTheme('nord');
    } else if (theme === 'sunset')  {
        root.style.setProperty('--bg-sidebar', '#4c1919');
        root.style.setProperty('--bg-main', '#381324');
        root.style.setProperty('--bg-card','#403949');
        root.style.setProperty('--bg-code', '#2e3440');
        root.style.setProperty('--text-primary', '#f3d59f');
        root.style.setProperty('--text-secondary', '#fbcba4');
        
        root.style.setProperty('--accent', `#aa74a8`);
        root.style.setProperty('--accent-secondary', '#9f50c2');
        root.style.setProperty('--link-primary', '#c678dd');
        root.style.setProperty('--link-hover', '#ff66b3');
        root.style.setProperty('--link-clicked', '#bf3cff');

        themeLight.classList.remove('selected');
        themeDark.classList.remove('selected');
        themeTransparent.classList.remove('selected');
        themeOcean.classList.remove('selected');
        themeDream.classList.remove('selected');
        themeFields.classList.remove('selected');
        themeSunset.classList.add('selected');
        themeMatrix.classList.remove('selected');
        themeNotebook.classList.remove('selected');

        changeHighlightTheme('nord');
    } else if (theme === 'matrix')  {
        root.style.setProperty('--bg-sidebar', '#000000');
        root.style.setProperty('--bg-main', '#0e0e0e');
        root.style.setProperty('--bg-card','#1a1a1a');
        root.style.setProperty('--bg-code', '#001100');
        root.style.setProperty('--text-primary', '#00ff00');
        root.style.setProperty('--text-secondary', '#00ff00');
        
        root.style.setProperty('--accent', `#00cd86`);
        root.style.setProperty('--accent-secondary', '#18c100');
        root.style.setProperty('--link-primary', '#00cd86');
        root.style.setProperty('--link-hover', '#18c100');
        root.style.setProperty('--link-clicked', '#33752a');

        themeLight.classList.remove('selected');
        themeDark.classList.remove('selected');
        themeTransparent.classList.remove('selected');
        themeOcean.classList.remove('selected');
        themeDream.classList.remove('selected');
        themeFields.classList.remove('selected');
        themeSunset.classList.remove('selected');
        themeMatrix.classList.add('selected');
        themeNotebook.classList.remove('selected');

        changeHighlightTheme('base16-green-screen');
    } else if (theme === 'notebook')  {
        root.style.setProperty('--bg-sidebar', '#f1e8d2');
        root.style.setProperty('--bg-main', '#fff9e8');
        root.style.setProperty('--bg-card','#f7f2e0');
        root.style.setProperty('--bg-code', '#e7e9db');
        root.style.setProperty('--text-primary', '#2f383e');
        root.style.setProperty('--text-secondary', '#1c2125');
        
        root.style.setProperty('--accent', `#ef4645`);
        root.style.setProperty('--accent-secondary', '#efc244');
        root.style.setProperty('--link-primary', '#ef4645');
        root.style.setProperty('--link-hover', '#efc244');
        root.style.setProperty('--link-clicked', '#85ef43');

        themeLight.classList.remove('selected');
        themeDark.classList.remove('selected');
        themeTransparent.classList.remove('selected');
        themeOcean.classList.remove('selected');
        themeDream.classList.remove('selected');
        themeFields.classList.remove('selected');
        themeSunset.classList.remove('selected');
        themeMatrix.classList.remove('selected');
        themeNotebook.classList.add('selected');

        changeHighlightTheme('paraiso-light');
    }
    else { 
        root.style.setProperty('--bg-sidebar', '#47474721');
        root.style.setProperty('--bg-main', '#1414143a');
        root.style.setProperty('--bg-card', '#3a3a3a');
        root.style.setProperty('--bg-code', '#22272e');
        root.style.setProperty('--text-primary', '#ffffff');
        root.style.setProperty('--text-secondary', '#a0a0a0');

        root.style.setProperty('--accent', `#3383d2`);
        root.style.setProperty('--accent-secondary', 'rgba(7,195,252,1)');
        root.style.setProperty('--link-primary', '#3383d2');
        root.style.setProperty('--link-hover', 'rgba(7,195,252,1)');
        root.style.setProperty('--link-clicked', 'rgba(191,8,246,1)');

        themeLight.classList.remove('selected');
        themeDark.classList.remove('selected');
        themeTransparent.classList.add('selected');
        themeOcean.classList.remove('selected');
        themeDream.classList.remove('selected');
        themeFields.classList.remove('selected');
        themeSunset.classList.remove('selected');
        themeMatrix.classList.remove('selected');
        themeNotebook.classList.remove('selected');

        changeHighlightTheme('github-dark-dimmed');
    }

    document.cookie = `theme=${theme}; path=/`; // Store theme in cookie
}

function changeHighlightTheme(theme) {
    let link = document.getElementById('codeTheme');
    
    if (!link) {
        link = document.createElement('link');
        link.rel = 'stylesheet';
        document.head.appendChild(link);
    }

    link.href = `https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.8.0/styles/${theme}.min.css`;
    console.log(link.href);
}


document.addEventListener('DOMContentLoaded', function() {
    const savedTheme = getCookie('theme');
    if (savedTheme) {
        setTheme(savedTheme);
        console.log(savedTheme);
    }
    else{
        console.log("Using prefered theme");
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            setTheme('dark');
          } else {
            setTheme('notebook');
          }
    }

    settingsButton.addEventListener('click', function() {
        settingsModal.style.opacity = 1;
        settingsBg.style.opacity = 1;
        settingsBg.style.pointerEvents = 'all';
        settingsModal.style.pointerEvents = 'all';
        settingsBg.style.backdropFilter = 4
    });

    closeModal.addEventListener('click', function() {
        settingsModal.style.opacity = 0;
        settingsBg.style.opacity = 0;
        settingsBg.style.pointerEvents = 'none';
        settingsModal.style.pointerEvents = 'none';
    });

    window.addEventListener('click', function(event) {
        if (!event.target.classList.contains('settingsbtn') && !event.target.classList.contains('themebtn')) {
            settingsModal.style.opacity = 0;
            settingsBg.style.opacity = 0;
            settingsBg.style.pointerEvents = 'none';
            settingsModal.style.pointerEvents = 'none';
        }
        else{
            settingsModal.style.opacity = 1;
            settingsBg.style.opacity = 1;
            settingsBg.style.pointerEvents = 'all';
            settingsModal.style.pointerEvents = 'all';
            settingsBg.style.backdropFilter = 4
        }
    });

    themeLight.addEventListener('click', function() {
        setTheme('light');
    });
    themeDark.addEventListener('click', function() {
        setTheme('dark');
    });
    themeTransparent.addEventListener('click', function() {
        setTheme('transparent');
    });
    themeOcean.addEventListener('click', function() {
        setTheme('ocean');
    });
    themeDream.addEventListener('click', function() {
        setTheme('dream');
    });
    themeFields.addEventListener('click', function() {
        setTheme('fields');
    });
    themeSunset.addEventListener('click', function() {
        setTheme('sunset');
    });
    themeMatrix.addEventListener('click', function() {
        setTheme('matrix');
    });
    themeNotebook.addEventListener('click', function() {
        setTheme('notebook');
    });
});