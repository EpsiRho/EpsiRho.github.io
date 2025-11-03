function createSlice(data, type) {
    const slice = document.createElement('div');
    slice.className = 'slice';
    
    const content = document.createElement('div');
    content.className = 'slice-content';
    content.style.backgroundImage = `url(${data.ImageURL || data.HeaderImage})`;

    const blur = document.createElement('div');
    blur.className = 'slice-blur';
    
    const info = document.createElement('div');
    info.className = 'slice-info';
    info.innerHTML = `
        <h3>${data.Name || data.Title}</h3>
        <p>${data.ShortDescription || data.Description}</p>
        <small>${new Date(data.AnnounceDate || data.PostDate).toLocaleDateString()}</small>
    `;
    
    slice.appendChild(content);
    slice.appendChild(blur);
    slice.appendChild(info);
    if (data.PostURL) {
        //content.style.viewTransitionName = `header-${data.PostURL}`;
        let fixedUrl = encodeURI(`/site/project?id=${data.PostURL}`);
        slice.addEventListener('click', () => {
            slice.style.viewTransitionName = `header-${data.PostURL}`;
            slice.classList.add('slice-transitioning');

            const go = () => window.location.href = fixedUrl;
            go();
        });
    }
    else{
        let transitionName = data.BlogPostURL.replaceAll("/posts/", "");
        //content.style.viewTransitionName = `header-${transitionName}`;

        slice.addEventListener('click', () => {
            slice.style.viewTransitionName = `header-${transitionName}`;
            slice.classList.add('slice-transitioning');

            const go = () => window.location.href = encodeURI(data.BlogPostURL);
            //return document.startViewTransition ? document.startViewTransition(go) : go();
            go();
        });
    }
    
    return slice;
}

document.addEventListener('DOMContentLoaded', () => {
    Promise.all([
        fetch('/json/PackedProjects.json').then(r => r.json()),
        fetch('/json/PackedBlogPosts.json').then(r => r.json())
    ]).then(([projects, blogs]) => {
        const projectsContainer = document.getElementById('projects-slices');
        const blogsContainer = document.getElementById('blogs-slices');

        projects.sort((a, b) => {
            const primarySort = new Date(b.AnnounceDate) - new Date(a.AnnounceDate);
            if (primarySort !== 0) return primarySort;
    
            // Secondary sorting: Name (alphabetical)
            const secondarySort = a.Name.localeCompare(b.Name);
            if (secondarySort !== 0) return secondarySort;
        });

        blogs.sort((a, b) => {
            const primarySort = new Date(b.PostDate) - new Date(a.PostDate);
            if (primarySort !== 0) return primarySort;
    
            // Secondary sorting: Name (alphabetical)
            const secondarySort = a.Name.localeCompare(b.Name);
            if (secondarySort !== 0) return secondarySort;
        });
        
        projects.slice(0, 3).forEach(project => {
            projectsContainer.appendChild(createSlice(project, 'project'));
        });
        
        blogs.slice(0, 3).forEach(blog => {
            blogsContainer.appendChild(createSlice(blog, 'blog'));
        });
        const background = document.querySelector('.slices-background');
        background.classList.add('animate-background');
        console.log("background enabled")
    });
    
    document.querySelector('.projects-icon').addEventListener('click', () => {
        window.location.href = '/site/projects';
    });
    
    document.querySelector('.blogs-icon').addEventListener('click', () => {
        window.location.href = '/site/posts';
    });

    
});

const sidebar = document.querySelector('.sidebar');
const toggleButton = document.querySelector('.nav-toggle');
