using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpsiDenTools.Classes
{
    public static class DummyHTML
    {
        public static string BlogHTML =
@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{{POSTTABTITLE}}</title>
    <link rel=""icon"" type=""image/x-icon"" href=""/images/favicon.ico"">
    <link rel=""stylesheet"" href=""/styles/main.css"">
    <link rel=""stylesheet"" href=""/styles/posts.css"">
<link rel=""stylesheet"" href=""/styles/comments.css"">
</head>
<body>
    <div class=""container"">
        <aside class=""sidebar"">
            <div class=""profile"">
                <img src=""/images/pfp.png"" alt=""Profile"" class=""profile-img"">
                <h1 class=""profile-name"">Epsi's Den</h1>
                <p class=""profile-bio"">Software developer, Lava lamp dog, totally a professional</p>
                <div class=""social-links"">
                    <a href=""https://bsky.app/profile/epsirho.com"" aria-label=""Bluesky"" class=""social-link"">
                        <svg xmlns=""http://www.w3.org/2000/svg"" class=""icon icon-tabler icon-tabler-brand-bluesky"" width=""24"" height=""24"" viewBox=""0 0 24 24"" stroke-width=""2"" stroke=""currentColor"" fill=""none"" stroke-linecap=""round"" stroke-linejoin=""round"">
                            <path stroke=""none"" d=""M0 0h24v24H0z"" fill=""none""></path>
                            <path d=""M6.335 5.144c-1.654 -1.199 -4.335 -2.127 -4.335 .826c0 .59 .35 4.953 .556 5.661c.713 2.463 3.13 2.75 5.444 2.369c-4.045 .665 -4.889 3.208 -2.667 5.41c1.03 1.018 1.913 1.59 2.667 1.59c2 0 3.134 -2.769 3.5 -3.5c.333 -.667 .5 -1.167 .5 -1.5c0 .333 .167 .833 .5 1.5c.366 .731 1.5 3.5 3.5 3.5c.754 0 1.637 -.571 2.667 -1.59c2.222 -2.203 1.378 -4.746 -2.667 -5.41c2.314 .38 4.73 .094 5.444 -2.369c.206 -.708 .556 -5.072 .556 -5.661c0 -2.953 -2.68 -2.025 -4.335 -.826c-2.293 1.662 -4.76 5.048 -5.665 6.856c-.905 -1.808 -3.372 -5.194 -5.665 -6.856z""></path>
                        </svg>
                    </a>
                    <a href=""https://github.com/EpsiRho"" aria-label=""GitHub"" class=""social-link"">
                        <svg xmlns=""http://www.w3.org/2000/svg"" class=""icon icon-tabler icon-tabler-brand-github"" width=""24"" height=""24"" viewBox=""0 0 24 24"" stroke-width=""2"" stroke=""currentColor"" fill=""none"" stroke-linecap=""round"" stroke-linejoin=""round"">
                            <path stroke=""none"" d=""M0 0h24v24H0z"" fill=""none""></path>
                            <path d=""M9 19c-4.3 1.4 -4.3 -2.5 -6 -3m12 5v-3.5c0 -1 .1 -1.4 -.5 -2c2.8 -.3 5.5 -1.4 5.5 -6a4.6 4.6 0 0 0 -1.3 -3.2a4.2 4.2 0 0 0 -.1 -3.2s-1.1 -.3 -3.5 1.3a12.3 12.3 0 0 0 -6.2 0c-2.4 -1.6 -3.5 -1.3 -3.5 -1.3a4.2 4.2 0 0 0 -.1 3.2a4.6 4.6 0 0 0 -1.3 3.2c0 4.6 2.7 5.7 5.5 6c-.6 .6 -.6 1.2 -.5 2v3.5""></path>
                          </svg>
                    </a>
                    <a href=""https://furry.engineer/@epsi"" aria-label=""Mastodon"" class=""social-link"">
                        <svg xmlns=""http://www.w3.org/2000/svg"" class=""icon icon-tabler icon-tabler-brand-mastodon"" width=""24"" height=""24"" viewBox=""0 0 24 24"" stroke-width=""2"" stroke=""currentColor"" fill=""none"" stroke-linecap=""round"" stroke-linejoin=""round"">
                            <path stroke=""none"" d=""M0 0h24v24H0z"" fill=""none""></path>
                            <path d=""M18.648 15.254c-1.816 1.763 -6.648 1.626 -6.648 1.626a18.262 18.262 0 0 1 -3.288 -.256c1.127 1.985 4.12 2.81 8.982 2.475c-1.945 2.013 -13.598 5.257 -13.668 -7.636l-.026 -1.154c0 -3.036 .023 -4.115 1.352 -5.633c1.671 -1.91 6.648 -1.666 6.648 -1.666s4.977 -.243 6.648 1.667c1.329 1.518 1.352 2.597 1.352 5.633s-.456 4.074 -1.352 4.944z""></path><path d=""M12 11.204v-2.926c0 -1.258 -.895 -2.278 -2 -2.278s-2 1.02 -2 2.278v4.722m4 -4.722c0 -1.258 .895 -2.278 2 -2.278s2 1.02 2 2.278v4.722""></path>
                        </svg>
                    </a>
                </div>
            </div>
            <nav>
                <ul class=""nav-menu"">
                    <li class=""nav-item"">
                        <a href=""/"" class=""nav-link"">
                            <svg xmlns=""http://www.w3.org/2000/svg"" class=""icon icon-tabler icon-tabler-home"" width=""24"" height=""24"" viewBox=""0 0 24 24"" stroke-width=""2"" stroke=""currentColor"" fill=""none"" stroke-linecap=""round"" stroke-linejoin=""round"">
                                <path stroke=""none"" d=""M0 0h24v24H0z""></path>
                                <polyline points=""5 12 3 12 12 3 21 12 19 12""></polyline>
                                <path d=""M5 12v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2 -2v-7""></path>
                                <path d=""M9 21v-6a2 2 0 0 1 2 -2h2a2 2 0 0 1 2 2v6""></path>
                              </svg>
                            Home
                        </a>
                    </li>
                    <li class=""nav-item"">
                        <a href=""/site/about"" class=""nav-link"">
                            <svg xmlns=""http://www.w3.org/2000/svg"" class=""icon icon-tabler icon-tabler-user-square"" width=""24"" height=""24"" viewBox=""0 0 24 24"" stroke-width=""2"" stroke=""currentColor"" fill=""none"" stroke-linecap=""round"" stroke-linejoin=""round""><path stroke=""none"" d=""M0 0h24v24H0z"" fill=""none""></path><path d=""M9 10a3 3 0 1 0 6 0a3 3 0 0 0 -6 0""></path><path d=""M6 21v-1a4 4 0 0 1 4 -4h4a4 4 0 0 1 4 4v1""></path><path d=""M3 5a2 2 0 0 1 2 -2h14a2 2 0 0 1 2 2v14a2 2 0 0 1 -2 2h-14a2 2 0 0 1 -2 -2v-14z""></path>
                            </svg>
                             About
                            </a>
                    </li>
                    <li class=""nav-item"">
                        <a href=""/site/posts"" class=""nav-link"">
                            <svg xmlns=""http://www.w3.org/2000/svg"" class=""icon icon-tabler icon-tabler-layout-dashboard"" width=""24"" height=""24"" viewBox=""0 0 24 24"" stroke-width=""2"" stroke=""currentColor"" fill=""none"" stroke-linecap=""round"" stroke-linejoin=""round""><path stroke=""none"" d=""M0 0h24v24H0z"" fill=""none""></path><path d=""M4 4h6v8h-6z""></path><path d=""M4 16h6v4h-6z""></path><path d=""M14 12h6v8h-6z""></path><path d=""M14 4h6v4h-6z""></path></svg>
                             Blog Posts
                        </a>
                    </li>
                    <li class=""nav-item"">
                        <a href=""/site/projects"" class=""nav-link"">
                            <svg xmlns=""http://www.w3.org/2000/svg""  width=""24""  height=""24""  viewBox=""0 0 24 24""  fill=""none""  stroke=""currentColor""  stroke-width=""2""  stroke-linecap=""round""  stroke-linejoin=""round""  class=""icon icon-tabler icons-tabler-outline icon-tabler-code""><path stroke=""none"" d=""M0 0h24v24H0z"" fill=""none""/><path d=""M7 8l-4 4l4 4"" /><path d=""M17 8l4 4l-4 4"" /><path d=""M14 4l-4 16"" /></svg>
                             Projects
                        </a>
                    </li>
                    <li class=""nav-item"">
                        <a href=""/site/music"" class=""nav-link"">
                            <svg  xmlns=""http://www.w3.org/2000/svg""  width=""24""  height=""24""  viewBox=""0 0 24 24""  fill=""none""  stroke=""currentColor""  stroke-width=""2""  stroke-linecap=""round""  stroke-linejoin=""round""  class=""icon icon-tabler icons-tabler-outline icon-tabler-music""><path stroke=""none"" d=""M0 0h24v24H0z"" fill=""none""/><path d=""M3 17a3 3 0 1 0 6 0a3 3 0 0 0 -6 0"" /><path d=""M13 17a3 3 0 1 0 6 0a3 3 0 0 0 -6 0"" /><path d=""M9 17v-13h10v13"" /><path d=""M9 8h10"" /></svg>
                             Music Recommendations
                        </a>
                    </li>
                </ul>
            </nav>
        </aside>

        <main class=""main-content"">
            <article class=""blog-post-page"">
                <img src=""{{HEADERIMAGE}}"" alt=""Blog Header"" class=""post-header-image"">
                </br>
                {{POSTTAGS}}
                <h1 class=""post-title"" id=""mdpost-title"">{{POSTTITLE}}</h1>
                <p class=""post-description"">{{POSTDESCRIPTION}}</p>
                <div class=""post-date"">{{POSTDATE}}</div>
                <div class=""post-body"">
                    {{POSTBODY}}
                </div>
            </article>
            <div class=""blog-post-page"">
                <div id=""comments-header"">
                    <h1>Comments</h1>
                </div>
                <hr id=""comments-divider""/>
                <div id=""comments-section"">
                    <div id=""loading-bar-container"" style=""position: relative; width: 100%; height: 5px; background-color: var(--accent-secondary); overflow: hidden; display: none;"">
                        <div id=""loading-bar"" style=""width: 30%; height: 100%; background-color: var(--accent); position: absolute; animation: loading 1.5s infinite;""></div>
                      </div>
                      
                </div>
            </div>
        </main>

        <div id=""settings-bg"">
            <div id=""settings-modal"" class=""themebtn"">
                <a aria-label=""page-settings"" class=""social-link"" id=""close-modal"">
                    <svg  xmlns=""http://www.w3.org/2000/svg""  width=""24""  height=""24""  viewBox=""0 0 24 24""  fill=""none""  stroke=""currentColor""  stroke-width=""2""  stroke-linecap=""round""  stroke-linejoin=""round""  class=""icon icon-tabler icons-tabler-outline icon-tabler-x""><path stroke=""none"" d=""M0 0h24v24H0z"" fill=""none""/><path d=""M18 6l-12 12"" /><path d=""M6 6l12 12"" /></svg>
                </a>
                <p class=""themebtn"" style=""font-size: 0.8em; margin-left: 0px; position: relative;""><i class=""themebtn""><span class=""version-display""></span> &#x2022; <a class=""changelog-link themebtn"" href=""/site/changelog"">changelog</a></i></p>
                <label class=""themebtn"">Select Theme:</label>
                <div class=""theme-picker themebtn"">
                    <div class=""theme-option tooltip themebtn"" id=""theme-option-notebook"">
                        <span class=""tooltiptext themebtn"">Notebook</span>
                        <div class=""theme-sidebar themebtn"" style=""background-color: #f1e4c1;""></div>
                        <div class=""theme-main themebtn"" style=""background-color: #fff9e8;"">
                            <div class=""theme-card themebtn"" style=""background-color: #f7f2e0;""></div>
                        </div>
                    </div>
                    <div class=""theme-option tooltip themebtn"" id=""theme-option-light"" >
                        <span class=""tooltiptext themebtn"">Flashbang</span>
                        <div class=""theme-sidebar themebtn"" style=""background-color: rgb(216, 216, 216);""></div>
                        <div class=""theme-main themebtn"" style=""background-color: rgb(228, 228, 228);"">
                            <div class=""theme-card themebtn"" style=""background-color: rgb(199, 199, 199);""></div>
                        </div>
                    </div>
                    <div class=""theme-option tooltip themebtn"" id=""theme-option-dark"">
                        <span class=""tooltiptext themebtn"">Dark</span>
                        <div class=""theme-sidebar themebtn"" style=""background-color: #2a2a2a;""></div>
                        <div class=""theme-main themebtn"" style=""background-color: #1a1a1a;"">
                            <div class=""theme-card themebtn"" style=""background-color: rgb(53, 53, 53);""></div>
                        </div>
                    </div>
                    <div class=""theme-option tooltip themebtn"" id=""theme-option-transparent"">
                        <span class=""tooltiptext themebtn"">Transparent</span>
                        <div class=""theme-sidebar themebtn"" style=""background: linear-gradient(230deg, rgb(48, 57, 68) 0%, rgb(54, 61, 63) 50%, rgb(45, 54, 61) 100%);""></div>
                        <div class=""theme-main themebtn"" style=""background: linear-gradient(120deg, rgb(48, 57, 68) 0%, rgb(54, 61, 63) 50%, rgb(45, 54, 61) 100%);"">
                            <div class=""theme-card themebtn"" style=""background-color: #3a3a3a;""></div>
                        </div>
                    </div>
                    <div class=""theme-option tooltip themebtn"" id=""theme-option-ocean"">
                        <span class=""tooltiptext themebtn"">Ocean</span>
                        <div class=""theme-sidebar themebtn"" style=""background-color: #073642;""></div>
                        <div class=""theme-main themebtn"" style=""background-color: #002b36;"">
                            <div class=""theme-card themebtn"" style=""background-color: #083c4a;""></div>
                        </div>
                    </div>
                    <div class=""theme-option tooltip themebtn"" id=""theme-option-dream"">
                        <span class=""tooltiptext themebtn"">Dream</span>
                        <div class=""theme-sidebar themebtn"" style=""background-color: #2c2a3e;""></div>
                        <div class=""theme-main themebtn"" style=""background-color: #2b2336;"">
                            <div class=""theme-card themebtn"" style=""background-color: #3b3f51;""></div>
                        </div>
                    </div>
                    <div class=""theme-option tooltip themebtn"" id=""theme-option-fields"">
                        <span class=""tooltiptext themebtn"">Fields</span>
                        <div class=""theme-sidebar themebtn"" style=""background-color: #365142;""></div>
                        <div class=""theme-main themebtn"" style=""background-color: #283c2e;"">
                            <div class=""theme-card themebtn"" style=""background-color: #2d543f;""></div>
                        </div>
                    </div>
                    <div class=""theme-option tooltip themebtn"" id=""theme-option-sunset"">
                        <span class=""tooltiptext themebtn"">Sunset</span>
                        <div class=""theme-sidebar themebtn"" style=""background-color: #4c1919;""></div>
                        <div class=""theme-main themebtn"" style=""background-color: #381324;"">
                            <div class=""theme-card themebtn"" style=""background-color: #403949;""></div>
                        </div>
                    </div>
                    <div class=""theme-option tooltip themebtn"" id=""theme-option-matrix"">
                        <span class=""tooltiptext themebtn"">Matrix</span>
                        <div class=""theme-sidebar themebtn"" style=""background-color: #000000;""></div>
                        <div class=""theme-main themebtn"" style=""background-color: #0e0e0e;"">
                            <div class=""theme-card themebtn"" style=""background-color: #1a1a1a;""></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <aside class=""toc"" onclick="""">
            <div class=""toc-icon"">☰</div> 
            <div class=""toc-content"" id=""toc-site-nav""></div>
            <div class=""toc-content"" id=""toc-post-nav"">
            <ul>
                <a aria-label=""page-settings"" class=""social-link"" id=""page-settings"">
                    <li class=""settingsbtn"">
                        <svg xmlns=""http://www.w3.org/2000/svg""  width=""24""  height=""24""  viewBox=""0 0 24 24""  fill=""none""  stroke=""currentColor""  stroke-width=""2""  stroke-linecap=""round""  stroke-linejoin=""round""  
                        class=""icon icon-tabler icons-tabler-outline icon-tabler-adjustments settingsbtn ""
                        style=""margin-right: 3px;"">
                            <path stroke=""none"" d=""M0 0h24v24H0z"" fill=""none"" class=""settingsbtn""/>
                            <path d=""M4 10a2 2 0 1 0 4 0a2 2 0 0 0 -4 0"" class=""settingsbtn""/>
                            <path d=""M6 4v4"" class=""settingsbtn""/>
                            <path d=""M6 12v8"" class=""settingsbtn""/>
                            <path d=""M10 16a2 2 0 1 0 4 0a2 2 0 0 0 -4 0"" class=""settingsbtn""/>
                            <path d=""M12 4v10"" class=""settingsbtn""/>
                            <path d=""M12 18v2"" class=""settingsbtn""/>
                            <path d=""M16 7a2 2 0 1 0 4 0a2 2 0 0 0 -4 0"" class=""settingsbtn""/>
                            <path d=""M18 4v1"" class=""settingsbtn""/>
                            <path d=""M18 9v11"" class=""settingsbtn""/>
                        </svg>
                        Settings
                    </li>
                </a>
                <hr style=""margin: 10px 10px 10px 5px;"" id=""setdivider""/>
                {{POSTTOC}}
            </ul>
            </div>
        </aside>
    </div>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/dompurify/2.3.10/purify.min.js""></script>
    <script src=""/js/toc.js""></script>
    <script src=""/js/settings.js""></script>
    <script src=""/js/comments.js""></script>
    <link id=""codeTheme"" rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/styles/atom-one-dark.min.css"">
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/highlight.min.js""></script>

    <script>hljs.highlightAll();</script>
</body>
</html>

";
        public static string CreateTagsHTML(List<string> tags)
        {
            string html = "";
            foreach (string tag in tags) 
            {
                html += $"<span class=\"post-tag\">{tag}</span>\n";
            }
            return html;
        }

        public static string CreateTOCHTML(string curhtml, string title)
        {
            var headers = curhtml.Split('\n').Where(x=>x.Contains("<h1") || x.Contains("<h2")).ToList();

            string html = "";
            foreach (string header in headers)
            {
                int idx = header.IndexOf("id=\"");
                if(idx == -1)
                {
                    continue;
                }
                idx = idx + 4;

                var end1 = header.IndexOf("\"", idx);
                var id = header.Substring(idx, end1 - idx);
                int idx2 = header.IndexOf(">") + 1;
                var end2 = header.IndexOf("<", idx2);
                var id2 = header.Substring(idx2, end2 - idx2);
                html += $"<a href=\"#{id}\"><li>{id2}</li></a>\n";
            }
            return html;
        }

        public static string CreatePostBodyHTML(string body)
        {
            string html = "";
            string[] lines = body.Split('\n');
            List<string> output = new List<string>();
            bool flag = false;
            foreach (string line in lines)
            {
                var sanatized = line;
                if (sanatized.StartsWith("```"))
                {
                    flag = !flag;
                    output.Add(sanatized);
                    continue;
                }
                if (sanatized == "" && !flag)
                {
                    output.Add("</br>");
                }
                else
                {
                    output.Add(sanatized);
                }
            }
            var temp = output.ToArray();
            temp = AddBullets(temp);
            temp = AddStyle(temp);
            temp = ConvertParagraphs(temp);
            temp = AddCodeBlocks(temp);
            temp = AddHeaders(temp);
            temp = AddLinks(temp);
            temp = AddBlockQuotes(temp);
            temp = AddImages(temp);

            foreach (string line in temp)
            {
                html += $"{line}\n";
            }
            return html;
        }
        private static string[] ConvertParagraphs(string[] lines)
        {
            List<string> output = new List<string>();
            bool flag = false;
            foreach (string line in lines)
            {
                var sanatized = line.Trim();
                if(sanatized == "")
                {
                    output.Add("</br>");
                    continue;
                }

                if (sanatized.StartsWith("```"))
                {
                    flag = !flag;
                    output.Add(line);
                    continue;
                }
                else if (sanatized.StartsWith("#") || sanatized.StartsWith("-")
                      || sanatized.StartsWith("*") || sanatized.StartsWith(">")
                      || sanatized.StartsWith("!") || sanatized.StartsWith("<"))
                {
                    output.Add(line);
                    continue;
                }
                if (flag)
                {
                    output.Add(line);
                    continue;
                }

                var temp = $"<p>{sanatized}</p>";
                output.Add(temp);
            }

            return output.ToArray();
        }
        private static string[] AddHeaders(string[] lines)
        {
            List<string> output = new List<string>();
            foreach (string line in lines)
            {
                var sanatized = line.Trim();
                if (!sanatized.StartsWith("#") || sanatized.StartsWith("<"))
                {
                    output.Add(line);
                    continue;
                }

                string headerMarker = sanatized.Split("# ").FirstOrDefault();
                headerMarker += "#";
                if (headerMarker == null)
                {
                    output.Add(line);
                    continue;
                }

                int count = headerMarker.Length;
                var temp = $"<h{count} id=\"mdheadr-{sanatized.Remove(0, count).Replace(" ", "-")}\">{sanatized.Remove(0, count)}</h{count}>";
                output.Add(temp);
            }

            return output.ToArray();
        }
        private static string[] AddLinks(string[] lines)
        {
            List<string> output = new List<string>();
            int lastRank = 0;
            foreach (string line in lines)
            {
                var sanatized = line;

                if (sanatized.Contains("!["))
                {
                    output.Add(line);
                    continue;
                }

                if (!sanatized.Contains("[") || !sanatized.Contains("]"))
                {
                    output.Add(line);
                    continue;
                }

                string modified = "";
                string link = "";
                int flag = 0;
                foreach (var c in sanatized)
                {
                    if(c == '[')
                    {
                        int precheck = 0;
                        foreach (var t in sanatized)
                        {
                            if (t == '[')
                            {
                                precheck = 1;
                            }
                            else if (t == ']' && precheck == 1)
                            {
                                precheck = 2;
                            }
                            else if (t == '(' && precheck == 2)
                            {
                                precheck = 3;
                            }
                            else if (t == ')' && precheck == 3)
                            {
                                precheck = 4;
                                break;
                            }
                            else
                            {
                                if (precheck == 2)
                                {
                                    precheck = 2;
                                    break;
                                }
                            }
                        }
                        if (precheck != 4)
                        {
                            modified += c;
                            continue;
                        }

                        flag = 1;
                        modified += "<a href=\"{{LINK}}\">";
                        continue;
                    }
                    else if(c == ']' && flag == 1)
                    {
                        flag = 2;
                        modified += "</a>";
                        continue;
                    }
                    else if (c == '(' && flag == 2)
                    {
                        flag = 3;
                        continue;
                    }
                    else if (c == ')' && flag == 3)
                    {
                        flag = 0;
                        modified = modified.Replace("{{LINK}}", link);
                        link = "";
                        continue;
                    }

                    if (flag == 3)
                    {
                        link += c;
                    }
                    else
                    {
                        if(flag == 2)
                        {
                            flag = 0;
                        }
                        modified += c;
                    }
                }

                output.Add($"{modified}");
            }

            return output.ToArray();
        }
        private static string[] AddStyle(string[] lines)
        {
            List<string> output = new List<string>();
            bool flag = false;
            foreach (string line in lines)
            {
                var sanatized = line.Trim();
                if (sanatized.Contains("```"))
                {
                    flag = !flag;
                    output.Add(line);
                    continue;
                }
                else if (sanatized.StartsWith("#") || sanatized.StartsWith("-")
                      || sanatized.StartsWith(">") || sanatized.StartsWith("!"))
                {
                    output.Add(line);
                    continue;
                }
                if (flag)
                {
                    output.Add(line);
                    continue;
                }

                string modified = "";
                int modifier = 0;
                string temp = "";
                foreach(var c in sanatized)
                {
                    if (c == '~') // Strikthrough
                    {
                        if (modifier == 1)
                        {
                            modifier = 2;
                        }
                        else if (modifier == 2)
                        {
                            modifier = 3;
                        }
                        else if (modifier == 3)
                        {
                            modifier = 0;
                            modified = modified.Replace("{{STRIKETEMP}}", $"<s>{temp}</s>");
                            temp = "";
                        }
                        else
                        {
                            if (sanatized[sanatized.IndexOf(c) - 1] != ' ')
                            {
                                continue;
                            }
                            modifier = 1;
                            modified += "{{STRIKETEMP}}";
                        }
                        continue;
                    }
                    else if (c == '_') // Underline
                    {
                        if (modifier == 11)
                        {
                            modifier = 12;
                        }
                        else if (modifier == 12)
                        {
                            modifier = 13;
                        }
                        else if (modifier == 13)
                        {
                            modifier = 0;
                            modified = modified.Replace("{{UNDERTEMP}}", $"<u>{temp}</u>");
                            temp = "";
                        }
                        else
                        {
                            if (sanatized[sanatized.IndexOf(c) - 1] != ' ')
                            {
                                continue;
                            }
                            modifier = 11;
                            modified += "{{UNDERTEMP}}";
                        }
                        continue;
                    }
                    else if (c == '*') // Bold and Italics
                    {
                        if (modifier == 5) // Now We Bold
                        {
                            modifier = 6;
                            modified = modified.Replace("{{ITALICSTEMP}}", "{{BOLDTEMP}}");
                        }
                        else if (modifier == 9) // Italics Closed
                        {
                            modifier = 0;
                            modified = modified.Replace("{{ITALICSTEMP}}", $"<i>{temp}</i>");
                            temp = "";
                        }
                        else if (modifier == 6) // Bold Closing
                        {
                            modifier = 7;
                        }
                        else if (modifier == 7) // Bold Closed
                        {
                            modifier = 0;
                            modified = modified.Replace("{{BOLDTEMP}}", $"<b>{temp}</b>");
                            temp = "";
                        }
                        else // First We Italics
                        {
                            if (sanatized[sanatized.IndexOf(c) - 1] != ' ')
                            {
                                continue;
                            }
                            modifier = 5;
                            modified += "{{ITALICSTEMP}}";
                        }
                        continue;
                    }
                    else if (c == '`') // Inline Code
                    {
                        if (modifier == 10) // Close inline block
                        {
                            modified = modified.Replace("{{CODETEMP}}", $"<code>{temp}</code>");
                            temp = "";
                            modifier = 0;
                        }
                        else
                        {
                            if (sanatized[sanatized.IndexOf(c) - 1] != ' ')
                            {
                                continue;
                            }
                            modifier = 10;
                            modified += "{{CODETEMP}}";
                        }
                        continue;
                    }
                    else
                    {
                        if (modifier == 1) // Missing second Strikethrough brace
                        {
                            modified = modified.Replace("{{STRIKETEMP}}", $"{temp}");
                            temp = "";
                            modifier = 0;
                        }
                        else if (modifier == 3) // Missing 4th strikthrough brace
                        {
                            modified = modified.Replace("{{STRIKETEMP}}", $"{temp}");
                            temp = "";
                            modifier = 0;
                        }
                        else if (modifier == 5) // Italics Confirmed
                        {
                            modifier = 9;
                        }
                        else if (modifier == 7) // Bold Missing 2nd closing brace
                        {
                            modified = modified.Replace("{{BOLDTEMP}}", $"{temp}");
                            temp = "";
                            modifier = 0;
                        }

                        if (modifier >= 1 && modifier < 4)
                        {
                            temp += c;
                        }
                        else if (modifier >= 5 && modifier <= 9)
                        {
                            temp += c;
                        }
                        else if (modifier >= 11 && modifier <= 14)
                        {
                            temp += c;
                        }
                        else if (modifier == 10)
                        {
                            temp += c;
                        }
                        else
                        {
                            modified += c;
                        }
                    }
                }

                modified = modified.Replace("{{STRIKETEMP}}", $"{temp}");
                modified = modified.Replace("{{ITALICSTEMP}}", $"{temp}");
                modified = modified.Replace("{{BOLDTEMP}}", $"{temp}");
                temp = "";

                output.Add(modified);
            }

            return output.ToArray();
        }
        private static string[] AddBullets(string[] lines)
        {
            List<string> output = new List<string>();
            bool flag = false;
            int lastRank = -1;
            bool lastOrdered = false;
            foreach (string line in lines)
            {
                var sanatized = line.Trim();
                if (sanatized == "")
                {
                    continue;
                }
                bool ordered = !sanatized.StartsWith("-");

                if (!sanatized.StartsWith("- ") && !char.IsDigit(sanatized.First()))
                {
                    if (lastRank != -1)
                    {
                        for(int i = lastRank; i >= 0; i--)
                        {
                            if (!lastOrdered)
                            {
                                output.Add("</ul>");
                            }
                            else
                            {
                                output.Add("</ol>");
                            }
                        }
                        lastRank--;
                    }
                    lastRank = -1;
                    output.Add(line);
                    continue;
                }

                if(lastOrdered != ordered)
                {
                    lastRank = -1;
                    if (!lastOrdered)
                    {
                        output.Add("</ul>");
                    }
                    else
                    {
                        output.Add("</ol>");
                    }
                    lastOrdered = ordered;
                }

                if (ordered)
                {
                    int period = 0;
                    int count = 0;
                    foreach(var c in sanatized)
                    {
                        if(c == '.')
                        {
                            period = 1;
                        }
                        else if(c == ' ' && period == 1)
                        {
                            period = 2;
                            break;
                        }
                        else if (period == 1)
                        {
                            break;
                        }
                        count++;
                    }
                    sanatized = sanatized.Substring(count, sanatized.Length - count);
                    //continue;
                }

                int curRank = 0;
                if (!ordered)
                {
                    curRank = line.Substring(0, line.IndexOf("- ")).Length;
                }
                else
                {
                    foreach (var c in line)
                    {
                        if (char.IsDigit(c))
                        {
                            break;
                        }
                        curRank++;
                    }
                }


                if(curRank > lastRank)
                {
                    if (!ordered)
                    {
                        output.Add("<ul>");
                        lastOrdered = false;
                    }
                    else
                    {
                        output.Add("<ol>");
                        lastOrdered = true;
                    }
                }
                else if (curRank < lastRank)
                {
                    if (!ordered)
                    {
                        output.Add("</ul>");
                    }
                    else
                    {
                        output.Add("</ol>");
                    }
                }

                if(sanatized.StartsWith("- [ ] "))
                {
                    output.Add($"<li style=\"list-style-type: none; padding: 0px; margin: 0;\"><input type=\"checkbox\">{sanatized.Substring(6, sanatized.Length - 6)}</li>");
                }
                else if(sanatized.StartsWith("- [x] "))
                {
                    output.Add($"<li style=\"list-style-type: none;\"><input type=\"checkbox\" checked>{sanatized.Substring(6, sanatized.Length - 6)}</li>");
                }
                else
                {
                    output.Add($"<li>{sanatized.Substring(1, sanatized.Length - 1)}</li>");
                }

                
                lastRank = curRank;
            }

            return output.ToArray();
        }
        private static string[] AddCodeBlocks(string[] lines)
        {
            List<string> output = new List<string>();
            bool inBlock = false;
            foreach (string line in lines)
            {
                if (!line.Contains("```"))
                {
                    if (inBlock)
                    {
                        var encoded = System.Net.WebUtility.HtmlEncode(line);
                        output.Add(encoded);
                    }
                    else
                    {
                        output.Add(line);
                    }
                    continue;
                }

                if (!inBlock)
                {
                    output.Add("<pre class=\"code-block\">");
                    output.Add("<code>");
                    inBlock = true;
                }
                else
                {
                    output.Add("</code>");
                    output.Add("</pre>");
                    inBlock = false;
                }
            }

            return output.ToArray();
        }
        private static string[] AddBlockQuotes(string[] lines)
        {
            List<string> output = new List<string>();
            bool inBlock = false;
            foreach (string line in lines)
            {
                var sanatized = line.Trim();
                if (!sanatized.StartsWith("> "))
                {
                    if (inBlock)
                    {
                        inBlock = false;
                        output.Add("</blockquote>");
                    }
                    output.Add(line);
                    continue;
                }

                if (!inBlock)
                {
                    output.Add("<blockquote>");
                    output.Add($"<p>{sanatized.Replace("> ", "")}</p>");
                    inBlock = true;
                }
                else
                {
                    output.Add($"<p>{sanatized.Replace("> ", "")}</p>");
                    inBlock = true;
                }
            }

            return output.ToArray();
        }
        private static string[] AddImages(string[] lines)
        {
            List<string> output = new List<string>();
            bool inBlock = false;
            foreach (string line in lines)
            {
                var sanatized = line.Trim();
                if (!sanatized.StartsWith("!["))
                {
                    output.Add(line);
                    continue;
                }

                string desc = "";
                string link = "";
                int modifier = 0;
                foreach(var c in sanatized.Substring(1, sanatized.Length - 1))
                {
                    if (c == '[') 
                    {
                        modifier = 1;
                        continue;
                    }
                    else if (c == ']') 
                    {
                        modifier = 2;
                        continue;
                    }
                    else if (c == '(') 
                    {
                        modifier = 3;
                        continue;
                    }
                    else if (c == ')') 
                    {
                        if (!link.StartsWith("https://"))
                        {
                            link = $"/images/{Path.GetFileName(link)}";
                        }
                        output.Add($"<img alt=\"{desc}\" title=\"{desc}\" src=\"{link}\" />");
                    }
                    else
                    {
                        if (modifier == 1)
                        {
                            desc += c;
                        }
                        else if (modifier == 2)
                        {
                            desc = "";
                            modifier = 0;
                            output.Add(line);
                        }
                        else if (modifier == 3)
                        {
                            link += c;
                        }
                    }
                }
            }

            return output.ToArray();
        }




    }
}
