using EpsiDenTools.json;
using MelonUI.Default;
using MelonUI.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EpsiDenTools.Classes
{
    public static class PostGenerator
    {
        public static CancellationTokenSource source;
        public static string CurPath { get; set; }
        public static string MetaText { get; set; }
        public static string BodyText { get; set; }
        public static void GenerateFromMarkdown(ConsoleWindowManager manager)
        {
            TextBox input = new TextBox()
            {
                X = "0",
                Y = "0",
                Width = "90%",
                Height = "30%",
                Label = "Input the markdown file path"
            };

            input.OnEnter += Input_OnEnter;
            manager.AddElement(input);

            source = new CancellationTokenSource();
            while (!source.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }
            input.RenderThreadDeleteMe = true;

            ProgressBar bar = new ProgressBar()
            {
                X = "0",
                Y = "0",
                Width = "50%",
                Height = "3",
                Style = ProgressBar.ProgressBarStyle.Loading
            };
            manager.AddElement(bar);

            if (Directory.Exists(CurPath))
            {
                var files = Directory.GetFiles(CurPath);
                int count = 0;
                foreach (var file in files)
                {
                    manager.SetStatus($"Processing files [{count}/{files.Count()}]");
                    var postJson = ConvertBlogFileToJson(file);
                    if (postJson == null)
                    {
                        //bar.RenderThreadDeleteMe = true;
                        //manager.SetStatus("Metadata error!");
                        //return;
                        continue;
                    }
                    var html = ConvertBlogFileToHTML(postJson);

                    manager.SetStatus("Saving Json + HTML");
                    var txt = JsonConvert.SerializeObject(postJson);
                    Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/BlogPostsJson");
                    Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/BlogPostsHtml");
                    string pathjson = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/BlogPostsJson/{Path.GetFileNameWithoutExtension(postJson.BlogPostURL)}.json";
                    string pathhtml = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/BlogPostsHtml/{Path.GetFileNameWithoutExtension(postJson.BlogPostURL)}.html";
                    File.WriteAllText(pathjson, txt);
                    File.WriteAllText(pathhtml, html);
                    manager.SetStatus("Files Saved!");
                }
            }
            else
            {
                manager.SetStatus("Creating Blog Post JSON");
                var postJson = ConvertBlogFileToJson(CurPath);
                if (postJson == null)
                {
                    bar.RenderThreadDeleteMe = true;
                    manager.SetStatus("Metadata error or marked as draft!");
                    return;
                }
                var html = ConvertBlogFileToHTML(postJson);

                manager.SetStatus("Saving Json + HTML");
                var txt = JsonConvert.SerializeObject(postJson);
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/BlogPostsJson");
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/BlogPostsHtml");
                string pathjson = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/BlogPostsJson/{Path.GetFileNameWithoutExtension(postJson.BlogPostURL)}.json";
                string pathhtml = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/BlogPostsHtml/{Path.GetFileNameWithoutExtension(postJson.BlogPostURL)}.html";
                File.WriteAllText(pathjson, txt);
                File.WriteAllText(pathhtml, html);
                manager.SetStatus("Files Saved!");
            }

            bar.RenderThreadDeleteMe = true;
        }

        private static void Input_OnEnter(string path, TextBox element)
        {
            if (string.IsNullOrEmpty(path))
            {
                element.Clear();
                element.ParentWindow.SetStatus("File Not Found!");
                return;
            }
            path = path.Replace("\"", "");
            if (!Path.Exists(path))
            {
                element.Clear();
                element.ParentWindow.SetStatus("File Not Found!");
                return;
            }
            CurPath = path;
            source.Cancel();
        }

        public static string ConvertBlogFileToHTML(BlogPost post)
        {
            string html = DummyHTML.BlogHTML;

            // Meta
            html = html.Replace("{{HEADERIMAGE}}", post.HeaderImage);
            html = html.Replace("{{POSTTITLE}}", post.Title);
            html = html.Replace("{{POSTTABTITLE}}", post.Title);
            html = html.Replace("{{POSTDESCRIPTION}}", post.Description);
            html = html.Replace("{{POSTDATE}}", post.PostDate.ToString());
            html = html.Replace("{{POSTTAGS}}", DummyHTML.CreateTagsHTML(post.Tags));

            // Body
            html = html.Replace("{{POSTBODY}}", DummyHTML.CreatePostBodyHTML(BodyText));

            // Table Of Contents
            html = html.Replace("{{POSTTOC}}", DummyHTML.CreateTOCHTML(html, post.Title));

            return html;
        }

        public static void PackPosts(ConsoleWindowManager manager)
        {
            manager.SetStatus("Packing Posts");

            ProgressBar bar = new ProgressBar()
            {
                X = "0",
                Y = "0",
                Width = "50%",
                Height = "3",
                Style = ProgressBar.ProgressBarStyle.Loading,
                ProgressColor = Color.FromArgb(255, 132, 52, 30)
            };
            manager.AddElement(bar);


            var files = Directory.GetFiles($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/BlogPostsJson");

            List<BlogPost> items = new List<BlogPost>();
            foreach (var file in files.Where(x => !x.Contains("PackedBlogPosts.json")))
            {
                try
                {
                    string txt = File.ReadAllText(file);
                    BlogPost item = JsonConvert.DeserializeObject<BlogPost>(txt);
                    items.Add(item);
                }
                catch { }

            }

            manager.SetStatus("Saving Json");
            var packedtxt = JsonConvert.SerializeObject(items);
            File.WriteAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/PackedBlogPosts.json", packedtxt);
            manager.SetStatus("File Saved!");
            bar.RenderThreadDeleteMe = true;
        }

        public static BlogPost ConvertBlogFileToJson(string path)
        {
            BlogPost blogPost = new BlogPost();
            string txt = File.ReadAllText(path);

            if (txt.StartsWith("[DRAFT]"))
            {
                return null;
            }

            txt = txt.Replace("\r\n", "\n");
            int start = txt.IndexOf("~~~meta~~~");
            if (start == -1)
            {
                return null;
            }
            int end = txt.IndexOf("~~~metaEnd~~~");
            if (end <= start) 
            {
                return null;
            }
            int len = end - start;

            MetaText = txt.Substring(start, len);
            BodyText = txt.Remove(start, len + 13).Trim();

            string[] lines = MetaText.Split("\n");
            foreach (var line in lines)
            {
                string propertyName = line.Split(":").FirstOrDefault();
                string propertyValue = line.Replace($"{propertyName}:", "");
                if (propertyName == null && propertyValue == null)
                {
                    continue;
                }
                if (propertyName == "Title")
                {
                    blogPost.Title = propertyValue.Trim();
                }
                else if (propertyName == "Description")
                {
                    blogPost.Description = propertyValue.Trim();
                }
                else if (propertyName == "PostDate")
                {
                    blogPost.PostDate = DateTime.Parse(propertyValue.Trim());
                }
                else if (propertyName == "HeaderImage")
                {
                    if (!propertyValue.StartsWith("https://"))
                    {
                        propertyValue = $"/images/{Path.GetFileName(propertyValue).Trim()}";
                    }
                    blogPost.HeaderImage = propertyValue;
                }
                else if (propertyName == "Tags")
                {
                    string[] tags = propertyValue.Split(",");
                    blogPost.Tags = new List<string>();
                    blogPost.Tags.AddRange(tags);
                }
            }

            blogPost.BlogPostURL = $"/posts/{Path.GetFileNameWithoutExtension(path).Trim()}";

            return blogPost;
        }
    }
}
