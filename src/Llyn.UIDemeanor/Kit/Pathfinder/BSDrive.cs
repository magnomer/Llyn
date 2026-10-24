using System;
using System.Collections.Generic;

namespace Llyn.UIDemeanor;

internal sealed class BSDrive : BSParleyBody
{
    private readonly BBlueprintPart _bsDriveBlueprint;
    private readonly IReadOnlyList<string> _bsDriveEntries;
    private readonly Action<string> _bsDriveChoose;
    private int _bsDriveLit;

    internal BSDrive(BBlueprintPart blueprint, IReadOnlyList<string> drives, Action<string> choose)
    {
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(drives);
        ArgumentNullException.ThrowIfNull(choose);

        _bsDriveBlueprint = BContract.BContractFind(blueprint, "ADrive");
        _bsDriveEntries = drives;
        _bsDriveChoose = choose;
        const BKeymapFocus focus = BKeymapFocus.BKeymapFocusParley;
        Func<BKeymapLive> shown = static () => BKeymapLive.BKeymapLiveShown;
        Action<int> move = step =>
            _bsDriveLit = Math.Clamp(_bsDriveLit + step, 0, Math.Max(_bsDriveEntries.Count - 1, 0));
        BSParleyBodyKeymap = new BKeymap(
        [
            new(ConsoleKey.UpArrow, 0, focus, "Window.Help.Item", _ => move(-1), shown),
            new(ConsoleKey.DownArrow, 0, focus, "Window.Help.Item", _ => move(1), shown),
            new(ConsoleKey.Enter, 0, focus, "Window.Help.Run", _ =>
            {
                if (_bsDriveLit < _bsDriveEntries.Count)
                {
                    _bsDriveChoose(_bsDriveEntries[_bsDriveLit]);
                }
            }, shown),
        ]);
    }

    public int BSParleyBodyWidth => BSParley.BSParleyWidth;

    public int BSParleyBodyHeight => Math.Max(_bsDriveEntries.Count, 1);

    public string BSParleyBodyPosition => string.Empty;

    public BKeymap BSParleyBodyKeymap { get; }

    public bool BSParleyBodyObserve(ConsoleKeyInfo key)
    {
        if (BSParleyBodyKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusParley, key) is not BKeymapRow row)
        {
            return false;
        }

        row.BKeymapRowAction(key);
        return true;
    }

    public IEnumerable<BCanvasText> BSParleyBodyDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int row,
        int column,
        int width,
        bool focused,
        string role,
        BCanvasWidth measure)
    {
        BBlueprint.BBlueprintPlace(_bsDriveBlueprint, row - 1, column - 1, width + 2, BSParleyBodyHeight + 2);
        BBlueprintPart list = BContract.BContractFind(_bsDriveBlueprint, "ADriveList");
        int top = Math.Max(_bsDriveLit - list.BBlueprintPartHeight + 1, 0);
        for (int line = 0; line < list.BBlueprintPartHeight; line++)
        {
            int index = top + line;
            string entry = index < _bsDriveEntries.Count ? _bsDriveEntries[index] : string.Empty;
            string shade = index != _bsDriveLit || index >= _bsDriveEntries.Count ? list.BBlueprintPartTheme
                : focused ? "lit"
                : "chosen";
            yield return new BCanvasText(
                list.BBlueprintPartRow + line,
                list.BBlueprintPartColumn,
                list.BBlueprintPartWidth,
                measure.BCanvasWidthClamp(entry, list.BBlueprintPartWidth),
                shade);
        }
    }
}
