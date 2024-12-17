function GetRandomMusic(){
    fetch("../json/PackedMusicCards.json")
    .then(response => response.json())
    .then(data => {
        let musicItems = data;
        let item = musicItems[Math.floor(Math.random() * musicItems.length)];

        let musicGrid = document.getElementById(`missing-body`);

        const card = document.createElement('div');
        card.className = 'music-card';
        card.style.background = getGradientForEnergy(item.Energy, item.Rating);
        card.innerHTML = `
        <div class="image-container">
        <img src="${item.thumbnailURL}" alt="${item.Name}" id="random-image">
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
}
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
// Wait for the DOM to fully load
window.addEventListener('DOMContentLoaded', () => {
    // Function to get the ID from the URL query parameters
    function getQueryParam(param) {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(param);
    }

    // Function to populate the blog post page
    function populatePage(projectData) {
        const postTitle = document.querySelector('.post-title');
        const postDescription = document.querySelector('.post-description');
        const postDate = document.querySelector('.post-date');
        const postBody = document.querySelector('.post-body');
        const postTags = document.querySelector('.post-tags');
        const postLinks = document.querySelector('.post-links');

        // Populate title, description, date, and body
        postTitle.textContent = projectData.Name;
        postDescription.textContent = projectData.ShortDescription;
        postDate.textContent = new Date(projectData.AnnounceDate).toLocaleString();
        let pee = document.createElement(`p`);
        pee.innerHTML = projectData.Description.replace("\n", "</br></br>");
        postBody.appendChild(pee);

        // Clear and populate tags
        postTags.innerHTML = ""; // Clear any existing tags
        projectData.Tags.forEach(tag => {
            const span = document.createElement('span');
            span.className = 'project-tag';
            span.textContent = tag;
            postTags.appendChild(span);
        });

        //postLinks.innerHTML = ""; // Clear any existing tags
        projectData.Links.forEach(link => {
            const span = document.createElement('span');
            const alink = document.createElement('a');
            span.className = 'post-link';
            span.textContent = link.Name;
            alink.href = link.Link;
            alink.appendChild(span);
            postLinks.appendChild(alink);
        });

        // Set the header image if provided
        const headerImage = document.querySelector('.project-header-image');
        if (projectData.ImageURL) {
            headerImage.src = projectData.ImageURL;
            headerImage.alt = `${projectData.Title} Header Image`;
            headerImage.style.display = 'block';
        } else {
            headerImage.style.display = 'none';
        }
    }

    // Main function to fetch JSON data and populate the page
    async function loadProjectData() {
        const projectId = getQueryParam('id');

        if (!projectId) {
            const card = document.getElementById('missing-card');
            card.style.display = 'block'
            GetRandomMusic();
            console.error('Project ID not found in URL query parameters.');
            return;
        }

        try {
            const response = await fetch('/json/PackedProjects.json');

            if (!response.ok) {
                const card = document.getElementById('missing-card');
                card.style.display = 'block'
                GetRandomMusic();
                throw new Error(`Failed to fetch JSON data: ${response.statusText}`);
            }

            const projects = await response.json();

            // Find the project by ID
            const project = projects.find(p => p.PostURL === projectId);

            if (!project) {
                const card = document.getElementById('missing-card');
                card.style.display = 'block'
                GetRandomMusic();
                console.error('Project not found with the given ID:', projectId);
                return;
            }

            // Populate the page with the project data
            populatePage(project);
            document.title = project.Name;
            const card = document.getElementById('project-card');
            card.style.display = 'block'
        } catch (error) {
            console.error('Error loading project data:', error);
            const card = document.getElementById('missing-card');
            card.style.display = 'block'
            GetRandomMusic();
        }
    }

    // Load the project data
    loadProjectData();
});
