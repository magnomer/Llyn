using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Llyn.UIDemeanor;

internal sealed class BCanvas
{
    private readonly BConsole _bCanvasConsole;
    private readonly BCanvasWidth _bCanvasWidth;
    private readonly BTheme _bCanvasTheme;
    private readonly int _bCanvasColumns;
    private BCanvasCell[] _bCanvasDrawn;
    private BCanvasCell[] _bCanvasShown;

    internal BCanvas(BConsole console, BCanvasWidth width, BTheme theme, int columns, int rows)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(width);
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentOutOfRangeException.ThrowIfNegative(columns);
        ArgumentOutOfRangeException.ThrowIfNegative(rows);

        _bCanvasConsole = console;
        _bCanvasWidth = width;
        _bCanvasTheme = theme;
        _bCanvasColumns = columns;
        _bCanvasDrawn = new BCanvasCell[columns * rows];
        _bCanvasShown = new BCanvasCell[columns * rows];
        Array.Fill(_bCanvasDrawn, new BCanvasCell(" ", 1, "text"));
    }

    internal void BCanvasFrameDraw(IReadOnlyList<BCanvasText> texts)
    {
        ArgumentNullException.ThrowIfNull(texts);

        List<BCanvasCell> drawn = [];
        drawn.AddRange(Enumerable.Repeat(new BCanvasCell(" ", 1, "text"), _bCanvasDrawn.Length));
        foreach (BCanvasText placed in texts)
        {
            (int row, int column, int span) = (placed.BCanvasTextRow, placed.BCanvasTextColumn, placed.BCanvasTextSpan);
            (string text, string role) = (placed.BCanvasTextValue, placed.BCanvasTextRole);
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(role);

            if (row < 0 || row * _bCanvasColumns >= drawn.Count || column < 0 || span <= 0)
            {
                continue;
            }

            int start = row * _bCanvasColumns;
            int end = start + Math.Min(column + span, _bCanvasColumns);
            int at = start + column;
            int index = 0;
            while (index < text.Length && at < end)
            {
                int length = StringInfo.GetNextTextElementLength(text, index);
                string cluster = text.Substring(index, length);
                int cells = _bCanvasWidth.BCanvasWidthResolve(cluster);
                index += length;
                if (cells == 0)
                {
                    continue;
                }

                if (at + cells > end)
                {
                    (cluster, cells, index) = (" ", 1, text.Length);
                }

                if (drawn[at].BCanvasCellWidth == 0 && at > start)
                {
                    drawn[at - 1] = drawn[at - 1] with { BCanvasCellText = " ", BCanvasCellWidth = 1 };
                }

                int last = at + cells - 1;
                if (drawn[last].BCanvasCellWidth == 2 && last + 1 < start + _bCanvasColumns)
                {
                    drawn[last + 1] = drawn[last + 1] with { BCanvasCellText = " ", BCanvasCellWidth = 1 };
                }

                drawn[at] = new BCanvasCell(cluster, cells, role);
                if (cells == 2)
                {
                    drawn[at + 1] = new BCanvasCell(string.Empty, 0, role);
                }

                at += cells;
            }
        }

        _bCanvasDrawn = [.. drawn];
    }

    internal void BCanvasSync()
    {
        StringBuilder frame = new();
        string? colour = null;
        for (int start = 0; start < _bCanvasDrawn.Length; start += _bCanvasColumns)
        {
            int end = start + _bCanvasColumns;
            int at = start;
            while (at < end)
            {
                if (_bCanvasDrawn[at].BCanvasCellText == _bCanvasShown[at].BCanvasCellText
                    && _bCanvasDrawn[at].BCanvasCellRole == _bCanvasShown[at].BCanvasCellRole)
                {
                    at++;
                    continue;
                }

                int first = _bCanvasDrawn[at].BCanvasCellWidth == 0 && at > start ? at - 1 : at;
                int stop = at + 1;
                while (stop < end
                    && (_bCanvasDrawn[stop].BCanvasCellWidth == 0
                        || _bCanvasDrawn[stop].BCanvasCellText != _bCanvasShown[stop].BCanvasCellText
                        || _bCanvasDrawn[stop].BCanvasCellRole != _bCanvasShown[stop].BCanvasCellRole))
                {
                    stop++;
                }

                frame.Append(CultureInfo.InvariantCulture, $"\e[{(start / _bCanvasColumns) + 1};{first - start + 1}H");
                for (int cell = first; cell < stop; cell++)
                {
                    if (_bCanvasDrawn[cell].BCanvasCellWidth == 0)
                    {
                        continue;
                    }

                    string escape = _bCanvasTheme.BThemeRead(_bCanvasDrawn[cell].BCanvasCellRole);
                    if (escape != colour)
                    {
                        frame.Append(escape);
                        colour = escape;
                    }

                    frame.Append(_bCanvasDrawn[cell].BCanvasCellText);
                }

                at = stop;
            }
        }

        _bCanvasShown = _bCanvasDrawn;
        if (frame.Length > 0)
        {
            frame.Append("\e[0m");
            _bCanvasConsole.BConsoleFramePrint(frame.ToString());
        }
    }
}
