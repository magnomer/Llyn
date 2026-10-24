using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.UIDemeanor;

internal sealed class BSPathfinder : BSParleyBody
{
    internal const int BSPathfinderHeight = 14;

    private readonly BBlueprintPart _bsPathfinderBlueprint;
    private readonly BLocalizationCatalog _bsPathfinderCatalog;
    private readonly IReadOnlyList<string>? _bsPathfinderSuffixes;
    private readonly Func<string, (IReadOnlyList<string>, IReadOnlyList<string>)> _bsPathfinderListing;
    private readonly Func<string, bool> _bsPathfinderPresence;
    private readonly Action<string> _bsPathfinderChoose;
    private readonly Func<string?> _bsPathfinderDrive;
    private readonly BSPathfinderMedium? _bsPathfinderMedium;
    private readonly BKeymap _bsPathfinderPathKeymap;
    private readonly BKeymap _bsPathfinderFormatKeymap;
    private readonly BKeymap _bsPathfinderListKeymap;
    private readonly string _bsPathfinderOrigin;
    private BKeymap _bsPathfinderKeymap;
    private readonly BField _bsPathfinderField;
    private (string? BSPathfinderSightFolder, IReadOnlyList<string> BSPathfinderSightEntries) _bsPathfinderSight =
        (null, []);
    private (string? BSPathfinderPreviewFolder, IReadOnlyList<string> BSPathfinderPreviewEntries) _bsPathfinderPreview =
        (null, []);
    private int _bsPathfinderLit;
    private string? _bsPathfinderAlert;

    internal BSPathfinder(
        BBlueprintPart blueprint,
        BLocalizationCatalog catalog,
        string folder,
        IReadOnlyList<string>? suffixes,
        Func<string, (IReadOnlyList<string>, IReadOnlyList<string>)> listing,
        Func<string, bool> presence,
        Action<string> choose,
        Func<string?> drive,
        BSPathfinderMedium? medium = null,
        string name = "")
    {
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(folder);
        ArgumentNullException.ThrowIfNull(listing);
        ArgumentNullException.ThrowIfNull(presence);
        ArgumentNullException.ThrowIfNull(choose);
        ArgumentNullException.ThrowIfNull(drive);
        ArgumentNullException.ThrowIfNull(name);

        _bsPathfinderBlueprint = BContract.BContractFind(blueprint, "APathfinder");
        _bsPathfinderCatalog = catalog;
        _bsPathfinderSuffixes = suffixes;
        _bsPathfinderListing = listing;
        _bsPathfinderPresence = presence;
        _bsPathfinderChoose = choose;
        _bsPathfinderDrive = drive;
        _bsPathfinderMedium = medium;
        _bsPathfinderField =
            new BField(BSPathfinderShownRefine, medium?.BSPathfinderMediumRemove(name) ?? string.Empty);
        const BKeymapFocus focus = BKeymapFocus.BKeymapFocusParley;
        Func<BKeymapLive> shown = static () => BKeymapLive.BKeymapLiveShown;
        Action<ConsoleKeyInfo> drives = _ =>
        {
            if (_bsPathfinderDrive() is string chosen)
            {
                BSPathfinderFolderOpen(chosen);
            }
        };
        Action<int> format = step =>
        {
            if (_bsPathfinderMedium is not BSPathfinderMedium owned)
            {
                return;
            }

            owned.BSPathfinderMediumMove(step);
            BSPathfinderEntriesRefine(BSPathfinderFolderShown, _bsPathfinderSight.BSPathfinderSightFolder);
        };
        _bsPathfinderFormatKeymap = new BKeymap(
        [
            new(ConsoleKey.Tab, 0, focus, "Window.Help.Part", _ => BSParleyBodyResume(true), shown),
            new(ConsoleKey.Tab, ConsoleModifiers.Shift, focus, "Window.Help.Part",
                _ => BSParleyBodyResume(false), shown),
            new(ConsoleKey.LeftArrow, 0, focus, "Pathfinder.Format", _ => format(-1), shown),
            new(ConsoleKey.RightArrow, 0, focus, "Pathfinder.Format", _ => format(1), shown),
            new(ConsoleKey.Enter, 0, focus, "Window.Help.Run", _ => BSPathfinderTypedAccept(), shown),
            new(ConsoleKey.F1, ConsoleModifiers.Alt, focus, "Pathfinder.Drive", drives, shown),
        ]);
        _bsPathfinderPathKeymap = new BKeymap(
        [
            new(ConsoleKey.Tab, 0, focus, "Window.Help.Part", _ =>
            {
                if (medium is null)
                {
                    BSParleyBodyResume(true);
                    return;
                }

                _bsPathfinderKeymap = _bsPathfinderFormatKeymap;
            }, shown),
            new(ConsoleKey.DownArrow, 0, focus, "Window.Help.Item", _ => BSParleyBodyResume(true), shown),
            new(ConsoleKey.Enter, 0, focus, "Window.Help.Run", _ => BSPathfinderTypedAccept(), shown),
            new(ConsoleKey.F1, ConsoleModifiers.Alt, focus, "Pathfinder.Drive", drives, shown),
        ]);
        _bsPathfinderListKeymap = new BKeymap(
        [
            new(ConsoleKey.Tab, ConsoleModifiers.Shift, focus, "Window.Help.Part", _ => _bsPathfinderKeymap =
                medium is null ? _bsPathfinderPathKeymap : _bsPathfinderFormatKeymap, shown),
            new(ConsoleKey.UpArrow, 0, focus, "Window.Help.Item", _ => BSPathfinderLitMove(-1), shown),
            new(ConsoleKey.DownArrow, 0, focus, "Window.Help.Item", _ => BSPathfinderLitMove(1), shown),
            new(ConsoleKey.Enter, 0, focus, "Window.Help.Run", _ => BSPathfinderLitAccept(), shown),
            new(ConsoleKey.Backspace, 0, focus, "Pathfinder.Climb",
                _ => BSPathfinderFolderOpen(BSPathfinderPath.BSPathfinderPathResolve(BSPathfinderFolderPreview, "..")),
                shown),
            new(ConsoleKey.F1, ConsoleModifiers.Alt, focus, "Pathfinder.Drive", drives, shown),
        ]);
        _bsPathfinderKeymap = _bsPathfinderPathKeymap;
        string origin = BSPathfinderPath.BSPathfinderPathResolve(folder, string.Empty);
        for (string start = origin; ; start = BSPathfinderPath.BSPathfinderPathResolve(start, ".."))
        {
            if (BSPathfinderEntriesRefine(start, start))
            {
                origin = start;
                break;
            }

            if (BSPathfinderPath.BSPathfinderPathResolve(start, "..") == start)
            {
                break;
            }
        }

        _bsPathfinderOrigin = origin;
    }

    public int BSParleyBodyWidth => BSParley.BSParleyWidth;

    public int BSParleyBodyHeight => BSPathfinderHeight;

    public string BSParleyBodyPosition => string.Empty;

    public BKeymap BSParleyBodyKeymap => _bsPathfinderKeymap;

    private string BSPathfinderFolderShown => _bsPathfinderSight.BSPathfinderSightFolder ?? _bsPathfinderOrigin;

    private string BSPathfinderFolderPreview =>
        _bsPathfinderPreview.BSPathfinderPreviewFolder ?? BSPathfinderFolderShown;

    internal void BSPathfinderAccept()
    {
        IReadOnlyList<string> shown = _bsPathfinderPreview.BSPathfinderPreviewEntries;
        if (_bsPathfinderField.BFieldText.Length > 0)
        {
            BSPathfinderTypedAccept();
        }
        else if (_bsPathfinderSuffixes is null && _bsPathfinderMedium is null)
        {
            BSPathfinderTypedAccept();
        }
        else if (_bsPathfinderLit < shown.Count && !shown[_bsPathfinderLit].EndsWith('\\'))
        {
            _bsPathfinderChoose(BSPathfinderPath.BSPathfinderPathResolve(
                BSPathfinderFolderPreview, shown[_bsPathfinderLit]));
        }
    }

    public bool BSParleyBodyObserve(ConsoleKeyInfo key)
    {
        _bsPathfinderAlert = null;
        if (_bsPathfinderKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusParley, key) is not BKeymapRow row)
        {
            return _bsPathfinderField.BSParleyBodyObserve(key);
        }

        row.BKeymapRowAction(key);
        return true;
    }

    public void BSParleyBodyResume(bool backward)
    {
        _bsPathfinderKeymap = backward ? _bsPathfinderListKeymap : _bsPathfinderPathKeymap;
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
        BBlueprint.BBlueprintPlace(_bsPathfinderBlueprint, row - 1, column - 1, width + 2, BSPathfinderHeight + 2);
        BBlueprintPart path = BContract.BContractFind(_bsPathfinderBlueprint, "APathfinderPath");
        BBlueprintPart list = BContract.BContractFind(_bsPathfinderBlueprint, "APathfinderList");
        BBlueprintPart medium = BContract.BContractFind(_bsPathfinderBlueprint, "APathfinderMedium");
        BBlueprintPart alert = BContract.BContractFind(_bsPathfinderBlueprint, "APathfinderAlert");
        BBlueprintPart folder = BContract.BContractFind(_bsPathfinderBlueprint, "APathfinderFolder");

        string label = _bsPathfinderCatalog.BLocalizationTextRead("Pathfinder.Path") + " ";
        int cells = Math.Min(measure.BCanvasWidthResolve(label), path.BBlueprintPartWidth);
        yield return new BCanvasText(
            path.BBlueprintPartRow, path.BBlueprintPartColumn, cells, measure.BCanvasWidthClamp(label, cells),
            path.BBlueprintPartTheme);
        string tail = _bsPathfinderMedium?.BSPathfinderMediumChoice.CPortraitChoiceSuffix ?? string.Empty;
        int tailCells = Math.Min(measure.BCanvasWidthResolve(tail), path.BBlueprintPartWidth - cells);
        foreach (BCanvasText text in _bsPathfinderField.BSParleyBodyDraw(
            glyphs,
            path.BBlueprintPartRow,
            path.BBlueprintPartColumn + cells,
            path.BBlueprintPartWidth - cells - tailCells,
            focused && _bsPathfinderKeymap == _bsPathfinderPathKeymap,
            role,
            measure))
        {
            yield return text;
        }

        if (tailCells > 0)
        {
            yield return new BCanvasText(
                path.BBlueprintPartRow,
                path.BBlueprintPartColumn + path.BBlueprintPartWidth - tailCells,
                tailCells,
                measure.BCanvasWidthClamp(tail, tailCells),
                path.BBlueprintPartTheme);
        }

        IReadOnlyList<string> shown = _bsPathfinderPreview.BSPathfinderPreviewEntries;
        int top = Math.Max(_bsPathfinderLit - list.BBlueprintPartHeight + 1, 0);
        for (int line = 0; line < list.BBlueprintPartHeight; line++)
        {
            int index = top + line;
            string entry = index < shown.Count ? shown[index] : string.Empty;
            string shade = index != _bsPathfinderLit || index >= shown.Count ? list.BBlueprintPartTheme
                : focused && _bsPathfinderKeymap == _bsPathfinderListKeymap ? "lit"
                : "chosen";
            yield return new BCanvasText(
                list.BBlueprintPartRow + line,
                list.BBlueprintPartColumn,
                list.BBlueprintPartWidth,
                measure.BCanvasWidthClamp(entry, list.BBlueprintPartWidth),
                shade);
        }

        string format = _bsPathfinderMedium is BSPathfinderMedium chosen
            ? string.Concat(
                _bsPathfinderCatalog.BLocalizationTextRead("Pathfinder.Format"),
                " ",
                _bsPathfinderCatalog.BLocalizationTextRead(chosen.BSPathfinderMediumChoice.CPortraitChoiceKey))
            : string.Empty;
        yield return new BCanvasText(
            medium.BBlueprintPartRow,
            medium.BBlueprintPartColumn,
            medium.BBlueprintPartWidth,
            measure.BCanvasWidthClamp(format, medium.BBlueprintPartWidth),
            focused && _bsPathfinderKeymap == _bsPathfinderFormatKeymap ? "lit" : medium.BBlueprintPartTheme);
        yield return new BCanvasText(
            alert.BBlueprintPartRow,
            alert.BBlueprintPartColumn,
            alert.BBlueprintPartWidth,
            measure.BCanvasWidthClamp(_bsPathfinderAlert ?? string.Empty, alert.BBlueprintPartWidth),
            alert.BBlueprintPartTheme);
        yield return new BCanvasText(
            folder.BBlueprintPartRow,
            folder.BBlueprintPartColumn,
            folder.BBlueprintPartWidth,
            measure.BCanvasWidthShorten(
                BSPathfinderFolderPreview,
                folder.BBlueprintPartWidth,
                glyphs.GetValueOrDefault("Ellipsis") is [string trail, ..] ? trail : "…"),
            folder.BBlueprintPartTheme);
    }

    private bool BSPathfinderFolderOpen(string folder)
    {
        if (!BSPathfinderEntriesRefine(folder, folder))
        {
            return false;
        }

        if (_bsPathfinderPreview.BSPathfinderPreviewFolder is not null)
        {
            _bsPathfinderField.BFieldClear();
            BSPathfinderShownRefine(string.Empty);
        }

        return true;
    }

    private bool BSPathfinderEntriesRefine(string folder, string? navigated)
    {
        if (BSPathfinderEntries.BSPathfinderEntriesRead(
                _bsPathfinderListing, _bsPathfinderSuffixes, _bsPathfinderMedium, folder, string.Empty)
            is not IReadOnlyList<string> entries)
        {
            _bsPathfinderAlert = _bsPathfinderCatalog.BLocalizationTextRead("Pathfinder.Unreadable");
            return false;
        }

        _bsPathfinderSight = (navigated, entries);
        if (_bsPathfinderMedium is null)
        {
            _bsPathfinderField.BFieldClear();
        }

        string typed = _bsPathfinderField.BFieldText;
        BSPathfinderShownRefine(typed[..(typed.LastIndexOfAny(['\\', '/']) + 1)]);
        return true;
    }

    private void BSPathfinderTypedAccept()
    {
        string path = BSPathfinderPath.BSPathfinderPathResolve(BSPathfinderFolderShown, _bsPathfinderField.BFieldText);
        if (_bsPathfinderSuffixes is null && _bsPathfinderMedium is null)
        {
            if (BSPathfinderFolderOpen(path))
            {
                _bsPathfinderChoose(path);
            }

            return;
        }

        if (_bsPathfinderMedium is not null && _bsPathfinderField.BFieldText.Length == 0)
        {
            return;
        }

        string suffix = _bsPathfinderMedium?.BSPathfinderMediumChoice.CPortraitChoiceSuffix ?? string.Empty;
        string named = path.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) ? path : path + suffix;
        if (_bsPathfinderPresence(named))
        {
            _bsPathfinderChoose(named);
            return;
        }

        if (BSPathfinderFolderOpen(path))
        {
            _bsPathfinderField.BFieldClear();
        }
        else if (_bsPathfinderMedium is not null)
        {
            _bsPathfinderChoose(named);
        }
        else
        {
            _bsPathfinderAlert = _bsPathfinderCatalog.BLocalizationTextRead("Pathfinder.Missing");
        }
    }

    private void BSPathfinderLitAccept()
    {
        IReadOnlyList<string> shown = _bsPathfinderPreview.BSPathfinderPreviewEntries;
        if (_bsPathfinderLit >= shown.Count)
        {
            return;
        }

        string entry = shown[_bsPathfinderLit];
        string path = BSPathfinderPath.BSPathfinderPathResolve(BSPathfinderFolderPreview, entry);
        if (entry.EndsWith('\\'))
        {
            BSPathfinderFolderOpen(path);
        }
        else if (_bsPathfinderMedium is not null || _bsPathfinderPresence(path))
        {
            _bsPathfinderChoose(path);
        }
        else
        {
            _bsPathfinderAlert = _bsPathfinderCatalog.BLocalizationTextRead("Pathfinder.Missing");
        }
    }

    private void BSPathfinderShownRefine(string text)
    {
        int cut = text.LastIndexOfAny(['\\', '/']) + 1;
        string segment = text[cut..];
        string? folder =
            cut == 0 ? null : BSPathfinderPath.BSPathfinderPathResolve(BSPathfinderFolderShown, text[..cut]);
        _bsPathfinderPreview = folder is null
            ? (null, [.. _bsPathfinderSight.BSPathfinderSightEntries
                .Where(entry => entry.StartsWith(segment, StringComparison.OrdinalIgnoreCase))])
            : (folder, BSPathfinderEntries.BSPathfinderEntriesRead(
                    _bsPathfinderListing, _bsPathfinderSuffixes, _bsPathfinderMedium, folder, segment)
                ?? ("..\\".StartsWith(segment, StringComparison.OrdinalIgnoreCase) ? ["..\\"] : []));
        _bsPathfinderLit = 0;
    }

    private void BSPathfinderLitMove(int step)
    {
        _bsPathfinderLit = Math.Clamp(
            _bsPathfinderLit + step, 0, Math.Max(_bsPathfinderPreview.BSPathfinderPreviewEntries.Count - 1, 0));
    }
}
