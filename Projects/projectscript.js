import { allComponents, baseLayerLuminance, StandardLuminance, provideFluentDesignSystem, } 
from 'https://unpkg.com/@fluentui/web-components';
/**
 * Register with the Fluent Design System.
 */
export const FluentDesignSystem = provideFluentDesignSystem().register(allComponents);
baseLayerLuminance.setValueFor(document.body, StandardLuminance.DarkMode);
const twit = document.getElementById('MButton');
twit.addEventListener('click', () => openTwitter());
const git = document.getElementById('GitButton');
git.addEventListener('click', () => openGithub());
const openTwitter = () => {
    window.open("https://www.microsoft.com/store/apps/9NJRHG6FVKL9");
};
const openGithub = () => {
    window.open("https://github.com/EpsiRho/Fluff");
};
