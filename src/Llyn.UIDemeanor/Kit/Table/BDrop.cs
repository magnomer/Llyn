using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.UIDemeanor;

internal sealed class BDrop
{
    private readonly bool _bDropEnabled;
    private readonly int _bDropOrigin;
    private readonly int _bDropPage;
    private int _bDropShift;
    private string _bDropTyped = string.Empty;

    internal BDrop(IReadOnlyList<string> labels, int lit, bool enabled, int page)
    {
        ArgumentNullException.ThrowIfNull(labels);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(page);

        BDropLabel = labels;
        _bDropEnabled = enabled;
        _bDropOrigin = lit;
        _bDropPage = page;
        BDropTop = Math.Max(BDropLit - page + 1, 0);
    }

    internal IReadOnlyList<string> BDropLabel { get; }

    internal int BDropLit => Math.Clamp(_bDropOrigin + _bDropShift, 0, Math.Max(BDropLabel.Count - 1, 0));

    internal int BDropTop { get; private set; }

    internal IEnumerable<int> BDropLabelShown =>
        Enumerable.Range(BDropTop, Math.Clamp(BDropLabel.Count - BDropTop, 0, _bDropPage));

    internal bool BDropEnabled => _bDropEnabled && BDropLabel.Count > 0;

    internal bool BDropObserve(ConsoleKeyInfo key)
    {
        int step = key.Key switch
        {
            ConsoleKey.UpArrow => -1,
            ConsoleKey.DownArrow => 1,
            _ => 0,
        };
        switch (key.Key)
        {
            case ConsoleKey.UpArrow or ConsoleKey.DownArrow:
                BDropLitMove(step);
                return true;
            case ConsoleKey.Enter or ConsoleKey.Spacebar or ConsoleKey.Escape or ConsoleKey.F10:
                return false;
            default:
                return true;
        }
    }

    internal void BDropLitMove(int step)
    {
        _bDropTyped = string.Empty;
        BDropShiftMove(step);
    }

    internal bool BDropSeek(char letter)
    {
        if (char.IsControl(letter) || BDropLabel.Count == 0)
        {
            return false;
        }

        string typed = _bDropTyped + letter;
        int found = BDropFind(typed, typed.Length > 1 ? 0 : 1);
        if (found < 0)
        {
            typed = new string(letter, 1);
            found = BDropFind(typed, 1);
        }

        _bDropTyped = found < 0 ? string.Empty : typed;
        BDropShiftMove((found < 0 ? BDropLit : found) - BDropLit);
        return found >= 0;
    }

    internal IEnumerable<BCanvasText> BDropDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        BBlueprintPart bounds,
        (int, int) field,
        (IReadOnlyList<string>, bool?) look)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(bounds);
        ArgumentNullException.ThrowIfNull(look.Item1);

        (int anchor, int start) = field;
        (IReadOnlyList<string> edges, bool? focused) = look;
        int[] shown = [.. BDropLabelShown];
        int label = BDropLabel.Select(text => measure.BCanvasWidthResolve(text)).DefaultIfEmpty(0).Max();
        int edge = edges.Select(text => measure.BCanvasWidthResolve(text)).DefaultIfEmpty(0).Max();
        int right = bounds.BBlueprintPartColumn + bounds.BBlueprintPartWidth;
        int across = Math.Min(label + (edge > 0 ? edge + 1 : 0) + 4, Math.Max(bounds.BBlueprintPartWidth - 1, 3));
        int inner = across - 2;
        int left = Math.Max(Math.Min(start, right - across - 1), bounds.BBlueprintPartColumn);
        int down = shown.Length + 2;
        int bottom = bounds.BBlueprintPartRow + bounds.BBlueprintPartHeight;
        bool flipped = anchor + down + 1 >= bottom && anchor - down - 1 >= bounds.BBlueprintPartRow;
        int top = Math.Max(
            Math.Min(flipped ? anchor - down - 1 : anchor + 1, bottom - down - 1), bounds.BBlueprintPartRow);
        IReadOnlyList<string> frame = glyphs.GetValueOrDefault("Frame") is [_, _, _, _, _, _] marks
            ? marks
            : ["+", "-", "+", "|", "+", "+"];
        string bar = string.Concat(Enumerable.Repeat(frame[1], inner));
        yield return new BCanvasText(top, left, across, $"{frame[0]}{bar}{frame[2]}", "menu");
        foreach (int index in shown)
        {
            int line = top + 1 + index - BDropTop;
            string mark = edges.ElementAtOrDefault(index) ?? string.Empty;
            int markCells = measure.BCanvasWidthResolve(mark);
            string name = measure.BCanvasWidthClamp(
                BDropLabel[index], Math.Max(inner - 2 - (markCells > 0 ? markCells + 1 : 0), 0));
            int space = Math.Max(inner - 2 - measure.BCanvasWidthResolve(name) - markCells, 0);
            string body = $" {name}{new string(' ', space)}{mark} ";
            bool lit = index == BDropLit && focused is not null;
            yield return new BCanvasText(line, left, 1, frame[3], "menu");
            string role = !BDropEnabled ? "muted" : !lit ? "menu" : focused is true ? "lit" : "lit-other";
            yield return new BCanvasText(line, left + 1, inner, body, role);
            yield return new BCanvasText(line, left + across - 1, 1, frame[3], "menu");
        }

        yield return new BCanvasText(top + down - 1, left, across, $"{frame[4]}{bar}{frame[5]}", "menu");
        for (int line = top + 1; line < top + down; line++)
        {
            yield return new BCanvasText(line, left + across, 1, " ", "shadow");
        }

        yield return new BCanvasText(top + down, left + 1, across, new string(' ', across), "shadow");
    }

    private void BDropShiftMove(int step)
    {
        _bDropShift = BDropLit + step - _bDropOrigin;
        BDropTop = Math.Clamp(BDropTop, BDropLit - _bDropPage + 1, BDropLit);
    }

    private int BDropFind(string typed, int skip)
    {
        int count = BDropLabel.Count;
        for (int turn = skip; turn < count + skip; turn++)
        {
            int at = (BDropLit + turn) % count;
            if (BDropLabel[at].StartsWith(typed, StringComparison.CurrentCultureIgnoreCase))
            {
                return at;
            }
        }

        return -1;
    }
}
