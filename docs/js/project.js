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
        pee.innerHTML = projectData.Description.replace("\n", "<br>");
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
            console.error('Project ID not found in URL query parameters.');
            return;
        }

        try {
            const response = await fetch('/json/PackedProjects.json');

            if (!response.ok) {
                const card = document.getElementById('missing-card');
                card.style.display = 'block'
                throw new Error(`Failed to fetch JSON data: ${response.statusText}`);
            }

            const projects = await response.json();

            // Find the project by ID
            const project = projects.find(p => p.PostURL === projectId);

            if (!project) {
                const card = document.getElementById('missing-card');
                card.style.display = 'block'
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
        }
    }

    // Load the project data
    loadProjectData();
});
