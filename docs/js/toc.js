document.addEventListener("DOMContentLoaded", () => {
    const toc = document.querySelector(".toc");
    const SidebarNav = document.querySelector(".nav-menu");
    const profile = document.querySelector(".profile");
    const tocNavContainer = document.getElementById("toc-site-nav");
    const tocAnchorContainer = document.getElementById("toc-post-nav");
    const setBtn = document.getElementById("page-settings");


    const navItems = document.createElement('div');
    let divide = document.createElement('hr');
    divide.style.margin = `10px 10px 10px 5px`
    divide.id = 'divider-line';
    navItems.innerHTML = `<ul>
        <a href="/">
            <li>
                <svg xmlns="http://www.w3.org/2000/svg" style="margin-right:5px;" width="1.5em" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">
                    <path stroke="none" d="M0 0h24v24H0z"></path>
                    <polyline points="5 12 3 12 12 3 21 12 19 12"></polyline>
                    <path d="M5 12v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2 -2v-7"></path>
                    <path d="M9 21v-6a2 2 0 0 1 2 -2h2a2 2 0 0 1 2 2v6"></path>
                </svg>
                <p>Home</p>
            </li>
        </a>
        <a href="/site/about">
            <li>
                <svg xmlns="http://www.w3.org/2000/svg" style="margin-right:5px;" width="1.5em" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"></path><path d="M9 10a3 3 0 1 0 6 0a3 3 0 0 0 -6 0"></path><path d="M6 21v-1a4 4 0 0 1 4 -4h4a4 4 0 0 1 4 4v1"></path><path d="M3 5a2 2 0 0 1 2 -2h14a2 2 0 0 1 2 2v14a2 2 0 0 1 -2 2h-14a2 2 0 0 1 -2 -2v-14z"></path>
                </svg>
                About
            </li>
        </a>
        <a href="/site/posts">
            <li>
                <svg xmlns="http://www.w3.org/2000/svg" style="margin-right:5px;" width="1.5em" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"></path><path d="M4 4h6v8h-6z"></path><path d="M4 16h6v4h-6z"></path><path d="M14 12h6v8h-6z"></path><path d="M14 4h6v4h-6z"></path></svg>
                Blog Posts
            </li>
        </a>
        <a href="/site/projects">
            <li>
                <svg xmlns="http://www.w3.org/2000/svg" style="margin-right:5px;" width="1.5em" viewBox="0 0 24 24"  fill="none"  stroke="currentColor"  stroke-width="2"  stroke-linecap="round"  stroke-linejoin="round"  class="icon icon-tabler icons-tabler-outline icon-tabler-code"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M7 8l-4 4l4 4" /><path d="M17 8l4 4l-4 4" /><path d="M14 4l-4 16" /></svg>
                Projects
            </li>
        </a>
        <a href="/site/music">
            <li>
                <svg  xmlns="http://www.w3.org/2000/svg" style="margin-right:5px;" width="1.5em" viewBox="0 0 24 24"  fill="none"  stroke="currentColor"  stroke-width="2"  stroke-linecap="round"  stroke-linejoin="round"  class="icon icon-tabler icons-tabler-outline icon-tabler-music"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 17a3 3 0 1 0 6 0a3 3 0 0 0 -6 0" /><path d="M13 17a3 3 0 1 0 6 0a3 3 0 0 0 -6 0" /><path d="M9 17v-13h10v13" /><path d="M9 8h10" /></svg>
                Music Recommendations
            </li>
        </a>
        </ul>
    `;
    let isOpen = false;

    const openToc = () => {
        toc.classList.add("open");
        toc.classList.remove("scale-hover");
        isOpen = true;
    };

    const closeToc = () => {
        toc.classList.remove("open");
        isOpen = false;
    };

    const clickEvent = (e) => {
        e.stopPropagation(); // Prevent closing immediately
        if (!isOpen) {
            openToc();
        } else {
            closeToc();
        }
    };

    const addScaleEffect = () => {
        toc.classList.add("scale-hover");
    };

    const removeScaleEffect = () => {
        toc.classList.remove("scale-hover");
    };

    const manageMouseover = () => {
        if (window.innerWidth > 990) {  // too big
            toc.addEventListener("mouseover", openToc);
            toc.removeEventListener("click", clickEvent);
            toc.removeEventListener("mouseover", addScaleEffect);
            toc.removeEventListener("mouseleave", removeScaleEffect);
            if(tocNavContainer.contains(navItems)){
                tocNavContainer.removeChild(navItems);
                divide.remove();
            }
            SidebarNav.style.display = 'block';
            profile.style.margin = "0px 0px 2rem 0px"
            if(tocAnchorContainer){
                toc.style.display ='block';
            }
            else{
                toc.style.display ='none';
            }
        } else { // too small
            toc.removeEventListener("mouseover", openToc);
            toc.addEventListener("click", clickEvent);
            toc.addEventListener("mouseover", addScaleEffect);
            toc.addEventListener("mouseleave", removeScaleEffect);
            if(!tocNavContainer.contains(navItems)){
                tocNavContainer.appendChild(navItems);
                let check = document.getElementById('setdivider');
                if(!check){
                    setBtn.after(divide);
                    
                }
            }
            SidebarNav.style.display = 'none';
            profile.style.margin = "0px 0px -60px 0px"
            toc.style.display ='block';
        }
    };

    document.addEventListener("click", (e) => {
        if (!toc.contains(e.target)) {
            closeToc();
        }
    });

    toc.addEventListener("mouseleave", closeToc);

    manageMouseover();
    window.addEventListener("resize", manageMouseover);

    let postTags = document.querySelectorAll('.post-tag');
    postTags.forEach((tag)=>{
        tag.addEventListener("click", () => window.location.href = encodeURI(`/site/posts?s=${tag.innerHTML.trim().replace("#", "%23")}`));
        //tag.innerHTML = `<a>${tag.innerHTML}</a>`
        tag.style.cursor = 'pointer'
    });
});