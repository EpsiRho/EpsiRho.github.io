using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace DenManager;

public sealed class CatalogItem
{
    public required object Value { get; init; }
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public ImageSource? PreviewImage { get; init; }
}

public sealed partial class MainWindow : Window
{
    private readonly ContentStore store = new();
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(20) };
    private string section = "blog";
    private object? selected;
    private CatalogItem? contextItem;
    private bool loading;

    public MainWindow()
    {
        InitializeComponent();
        for (int i = 0; i <= 10; i++) MusicRating.Items.Add((i / 2.0).ToString("0.0", CultureInfo.InvariantCulture));
        for (int i = 1; i <= 5; i++) MusicEnergy.Items.Add(i.ToString(CultureInfo.InvariantCulture));
        try { store.Load(); }
        catch (Exception ex) { PublishStatus.Text = "Could not load saved content: " + ex.Message; }
        WebsitePath.Text = store.Settings.WebsiteFolderPath;
        ManagerPath.Text = store.Settings.ManagerSavePath;
        RefreshCatalog();
        Navigation.SelectedItem = Navigation.MenuItems[0];
    }

    private static List<string> Tags(string text) => text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

    private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (CatalogGrid == null) return;
        CommitEditor();
        selected = null;
        EditorOverlay.Visibility = Visibility.Collapsed;
        section = args.IsSettingsSelected ? "settings" : (args.SelectedItem as NavigationViewItem)?.Tag?.ToString() ?? "blog";
        CatalogPanel.Visibility = section is "blog" or "projects" or "music" ? Visibility.Visible : Visibility.Collapsed;
        PublishPanel.Visibility = section == "publish" ? Visibility.Visible : Visibility.Collapsed;
        SettingsPanel.Visibility = section == "settings" ? Visibility.Visible : Visibility.Collapsed;
        if (CatalogPanel.Visibility == Visibility.Visible) RefreshCatalog();
        if (section == "publish") ChangesList.ItemsSource = store.Changes();
    }

    private void RefreshCatalog()
    {
        loading = true;
        var items = section switch
        {
            "projects" => store.Data.Projects.Select(p => new CatalogItem { Value = p, Title = p.Name, Subtitle = p.AnnounceDate.ToShortDateString(), PreviewImage = PreviewSource(p.ImageURL) }).ToList(),
            "music" => store.Data.MusicRecommendations.Select(p => new CatalogItem { Value = p, Title = p.Name, Subtitle = p.Artist, PreviewImage = PreviewSource(p.PendingImagePath ?? p.ThumbnailURL) }).ToList(),
            _ => store.Data.BlogPosts.Select(p => new CatalogItem { Value = p, Title = p.Title, Subtitle = p.Draft ? "Draft" : p.PostDate.ToShortDateString(), PreviewImage = PreviewSource(p.HeaderImage) }).ToList()
        };
        CatalogGrid.ItemsSource = items;
        CatalogHeading.Text = section switch { "projects" => "Projects", "music" => "Music", _ => "Blog posts" };
        AddButton.Content = section switch { "projects" => "Add project", "music" => "Add recommendation", _ => "Add post" };
        BlogFields.Visibility = section == "blog" ? Visibility.Visible : Visibility.Collapsed;
        ProjectFields.Visibility = section == "projects" ? Visibility.Visible : Visibility.Collapsed;
        MusicFields.Visibility = section == "music" ? Visibility.Visible : Visibility.Collapsed;
        loading = false;
    }

    private void CatalogGrid_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (loading || e.ClickedItem is not CatalogItem item) return;
        OpenEditor(item.Value);
    }

    private void OpenEditor(object item)
    {
        selected = item;
        PopulateEditor();
        EditorOverlay.Visibility = Visibility.Visible;
    }

    private void CatalogCard_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is CatalogItem item)
            contextItem = item;
    }

    private void PopulateEditor()
    {
        loading = true;
        if (selected is BlogEntry blog)
        {
            EditorHeading.Text = blog.Title;
            BlogTitle.Text = blog.Title;
            BlogDescription.Text = blog.Description;
            BlogSlug.Text = blog.BlogPostURL.Replace("/posts/", "", StringComparison.OrdinalIgnoreCase);
            BlogDate.Date = new DateTimeOffset(blog.PostDate);
            BlogImage.Text = blog.HeaderImage;
            BlogTags.Text = string.Join(", ", blog.Tags);
            BlogDraft.IsChecked = blog.Draft;
            BlogMarkdown.Text = blog.Markdown;
        }
        else if (selected is ProjectEntry project)
        {
            EditorHeading.Text = project.Name;
            ProjectName.Text = project.Name;
            ProjectSummary.Text = project.ShortDescription;
            ProjectSlug.Text = project.PostURL;
            ProjectDate.Date = new DateTimeOffset(project.AnnounceDate);
            ProjectImage.Text = project.ImageURL;
            ProjectTags.Text = string.Join(", ", project.Tags);
            ProjectLinks.Text = string.Join(Environment.NewLine, project.Links.Select(x => x.Name + " | " + x.Link));
            ProjectMarkdown.Text = project.Markdown;
        }
        else if (selected is MusicEntry music)
        {
            EditorHeading.Text = music.Name;
            MusicLinkLookup.Text = "";
            MusicName.Text = music.Name;
            MusicArtist.Text = music.Artist;
            MusicURL.Text = music.URL;
            MusicImage.Text = music.ThumbnailURL;
            SelectedImageText.Text = music.PendingImagePath ?? "No local image selected";
            MusicRating.SelectedIndex = Math.Clamp((int)Math.Round(music.Rating * 2), 0, 10);
            MusicEnergy.SelectedIndex = Math.Clamp(music.Energy - 1, 0, 4);
            MusicDate.Date = new DateTimeOffset(music.DateAdded);
        }
        loading = false;
    }

    private void CloseEditor_Click(object sender, RoutedEventArgs e)
    {
        CommitEditor();
        selected = null;
        EditorOverlay.Visibility = Visibility.Collapsed;
        RefreshCatalog();
    }

    private void CommitEditor()
    {
        if (loading || selected == null) return;
        if (selected is BlogEntry blog)
        {
            blog.Title = BlogTitle.Text.Trim();
            blog.Description = BlogDescription.Text.Trim();
            blog.BlogPostURL = "/posts/" + BlogSlug.Text.Trim();
            blog.PostDate = BlogDate.Date.DateTime;
            blog.HeaderImage = BlogImage.Text.Trim();
            blog.Tags = Tags(BlogTags.Text);
            blog.Draft = BlogDraft.IsChecked == true;
            blog.Markdown = BlogMarkdown.Text;
        }
        else if (selected is ProjectEntry project)
        {
            project.Name = ProjectName.Text.Trim();
            project.ShortDescription = ProjectSummary.Text.Trim();
            project.PostURL = ProjectSlug.Text.Trim();
            project.AnnounceDate = ProjectDate.Date.DateTime;
            project.ImageURL = ProjectImage.Text.Trim();
            project.Tags = Tags(ProjectTags.Text);
            project.Links = ProjectLinks.Text.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split('|', 2, StringSplitOptions.TrimEntries))
                .Select(parts => new ProjectLink { Name = parts[0], Link = parts.Length == 2 ? parts[1] : "" }).ToList();
            project.Markdown = ProjectMarkdown.Text;
        }
        else if (selected is MusicEntry music)
        {
            music.Name = MusicName.Text.Trim();
            music.Artist = MusicArtist.Text.Trim();
            music.URL = MusicURL.Text.Trim();
            music.ThumbnailURL = MusicImage.Text.Trim();
            music.Rating = MusicRating.SelectedIndex >= 0 ? MusicRating.SelectedIndex / 2f : 0;
            music.Energy = MusicEnergy.SelectedIndex + 1;
            music.DateAdded = MusicDate.Date.DateTime;
        }
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        CommitEditor();
        object item;
        if (section == "projects")
        {
            var project = new ProjectEntry { Name = "New project", PostURL = UniqueSlug("new-project", store.Data.Projects.Select(x => x.PostURL)) };
            store.Data.Projects.Add(project); item = project;
        }
        else if (section == "music")
        {
            var music = new MusicEntry { Name = "New recommendation", Artist = "Unknown artist", DateAdded = DateTime.Now };
            store.Data.MusicRecommendations.Add(music); item = music;
        }
        else
        {
            var blog = new BlogEntry { Title = "New post", BlogPostURL = "/posts/" + UniqueSlug("new-post", store.Data.BlogPosts.Select(x => x.BlogPostURL.Replace("/posts/", ""))), Draft = true };
            store.Data.BlogPosts.Add(blog); item = blog;
        }
        selected = null;
        RefreshCatalog();
        OpenEditor(item);
    }

    private static string UniqueSlug(string basis, IEnumerable<string> existing)
    {
        var used = existing.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var slug = basis;
        for (int i = 2; used.Contains(slug); i++) slug = basis + "-" + i;
        return slug;
    }

    private async void Remove_Click(object sender, RoutedEventArgs e)
    {
        if (contextItem is not CatalogItem item) return;
        var dialog = new ContentDialog
        {
            XamlRoot = CatalogPanel.XamlRoot,
            Title = "Remove " + item.Title + "?",
            Content = "This change will be applied to the website when you save and publish.",
            PrimaryButtonText = "Remove",
            CloseButtonText = "Cancel"
        };
        if (await dialog.ShowAsync() != ContentDialogResult.Primary) return;
        if (item.Value is BlogEntry blog) store.Data.BlogPosts.Remove(blog);
        if (item.Value is ProjectEntry project) store.Data.Projects.Remove(project);
        if (item.Value is MusicEntry music) store.Data.MusicRecommendations.Remove(music);
        selected = null;
        contextItem = null;
        RefreshCatalog();
    }

    private async void FetchMusic_Click(object sender, RoutedEventArgs e)
    {
        if (selected is not MusicEntry) return;
        var link = MusicLinkLookup.Text.Trim();
        if (!Uri.TryCreate(link, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
        {
            await Message("Enter a valid song or album link first."); return;
        }
        try
        {
            var endpoint = "https://api.song.link/v1-alpha.1/links?userCountry=US&songIfSingle=true&url=" + Uri.EscapeDataString(link);
            using var response = await Http.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var root = document.RootElement;
            var id = root.GetProperty("entityUniqueId").GetString() ?? throw new InvalidOperationException("Missing song ID.");
            var entity = root.GetProperty("entitiesByUniqueId").GetProperty(id);
            MusicName.Text = entity.GetProperty("title").GetString() ?? "";
            MusicArtist.Text = entity.GetProperty("artistName").GetString() ?? "";
            MusicImage.Text = entity.TryGetProperty("thumbnailUrl", out var image) ? image.GetString() ?? "" : "";
            MusicURL.Text = root.GetProperty("pageUrl").GetString() ?? link;
        }
        catch (Exception ex) { await Message("Could not fetch music details: " + ex.Message); }
    }

    private async void ChooseMusicImage_Click(object sender, RoutedEventArgs e)
    {
        if (selected is not MusicEntry music) return;
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".png"); picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg"); picker.FileTypeFilter.Add(".webp"); picker.FileTypeFilter.Add(".gif");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(this));
        var file = await picker.PickSingleFileAsync();
        if (file == null) return;
        music.PendingImagePath = file.Path;
        SelectedImageText.Text = file.Path;
        SetPreview(MusicImagePreview, file.Path);
    }

    private void BlogImage_TextChanged(object sender, TextChangedEventArgs e) => SetPreview(BlogImagePreview, BlogImage.Text);
    private void ProjectImage_TextChanged(object sender, TextChangedEventArgs e) => SetPreview(ProjectImagePreview, ProjectImage.Text);
    private void MusicImage_TextChanged(object sender, TextChangedEventArgs e) => SetPreview(MusicImagePreview, MusicImage.Text);

    private void SetPreview(Image image, string? value)
    {
        if (image == null) return;
        image.Source = PreviewSource(value);
        image.Visibility = image.Source == null ? Visibility.Collapsed : Visibility.Visible;
    }

    private ImageSource? PreviewSource(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        value = value.Trim();
        try
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out var webUri) &&
                webUri.Scheme is "http" or "https")
                return new BitmapImage(webUri);

            string path;
            if (Path.IsPathFullyQualified(value) && !value.StartsWith('/')) path = value;
            else
            {
                var root = Path.GetFullPath(WebsitePath.Text.Trim());
                path = Path.GetFullPath(Path.Combine(root, value.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));
                if (!path.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) return null;
            }
            if (!File.Exists(path)) return null;
            var bitmap = new BitmapImage { DecodePixelWidth = 900 };
            _ = LoadLocalPreviewAsync(bitmap, path);
            return bitmap;
        }
        catch (Exception) { return null; }
    }

    private static async System.Threading.Tasks.Task LoadLocalPreviewAsync(BitmapImage bitmap, string path)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            using var stream = await file.OpenAsync(FileAccessMode.Read);
            await bitmap.SetSourceAsync(stream);
        }
        catch (Exception) { /* The card or editor keeps its no-image placeholder. */ }
    }

    private void TileImage_Failed(object sender, ExceptionRoutedEventArgs e)
    {
        if (sender is Image image) image.Visibility = Visibility.Collapsed;
    }

    private void EditorImage_Failed(object sender, ExceptionRoutedEventArgs e)
    {
        if (sender is Image image) image.Visibility = Visibility.Collapsed;
    }

    private async void BrowseWebsite_Click(object sender, RoutedEventArgs e)
    {
        var path = await PickFolder();
        if (path != null) WebsitePath.Text = path;
    }

    private async void BrowseManager_Click(object sender, RoutedEventArgs e)
    {
        var path = await PickFolder();
        if (path != null) ManagerPath.Text = path;
    }

    private async System.Threading.Tasks.Task<string?> PickFolder()
    {
        var picker = new FolderPicker();
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(this));
        return (await picker.PickSingleFolderAsync())?.Path;
    }

    private async void SaveAndPublish_Click(object sender, RoutedEventArgs e)
    {
        CommitEditor();
        try
        {
            store.SetPaths(WebsitePath.Text, ManagerPath.Text);
            store.SaveAndPublish();
            ChangesList.ItemsSource = store.Changes();
            PublishStatus.Text = "Saved and published to " + store.Settings.WebsiteFolderPath;
        }
        catch (Exception ex)
        {
            PublishStatus.Text = "Publish failed: " + ex.Message;
            await Message(PublishStatus.Text);
        }
    }

    private async System.Threading.Tasks.Task Message(string text)
    {
        await new ContentDialog { XamlRoot = CatalogPanel.XamlRoot, Title = "Den Manager", Content = text, CloseButtonText = "OK" }.ShowAsync();
    }
}
