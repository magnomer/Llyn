using System;
using System.Collections.Generic;

namespace Llyn.UIDemeanor;

internal sealed class BKeymap
{
    private readonly IReadOnlyList<BKeymapRow> _bKeymapRows;

    internal BKeymap(IReadOnlyList<BKeymapRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        foreach (BKeymapRow row in rows)
        {
            ArgumentNullException.ThrowIfNull(row);

            ConsoleModifiers modifiers = row.BKeymapRowModifiers;
            bool letter = row.BKeymapRowKey is >= ConsoleKey.A and <= ConsoleKey.Z;
            bool control = modifiers.HasFlag(ConsoleModifiers.Control);
            bool alt = modifiers.HasFlag(ConsoleModifiers.Alt);
            bool roof = letter && alt && row.BKeymapRowFocus != BKeymapFocus.BKeymapFocusRoof;
            bool shift = modifiers.HasFlag(ConsoleModifiers.Shift);
            bool terminal = row.BKeymapRowKey == ConsoleKey.F11
                || (alt && row.BKeymapRowKey is ConsoleKey.F4 or ConsoleKey.Enter)
                || (control && row.BKeymapRowKey is ConsoleKey.V or ConsoleKey.Tab)
                || (letter && control && shift && row.BKeymapRowKey != ConsoleKey.Z);
            if (roof || terminal)
            {
                throw new ArgumentException(
                    $"The key {modifiers} {row.BKeymapRowKey} belongs to the roof or the terminal.", nameof(rows));
            }
        }

        _bKeymapRows = rows;
    }

    internal BKeymapRow? BKeymapFind(BKeymapFocus focus, ConsoleKeyInfo key)
    {
        (ConsoleKey pressed, ConsoleModifiers modifiers) = key.KeyChar == '\u0003'
            ? (ConsoleKey.C, ConsoleModifiers.Control)
            : (key.Key, key.Modifiers);
        foreach (BKeymapRow row in _bKeymapRows)
        {
            bool matched = row.BKeymapRowSign is char sign
                ? key.KeyChar == sign
                : row.BKeymapRowKey == pressed && row.BKeymapRowModifiers == modifiers;
            if (row.BKeymapRowFocus == focus && matched && row.BKeymapRowLive() != BKeymapLive.BKeymapLiveHidden)
            {
                return row;
            }
        }

        return null;
    }

    internal IReadOnlyList<BKeymapRow> BKeymapRead(BKeymapFocus focus)
    {
        List<BKeymapRow> rows = [];
        foreach (BKeymapRow row in _bKeymapRows)
        {
            if (row.BKeymapRowFocus == focus)
            {
                rows.Add(row);
            }
        }

        return rows;
    }
}
