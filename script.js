import { allComponents, baseLayerLuminance, StandardLuminance, provideFluentDesignSystem, } 
from 'https://unpkg.com/@fluentui/web-components';
/**
 * Register with the Fluent Design System.
 */
export const FluentDesignSystem = provideFluentDesignSystem().register(allComponents);
baseLayerLuminance.setValueFor(document.body, StandardLuminance.DarkMode);
const twit = document.getElementById('TwitButton');
twit.addEventListener('click', () => openTwitter());
const git = document.getElementById('GitButton');
git.addEventListener('click', () => openGithub());
const openTwitter = () => {
    window.open("https://twitter.com/EpsilonRho");
};
const openGithub = () => {
    window.open("https://github.com/EpsiRho/");
};
