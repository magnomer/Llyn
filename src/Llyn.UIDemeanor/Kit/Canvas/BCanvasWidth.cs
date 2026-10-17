using System;
using System.Collections.Generic;
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

    public IReadOnlyList<string> BCanvasWidthDivide(string text, int span)
    {
        ArgumentNullException.ThrowIfNull(text);

        List<(string BCanvasWidthCluster, int BCanvasWidthCells, bool BCanvasWidthHangul)> clusters = [];
        for (int index = 0; index < text.Length;)
        {
            int length = StringInfo.GetNextTextElementLength(text, index);
            string cluster = text.Substring(index, length);
            Rune.DecodeFromUtf16(cluster, out Rune lead, out _);
            bool hangul = lead.Value is (>= 0x1100 and <= 0x11FF) or (>= 0x3130 and <= 0x318F)
                or (>= 0xA960 and <= 0xA97F) or (>= 0xAC00 and <= 0xD7FF);
            clusters.Add((cluster, BCanvasWidthResolve(cluster), hangul));
            index += length;
        }

        List<string> lines = [];
        int first = 0;
        while (span > 0 && first < clusters.Count)
        {
            if (clusters[first].BCanvasWidthCluster == " ")
            {
                first++;
                continue;
            }

            (int used, int stop, int cut) = (0, first, -1);
            while (stop < clusters.Count && used + clusters[stop].BCanvasWidthCells <= span)
            {
                used += clusters[stop].BCanvasWidthCells;
                stop++;
                bool wide = stop < clusters.Count
                    && clusters[stop - 1] is { BCanvasWidthCells: 2, BCanvasWidthHangul: false }
                    && clusters[stop] is { BCanvasWidthCells: 2, BCanvasWidthHangul: false };
                if (stop < clusters.Count && (clusters[stop].BCanvasWidthCluster == " " || wide))
                {
                    cut = stop;
                }
            }

            cut = stop == clusters.Count ? stop : cut >= 0 ? cut : Math.Max(stop, first + 1);
            StringBuilder line = new();
            for (int at = first; at < cut; at++)
            {
                line.Append(clusters[at].BCanvasWidthCluster);
            }

            lines.Add(line.ToString());
            first = cut;
        }

        return lines;
    }
}
