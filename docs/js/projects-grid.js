const musicGrid = document.getElementById('music-grid');
const sortOptions = document.getElementById('sort-options');
const searchBar = document.getElementById('search-bar');

fetch("../json/PackedProjects.json")
    .then(response => response.json())
    .then(data => {
        let projects = data;
        let filteredItems = projects;
        const renderGrid = () => {
            musicGrid.innerHTML = '';
            filteredItems.forEach(item => {
                const card = document.createElement('div');
                card.className = 'music-card';
                card.style.background = '#252525';
                let fixedUrl = `/site/project?id=${item.PostURL}`;
                card.innerHTML = `
                    <div class="image-container">
                        <img src="${item.ImageURL}" alt="${item.Name}">
                    </div>
                    <div class="details">
                        <h3>${item.Name}</h3>
                        <p>${new Date(item.AnnounceDate).toLocaleDateString()}</p>
                    </div>
                `;
                card.onclick = () => window.location.href = fixedUrl;
                musicGrid.appendChild(card);
            });
        };

        const filterItems = (searchTerm) => {
            const lowerCaseTerm = searchTerm.toLowerCase();
            filteredItems = projects.filter(
                item =>
                    item.Name.toLowerCase().includes(lowerCaseTerm) ||
                    item.Tags.some(tag => tag.toLowerCase().includes(lowerCaseTerm))
            );
            renderGrid();
        };
        
        // Listen for input events on the search bar
        searchBar.addEventListener('input', (e) => {
            filterItems(e.target.value);
        });

        sortOptions.addEventListener('change', (e) => {
            const key = e.target.value;
        
            projects.sort((a, b) => {
                // Primary sorting
                if (key === 'Name') {
                    const primarySort = a[key].localeCompare(b[key]);
                    if (primarySort !== 0) return primarySort;
                } else if (key === 'DateAdded') {
                    const primarySort = new Date(b.AnnounceDate) - new Date(a.AnnounceDate);
                    if (primarySort !== 0) return primarySort;
                }
        
                // Secondary sorting: Name (alphabetical)
                const secondarySort = a.Name.localeCompare(b.Name);
                if (secondarySort !== 0) return secondarySort;
        
                // Tertiary sorting: Date Added (most recent first)
                return new Date(b.AnnounceDate) - new Date(a.AnnounceDate);
            });
        
            renderGrid();
        });
        
        
        projects.sort((a, b) => {
            return new Date(b.AnnounceDate || 0) - new Date(a.AnnounceDate || 0);
        });
        renderGrid();
    });
