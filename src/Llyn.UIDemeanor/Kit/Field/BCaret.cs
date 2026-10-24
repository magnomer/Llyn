using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Llyn.UIDemeanor;

internal static class BCaret
{
    internal static (string, int, int, bool, bool) BCaretKeyApply(
        ConsoleKeyInfo key,
        string text,
        int caret,
        int anchor,
        Action<int>? remove,
        Action<int>? step)
    {
        ArgumentNullException.ThrowIfNull(text);

        (string, int, int, bool, bool) kept = (text, caret, anchor, false, false);
        (string, int, int, bool, bool) taken = (text, caret, anchor, true, false);
        Func<string, int, (string, int, int, bool, bool)> edit = (changed, at) => (changed, at, at, true, true);
        (int from, int to) = (Math.Min(caret, anchor), Math.Max(caret, anchor));
        bool shift = key.Modifiers.HasFlag(ConsoleModifiers.Shift);
        bool control = key.Modifiers.HasFlag(ConsoleModifiers.Control);
        bool alt = key.Modifiers.HasFlag(ConsoleModifiers.Alt);
        if (key.Key == ConsoleKey.Backspace || (key.Key == ConsoleKey.Delete && !control && !alt))
        {
            if (from != to)
            {
                return edit(text.Remove(from, to - from), from);
            }

            if (remove is not null && BCaretEdgeApply(key, caret, text.Length, 0, remove))
            {
                return taken;
            }

            if (key.Key == ConsoleKey.Delete)
            {
                return caret == text.Length
                    ? kept
                    : edit(text.Remove(caret, BCaretClusterFind(text, caret, 1) - caret), caret);
            }

            if (caret == 0)
            {
                return kept;
            }

            int start = BCaretClusterFind(text, caret, -1);
            return edit(text.Remove(start, caret - start), start);
        }

        int? target = alt ? null : key.Key switch
        {
            ConsoleKey.LeftArrow when !shift && !control && from != to => from,
            ConsoleKey.RightArrow when !shift && !control && from != to => to,
            ConsoleKey.LeftArrow => control ? BCaretWordFind(text, caret, -1) : BCaretClusterFind(text, caret, -1),
            ConsoleKey.RightArrow => control ? BCaretWordFind(text, caret, 1) : BCaretClusterFind(text, caret, 1),
            ConsoleKey.Home when !control => 0,
            ConsoleKey.End when !control => text.Length,
            _ => null,
        };
        if (target is int moved)
        {
            if (text.Length > 0)
            {
                return (text, moved, shift ? anchor : moved, true, false);
            }

            return !shift && !control && step is not null && BCaretStepApply(key, text.Length, 0, step)
                ? taken
                : kept;
        }

        if (char.IsControl(key.KeyChar))
        {
            return kept;
        }

        return edit(string.Concat(text.AsSpan(0, from), key.KeyChar.ToString(), text.AsSpan(to)), from + 1);
    }

    internal static int BCaretClusterFind(string text, int caret, int step)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (step > 0)
        {
            return caret >= text.Length ? text.Length : caret + StringInfo.GetNextTextElementLength(text, caret);
        }

        List<int> starts = BCaretStartRead(text);
        int index = starts.FindLastIndex(start => start < caret);
        return index < 0 ? 0 : starts[index];
    }

    internal static int BCaretWordFind(string text, int caret, int step)
    {
        ArgumentNullException.ThrowIfNull(text);

        List<int> starts = BCaretStartRead(text);
        if (step < 0)
        {
            int index = starts.FindLastIndex(start => start < caret);
            while (index >= 0 && !BCaretWordCheck(text, starts[index]))
            {
                index--;
            }

            while (index > 0 && BCaretWordCheck(text, starts[index - 1]))
            {
                index--;
            }

            return index < 0 ? 0 : starts[index];
        }

        int at = starts.FindIndex(start => start >= caret);
        if (at < 0)
        {
            return text.Length;
        }

        while (at < starts.Count && BCaretWordCheck(text, starts[at]))
        {
            at++;
        }

        while (at < starts.Count && !BCaretWordCheck(text, starts[at]))
        {
            at++;
        }

        return at < starts.Count ? starts[at] : text.Length;
    }

    internal static bool BCaretEdgeApply(ConsoleKeyInfo key, int caret, int length, int selection, Action<int> remove)
    {
        ArgumentNullException.ThrowIfNull(remove);

        if (selection != 0)
        {
            return false;
        }

        if (key.Key == ConsoleKey.Backspace && caret == 0)
        {
            remove(-1);
            return true;
        }

        if (key.Key == ConsoleKey.Delete && caret == length)
        {
            remove(1);
            return true;
        }

        return false;
    }

    internal static bool BCaretStepApply(ConsoleKeyInfo key, int length, int selection, Action<int> move)
    {
        ArgumentNullException.ThrowIfNull(move);

        int step = key.Key == ConsoleKey.LeftArrow ? -1 : key.Key == ConsoleKey.RightArrow ? 1 : 0;
        if (selection != 0 || step == 0 || length > 0)
        {
            return false;
        }

        move(step);
        return true;
    }

    private static List<int> BCaretStartRead(string text)
    {
        List<int> starts = [];
        for (int index = 0; index < text.Length; index += StringInfo.GetNextTextElementLength(text, index))
        {
            starts.Add(index);
        }

        return starts;
    }

    private static bool BCaretWordCheck(string text, int index)
    {
        return Rune.DecodeFromUtf16(text.AsSpan(index), out Rune rune, out _) == System.Buffers.OperationStatus.Done
            && Rune.IsLetterOrDigit(rune);
    }
}
