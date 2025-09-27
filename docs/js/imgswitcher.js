document.addEventListener('DOMContentLoaded', () => {
    const bannerArr = ["/images/aboutbanner.webp", "/images/Dream169.webp", "/images/epsi-tile.webp", "/images/gradient-tile.webp"];
    const iconArr = ["/images/right-twix-pfp.webp", "/images/left-twix-pfp.webp", "/images/wink-pfp.webp", "/images/blep-pfp.webp", "/images/laptop-pfp.webp"];

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