const musicGrid = document.getElementById('music-grid');
const sortOptions = document.getElementById('sort-options');
const searchBar = document.getElementById('search-bar');

const getGradientForEnergy = (energy, rating) => {
    let bright = 0;
    let bright2 = 0;
    if(rating >= 5.0) {
        bright = 1;
        bright2 = 1;
    } else if(rating >= 4.5) {
        bright = 0.3;
        bright2 = 0.5;
    } else if(rating >= 4.0) {
        bright = 0.3;
        bright2 = 0.07;
    } else if(rating >= 3.5) {
        bright = 0.1;
        bright2 = 0.01;
    } else if(rating >= 3.0) {
        bright = 0.008;
    } else if(rating >= 2.5) {
        bright = 0.002;
    } else if(rating >= 2.0) {
        bright = 0.001;
    } else if(rating >= 1.5) {
        bright = 0.0001;
    } else if(rating >= 1.0) {
        bright = 0.0;
    }

    switch(energy){
        case 1:
            return `linear-gradient(0deg, rgba(0, 128, 255, ${bright}) 0%, rgba(1, 46, 90, ${bright2}) 40%, var(--bg-card) 90%)`;
        case 2:
            return `linear-gradient(0deg, rgba(0, 200, 200, ${bright}) 0%, rgba(1, 99, 99, ${bright2}) 40%, var(--bg-card) 90%)`;
        case 3:
            return `linear-gradient(0deg, rgba(100, 255, 100, ${bright}) 0%, rgba(2, 116, 2, ${bright2}) 40%, var(--bg-card) 90%)`;
        case 4:
            return `linear-gradient(0deg, rgba(255, 200, 0, ${bright}) 0%, rgba(97, 77, 3, ${bright2}) 40%, var(--bg-card) 90%)`;
        case 5:
            return `linear-gradient(0deg, rgba(255, 0, 0, ${bright}) 0%, rgba(109, 1, 1, ${bright2}) 40%, var(--bg-card) 90%)`;
        default:
            return `linear-gradient(0deg, rgba(128, 128, 128, ${bright}) 0%, rgba(64, 64, 64, ${bright2}) 40%, var(--bg-card) 90%)`;
    }
};

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

// Main initialization
fetch("../json/PackedMusicCards.json")
    .then(response => response.json())
    .then(async data => {
        let musicItems = data;
        musicItems.sort((a, b) => {
            return new Date(b.DateAdded || 0) - new Date(a.DateAdded || 0);
        });
        
        let filteredItems = musicItems;

        const renderGrid = async () => {
            musicGrid.innerHTML = '';
            
            // Render music cards
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
                card.onclick = () => window.location.href = encodeURI(item.URL);
                musicGrid.appendChild(card);
            });
        };

        const renderChart = (normalizedData) => {
            const ctx = document.getElementById('MusicStatsChart');
            if (!ctx) return;

            const names = normalizedData.map(obj => obj.name).slice(0, 10);
            const playCounts = normalizedData.map(obj => obj.normalizedValue).slice(0, 10);

            // Add this function to create chart-compatible colors from energy levels
            const getChartColorForEnergy = (energy, rating) => {
                let bright = 0;
                if(rating >= 5.0) bright = 1;
                else if(rating >= 4.5) bright = 0.3;
                else if(rating >= 4.0) bright = 0.3;
                else if(rating >= 3.5) bright = 0.1;
                else if(rating >= 3.0) bright = 0.008;
                else if(rating >= 2.5) bright = 0.002;
                else if(rating >= 2.0) bright = 0.001;
                else if(rating >= 1.5) bright = 0.0001;
                else if(rating >= 1.0) bright = 0.0;

                switch(energy) {
                    case 1:
                        return `rgba(0, 128, 255, ${bright})`;
                    case 2:
                        return `rgba(0, 200, 200, ${bright})`;
                    case 3:
                        return `rgba(100, 255, 100, ${bright})`;
                    case 4:
                        return `rgba(255, 200, 0, ${bright})`;
                    case 5:
                        return `rgba(255, 0, 0, ${bright})`;
                    default:
                        return `rgba(128, 128, 128, ${bright})`;
                }
            };

            // Modify the chart creation part
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: names,
                    datasets: [{
                        label: '# of plays',
                        data: playCounts,
                        borderWidth: 1,
                        backgroundColor: normalizedData.map(album => getChartColorForEnergy(album.Energy, 5.0)).slice(0, 10)
                    }]
                },
                options: {
                    maintainAspectRatio: false,
                    indexAxis: 'y',
                    plugins: {
                        legend: {
                            display: false
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            ticks: {
                                display: true,
                                autoSkip: false,
                                callback: function(value, index) {
                                    const label = this.getLabelForValue(value);
                                    return label.length > 30 ? label.substr(0, 30) + '...' : label;
                                }
                            }
                        },
                        x: {
                            ticks: {
                                display: true,
                                autoSkip: false
                            }
                        }
                    }
                }
            });
        };

        // Event handlers
        const filterItems = (searchTerm) => {
            const lowerCaseTerm = searchTerm.toLowerCase();
            filteredItems = musicItems.filter(item =>
                item.Name.toLowerCase().includes(lowerCaseTerm) ||
                item.Artist.toLowerCase().includes(lowerCaseTerm)
            );
            renderGrid();
        };

        searchBar.addEventListener('input', (e) => {
            filterItems(e.target.value);
        });

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
                if (key === 'DateAdded') {
                    return new Date(b.DateAdded || 0) - new Date(a.DateAdded || 0);
                } else if (key === 'Name' || key === 'Artist') {
                    const primarySort = a[key].localeCompare(b[key]);
                    if (primarySort !== 0) return primarySort;
                } else if (key === 'Rating' || key === 'Energy') {
                    const primarySort = b[key] - a[key];
                    if (primarySort !== 0) return primarySort;
                }
        
                // Secondary sorting: Name (alphabetical)
                const secondarySort = a.Name.localeCompare(b.Name);
                if (secondarySort !== 0) return secondarySort;
        
                // Tertiary sorting: Date Added (most recent first)
                return new Date(b.DateAdded || 0) - new Date(a.DateAdded || 0);
            });
        
            filteredItems = musicItems;
            renderGrid();
        });

        // Initialize with URL parameters
        const search = new URLSearchParams(window.location.search).get('s');
        if (search) {
            filterItems(search.replace("%23", "#"));
            searchBar.value = search;
        } else {
            renderGrid();
        }
    })
    .catch(error => {
        console.error('Error loading music data:', error);
    });