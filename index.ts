import {
    allComponents,
    baseLayerLuminance,
    StandardLuminance,
    provideFluentDesignSystem,
    Button,
  } from '@fluentui/web-components';
  
  /**
   * Register with the Fluent Design System.
   */
  export const FluentDesignSystem =
    provideFluentDesignSystem().register(allComponents);
  

  baseLayerLuminance.setValueFor( document.body,StandardLuminance.DarkMode );
  
  const twit = document.getElementById('TwitButton') as Button;
  twit.addEventListener('click', () => openTwitter());

  const git = document.getElementById('GitButton') as Button;
  git.addEventListener('click', () => openGithub());

  const dis = document.getElementById('DisButton') as Button;
  dis.addEventListener('click', () => openDiscord());
  
  const openTwitter = () => {
    window.open("https://twitter.com/EpsilonRho");
  };
  
  const openGithub = () => {
    window.open("https://github.com/EpsiRho/");
  };
  

  const openDiscord = () => {
    window.open("https://discordapp.com/users/142661157210161152");
  };
  