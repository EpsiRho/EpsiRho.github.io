const musicGrid = document.getElementById('music-grid');
const sortOptions = document.getElementById('sort-options');
const searchBar = document.getElementById('search-bar');

fetch("../json/PackedMusicCards.json")
    .then(response => response.json())
    .then(data => {
        let musicItems = data;
        let filteredItems = musicItems;
        const renderGrid = () => {
            musicGrid.innerHTML = '';
            filteredItems.forEach(item => {
                const card = document.createElement('div');
                card.className = 'music-card';
                card.style.background = getGradientForEnergy(item.Energy, item.Rating);
                card.innerHTML = `
                    <div class="image-container">
                        <img src="${item.thumbnailURL}" alt="${item.Name}">
                    </div>
                    <div class="details">
                        <h3>${item.Name}</h3>
                        <p>${item.Artist}</p>
                        <div class="rating">
                            ${getStarsHTML(item.Rating)}
                        </div>
                    </div>
                `;
                card.onclick = () => window.location.href = item.URL;
                musicGrid.appendChild(card);
            });
        };

        const filterItems = (searchTerm) => {
            const lowerCaseTerm = searchTerm.toLowerCase();
            filteredItems = musicItems.filter(
                item =>
                    item.Name.toLowerCase().includes(lowerCaseTerm) ||
                    item.Artist.toLowerCase().includes(lowerCaseTerm)
            );
            renderGrid();
        };
        
        // Listen for input events on the search bar
        searchBar.addEventListener('input', (e) => {
            filterItems(e.target.value);
        });
        
        // Helper function to generate gradients based on energy
        const getGradientForEnergy = (energy, rating) => {
            let bright = 0;
            let bright2 = 0;
            if(rating >= 5.0)
            {
                bright = 1;
                bright2 = 1;
            }
            else if(rating >= 4.5)
            {
                bright = 0.3;
                bright2 = 0.5;
            }
            else if(rating >= 4.0)
            {
                bright = 0.3;
                bright2 = 0.07;
            }
            else if(rating >= 3.5)
            {
                bright = 0.1;
                bright2 = 0.01;
            }
            else if(rating >= 3.0)
            {
                bright = 0.008;
            }
            else if(rating >= 2.5)
            {
                bright = 0.002;
            }
            else if(rating >= 2.0)
            {
                bright = 0.001;
            }
            else if(rating >= 1.5)
            {
                bright = 0.0001;
            }
            else if(rating >= 1.0)
            {
                bright = 0.0;
            }

            switch(energy){
                case 1:
                    return `linear-gradient(0deg, rgba(0, 128, 255, ${bright}) 0%, rgba(1, 46, 90, ${bright2}) 40%, #2a2a2a 90%)`;
                case 2:
                    return `linear-gradient(0deg, rgba(0, 200, 200, ${bright}) 0%, rgba(1, 99, 99, ${bright2}) 40%, #2a2a2a 90%)`;
                case 3:
                    return `linear-gradient(0deg, rgba(100, 255, 100, ${bright}) 0%, rgba(2, 116, 2, ${bright2}) 40%, #2a2a2a 90%)`;
                case 4:
                    return `linear-gradient(0deg, rgba(255, 200, 0, ${bright}) 0%, rgba(97, 77, 3, ${bright2}) 40%, #2a2a2a 90%)`;
                case 5:
                    return `linear-gradient(0deg, rgba(255, 0, 0, ${bright}) 0%, rgba(109, 1, 1, ${bright2}) 40%, #2a2a2a 90%)`;
            }
        };
        
        // Helper function to generate stars
        const getStarsHTML = (rating) => {
            const stars = [];
            for (let i = 1; i <= 5; i++) {
                if (i <= Math.floor(rating)) {
                    stars.push('<span class="star star-full">&#9733;</span>'); // Full star
                } else if (i - rating < 1) {
                    stars.push('<span class="star star-half">&#9733;</span>'); // Half star
                } else {
                    stars.push('<span class="star star-empty">&#9733;</span>'); // Empty star
                }
            }
            return stars.join('');
        };

        const sortKeyMap = {
            name: 'Name',
            artist: 'Artist',
            rating: 'Rating',
            energy: 'Energy',
            dateAdded: 'DateAdded'
        };
        

        sortOptions.addEventListener('change', (e) => {
            const sortBy = e.target.value;
            const key = sortKeyMap[sortBy];
        
            musicItems.sort((a, b) => {
                // Primary sorting
                if (key === 'Name' || key === 'Artist') {
                    const primarySort = a[key].localeCompare(b[key]);
                    if (primarySort !== 0) return primarySort;
                } else if (key === 'Rating' || key === 'Energy') {
                    const primarySort = b[key] - a[key];
                    if (primarySort !== 0) return primarySort;
                } else if (key === 'DateAdded') {
                    const primarySort = new Date(b.DateAdded) - new Date(a.DateAdded);
                    if (primarySort !== 0) return primarySort;
                }
        
                // Secondary sorting: Name (alphabetical)
                const secondarySort = a.Name.localeCompare(b.Name);
                if (secondarySort !== 0) return secondarySort;
        
                // Tertiary sorting: Date Added (most recent first)
                return new Date(b.DateAdded) - new Date(a.DateAdded);
            });

            filteredItems = musicItems;
            renderGrid();
        });
        
        
        musicItems.sort((a, b) => {
            return new Date(b.DateAdded || 0) - new Date(a.DateAdded || 0);
        });
        renderGrid();
    });
