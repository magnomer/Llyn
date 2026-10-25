using System;
using System.Collections.Generic;

namespace Llyn.UIDemeanor;

internal sealed record BLecternLine(
    IReadOnlyList<(string BLecternLineText, string BLecternLineRole, string? BLecternLineItem)> BLecternLinePieces,
    int BLecternLineIndent)
{
    private const int BLecternLineHang = 2;

    internal IReadOnlyList<BLecternLine> BLecternLineDivide(int width, BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(measure);

        if (width <= 0)
        {
            return [];
        }

        int first = Math.Clamp(BLecternLineIndent, 0, width - 1);
        int hang = first + BLecternLineHang < width ? first + BLecternLineHang : first;
        List<BLecternLine> rows = [];
        List<(string BLecternLineText, string BLecternLineRole, string? BLecternLineItem)> row = [];
        int used = 0;
        int room = width - first;

        void BLecternLineFinish()
        {
            if (row is [.., (string text, string role, var owner)] && (owner is null || text.Trim(' ').Length > 0))
            {
                row[^1] = (text.TrimEnd(' '), role, owner);
            }

            rows.Add(new BLecternLine([.. BLecternPieceNormalize(row)], rows.Count == 0 ? first : hang));
            row = [];
            used = 0;
            room = width - hang;
        }

        List<(string, string, string?)> words = [.. BLecternWordScan(BLecternLinePieces)];
        for (int index = 0; index < words.Count; index++)
        {
            (string text, string role, string? item) = words[index];
            string word = row.Count == 0 && item is null ? text.TrimStart(' ') : text;
            int solid = measure.BCanvasWidthResolve(word.TrimEnd(' '));
            int glued = solid;
            bool starts = index == 0 || words[index - 1].Item1.EndsWith(' ') || text.StartsWith(' ');
            for (int next = index + 1; starts && next < words.Count && !words[next - 1].Item1.EndsWith(' ')
                && !words[next].Item1.StartsWith(' '); next++)
            {
                glued += measure.BCanvasWidthResolve(words[next].Item1.TrimEnd(' '));
            }

            if (row.Count > 0 && used + glued > room)
            {
                BLecternLineFinish();
                word = word.TrimStart(' ');
                solid = measure.BCanvasWidthResolve(word.TrimEnd(' '));
            }

            if (word.Length == 0)
            {
                continue;
            }

            while (solid > room && measure.BCanvasWidthDivide(word.TrimEnd(' '), room) is [string head, _, ..])
            {
                row.Add((head, role, item));
                BLecternLineFinish();
                word = word[head.Length..].TrimStart(' ');
                solid = measure.BCanvasWidthResolve(word.TrimEnd(' '));
            }

            row.Add((word, role, item));
            used += measure.BCanvasWidthResolve(word);
        }

        BLecternLineFinish();
        return rows;
    }

    private static IEnumerable<(string, string, string?)> BLecternWordScan(
        IEnumerable<(string BLecternLineText, string BLecternLineRole, string? BLecternLineItem)> pieces)
    {
        foreach ((string text, string role, string? item) in pieces)
        {
            if (item is not null)
            {
                yield return (text, role, item);
                continue;
            }

            int start = 0;
            for (int at = 1; at <= text.Length; at++)
            {
                if (at == text.Length || (text[at] != ' ' && text[at - 1] == ' '))
                {
                    yield return (text[start..at], role, null);
                    start = at;
                }
            }
        }
    }

    private static IEnumerable<(string, string, string?)> BLecternPieceNormalize(
        IReadOnlyList<(string BLecternLineText, string BLecternLineRole, string? BLecternLineItem)> pieces)
    {
        int at = 0;
        while (at < pieces.Count)
        {
            (string text, string role, string? item) = pieces[at];
            int next = at + 1;
            while (next < pieces.Count && pieces[next].BLecternLineRole == role
                && pieces[next].BLecternLineItem == item)
            {
                text += pieces[next].BLecternLineText;
                next++;
            }

            if (text.Length > 0)
            {
                yield return (text, role, item);
            }

            at = next;
        }
    }
}
