using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

public sealed record LPressRange(string LPressRangeText)
{
    public static LPressRange? LPressRangeParse(string text, int count)
    {
        ArgumentNullException.ThrowIfNull(text);

        IReadOnlyList<(int LPressRangeFirst, int LPressRangeLast)>? spans = LPressRangeRead(text);

        if (spans is null)
        {
            return null;
        }

        List<string> parts = new();

        foreach ((int start, int end) in spans)
        {
            if (start > count || (end != int.MaxValue && end > count))
            {
                return null;
            }

            string first = start.ToString(CultureInfo.InvariantCulture);
            parts.Add(end == int.MaxValue ? first + "-"
                : end == start ? first
                : first + "-" + end.ToString(CultureInfo.InvariantCulture));
        }

        return new LPressRange(string.Join(", ", parts));
    }

    public bool LPressRangeCheck(int page)
    {
        IReadOnlyList<(int LPressRangeFirst, int LPressRangeLast)>? spans =
            LPressRangeRead(LPressRangeText ?? string.Empty);

        if (spans is null || page < 0)
        {
            return false;
        }

        if (spans.Count == 0)
        {
            return true;
        }

        int number = page + 1;

        foreach ((int start, int end) in spans)
        {
            if (number >= start && number <= end)
            {
                return true;
            }
        }

        return false;
    }

    private static IReadOnlyList<(int LPressRangeFirst, int LPressRangeLast)>? LPressRangeRead(string text)
    {
        List<(int LPressRangeFirst, int LPressRangeLast)> spans = new();

        if (string.IsNullOrWhiteSpace(text))
        {
            return spans;
        }

        foreach (string piece in text.Split(','))
        {
            string part = piece.Trim();
            int dash = part.IndexOf('-');
            string head = dash < 0 ? part : part[..dash].Trim();
            string tail = dash < 0 ? part : part[(dash + 1)..].Trim();

            if (head.Length == 0 && tail.Length == 0)
            {
                return null;
            }

            int start = 1;
            int end = int.MaxValue;

            if (head.Length > 0 && !int.TryParse(head, NumberStyles.None, CultureInfo.InvariantCulture, out start))
            {
                return null;
            }

            if (tail.Length > 0 && !int.TryParse(tail, NumberStyles.None, CultureInfo.InvariantCulture, out end))
            {
                return null;
            }

            if (start < 1 || end < start)
            {
                return null;
            }

            spans.Add((start, end));
        }

        return spans;
    }
}
