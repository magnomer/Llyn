using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Llyn.Core;

public sealed record LInflectionMark(int LInflectionMarkOffset, int LInflectionMarkLength)
{
    public static string LInflectionMarkFormat(IReadOnlyList<LInflectionMark> marks)
    {
        ArgumentNullException.ThrowIfNull(marks);

        return string.Join(",", marks.Select(static mark => string.Create(
            CultureInfo.InvariantCulture,
            $"{mark.LInflectionMarkOffset}:{mark.LInflectionMarkLength}")));
    }

    public static IReadOnlyList<LInflectionMark> LInflectionMarkParse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        List<LInflectionMark> marks = [];
        foreach (string pair in text.Split(','))
        {
            string[] parts = pair.Split(':');
            if (parts.Length == 2
                && int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out int offset)
                && int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out int length))
            {
                marks.Add(new LInflectionMark(offset, length));
            }
        }

        return marks;
    }
}
