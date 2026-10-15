using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace DenManager;

public static class MarkdownRenderer
{
    private static readonly Regex Heading = new(@"^(#{1,6})\s+(.+)$", RegexOptions.Compiled);
    private static readonly Regex ListItem = new(@"^(?<indent>[ \t]*)(?<marker>[-*+]|\d+[.)])[ \t]+(?<body>.*)$", RegexOptions.Compiled);
    private static readonly Regex Link = new(@"(!?)\[([^\]]+)\]\(([^)]+)\)(?:\{width=(\d{1,3})%\})?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex TableDivider = new(@"^\|?\s*:?-{3,}:?\s*(\|\s*:?-{3,}:?\s*)+\|?$", RegexOptions.Compiled);

    public static string Render(string markdown)
    {
        var lines = (markdown ?? "").Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var output = new StringBuilder();
        var paragraph = new List<string>();
        var lists = new Stack<ListFrame>();
        bool code = false;

        void FlushParagraph()
        {
            if (paragraph.Count == 0) return;
            output.Append("<p>").Append(Inline(string.Join(" ", paragraph))).AppendLine("</p>");
            paragraph.Clear();
        }
        void CloseListLevel()
        {
            var frame = lists.Pop();
            if (frame.ItemOpen) output.AppendLine("</li>");
            output.Append("</").Append(frame.Tag).AppendLine(">");
        }
        void CloseLists()
        {
            while (lists.Count > 0) CloseListLevel();
        }

        for (int index = 0; index < lines.Length; index++)
        {
            var original = lines[index];
            var line = original.Trim();
            if (line.StartsWith("```", StringComparison.Ordinal))
            {
                FlushParagraph(); CloseLists();
                output.AppendLine(code ? "</code></pre>" : "<pre><code>");
                code = !code;
                continue;
            }
            if (code)
            {
                output.AppendLine(WebUtility.HtmlEncode(original));
                continue;
            }
            if (line.Length == 0)
            {
                FlushParagraph();
                var next = index + 1;
                while (next < lines.Length && string.IsNullOrWhiteSpace(lines[next])) next++;
                if (lists.Count == 0 || next >= lines.Length || !ListItem.IsMatch(lines[next]))
                    CloseLists();
                continue;
            }
            if (line.Contains('|') && index + 1 < lines.Length && TableDivider.IsMatch(lines[index + 1].Trim()))
            {
                FlushParagraph(); CloseLists();
                output.AppendLine("<table><thead><tr>");
                foreach (var cell in TableCells(line))
                    output.Append("<th>").Append(Inline(cell)).AppendLine("</th>");
                output.AppendLine("</tr></thead><tbody>");
                index++;
                while (index + 1 < lines.Length && lines[index + 1].Contains('|') &&
                       !string.IsNullOrWhiteSpace(lines[index + 1]))
                {
                    index++;
                    output.AppendLine("<tr>");
                    foreach (var cell in TableCells(lines[index]))
                        output.Append("<td>").Append(Inline(cell)).AppendLine("</td>");
                    output.AppendLine("</tr>");
                }
                output.AppendLine("</tbody></table>");
                continue;
            }
            var heading = Heading.Match(line);
            if (heading.Success)
            {
                FlushParagraph(); CloseLists();
                int level = heading.Groups[1].Length;
                var title = heading.Groups[2].Value.Trim();
                output.Append("<h").Append(level).Append(" id=\"mdheadr-")
                    .Append(Slug(title)).Append("\">").Append(Inline(title))
                    .Append("</h").Append(level).AppendLine(">");
                continue;
            }
            if (line.StartsWith("> ", StringComparison.Ordinal))
            {
                FlushParagraph(); CloseLists();
                output.Append("<blockquote><p>").Append(Inline(line[2..])).AppendLine("</p></blockquote>");
                continue;
            }
            var listItem = ListItem.Match(original);
            if (listItem.Success)
            {
                FlushParagraph();
                var indent = IndentWidth(listItem.Groups["indent"].Value);
                var marker = listItem.Groups["marker"].Value;
                var wanted = char.IsDigit(marker[0]) ? "ol" : "ul";
                while (lists.Count > 0 && indent < lists.Peek().Indent) CloseListLevel();
                if (lists.Count > 0 && indent == lists.Peek().Indent && wanted != lists.Peek().Tag)
                    CloseListLevel();
                if (lists.Count == 0 || indent > lists.Peek().Indent)
                {
                    output.Append('<').Append(wanted);
                    if (wanted == "ol" && int.TryParse(marker[..^1], out var start) && start != 1)
                        output.Append(" start=\"").Append(start).Append('"');
                    output.AppendLine(">");
                    lists.Push(new ListFrame(indent, wanted));
                }
                else if (lists.Peek().ItemOpen)
                    output.AppendLine("</li>");
                output.Append("<li>").Append(Inline(listItem.Groups["body"].Value));
                lists.Peek().ItemOpen = true;
                continue;
            }
            var leading = original.Length - original.TrimStart(' ', '\t').Length;
            if (lists.Count > 0 && IndentWidth(original[..leading]) > lists.Peek().Indent)
            {
                output.Append("<br>").Append(Inline(line));
                continue;
            }
            CloseLists();
            if (line is "---" or "***")
            {
                FlushParagraph(); output.AppendLine("<hr>");
                continue;
            }
            paragraph.Add(line);
        }
        FlushParagraph(); CloseLists();
        if (code) output.AppendLine("</code></pre>");
        return output.ToString();
    }

    private static IEnumerable<string> TableCells(string line) =>
        line.Trim().Trim('|').Split('|').Select(x => x.Trim());

    private static int IndentWidth(string whitespace)
    {
        var width = 0;
        foreach (var character in whitespace)
            width += character == '\t' ? 4 - width % 4 : 1;
        return width;
    }

    private sealed class ListFrame(int indent, string tag)
    {
        public int Indent { get; } = indent;
        public string Tag { get; } = tag;
        public bool ItemOpen { get; set; }
    }

    public static string Slug(string value)
    {
        var slug = Regex.Replace(value.ToLowerInvariant().Trim(), @"[^a-z0-9-]+", "-").Trim('-');
        return slug.Length == 0 ? "item" : slug;
    }

    private static string Inline(string value)
    {
        var tokens = new List<string>();
        string Token(string html)
        {
            tokens.Add(html);
            return $"\u0001{tokens.Count - 1}\u0002";
        }
        var encoded = WebUtility.HtmlEncode(value);
        encoded = Regex.Replace(encoded, @"`([^`]+)`", m => Token("<code>" + m.Groups[1].Value + "</code>"));
        encoded = Link.Replace(encoded, m =>
        {
            if (m.Groups[4].Success && m.Groups[1].Value != "!") return m.Value;
            var width = m.Groups[4].Success ? int.Parse(m.Groups[4].Value) : 0;
            if (m.Groups[4].Success && width is < 1 or > 100) return m.Value;
            var url = WebUtility.HtmlDecode(m.Groups[3].Value.Trim());
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var parsed) ||
                (parsed.IsAbsoluteUri && parsed.Scheme is not ("http" or "https")))
                return m.Value;
            var safeUrl = WebUtility.HtmlEncode(url);
            var label = m.Groups[2].Value;
            var size = width > 0 ? $" style=\"width:{width}%;max-width:100%;height:auto;\"" : "";
            return Token(m.Groups[1].Value == "!"
                ? $"<img src=\"{safeUrl}\" alt=\"{label}\" loading=\"lazy\"{size}>"
                : $"<a href=\"{safeUrl}\">{label}</a>");
        });
        encoded = Regex.Replace(encoded, @"\*\*(.+?)\*\*|__(.+?)__", m => "<strong>" + (m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value) + "</strong>");
        encoded = Regex.Replace(encoded, @"(?<!\*)\*([^*]+)\*(?!\*)|(?<!_)_([^_]+)_(?!_)", m => "<em>" + (m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value) + "</em>");
        return Regex.Replace(encoded, "\u0001(\\d+)\u0002", m => tokens[int.Parse(m.Groups[1].Value)]);
    }
}
