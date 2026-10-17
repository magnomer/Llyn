using System;
using System.Globalization;
using System.Text;

namespace Llyn.UIDemeanor;

internal sealed class BCanvasWidth
{
    private readonly bool _bCanvasWidthAmbiguous;

    public BCanvasWidth(bool ambiguousWide)
    {
        _bCanvasWidthAmbiguous = ambiguousWide;
    }

    public int BCanvasWidthResolve(ReadOnlySpan<char> text)
    {
        ReadOnlySpan<int> first = BCanvasRange.BCanvasRangeFirst;
        ReadOnlySpan<byte> cells = _bCanvasWidthAmbiguous
            ? BCanvasRange.BCanvasRangeWide
            : BCanvasRange.BCanvasRangeNarrow;
        int width = 0;

        while (!text.IsEmpty)
        {
            int length = StringInfo.GetNextTextElementLength(text);
            int cluster = 0;

            foreach (Rune rune in text[..length].EnumerateRunes())
            {
                int index = first.BinarySearch(rune.Value);
                int point = rune.Value == 0xFE0F ? 2 : Rune.GetUnicodeCategory(rune) switch
                {
                    UnicodeCategory.NonSpacingMark or UnicodeCategory.EnclosingMark => 0,
                    UnicodeCategory.Format or UnicodeCategory.Control => 0,
                    _ => cells[index < 0 ? ~index - 1 : index],
                };
                cluster = Math.Max(cluster, point);
            }

            width += cluster;
            text = text[length..];
        }

        return width;
    }
}
