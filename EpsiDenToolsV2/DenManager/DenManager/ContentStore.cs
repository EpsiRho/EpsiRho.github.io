using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DenManager;

public sealed class ContentStore
{
    private const int CurrentRendererVersion = 2;
    private static readonly JsonSerializerOptions Json = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };
    private static readonly string SettingsFile = Environment.GetEnvironmentVariable("DENMANAGER_SETTINGS_PATH")
        ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DenManager", "settings.json");
    private ManagerData saved = new();
    public ManagerSettings Settings { get; private set; } = new();
    public ManagerData Data { get; private set; } = new();

    public void Load()
    {
        if (File.Exists(SettingsFile))
            Settings = JsonSerializer.Deserialize<ManagerSettings>(File.ReadAllText(SettingsFile), Json) ?? new();
        if (!Directory.Exists(Settings.WebsiteFolderPath))
        {
            var path = AppContext.BaseDirectory;
            while (path != null)
            {
                var candidate = Path.Combine(path, "docs");
                if (Directory.Exists(Path.Combine(candidate, "json")))
                {
                    Settings.WebsiteFolderPath = candidate;
                    break;
                }
                path = Directory.GetParent(path)?.FullName;
            }
        }
        var managerFile = Path.Combine(Settings.ManagerSavePath, "manager.json");
        Data = File.Exists(managerFile)
            ? JsonSerializer.Deserialize<ManagerData>(File.ReadAllText(managerFile), Json) ?? new()
            : ImportLegacy();
        Data.BlogPosts ??= [];
        Data.Projects ??= [];
        Data.MusicRecommendations ??= [];
        saved = Clone(Data);
    }

    public void SetPaths(string website, string manager)
    {
        Settings.WebsiteFolderPath = Path.GetFullPath(website.Trim().Trim('"'));
        Settings.ManagerSavePath = Path.GetFullPath(manager.Trim().Trim('"'));
    }

    private ManagerData ImportLegacy()
    {
        // Preserve hand-edited legacy pages until their content is explicitly changed.
        var result = new ManagerData { RendererVersion = CurrentRendererVersion };
        result.BlogPosts = ReadPacked<BlogEntry>("PackedBlogPosts.json", "BlogPostsJson");
        result.Projects = ReadPacked<ProjectEntry>("PackedProjects.json", "ProjectsJson");
        result.MusicRecommendations = ReadPacked<MusicEntry>("PackedMusicCards.json", "MusicCards");
        MergeLocalExtras(result.BlogPosts, "PackedBlogPosts.json", x => x.BlogPostURL);
        MergeLocalExtras(result.Projects, "PackedProjects.json", x => x.PostURL);
        MergeLocalExtras(result.MusicRecommendations, "PackedMusicCards.json", x => x.Name + " / " + x.Artist);
        // The old packer could include empty records after a failed metadata parse.
        result.BlogPosts.RemoveAll(x => string.IsNullOrWhiteSpace(x.Title));
        result.Projects.RemoveAll(x => string.IsNullOrWhiteSpace(x.Name));
        result.MusicRecommendations.RemoveAll(x => string.IsNullOrWhiteSpace(x.Name) || string.IsNullOrWhiteSpace(x.Artist));
        foreach (var post in result.BlogPosts)
        {
            post.Tags ??= [];
            var slug = Path.GetFileName(post.BlogPostURL.TrimEnd('/'));
            post.BlogPostURL = "/posts/" + slug;
            var source = FindLegacySource("Blogs", slug);
            if (source == null) continue;
            var content = File.ReadAllText(source).Replace("\r\n", "\n");
            post.Draft = content.StartsWith("[DRAFT]", StringComparison.Ordinal);
            var end = content.IndexOf("~~~metaEnd~~~", StringComparison.Ordinal);
            if (end >= 0) post.Markdown = RewriteLegacyImages(content[(end + "~~~metaEnd~~~".Length)..].Trim());
        }
        foreach (var project in result.Projects)
        {
            project.Tags ??= [];
            project.Links ??= [];
            var source = FindLegacySource("Projects", project.PostURL);
            if (source != null)
            {
                var content = File.ReadAllText(source).Replace("\r\n", "\n");
                var match = Regex.Match(content, @"(?m)^Description\s*$");
                project.Markdown = RewriteLegacyImages(match.Success ? content[(match.Index + match.Length)..].Trim() : project.Description);
            }
            else project.Markdown = project.Description;
        }
        return result;
    }

    private string RewriteLegacyImages(string markdown)
    {
        return Regex.Replace(markdown, @"!\[([^\]]*)\]\(([^)]+)\)", match =>
        {
            var path = match.Groups[2].Value;
            if (path.StartsWith('/') || Uri.IsWellFormedUriString(path, UriKind.Absolute)) return match.Value;
            var name = Path.GetFileNameWithoutExtension(path);
            foreach (var extension in new[] { ".webp", Path.GetExtension(path) })
            {
                if (string.IsNullOrWhiteSpace(extension)) continue;
                var candidate = name + extension;
                if (File.Exists(Path.Combine(Settings.WebsiteFolderPath, "images", candidate)))
                    return "![" + match.Groups[1].Value + "](/images/" + candidate + ")";
            }
            return match.Value;
        });
    }

    private string? FindLegacySource(string folder, string slug)
    {
        var dir = Path.Combine(Settings.ManagerSavePath, "MarkdownDocs", folder);
        if (!Directory.Exists(dir)) return null;
        return Directory.EnumerateFiles(dir)
            .FirstOrDefault(path => string.Equals(Path.GetFileNameWithoutExtension(path), slug, StringComparison.OrdinalIgnoreCase));
    }

    private List<T> ReadPacked<T>(string file, string legacyFolder)
    {
        foreach (var path in new[] { Path.Combine(Settings.WebsiteFolderPath, "json", file),
            Path.Combine(Settings.ManagerSavePath, file) })
        {
            if (File.Exists(path))
                return JsonSerializer.Deserialize<List<T>>(File.ReadAllText(path), Json) ?? [];
        }
        var dir = Path.Combine(Settings.ManagerSavePath, legacyFolder);
        var items = new List<T>();
        if (Directory.Exists(dir))
            foreach (var path in Directory.EnumerateFiles(dir, "*.json"))
            {
                try
                {
                    var item = JsonSerializer.Deserialize<T>(File.ReadAllText(path), Json);
                    if (item != null) items.Add(item);
                }
                catch (JsonException) { /* A bad legacy record does not hide the good ones. */ }
            }
        return items;
    }

    private void MergeLocalExtras<T>(List<T> current, string file, Func<T, string> key)
    {
        var path = Path.Combine(Settings.ManagerSavePath, file);
        if (!File.Exists(path)) return;
        var local = JsonSerializer.Deserialize<List<T>>(File.ReadAllText(path), Json) ?? [];
        var used = current.Select(key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var item in local)
            if (used.Add(key(item))) current.Add(item);
    }

    public IReadOnlyList<string> Changes()
    {
        var changes = new List<string>();
        if (saved.RendererVersion < CurrentRendererVersion)
            changes.Add("Rebuild published Markdown pages with corrected nested lists.");
        Compare(saved.BlogPosts, Data.BlogPosts, p => p.BlogPostURL, p => p.Title, "Post", changes);
        Compare(saved.Projects, Data.Projects, p => p.PostURL, p => p.Name, "Project", changes);
        Compare(saved.MusicRecommendations, Data.MusicRecommendations,
            p => p.Name + " / " + p.Artist, p => p.Name + " — " + p.Artist, "Music", changes);
        if (changes.Count == 0) changes.Add("No pending content changes.");
        return changes;
    }

    private static void Compare<T>(List<T> before, List<T> after, Func<T, string> key,
        Func<T, string> title, string label, List<string> output)
    {
        var old = before.GroupBy(key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var current = after.GroupBy(key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        foreach (var item in after)
        {
            if (!old.TryGetValue(key(item), out var previous)) output.Add($"Add {label}: {title(item)}");
            else if (JsonSerializer.Serialize(previous, Json) != JsonSerializer.Serialize(item, Json))
                output.Add($"Edit {label}: {title(item)}");
        }
        foreach (var item in before)
            if (!current.ContainsKey(key(item))) output.Add($"Remove {label}: {title(item)}");
    }

    public void SaveAndPublish()
    {
        var site = Settings.WebsiteFolderPath;
        var save = Settings.ManagerSavePath;
        if (!Directory.Exists(site) || !File.Exists(Path.Combine(site, "site", "project.html")))
            throw new InvalidOperationException("Choose the website's docs folder in Settings.");
        if (string.Equals(Path.GetFullPath(site).TrimEnd('\\'), Path.GetFullPath(save).TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The manager save folder must differ from the website folder.");
        Validate();
        Directory.CreateDirectory(save);
        Directory.CreateDirectory(Path.Combine(site, "json"));
        Directory.CreateDirectory(Path.Combine(site, "posts"));
        Directory.CreateDirectory(Path.Combine(site, "projects"));

        foreach (var item in Data.MusicRecommendations.Where(x => x.PendingImagePath != null))
        {
            var source = item.PendingImagePath!;
            if (!File.Exists(source)) throw new FileNotFoundException("Selected music image was moved.", source);
            var extension = Path.GetExtension(source).ToLowerInvariant();
            if (extension is not (".png" or ".jpg" or ".jpeg" or ".webp" or ".gif"))
                throw new InvalidOperationException("Choose a PNG, JPG, WebP, or GIF image.");
            var bytes = File.ReadAllBytes(source);
            var hash = Convert.ToHexString(SHA256.HashData(bytes))[..10].ToLowerInvariant();
            var name = $"music-{MarkdownRenderer.Slug(item.Name + "-" + item.Artist)}-{hash}{extension}";
            var imageDir = Path.Combine(site, "images", "custom-album-art");
            Directory.CreateDirectory(imageDir);
            var target = Path.Combine(imageDir, name);
            if (!File.Exists(target)) File.WriteAllBytes(target, bytes);
            item.ThumbnailURL = "/images/custom-album-art/" + name;
            item.PendingImagePath = null;
        }

        var publishedPosts = Data.BlogPosts.Where(x => !x.Draft).ToList();
        var rebuildPages = saved.RendererVersion < CurrentRendererVersion;
        var template = Directory.EnumerateFiles(Path.Combine(site, "posts"), "*.html").FirstOrDefault();
        string? templateText = template == null ? null : File.ReadAllText(template);
        foreach (var post in publishedPosts)
        {
            var slug = PostSlug(post);
            var path = Path.Combine(site, "posts", slug + ".html");
            var previous = saved.BlogPosts.FirstOrDefault(x =>
                string.Equals(x.BlogPostURL, post.BlogPostURL, StringComparison.OrdinalIgnoreCase));
            if (!rebuildPages && previous != null && File.Exists(path) &&
                JsonSerializer.Serialize(previous, Json) == JsonSerializer.Serialize(post, Json)) continue;
            AtomicWrite(path, BlogHtml(post, templateText));
        }
        var currentSlugs = publishedPosts.Select(PostSlug).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var old in saved.BlogPosts.Where(x => !x.Draft))
        {
            var slug = PostSlug(old);
            if (!currentSlugs.Contains(slug))
            {
                var path = Path.Combine(site, "posts", slug + ".html");
                if (File.Exists(path)) File.Delete(path);
            }
        }
        var projectTemplate = File.ReadAllText(Path.Combine(site, "site", "project.html"));
        foreach (var project in Data.Projects)
            AtomicWrite(Path.Combine(site, "projects", project.PostURL + ".html"), ProjectHtml(project, projectTemplate));
        var projectSlugs = Data.Projects.Select(x => x.PostURL).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var old in saved.Projects)
        {
            if (projectSlugs.Contains(old.PostURL)) continue;
            var path = Path.Combine(site, "projects", old.PostURL + ".html");
            if (File.Exists(path)) File.Delete(path);
        }

        var blogJson = publishedPosts.Select(x => new {
            x.Title, x.Description, x.PostDate, x.HeaderImage, x.BlogPostURL, x.Tags
        }).ToList();
        var projectJson = Data.Projects.Select(x => new {
            x.Name, x.AnnounceDate, x.ImageURL, x.PostURL, x.ShortDescription,
            Description = x.Markdown, DescriptionHtml = MarkdownRenderer.Render(x.Markdown), x.Tags, x.Links
        }).ToList();
        foreach (var (name, content) in new[] {
            ("PackedBlogPosts.json", JsonSerializer.Serialize(blogJson, Json)),
            ("PackedProjects.json", JsonSerializer.Serialize(projectJson, Json)),
            ("PackedMusicCards.json", JsonSerializer.Serialize(Data.MusicRecommendations, Json)) })
        {
            AtomicWrite(Path.Combine(save, name), content);
            AtomicWrite(Path.Combine(site, "json", name), content);
        }
        Data.RendererVersion = CurrentRendererVersion;
        AtomicWrite(Path.Combine(save, "manager.json"), JsonSerializer.Serialize(Data, Json));
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsFile)!);
        AtomicWrite(SettingsFile, JsonSerializer.Serialize(Settings, Json));
        saved = Clone(Data);
    }

    private void Validate()
    {
        foreach (var post in Data.BlogPosts)
        {
            if (string.IsNullOrWhiteSpace(post.Title)) throw new InvalidOperationException("Every post needs a title.");
            if (!post.Draft && string.IsNullOrWhiteSpace(post.Markdown))
                throw new InvalidOperationException($"Post '{post.Title}' has no markdown. Add content or mark it as a draft.");
            _ = PostSlug(post);
        }
        foreach (var project in Data.Projects)
        {
            if (string.IsNullOrWhiteSpace(project.Name)) throw new InvalidOperationException("Every project needs a name.");
            if (!Regex.IsMatch(project.PostURL, @"^[A-Za-z0-9_-]+$"))
                throw new InvalidOperationException($"Project '{project.Name}' needs a URL ID using letters, numbers, _ or -.");
        }
        foreach (var music in Data.MusicRecommendations)
        {
            if (string.IsNullOrWhiteSpace(music.Name) || string.IsNullOrWhiteSpace(music.Artist))
                throw new InvalidOperationException("Every music recommendation needs a title and artist.");
            if (music.Rating is < 0 or > 5 || music.Energy is < 1 or > 5)
                throw new InvalidOperationException($"Check the rating and energy for '{music.Name}'.");
        }
        EnsureUnique(Data.BlogPosts.Select(PostSlug), "post URL");
        EnsureUnique(Data.Projects.Select(x => x.PostURL), "project URL ID");
        EnsureUnique(Data.MusicRecommendations.Select(x => x.Name + " / " + x.Artist), "music title and artist");
    }

    private static void EnsureUnique(IEnumerable<string> keys, string label)
    {
        var duplicate = keys.GroupBy(x => x, StringComparer.OrdinalIgnoreCase).FirstOrDefault(x => x.Count() > 1);
        if (duplicate != null) throw new InvalidOperationException($"Duplicate {label}: {duplicate.Key}");
    }

    private static string PostSlug(BlogEntry post)
    {
        var slug = post.BlogPostURL.Replace("/posts/", "", StringComparison.OrdinalIgnoreCase).Trim('/');
        if (!Regex.IsMatch(slug, @"^[A-Za-z0-9_-]+$"))
            throw new InvalidOperationException($"Post '{post.Title}' needs a URL ID using letters, numbers, _ or -.");
        return slug;
    }

    private static string BlogHtml(BlogEntry post, string? template)
    {
        var title = WebUtility.HtmlEncode(post.Title);
        var description = WebUtility.HtmlEncode(post.Description);
        var image = WebUtility.HtmlEncode(post.HeaderImage);
        var tags = string.Join("", post.Tags.Select(x => "<span class=\"post-tag\">" + WebUtility.HtmlEncode(x.Trim()) + "</span>"));
        var article = $"<article class=\"blog-post-page\"><img src=\"{image}\" alt=\"\" class=\"post-header-image\"><div>{tags}</div><h1 class=\"post-title\" id=\"mdpost-title\">{title}</h1><p class=\"post-description\">{description}</p><div class=\"post-date\">{post.PostDate:MMMM d, yyyy}</div><div class=\"post-body\">{MarkdownRenderer.Render(post.Markdown)}</div></article>";
        if (template != null && Regex.IsMatch(template, "<article class=\"blog-post-page\".*?</article>", RegexOptions.Singleline))
        {
            var html = Regex.Replace(template, "<article class=\"blog-post-page\".*?</article>", _ => article, RegexOptions.Singleline);
            html = Regex.Replace(html, @"<title>.*?</title>", _ => "<title>" + title + "</title>", RegexOptions.Singleline);
            html = Regex.Replace(html, "<meta property=\"og:title\" content=\"[^\"]*\"", _ => "<meta property=\"og:title\" content=\"" + title + "\"", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "<meta property=\"og:description\" content=\"[^\"]*\"", _ => "<meta property=\"og:description\" content=\"" + description + "\"", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "<meta property=\"og:image\" content=\"[^\"]*\"", _ => "<meta property=\"og:image\" content=\"" + image + "\"", RegexOptions.IgnoreCase);
            var toc = new StringBuilder("<a href=\"#mdpost-title\"><li>" + title + "</li></a>");
            foreach (Match heading in Regex.Matches(post.Markdown, @"(?m)^#{1,2}\s+(.+)$"))
            {
                var headingTitle = heading.Groups[1].Value.Trim();
                toc.Append("<a href=\"#mdheadr-").Append(MarkdownRenderer.Slug(headingTitle))
                    .Append("\"><li>").Append(WebUtility.HtmlEncode(headingTitle)).Append("</li></a>");
            }
            html = Regex.Replace(html, "(<hr[^>]*id=\"setdivider\"[^>]*/>).*?(</ul>)",
                m => m.Groups[1].Value + toc + m.Groups[2].Value, RegexOptions.Singleline);
            return html;
        }
        return $"<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>{title}</title><link rel=\"stylesheet\" href=\"/styles/main.css\"><link rel=\"stylesheet\" href=\"/styles/posts.css\"></head><body><div class=\"container\"><aside class=\"sidebar\"><h1>Epsi's Den</h1><nav><a href=\"/\">Home</a> · <a href=\"/site/posts\">Blog posts</a> · <a href=\"/site/projects\">Projects</a> · <a href=\"/site/music\">Music</a></nav></aside><main class=\"main-content\">{article}</main></div></body></html>";
    }

    private static string ProjectHtml(ProjectEntry project, string template)
    {
        var title = WebUtility.HtmlEncode(project.Name);
        var summary = WebUtility.HtmlEncode(project.ShortDescription);
        var image = WebUtility.HtmlEncode(project.ImageURL);
        var tags = string.Join("", project.Tags.Select(x => "<span class=\"project-tag\">" + WebUtility.HtmlEncode(x.Trim()) + "</span>"));
        var links = string.Join("", project.Links.Where(x => Uri.TryCreate(x.Link, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https")
            .Select(x => "<a href=\"" + WebUtility.HtmlEncode(x.Link) + "\"><span class=\"post-link\">" + WebUtility.HtmlEncode(x.Name) + "</span></a>"));
        var article = $"<article class=\"blog-post-page\" id=\"project-card\"><div class=\"project-header-container\"><div class=\"project-header-shadow\"><div class=\"project-header-info\"><h1 class=\"post-title\">{title}</h1><p class=\"post-description\">{summary}</p><div class=\"post-tags\">{tags}</div></div></div><img alt=\"\" src=\"{image}\" class=\"project-header-image\"></div><div class=\"post-links\"><div class=\"post-links-fade\"><h2>Links</h2></div>{links}</div><div class=\"post-date\">{project.AnnounceDate:MMMM d, yyyy}</div><div class=\"post-body\">{MarkdownRenderer.Render(project.Markdown)}</div></article>";
        var html = Regex.Replace(template, "<article class=\"blog-post-page\" id=\"project-card\".*?</article>", _ => article, RegexOptions.Singleline);
        html = Regex.Replace(html, "<article class=\"blog-post-page\" id=\"missing-card\".*?</article>", "", RegexOptions.Singleline);
        html = html.Replace("<script src=\"/js/project.js\"></script>", "");
        html = Regex.Replace(html, @"<title>.*?</title>", _ => "<title>" + title + "</title>", RegexOptions.Singleline);
        html = Regex.Replace(html, "<meta property=\"og:title\" content=\"[^\"]*\"", _ => "<meta property=\"og:title\" content=\"" + title + "\"", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, "<meta property=\"og:description\" content=\"[^\"]*\"", _ => "<meta property=\"og:description\" content=\"" + summary + "\"", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, "<meta property=\"og:image\" content=\"[^\"]*\"", _ => "<meta property=\"og:image\" content=\"" + image + "\"", RegexOptions.IgnoreCase);
        return html;
    }

    private static void AtomicWrite(string path, string content)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var temporary = path + ".tmp";
        File.WriteAllText(temporary, content, new UTF8Encoding(false));
        File.Move(temporary, path, true);
    }

    private static ManagerData Clone(ManagerData source) =>
        JsonSerializer.Deserialize<ManagerData>(JsonSerializer.Serialize(source, Json), Json) ?? new();
}
