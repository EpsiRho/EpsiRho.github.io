const musicGrid = document.getElementById('gallery-grid');

// Helper function to generate stars HTML
const getStarsHTML = (rating) => {
    const stars = [];
    for (let i = 1; i <= 5; i++) {
        if (i <= Math.floor(rating)) {
            stars.push('<span class="star star-full">&#9733;</span>');
        } else if (i - rating < 1) {
            stars.push('<span class="star star-half">&#9733;</span>');
        } else {
            stars.push('<span class="star star-empty">&#9733;</span>');
        }
    }
    return stars.join('');
};

function shuffle(array) {
  let currentIndex = array.length;

  // While there remain elements to shuffle...
  while (currentIndex != 0) {

    // Pick a remaining element...
    let randomIndex = Math.floor(Math.random() * currentIndex);
    currentIndex--;

    // And swap it with the current element.
    [array[currentIndex], array[randomIndex]] = [
      array[randomIndex], array[currentIndex]];
  }
}

// img defs
const bannerArr = ["/images/aboutbanner.png", "/images/epsi-tile.png", "/images/gradient-tile.png"];
const iconArr = ["/images/right-twix-pfp.jpg", "/images/left-twix-pfp.png", "/images/wink-pfp.png", "/images/blep-pfp.png"];
const extraArr = [ "/images/refsheet.png", "/images/heyo.png", "/images/what.png", "/images/Suprize.png", "/images/EpsiMilkshake.png",
                   "/images/EpsiDream.png", "/images/firstcomm.png",
];

let combinedArr = bannerArr.concat(iconArr, extraArr);

shuffle(combinedArr);

const renderGrid = async () => {
    musicGrid.innerHTML = '';
    
    // Render music cards
    combinedArr.forEach(item => {
        const card = document.createElement('div');
        // pick random number 1-4 to choose rotation
        const rotation = Math.floor(Math.random() * 4) + 1;
        const speed = Math.random() * 6 + 2;
        const rotationDeg = rotation * 90;
        let rotationAni = 'gradient-h'
        if (rotationDeg == 90 ) rotationAni = 'gradient-h';
        else if (rotationDeg == 180 ) rotationAni = 'gradient-v';
        else if (rotationDeg == 270 ) rotationAni = 'gradient-hr';
        else rotationAni = 'gradient-vr';
        card.style = `--deg: ${rotationDeg}deg; --ani: ${rotationAni}; --speed: ${speed}s;`; // Random rotation between 0 and 360 degrees

        
        card.className = 'gallery-card';
        card.innerHTML = `
            <img src="${item}">
        `;
        //card.onclick = () => window.location.href = encodeURI(item.URL);
        musicGrid.appendChild(card);
    });
};

// Initialize with URL parameters
const search = new URLSearchParams(window.location.search).get('s');
if (search) {
    filterItems(search.replace("%23", "#"));
    searchBar.value = search;
} else {
    renderGrid();
}