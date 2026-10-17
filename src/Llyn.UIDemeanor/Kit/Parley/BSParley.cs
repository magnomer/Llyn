using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Llyn.UIDemeanor;

internal sealed class BSParley
{
    internal const int BSParleyWidth = 58;
    private const int BSParleyGap = 5;

    private readonly BBlueprintPart _bsParleyBlueprint;
    private readonly BBlueprintPart _bsParleyText;
    private readonly BBlueprintPart _bsParleyButton;
    private readonly string _bsParleyTitle;
    private readonly string _bsParleyRole;
    private readonly IReadOnlyList<BSParleyLine> _bsParleyLines;
    private readonly IReadOnlyList<BSParleyButton> _bsParleyButtons;
    private readonly int _bsParleySafe;
    private readonly BSParleyBody? _bsParleyBody;
    private readonly TaskCompletionSource<int> _bsParleyAnswer = new();
    private BSParleyBody? _bsParleyFocus;
    private int _bsParleyLit;

    internal BSParley(
        BBlueprintPart blueprint,
        string title,
        string role,
        IReadOnlyList<BSParleyLine> lines,
        IReadOnlyList<BSParleyButton> buttons,
        int safe,
        BSParleyBody? body = null)
    {
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(lines);
        ArgumentNullException.ThrowIfNull(buttons);
        ArgumentOutOfRangeException.ThrowIfNegative(safe);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(safe, buttons.Count);

        _bsParleyBlueprint = blueprint;
        _bsParleyText = BContract.BContractFind(blueprint, "ASParleyText");
        _bsParleyButton = BContract.BContractFind(blueprint, "ASParleyButton");
        _bsParleyTitle = title;
        _bsParleyRole = role;
        _bsParleyLines = lines;
        _bsParleyButtons = buttons;
        _bsParleySafe = safe;
        _bsParleyBody = body;
        _bsParleyFocus = body;
    }

    internal (int BSParleyRow, int BSParleyColumn)? BSParleyAnchor { get; init; }

    internal Task<int> BSParleyAnswer => _bsParleyAnswer.Task;

    internal void BSParleyKeyObserve(ConsoleKeyInfo key)
    {
        if (_bsParleyFocus is BSParleyBody focus && focus.BSParleyBodyObserve(key))
        {
            return;
        }

        int pressed = key.Key == ConsoleKey.Enter
            ? _bsParleyLit
            : _bsParleyButtons.ToList().FindIndex(button => button.BSParleyButtonKey == key.Key);
        if (pressed >= 0)
        {
            if (_bsParleyButtons[pressed].BSParleyButtonEnabled)
            {
                _bsParleyAnswer.TrySetResult(pressed);
            }

            return;
        }

        switch (key.Key)
        {
            case ConsoleKey.Escape or ConsoleKey.F10:
                _bsParleyAnswer.TrySetResult(_bsParleySafe);
                break;
            case ConsoleKey.LeftArrow:
                BSParleyLitMove(-1);
                break;
            case ConsoleKey.RightArrow:
                BSParleyLitMove(1);
                break;
            case ConsoleKey.Tab when _bsParleyBody is BSParleyBody body:
                _bsParleyFocus = _bsParleyFocus is null ? body : null;
                break;
            case ConsoleKey.Tab:
                BSParleyLitMove(key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? -1 : 1);
                break;
        }
    }

    internal IReadOnlyList<BCanvasText> BSParleyDraw(
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> glyphs, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(glyphs);

        BCanvasWidth measure = new(false);
        int across = Math.Clamp(width - 2, 0, _bsParleyBody?.BSParleyBodyWidth ?? BSParleyWidth);
        BBlueprint.BBlueprintPlace(_bsParleyBlueprint, 0, 0, across, height);
        int span = _bsParleyText.BBlueprintPartWidth;
        List<string> rows = [];
        foreach (BSParleyLine line in _bsParleyLines)
        {
            IReadOnlyList<string> cut = measure.BCanvasWidthDivide(
                line.BSParleyLineText, span, line.BSParleyLinePath ? "\\/" : null);
            rows.AddRange(cut.Count == 0 ? [string.Empty] : cut);
        }

        int gap = rows.Count > 0 ? 1 : 0;
        int body = _bsParleyBody is BSParleyBody part ? part.BSParleyBodyHeight + gap : 0;
        int down = Math.Clamp(
            rows.Count + body + height - _bsParleyText.BBlueprintPartHeight, 0, Math.Max(height - 2, 0));
        int lowest = Math.Max(height - down - 1, 0);
        int rightmost = Math.Max(width - across - 1, 0);
        (int top, int left) = BSParleyAnchor is (int row, int column)
            ? (Math.Clamp(row, 0, lowest), Math.Clamp(column, 0, rightmost))
            : (lowest / 2, rightmost / 2);
        BBlueprint.BBlueprintPlace(_bsParleyBlueprint, top, left, across, down);
        List<BCanvasText> texts =
            [.. BBlueprint.BBlueprintDraw(_bsParleyBlueprint, glyphs).Select(scaffold => scaffold with
            {
                BCanvasTextRole = _bsParleyRole,
            })];
        if (across == 0 || down == 0)
        {
            return texts;
        }

        for (int line = top + 1; line < top + down; line++)
        {
            texts.Add(new BCanvasText(line, left + across, 1, " ", "shadow"));
        }

        texts.Add(new BCanvasText(top + down, left + 1, across, new string(' ', across), "shadow"));
        string title = $" {_bsParleyTitle} ";
        int cells = measure.BCanvasWidthResolve(title);
        if (cells <= across - 2)
        {
            texts.Add(new BCanvasText(top, left + ((across - cells) / 2), cells, title, _bsParleyRole));
        }

        BBlueprintPart text = _bsParleyText;
        for (int line = 0; line < Math.Min(rows.Count, text.BBlueprintPartHeight); line++)
        {
            texts.Add(new BCanvasText(
                text.BBlueprintPartRow + line, text.BBlueprintPartColumn, span, rows[line], _bsParleyRole));
        }

        BBlueprintPart bar = _bsParleyButton;
        string[] labels = [.. _bsParleyButtons.Select(static button => $"[ {button.BSParleyButtonLabel} ]")];
        int[] sizes = [.. labels.Select(label => measure.BCanvasWidthResolve(label))];
        int space = BSParleyGap;
        int total = sizes.Sum() + (space * (sizes.Length - 1));
        if (total > bar.BBlueprintPartWidth)
        {
            space = 1;
            total = sizes.Sum() + (sizes.Length - 1);
        }

        int end = bar.BBlueprintPartColumn + bar.BBlueprintPartWidth;
        int at = bar.BBlueprintPartColumn + Math.Max((bar.BBlueprintPartWidth - total) / 2, 0);
        for (int index = 0; bar.BBlueprintPartHeight > 0 && index < labels.Length && at < end; index++)
        {
            string role = !_bsParleyButtons[index].BSParleyButtonEnabled ? "disabled"
                : index == _bsParleyLit ? "lit"
                : _bsParleyRole;
            texts.Add(new BCanvasText(
                bar.BBlueprintPartRow, at, Math.Min(sizes[index], end - at), labels[index], role));
            at += sizes[index] + space;
        }

        if (_bsParleyBody is BSParleyBody drawn)
        {
            IReadOnlyDictionary<string, IReadOnlyList<string>> set =
                _bsParleyBlueprint.BBlueprintPartChildren is [{ BBlueprintPartGlyphs: string name }]
                && glyphs.TryGetValue(name, out IReadOnlyDictionary<string, IReadOnlyList<string>>? found)
                    ? found
                    : new Dictionary<string, IReadOnlyList<string>>();
            texts.AddRange(drawn.BSParleyBodyDraw(
                set, text.BBlueprintPartRow + rows.Count + gap, text.BBlueprintPartColumn, span,
                _bsParleyFocus is not null, _bsParleyRole));
            string position = $" {drawn.BSParleyBodyPosition} ";
            int length = measure.BCanvasWidthResolve(position);
            if (drawn.BSParleyBodyPosition.Length > 0 && length <= across - 4)
            {
                texts.Add(new BCanvasText(top + down - 1, left + 2, length, position, _bsParleyRole));
            }
        }

        return texts;
    }

    private void BSParleyLitMove(int step)
    {
        int count = _bsParleyButtons.Count;
        for (int at = (_bsParleyLit + step + count) % count; at != _bsParleyLit; at = (at + step + count) % count)
        {
            if (_bsParleyButtons[at].BSParleyButtonEnabled)
            {
                _bsParleyLit = at;
                return;
            }
        }
    }
}
