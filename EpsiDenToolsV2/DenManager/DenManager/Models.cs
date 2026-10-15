using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DenManager;

public sealed class ManagerSettings
{
    public string WebsiteFolderPath { get; set; } = @"C:\Users\jhset\Documents\GitHub\EpsiRho.github.io\docs";
    public string ManagerSavePath { get; set; } = @"D:\Documents\EpsiDenTools";
}

public sealed class ManagerData
{
    public int RendererVersion { get; set; }
    public List<BlogEntry> BlogPosts { get; set; } = [];
    public List<ProjectEntry> Projects { get; set; } = [];
    public List<MusicEntry> MusicRecommendations { get; set; } = [];
}

public sealed class BlogEntry
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime PostDate { get; set; } = DateTime.Today;
    public string HeaderImage { get; set; } = "";
    public string BlogPostURL { get; set; } = "";
    public List<string> Tags { get; set; } = [];
    public string Markdown { get; set; } = "";
    public bool Draft { get; set; }
}

public sealed class ProjectEntry
{
    public string Name { get; set; } = "";
    public DateTime AnnounceDate { get; set; } = DateTime.Today;
    public string ImageURL { get; set; } = "";
    public string PostURL { get; set; } = "";
    public string ShortDescription { get; set; } = "";
    public string Description { get; set; } = "";
    public string DescriptionHtml { get; set; } = "";
    public List<string> Tags { get; set; } = [];
    public List<ProjectLink> Links { get; set; } = [];
    public string Markdown { get; set; } = "";
}

public sealed class ProjectLink
{
    public string Name { get; set; } = "";
    public string Link { get; set; } = "";
}

public sealed class MusicEntry
{
    public string Name { get; set; } = "";
    public string Artist { get; set; } = "";
    [JsonPropertyName("thumbnailURL")]
    public string ThumbnailURL { get; set; } = "";
    public string URL { get; set; } = "";
    public DateTime DateAdded { get; set; } = DateTime.Now;
    public float Rating { get; set; } = 3;
    public int Energy { get; set; } = 3;
    [JsonIgnore]
    public string? PendingImagePath { get; set; }
}
