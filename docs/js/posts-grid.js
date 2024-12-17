const musicGrid = document.getElementById('music-grid');
const sortOptions = document.getElementById('sort-options');
const searchBar = document.getElementById('search-bar');

fetch("../json/PackedBlogPosts.json")
    .then(response => response.json())
    .then(data => {
        let posts = data;
        let filteredItems = posts;
        const renderGrid = () => {
            musicGrid.innerHTML = '';
            filteredItems.forEach(item => {
                const card = document.createElement('div');
                card.className = 'music-card';
                card.style.background = '#252525';
                card.innerHTML = `
                    <div class="image-container">
                        <img src="${item.HeaderImage}" alt="${item.Title}">
                    </div>
                    <div class="details">
                        <h3>${item.Title}</h3>
                        <p>${new Date(item.PostDate).toLocaleDateString()}</p>
                    </div>
                `;
                card.onclick = () => window.location.href = encodeURI(item.BlogPostURL);
                musicGrid.appendChild(card);
            });
        };

        const filterItems = (searchTerm) => {
            const lowerCaseTerm = searchTerm.toLowerCase();
            filteredItems = posts.filter(
                item =>
                    item.Title.toLowerCase().includes(lowerCaseTerm) ||
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
        
            posts.sort((a, b) => {
                // Primary sorting
                if (key === 'Name') {
                    const primarySort = a["Title"].localeCompare(b["Title"]);
                    if (primarySort !== 0) return primarySort;
                } else if (key === 'DateAdded') {
                    const primarySort = new Date(b.PostDate) - new Date(a.PostDate);
                    if (primarySort !== 0) return primarySort;
                }
        
                // Secondary sorting: Name (alphabetical)
                const secondarySort = a.Name.localeCompare(b.Name);
                if (secondarySort !== 0) return secondarySort;
        
                // Tertiary sorting: Date Added (most recent first)
                return new Date(b.DateAdded) - new Date(a.DateAdded);
            });
        
            filteredItems = posts;
            renderGrid();
        });
        
        
        posts.sort((a, b) => {
            return new Date(b.PostDate || 0) - new Date(a.PostDate || 0);
        });

        function getQueryParam(param) {
            const urlParams = new URLSearchParams(window.location.search);
            return urlParams.get(param);
        }

        let search = getQueryParam('s');
        if(search){
            search = search.replace("%23", "#");
            filterItems(search);
            searchBar.value = search;
        }

        renderGrid();
    });
