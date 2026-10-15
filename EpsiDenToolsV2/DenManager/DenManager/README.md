# Den Manager

WinUI 3 editor for the Epsi's Den website.

## First launch

The app reads the current `docs/json/Packed*.json` files, then imports local-only records and Markdown from `D:\Documents\EpsiDenTools`. A malformed empty legacy project record is ignored. The old source files are left in place.

On the first **Save and publish**, the app writes `manager.json` in the manager save folder. That file becomes the editable source for later launches. Change both folder paths under **Settings** before publishing if the defaults are different on your machine.

## Editing

- Choose **Blog posts**, **Projects**, or **Music recommendations** in the left navigation.
- Select a card to open its editor over the grid. **Close** keeps the edits in memory for the next publish. **Add** creates and opens a new item. Right-click a card to stage its removal.
- Cards and editors preview the blog, project, or music image URL. Site-relative paths such as `/images/header.webp` resolve against the website folder.
- Blog and project bodies use Markdown. To size an image, write `![caption](/images/photo.webp){width=60%}`. Width can be 1–100% of the article content; the image keeps its aspect ratio. Project links are one `Label | https://example.com` pair per line.
- Music accepts manual details or a song/album URL for the song.link lookup. A selected cover image is copied into `docs/images/custom-album-art` when publishing.
- Mark an unfinished blog post as a draft to keep it in `manager.json` without publishing it.
- Open **Publish** to review pending changes, then choose **Save and publish**.

## Published files

The app writes `docs/json/PackedBlogPosts.json`, `PackedProjects.json`, and `PackedMusicCards.json`, matching the existing site data shapes. It renders posts to `docs/posts/*.html` and projects to `docs/projects/*.html`. The existing project grid keeps using `/site/project?id=...` and renders the new Markdown HTML from packed JSON; generated project pages are also available by direct URL. The same packed JSON files and `manager.json` are saved to the manager folder.

Publishing checks required names, safe URL IDs, duplicate IDs, ratings, and folder paths before writing. Website files are written through temporary files to avoid incomplete files. The app does not commit or deploy the website repository.

When the Markdown renderer is updated, **Publish** lists a one-time page rebuild. The next **Save and publish** regenerates existing managed post pages and project descriptions so formatting fixes reach the website even when the text itself has not changed.
