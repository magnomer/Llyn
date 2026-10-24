using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Llyn.UIDemeanor;

internal sealed class BField : BSParleyBody
{
    private readonly Action<string> _bFieldEdited;
    private readonly BLocalizationCatalog? _bFieldCatalog;
    private readonly string _bFieldHint;
    private readonly Action<int>? _bFieldRemoval;
    private readonly Action<int>? _bFieldStep;
    private readonly string _bFieldOrigin;
    private readonly StringBuilder _bFieldPending = new();
    private string? _bFieldTyped;
    private int? _bFieldCaret;
    private int? _bFieldAnchor;

    internal BField(
        Action<string> edited,
        string text = "",
        BLocalizationCatalog? catalog = null,
        string hint = "",
        Action<int>? remove = null,
        Action<int>? step = null)
    {
        ArgumentNullException.ThrowIfNull(edited);
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(hint);
        if (hint.Length > 0)
        {
            ArgumentNullException.ThrowIfNull(catalog);
        }

        _bFieldEdited = edited;
        _bFieldCatalog = catalog;
        _bFieldHint = hint;
        _bFieldRemoval = remove;
        _bFieldStep = step;
        _bFieldOrigin = text;
    }

    public int BSParleyBodyWidth => BSParley.BSParleyWidth;

    public int BSParleyBodyHeight => 1;

    public string BSParleyBodyPosition => string.Empty;

    internal string BFieldText => _bFieldTyped ?? _bFieldOrigin;

    public bool BSParleyBodyObserve(ConsoleKeyInfo key)
    {
        if (!char.IsControl(key.KeyChar)
            && key.Key is not (ConsoleKey.Backspace or ConsoleKey.Delete or ConsoleKey.LeftArrow
                or ConsoleKey.RightArrow or ConsoleKey.Home or ConsoleKey.End))
        {
            _bFieldPending.Append(key.KeyChar);
            if (SynchronizationContext.Current is BLoop loop)
            {
                loop.BLoopDefer(() => BFieldPendingApply(null));
            }
            else
            {
                BFieldPendingApply(null);
            }

            return true;
        }

        return BFieldPendingApply(key);
    }

    internal void BFieldClear()
    {
        _bFieldPending.Clear();
        _bFieldTyped = string.Empty;
        _bFieldCaret = null;
        _bFieldAnchor = null;
    }

    public IEnumerable<BCanvasText> BSParleyBodyDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int row,
        int column,
        int width,
        int height,
        bool focused,
        string role,
        BCanvasWidth measure)
    {
        if (width <= 0)
        {
            yield break;
        }

        string text = BFieldText;
        if (text.Length == 0 && _bFieldHint.Length > 0 && _bFieldCatalog is BLocalizationCatalog catalog)
        {
            yield return new BCanvasText(row, column, width, new string(' ', width), "field");
            yield return new BCanvasText(
                row, column, width, measure.BCanvasWidthClamp(catalog.BLocalizationTextRead(_bFieldHint), width),
                "muted");
            if (focused)
            {
                yield return new BCanvasText(row, column, 0, string.Empty, "caret");
            }

            yield break;
        }

        (int start, int cells, bool before, bool after) = BFieldSightRead(width, measure);
        yield return new BCanvasText(
            row,
            column,
            width,
            string.Concat(
                before ? "<" : string.Empty,
                BFieldCellsRead(start, start + cells, measure),
                after ? ">" : string.Empty),
            "field");
        int caret = _bFieldCaret ?? text.Length;
        int anchor = _bFieldAnchor ?? caret;
        int from = Math.Max(measure.BCanvasWidthResolve(text.AsSpan(0, Math.Min(caret, anchor))), start);
        int to = Math.Min(measure.BCanvasWidthResolve(text.AsSpan(0, Math.Max(caret, anchor))), start + cells);
        if (from < to)
        {
            yield return new BCanvasText(
                row, column + (before ? 1 : 0) + from - start, to - from, BFieldCellsRead(from, to, measure), "chosen");
        }

        if (focused)
        {
            int cell = measure.BCanvasWidthResolve(text.AsSpan(0, caret));
            yield return new BCanvasText(row, column + (before ? 1 : 0) + cell - start, 0, string.Empty, "caret");
        }
    }

    private bool BFieldPendingApply(ConsoleKeyInfo? key)
    {
        string typed = _bFieldPending.ToString();
        int ready = typed.Length > 0 && char.IsHighSurrogate(typed[^1]) ? typed.Length - 1 : typed.Length;
        _bFieldPending.Remove(0, key is null ? ready : typed.Length);
        StringBuilder whole = new();
        for (int index = 0; index < ready; index++)
        {
            if (index + 1 < ready && char.IsSurrogatePair(typed[index], typed[index + 1]))
            {
                whole.Append(typed, index, 2);
                index++;
            }
            else if (!char.IsSurrogate(typed[index]))
            {
                whole.Append(typed[index]);
            }
        }

        if (whole.Length == 0 && key is null)
        {
            return false;
        }

        string text = BFieldText;
        int caret = _bFieldCaret ?? text.Length;
        int anchor = _bFieldAnchor ?? caret;
        bool taken = false;
        bool changed = whole.Length > 0;
        if (changed)
        {
            (int from, int to) = (Math.Min(caret, anchor), Math.Max(caret, anchor));
            text = string.Concat(text.AsSpan(0, from), whole.ToString(), text.AsSpan(to));
            (caret, anchor) = (from + whole.Length, from + whole.Length);
        }

        if (key is ConsoleKeyInfo pressed)
        {
            (text, caret, anchor, taken, bool edited) = BCaret.BCaretKeyApply(
                pressed, text, caret, anchor, _bFieldRemoval, _bFieldStep);
            changed |= edited;
        }

        _bFieldTyped = text;
        _bFieldCaret = caret;
        _bFieldAnchor = anchor;
        if (changed)
        {
            _bFieldEdited(text);
        }

        return taken;
    }

    private (int, int, bool, bool) BFieldSightRead(int width, BCanvasWidth measure)
    {
        string text = BFieldText;
        int total = measure.BCanvasWidthResolve(text);
        int at = _bFieldCaret ?? text.Length;
        int caret = measure.BCanvasWidthResolve(text.AsSpan(0, at));
        int size = at < text.Length
            ? Math.Max(measure.BCanvasWidthResolve(text.AsSpan(at, BCaret.BCaretClusterFind(text, at, 1) - at)), 1)
            : 1;
        if (total + 1 <= width)
        {
            return (0, width, false, false);
        }

        if (width < 3)
        {
            return (Math.Max(caret + size - width, 0), width, false, false);
        }

        if (caret + size <= width - 1)
        {
            return (0, width - 1, false, true);
        }

        bool after = total > caret + size;
        int cells = width - 1 - (after ? 1 : 0);
        return (caret + size - cells, cells, true, after);
    }

    private string BFieldCellsRead(int from, int to, BCanvasWidth measure)
    {
        string text = BFieldText;
        StringBuilder cells = new();
        int at = 0;
        int filled = 0;
        for (int index = 0; index < text.Length && at < to;)
        {
            int length = StringInfo.GetNextTextElementLength(text, index);
            int size = measure.BCanvasWidthResolve(text.AsSpan(index, length));
            if (at >= from && at + size <= to)
            {
                cells.Append(text, index, length);
                filled += size;
            }
            else if (at + size > from && at < to)
            {
                int shown = Math.Min(at + size, to) - Math.Max(at, from);
                cells.Append(' ', shown);
                filled += shown;
            }

            at += size;
            index += length;
        }

        cells.Append(' ', Math.Max(to - from - filled, 0));
        return cells.ToString();
    }
}
