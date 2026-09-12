using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheetNote
{
    public static void LSheetNoteAppend(StringBuilder page, string? markdown)
    {
        ArgumentNullException.ThrowIfNull(page);

        Stack<LMarkdownKind> open = new Stack<LMarkdownKind>();

        foreach (LMarkdownBlock block in LMarkdown.LMarkdownParse(markdown))
        {
            bool item = block.LMarkdownBlockKind
                is LMarkdownKind.LMarkdownKindBullet or LMarkdownKind.LMarkdownKindNumber;
            int depth = item ? block.LMarkdownBlockLevel + 1 : 0;

            while (open.Count > depth || (open.Count == depth && item && open.Peek() != block.LMarkdownBlockKind))
            {
                LSheetNoteClose(page, open.Pop());
            }

            while (open.Count < depth)
            {
                open.Push(block.LMarkdownBlockKind);
                page.Append(block.LMarkdownBlockKind == LMarkdownKind.LMarkdownKindBullet ? "<ul>\n" : "<ol>\n");
            }

            switch (block.LMarkdownBlockKind)
            {
                case LMarkdownKind.LMarkdownKindHeading:
                    string tag = "h" + Math.Min(block.LMarkdownBlockLevel + 2, 6).ToString(CultureInfo.InvariantCulture);
                    page.Append('<').Append(tag).Append('>')
                        .Append(LSheetNoteFormat(block.LMarkdownBlockSpan))
                        .Append("</").Append(tag).Append(">\n");
                    break;
                case LMarkdownKind.LMarkdownKindBullet:
                case LMarkdownKind.LMarkdownKindNumber:
                    page.Append("<li>").Append(LSheetNoteFormat(block.LMarkdownBlockSpan)).Append("</li>\n");
                    break;
                case LMarkdownKind.LMarkdownKindQuote:
                    page.Append("<blockquote>").Append(LSheetNoteFormat(block.LMarkdownBlockSpan)).Append("</blockquote>\n");
                    break;
                case LMarkdownKind.LMarkdownKindCode:
                    page.Append("<pre><code>").Append(LSheet.LSheetNormalize(block.LMarkdownBlockText)).Append("</code></pre>\n");
                    break;
                case LMarkdownKind.LMarkdownKindRule:
                    page.Append("<hr>\n");
                    break;
                default:
                    page.Append("<p>").Append(LSheetNoteFormat(block.LMarkdownBlockSpan)).Append("</p>\n");
                    break;
            }
        }

        while (open.Count > 0)
        {
            LSheetNoteClose(page, open.Pop());
        }
    }

    private static void LSheetNoteClose(StringBuilder page, LMarkdownKind kind)
    {
        page.Append(kind == LMarkdownKind.LMarkdownKindBullet ? "</ul>\n" : "</ol>\n");
    }

    private static string LSheetNoteFormat(IReadOnlyList<LMarkdownSpan> spans)
    {
        StringBuilder line = new StringBuilder();

        foreach (LMarkdownSpan span in spans)
        {
            string text = LSheet.LSheetNormalize(span.LMarkdownSpanText)
                .Replace("\n", "<br>\n", StringComparison.Ordinal);

            if (span.LMarkdownSpanCode)
            {
                line.Append("<code>").Append(text).Append("</code>");
                continue;
            }

            if (span.LMarkdownSpanLink.Length > 0)
            {
                line.Append("<a href=\"").Append(LSheet.LSheetNormalize(span.LMarkdownSpanLink)).Append("\">");
            }

            if (span.LMarkdownSpanBold)
            {
                line.Append("<strong>");
            }

            if (span.LMarkdownSpanItalic)
            {
                line.Append("<em>");
            }

            line.Append(text);

            if (span.LMarkdownSpanItalic)
            {
                line.Append("</em>");
            }

            if (span.LMarkdownSpanBold)
            {
                line.Append("</strong>");
            }

            if (span.LMarkdownSpanLink.Length > 0)
            {
                line.Append("</a>");
            }
        }

        return line.ToString();
    }
}
