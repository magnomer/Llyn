using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Llyn.Core;

public static class LGlyphOrder
{
    public static Comparer<string> LGlyphOrderComparer { get; } = Comparer<string>.Create(static (left, right) =>
    {
        StringRuneEnumerator first = (left ?? string.Empty).EnumerateRunes();
        StringRuneEnumerator second = (right ?? string.Empty).EnumerateRunes();
        while (true)
        {
            bool firstMore = first.MoveNext();
            bool secondMore = second.MoveNext();
            if (!firstMore || !secondMore)
            {
                return firstMore.CompareTo(secondMore);
            }

            int step = first.Current.Value.CompareTo(second.Current.Value);
            if (step != 0)
            {
                return step;
            }
        }
    });

    public static IReadOnlyList<string> LGlyphOrderSort(IReadOnlyList<string> characters)
    {
        ArgumentNullException.ThrowIfNull(characters);

        return characters.OrderBy(static character => character, LGlyphOrderComparer).ToList();
    }
}
