using System;
using System.Collections.Generic;
using System.Text;

namespace Llyn.Core;

public static class LCueMarkup
{
    public static string LCueMarkupResolve(string? markdown)
    {
        List<string> lines = [];

        foreach (LMarkdownBlock block in LMarkdown.LMarkdownParse(markdown))
        {
            if (block.LMarkdownBlockRuled)
            {
                continue;
            }

            StringBuilder line = new StringBuilder();

            if (block.LMarkdownBlockFenced)
            {
                line.Append(LCueMarkupFormat(block.LMarkdownBlockText));
            }
            else
            {
                string mark = block.LMarkdownBlockMark;

                if (mark.Length > 0)
                {
                    line.Append(mark).Append(' ');
                }

                LCueMarkupAppend(line, block.LMarkdownBlockSpan);
            }

            lines.Add(line.Replace("\n", "|").ToString());
        }

        return string.Join('|', lines);
    }

    public static string LCueMarkupFormat(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return text.Replace('|', '\u2223');
    }

    private static void LCueMarkupAppend(StringBuilder line, IReadOnlyList<LMarkdownSpan> spans)
    {
        foreach (LMarkdownSpan span in spans)
        {
            string text = LCueMarkupFormat(span.LMarkdownSpanText);

            if (span.LMarkdownSpanCode)
            {
                line.Append(text);
                continue;
            }

            if (span.LMarkdownSpanBold)
            {
                line.Append("<b>");
            }

            if (span.LMarkdownSpanItalic)
            {
                line.Append("<i>");
            }

            line.Append(text);

            if (span.LMarkdownSpanItalic)
            {
                line.Append("</i>");
            }

            if (span.LMarkdownSpanBold)
            {
                line.Append("</b>");
            }
        }
    }
}
