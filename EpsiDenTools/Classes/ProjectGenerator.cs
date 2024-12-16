using EpsiDenTools.json;
using MelonUI.Default;
using MelonUI.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpsiDenTools.Classes
{
    public static class ProjectGenerator
    {
        public static CancellationTokenSource source;
        public static string CurPath { get; set; }
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
                foreach(var file in files)
                {
                    manager.SetStatus($"Processing files [{count}/{files.Count()}]");
                    var postJson = ConvertFileToJson(file);
                    if (postJson == null)
                    {
                        bar.RenderThreadDeleteMe = true;
                        manager.SetStatus("Metadata error!");
                        return;
                    }

                    manager.SetStatus("Saving Json + HTML");
                    var txt = JsonConvert.SerializeObject(postJson);
                    Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/ProjectsJson");
                    string pathjson = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/ProjectsJson/{Path.GetFileNameWithoutExtension(postJson.PostURL)}.json";
                    File.WriteAllText(pathjson, txt);
                    manager.SetStatus("Files Saved!");
                }
            }
            else
            {
                manager.SetStatus("Creating Blog Post JSON");
                var postJson = ConvertFileToJson(CurPath);
                if (postJson == null)
                {
                    bar.RenderThreadDeleteMe = true;
                    manager.SetStatus("Metadata error!");
                    return;
                }

                manager.SetStatus("Saving Json + HTML");
                var txt = JsonConvert.SerializeObject(postJson);
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/ProjectsJson");
                string pathjson = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/ProjectsJson/{Path.GetFileNameWithoutExtension(postJson.PostURL)}.json";
                File.WriteAllText(pathjson, txt);
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
        public static void PackProjects(ConsoleWindowManager manager)
        {
            manager.SetStatus("Packing Projects");

            ProgressBar bar = new ProgressBar()
            {
                X = "0",
                Y = "0",
                Width = "50%",
                Height = "3",
                Style = ProgressBar.ProgressBarStyle.Loading,
                ProgressColor = Color.FromArgb(255, 42, 149, 160)
            };
            manager.AddElement(bar);


            var files = Directory.GetFiles($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/ProjectsJson");

            List<ProjectPost> items = new List<ProjectPost>();
            foreach (var file in files.Where(x => !x.Contains("PackedProjects.json")))
            {
                try
                {
                    string txt = File.ReadAllText(file);
                    ProjectPost item = JsonConvert.DeserializeObject<ProjectPost>(txt);
                    items.Add(item);
                }
                catch { }

            }

            manager.SetStatus("Saving Json");
            var packedtxt = JsonConvert.SerializeObject(items);
            File.WriteAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/PackedProjects.json", packedtxt);
            manager.SetStatus("File Saved!");
            bar.RenderThreadDeleteMe = true;
        }
        public static ProjectPost ConvertFileToJson(string path)
        {
            ProjectPost project = new ProjectPost();
            string txt = File.ReadAllText(path);
            txt = txt.Replace("\r\n", "\n");

            var lines = txt.Split("\n");

            bool DescriptionFlag = false;
            foreach (var line in lines)
            {
                string propertyName = line.Split("==").FirstOrDefault();
                string propertyValue = line.Replace($"{propertyName}==", "");

                if (DescriptionFlag)
                {
                    project.Description += $"{propertyValue}\n";
                    continue;
                }

                if (propertyName == null && propertyValue == null)
                {
                    continue;
                }
                if (propertyName == "Name")
                {
                    project.Name = propertyValue.Trim();
                }
                else if (propertyName == "ShortDescription")
                {
                    project.ShortDescription = propertyValue.Trim();
                }
                else if (propertyName == "AnnounceDate")
                {
                    project.AnnounceDate = DateTime.Parse(propertyValue.Trim());
                }
                else if (propertyName == "ImageURL")
                {
                    if (!propertyValue.StartsWith("https://"))
                    {
                        propertyValue = $"/images/{Path.GetFileName(propertyValue)}";
                    }
                    project.ImageURL = propertyValue;
                }
                else if (propertyName == "Tags")
                {
                    string[] tags = propertyValue.Split(",");
                    project.Tags = new List<string>();
                    project.Tags.AddRange(tags);
                }
                else if (propertyName == "Links")
                {
                    string[] tags = propertyValue.Split(",");
                    project.Links = new List<ProjectLink>();
                    for (int i = 0; i < tags.Length; i += 2)
                    {
                        project.Links.Add(new ProjectLink()
                        {
                            Name = tags[i],
                            Link = tags[i + 1],
                        });
                    }
                }
                else if (propertyName == "Description")
                {
                    DescriptionFlag = true;
                }
            }

            project.PostURL = $"{Path.GetFileNameWithoutExtension(path)}";

            return project;
        }
    }
}
