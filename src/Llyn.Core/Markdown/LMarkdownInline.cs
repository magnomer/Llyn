using System;
using System.Collections.Generic;
using System.Text;

namespace Llyn.Core;

public static class LMarkdownInline
{
    public static IReadOnlyList<LMarkdownSpan> LMarkdownInlineParse(string? text)
    {
        List<LMarkdownSpan> spans = new List<LMarkdownSpan>();

        if (string.IsNullOrEmpty(text))
        {
            return spans;
        }

        StringBuilder buffer = new StringBuilder();
        bool bold = false;
        bool italic = false;
        int place = 0;

        while (place < text.Length)
        {
            char letter = text[place];

            if (letter == '\\' && place + 1 < text.Length && char.IsPunctuation(text[place + 1]))
            {
                buffer.Append(text[place + 1]);
                place += 2;
                continue;
            }

            if (letter == '`')
            {
                int close = text.IndexOf('`', place + 1);

                if (close > place + 1)
                {
                    LMarkdownInlineAdd(spans, buffer, bold, italic);
                    spans.Add(new LMarkdownSpan(
                        text.Substring(place + 1, close - place - 1), false, false, true, string.Empty));
                    place = close + 1;
                    continue;
                }
            }

            if (letter == '[')
            {
                int middle = text.IndexOf("](", place + 1, StringComparison.Ordinal);
                int close = middle < 0 ? -1 : text.IndexOf(')', middle + 2);

                if (middle > place + 1 && close > middle + 2)
                {
                    LMarkdownInlineAdd(spans, buffer, bold, italic);
                    spans.Add(new LMarkdownSpan(
                        text.Substring(place + 1, middle - place - 1),
                        bold,
                        italic,
                        false,
                        text.Substring(middle + 2, close - middle - 2).Trim()));
                    place = close + 1;
                    continue;
                }
            }

            if (letter is '*' or '_')
            {
                int width = place + 1 < text.Length && text[place + 1] == letter ? 2 : 1;
                string marker = new string(letter, width);
                bool inside = width == 1 ? italic : bold;
                bool boundary = letter == '*' || LMarkdownInlineCheck(text, place, width);

                if (boundary && (inside || LMarkdownInlineFind(text, marker, place + width)))
                {
                    LMarkdownInlineAdd(spans, buffer, bold, italic);

                    if (width == 1)
                    {
                        italic = !italic;
                    }
                    else
                    {
                        bold = !bold;
                    }

                    place += width;
                    continue;
                }
            }

            buffer.Append(letter);
            place++;
        }

        LMarkdownInlineAdd(spans, buffer, bold, italic);
        return spans;
    }

    private static bool LMarkdownInlineFind(string text, string marker, int from)
    {
        int close = text.IndexOf(marker, from, StringComparison.Ordinal);
        return close > from;
    }

    private static bool LMarkdownInlineCheck(string text, int place, int width)
    {
        bool before = place > 0 && char.IsLetterOrDigit(text[place - 1]);
        bool after = place + width < text.Length && char.IsLetterOrDigit(text[place + width]);
        return !(before && after);
    }

    private static void LMarkdownInlineAdd(
        List<LMarkdownSpan> spans, StringBuilder buffer, bool bold, bool italic)
    {
        if (buffer.Length == 0)
        {
            return;
        }

        spans.Add(new LMarkdownSpan(buffer.ToString(), bold, italic, false, string.Empty));
        buffer.Clear();
    }
}
