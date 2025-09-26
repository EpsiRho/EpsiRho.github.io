document.addEventListener('DOMContentLoaded', () => {
    const bannerArr = ["/images/aboutbanner.png", "/images/Dream169.png", "/images/epsi-tile.png", "/images/gradient-tile.png"];
    const iconArr = ["/images/right-twix-pfp.jpg", "/images/left-twix-pfp.png", "/images/wink-pfp.png", "/images/blep-pfp.png", "/images/laptop-pfp.png"];

    // Choose a random banner and icon
    const randomBanner = bannerArr[Math.floor(Math.random() * bannerArr.length)];
    const randomIcon = iconArr[Math.floor(Math.random() * iconArr.length)];

    // Find the profile-img class element and, if on the /site/about page get the post-header-image class element
    const profileImg = document.querySelector('.profile-img');
    const aboutHeaderImg = document.querySelector('.post-header-image');

    console.log("Random Banner Selected: " + randomBanner);
    console.log("Random Icon Selected: " + randomIcon);

    if (profileImg) {
        profileImg.src = randomIcon; // Update to the new image path
    }
    if (aboutHeaderImg && window.location.pathname === '/site/about') {
        aboutHeaderImg.src = randomBanner; // Update to the new image path
    }

});