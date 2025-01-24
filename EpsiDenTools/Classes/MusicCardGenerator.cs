using EpsiDenTools.json;
using MelonUI.Default;
using MelonUI.Managers;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EpsiDenTools.Classes
{
    public static class MusicCardGenerator
    {
        public static CancellationTokenSource source;
        public static string Link { get; set; }
        public static int Energy { get; set; }
        public static float Rating { get; set; }
        public static string CustomProperty { get; set; }
        public static MusicItem CustomItem { get; set; }
        public static void CreateCard(ConsoleWindowManager manager)
        {
            TextBox input = new TextBox()
            {
                X = "0",
                Y = "0",
                Width = "90%",
                Height = "30%",
                Label = "Input any song link"
            };

            input.OnEnter += Input_OnEnter;
            source = new CancellationTokenSource();

            manager.AddElement(input);
            while (!source.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            input.RenderThreadDeleteMe = true;

            OptionsMenu rating = new OptionsMenu()
            {
                X = "0",
                Y = "0",
                Width = "50%",
                Height = "70%",
                MenuName = "Enter a song rating"
            };

            rating.Options.Add(new MenuItem("1", () =>
            {
                Rating = 1.0f;
                rating.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));
            rating.Options.Add(new MenuItem("1.5", () =>
            {
                Rating = 1.5f;
                rating.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));
            rating.Options.Add(new MenuItem("2", () =>
            {
                Rating = 2.0f;
                rating.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));
            rating.Options.Add(new MenuItem("2.5", () =>
            {
                Rating = 2.5f;
                rating.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));
            rating.Options.Add(new MenuItem("3", () =>
            {
                Rating = 3.0f;
                rating.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));
            rating.Options.Add(new MenuItem("3.5", () =>
            {
                Rating = 3.5f;
                rating.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));
            rating.Options.Add(new MenuItem("4", () =>
            {
                Rating = 4.0f;
                rating.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));
            rating.Options.Add(new MenuItem("4.5", () =>
            {
                Rating = 4.5f;
                rating.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));
            rating.Options.Add(new MenuItem("5", () =>
            {
                Rating = 5.0f;
                rating.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));

            source = new CancellationTokenSource();

            manager.AddElement(rating);
            while (!source.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            OptionsMenu energy = new OptionsMenu()
            {
                X = "0",
                Y = "0",
                Width = "50%",
                Height = "70%",
                MenuName = "Enter a song energy"
            };

            energy.Options.Add(new MenuItem("1", () =>
            {
                Energy = 1;
                energy.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));
            energy.Options.Add(new MenuItem("2", () =>
            {
                Energy = 2;
                energy.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));
            energy.Options.Add(new MenuItem("3", () =>
            {
                Energy = 3;
                energy.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));
            energy.Options.Add(new MenuItem("4", () =>
            {
                Energy = 4;
                energy.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));
            energy.Options.Add(new MenuItem("5", () =>
            {
                Energy = 5;
                energy.RenderThreadDeleteMe = true;
                source.Cancel();
            }
            ));
            

            source = new CancellationTokenSource();

            manager.AddElement(energy);
            while (!source.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            ProgressBar bar = new ProgressBar()
            {
                X = "0",
                Y = "0",
                Width = "50%",
                Height = "3",
                Style = ProgressBar.ProgressBarStyle.Loading,
                ProgressColor = Color.FromArgb(255, 25, 187, 255)
            };
            manager.AddElement(bar);

            manager.SetStatus("Getting song info");
            var item = GetSongInfo();

            manager.SetStatus("Saving Json");
            var txt = JsonConvert.SerializeObject(item);
            Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/MusicCards");
            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/MusicCards/{item.Name.Replace("/", "").Replace("\\", "").Replace(":", "")}-{item.Artist.Replace("/", "").Replace("\\", "").Replace(":", "")}.json";
            path = path.Replace("|", "")
                       .Replace("?", "")
                       .Replace("*", "")
                       .Replace("\"", "")
                       .Replace("<", "")
                       .Replace(">", "")
                       .Replace("!", "");
            File.WriteAllText(path, txt);
            manager.SetStatus("File Saved!");
            bar.RenderThreadDeleteMe = true;
        }
        private static void Input_OnEnter(string link, TextBox element)
        {
            Link = link;
            source.Cancel();
        }
        private static void CustomInput_OnEnter(string link, TextBox element)
        {
            switch (CustomProperty)
            {
                case "Name":
                    CustomItem.Name = link;
                    break;
                case "Artist":
                    CustomItem.Artist = link;
                    break;
                case "Thumbnail":
                    CustomItem.thumbnailURL = link;
                    break;
                case "URL":
                    CustomItem.URL = link;
                    break;
                case "Rating":
                    CustomItem.Rating = float.Parse(link);
                    break;
                case "Energy":
                    CustomItem.Energy = int.Parse(link);
                    break;
            }
            source.Cancel();
        }
        private static MusicItem GetSongInfo()
        {
            RestClient client = new RestClient(new Uri("https://api.song.link/v1-alpha.1/"));
            RestRequest req = new RestRequest("links", Method.Get);
            req.AddQueryParameter("url", Link);
            req.AddQueryParameter("userCountry", "US");
            req.AddQueryParameter("songIfSingle", "true");

            var res = client.Execute(req);

            var json = JsonConvert.DeserializeObject<OdesliResponse>(res.Content);

            string Title = json.entitiesByUniqueId[json.entityUniqueId].title;
            string Artist = json.entitiesByUniqueId[json.entityUniqueId].artistName;
            string ImageUrl = json.entitiesByUniqueId[json.entityUniqueId].thumbnailUrl;
            string PageUrl = json.pageUrl;

            var item = new MusicItem()
            {
                Name = Title,
                Artist = Artist,
                thumbnailURL = ImageUrl,
                URL = PageUrl,
                Rating = Rating,
                Energy = Energy,
                DateAdded = DateTime.Now
            };

            return item;
        }
        public static void CreateCustomCard(ConsoleWindowManager manager)
        {
            CustomItem = new MusicItem();


            manager.SetStatus("[Name/A/T/U/R/E]");
            CustomProperty = "Name";
            TextBox input = new TextBox()
            {
                X = "0",
                Y = "0",
                Width = "90%",
                Height = "30%",
                Label = "Input the item name"
            };

            input.OnEnter += CustomInput_OnEnter;
            manager.AddElement(input);

            source = new CancellationTokenSource();
            while (!source.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            manager.SetStatus("[N/Artist/T/U/R/E]");
            input.Label = "Input the artist name";
            CustomProperty = "Artist";
            input.Clear();
            source = new CancellationTokenSource();
            while (!source.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            manager.SetStatus("[N/A/Thumbnail/U/R/E]");
            input.Label = "Input the thumbnail's URL";
            CustomProperty = "Thumbnail";
            input.Clear();
            source = new CancellationTokenSource();
            while (!source.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            manager.SetStatus("[N/A/T/URL/R/E]");
            input.Label = "Input the URL";
            CustomProperty = "URL";
            input.Clear();
            source = new CancellationTokenSource();
            while (!source.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            manager.SetStatus("[N/A/T/U/Ratings/E]");
            input.Label = "Input the rating";
            CustomProperty = "Rating";
            input.Clear();
            source = new CancellationTokenSource();
            while (!source.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }
            manager.SetStatus("[N/A/T/U/R/Energy]");
            input.Label = "Input the energy";
            CustomProperty = "Energy";
            input.Clear();
            source = new CancellationTokenSource();
            while (!source.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            input.RenderThreadDeleteMe = true;


            manager.SetStatus("Saving Json");
            CustomItem.DateAdded = DateTime.Now;
            var txt = JsonConvert.SerializeObject(CustomItem);
            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/MusicCards/{CustomItem.Name.Replace("/", "").Replace("\\", "").Replace(":", "")}-{CustomItem.Artist.Replace("/", "").Replace("\\", "").Replace(":", "")}.json";
            path = path.Replace("|", "")
                       .Replace("?", "")
                       .Replace("*", "")
                       .Replace("\"", "")
                       .Replace("<", "")
                       .Replace(">", "")
                       .Replace("!", "");
            File.WriteAllText(path, txt);
            manager.SetStatus("File Saved!");

        }

        public static void PackMusicCards(ConsoleWindowManager manager)
        {
            manager.SetStatus("Packing Cards");

            ProgressBar bar = new ProgressBar()
            {
                X = "0",
                Y = "0",
                Width = "50%",
                Height = "3",
                Style = ProgressBar.ProgressBarStyle.Loading,
                ProgressColor = Color.FromArgb(255, 25, 187, 255)
            };
            manager.AddElement(bar);


            var files = Directory.GetFiles($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/MusicCards");

            List<MusicItem> items = new List<MusicItem>();
            foreach (var file in files.Where(x=>!x.Contains("PackedMusicCards.json")))
            {
                try
                {
                    string txt = File.ReadAllText(file);
                    MusicItem item = JsonConvert.DeserializeObject<MusicItem>(txt);
                    items.Add(item);
                }
                catch { }

            }

            manager.SetStatus("Saving Json");
            var packedtxt = JsonConvert.SerializeObject(items);
            File.WriteAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/EpsiDenTools/PackedMusicCards.json", packedtxt);
            manager.SetStatus("File Saved!");
            bar.RenderThreadDeleteMe = true;
        }
    }
}
