using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BWindow
{
    internal const int BWindowWidthMinimum = 80;
    internal const int BWindowHeightMinimum = 24;
    private const int BWindowLegendWidth = 8;
    private const int BWindowNoticeWidth = 44;
    private const int BWindowNoticeHeight = 8;

    private readonly BConsole _bWindowConsole;
    private readonly BLoop _bWindowLoop;
    private readonly BLocalizationCatalog _bWindowCatalog;
    private readonly CAtelier? _bWindowAtelier;
    private readonly CEnvoy _bWindowEnvoy;
    private readonly BTheme _bWindowTheme;
    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> _bWindowGlyphs;
    private readonly BBlueprintPart _bWindowBlueprint;
    private readonly BBlueprintPart _bWindowEstablishment;
    private readonly BBlueprintPart _bWindowLegend;
    private readonly BBlueprintPart _bWindowNotice;
    private readonly BBlueprintPart _bWindowNoticeText;
    private readonly BBlueprintPart _bWindowHelpBlueprint;
    private readonly TaskCompletionSource _bWindowExit = new();
    private readonly List<BSParley> _bWindowParleys = [];
    private (int BConsoleWidth, int BConsoleHeight) _bWindowSize;
    private BCanvas? _bWindowCanvas;
    private bool _bWindowChanged;

    internal BWindow(
        BConsole console,
        BLoop loop,
        BLocalizationCatalog catalog,
        CAtelier? atelier,
        CEnvoy envoy,
        BBlueprint blueprint,
        BTheme theme)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(loop);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(theme);

        _bWindowConsole = console;
        _bWindowLoop = loop;
        _bWindowCatalog = catalog;
        _bWindowAtelier = atelier;
        _bWindowEnvoy = envoy;
        _bWindowTheme = theme;
        _bWindowGlyphs = blueprint.BBlueprintGlyphs;
        _bWindowBlueprint = BContract.BContractBlueprintFind(blueprint, "AWindow");
        _bWindowEstablishment = BContract.BContractFind(_bWindowBlueprint, "AEstablishment");
        _bWindowLegend = BContract.BContractFind(_bWindowBlueprint, "ALegend");
        _bWindowNotice = BContract.BContractBlueprintFind(blueprint, "ANotice");
        _bWindowNoticeText = BContract.BContractFind(_bWindowNotice, "ANoticeText");
        _bWindowHelpBlueprint = BContract.BContractBlueprintFind(blueprint, "ASParley");
        Func<BKeymapLive> shown = static () => BKeymapLive.BKeymapLiveShown;
        Action<ConsoleKeyInfo> quit = _ => BWindowQuitConfirm();
        const BKeymapFocus window = BKeymapFocus.BKeymapFocusWindow;
        const BKeymapFocus parley = BKeymapFocus.BKeymapFocusParley;
        const BKeymapFocus notice = BKeymapFocus.BKeymapFocusNotice;
        const BKeymapFocus help = BKeymapFocus.BKeymapFocusHelp;
        BWindowKeymap = new BKeymap(
        [
            new(ConsoleKey.F1, 0, window, "Legend.Help", _ => BWindowHelpShow(window), shown),
            new(ConsoleKey.F10, 0, window, "Legend.Quit", quit, shown),
            new(ConsoleKey.C, ConsoleModifiers.Control, window, "Legend.Quit", quit, shown),
            new(ConsoleKey.F10, 0, notice, "Legend.Quit", quit, shown),
            new(ConsoleKey.C, ConsoleModifiers.Control, notice, "Legend.Quit", quit, shown),
            new(ConsoleKey.F1, 0, parley, "Legend.Help", _ => BWindowHelpShow(parley), shown),
            new(ConsoleKey.C, ConsoleModifiers.Control, parley, "Legend.Quit", quit, shown),
            new(ConsoleKey.C, ConsoleModifiers.Control, help, "Legend.Quit", quit, shown),
        ]);
    }

    internal Task BWindowExit => _bWindowExit.Task;

    internal BKeymap BWindowKeymap { get; }

    internal void BWindowOpen()
    {
        (int width, int height) = _bWindowConsole.BConsoleSizeRead();
        BWindowSizeObserve(width, height);
        if (_bWindowAtelier is CAtelier atelier)
        {
            atelier.CAtelierOpen(_bWindowEnvoy, BObserver.BObserverCreate<Action>(static work => work()));
        }
    }

    internal void BWindowSizeObserve(int width, int height)
    {
        (width, height) = (Math.Max(width, 0), Math.Max(height, 0));
        if (_bWindowCanvas is not null && (width, height) == _bWindowSize)
        {
            return;
        }

        _bWindowSize = (width, height);
        _bWindowCanvas = new BCanvas(_bWindowConsole, new BCanvasWidth(false), _bWindowTheme, width, height);
        _bWindowChanged = true;
    }

    internal void BWindowBatchObserve()
    {
        if (!_bWindowChanged || _bWindowCanvas is not BCanvas canvas)
        {
            return;
        }

        _bWindowChanged = false;
        (int width, int height) = _bWindowSize;
        bool small = BWindowFocusRead() == BKeymapFocus.BKeymapFocusNotice;
        canvas.BCanvasFrameDraw([.. small ? BWindowNoticeBuild(width, height) : BWindowFrameBuild(width, height)]);
        canvas.BCanvasSync();
    }

    internal void BWindowKeyObserve(ConsoleKeyInfo key)
    {
        BKeymapFocus focus = BWindowFocusRead();
        if (BWindowKeymap.BKeymapFind(focus, key) is BKeymapRow row)
        {
            row.BKeymapRowAction(key);
        }
        else if (focus is BKeymapFocus.BKeymapFocusParley or BKeymapFocus.BKeymapFocusHelp)
        {
            BWindowParleyObserve(key);
        }
    }

    internal int? BWindowParleyConsult(BSParley parley, Action<Exception>? fault = null)
    {
        ArgumentNullException.ThrowIfNull(parley);

        if (_bWindowExit.Task.IsCompleted)
        {
            return null;
        }

        _bWindowParleys.Add(parley);
        _bWindowChanged = true;
        _bWindowLoop.BLoopRun(Task.WhenAny(parley.BSParleyAnswer, _bWindowExit.Task), fault);
        parley.BSParleyKeyObserve(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        return parley.BSParleyAnswer.Result;
    }

    private BKeymapFocus BWindowFocusRead()
    {
        (int width, int height) = _bWindowSize;
        return width < BWindowWidthMinimum || height < BWindowHeightMinimum ? BKeymapFocus.BKeymapFocusNotice
            : _bWindowParleys is [.., BSParley top] ? top.BSParleyKeymapFocus
            : BKeymapFocus.BKeymapFocusWindow;
    }

    private void BWindowHelpShow(BKeymapFocus focus)
    {
        BSHelp help = new(
            _bWindowCatalog,
            kind =>
            {
                BSParley? box = kind == BKeymapFocus.BKeymapFocusWindow ? null
                    : _bWindowParleys.LastOrDefault(parley => parley.BSParleyKeymapFocus == kind)
                        ?? _bWindowParleys.LastOrDefault();
                return
                [
                    .. BWindowKeymap.BKeymapRead(kind),
                    .. box?.BSParleyKeymap.BKeymapRead(box.BSParleyKeymapFocus) ?? [],
                ];
            },
            focus);
        BSParleyButton close = new(_bWindowCatalog.BLocalizationTextRead("Window.Help.Close"));
        BSParley parley = new(
            _bWindowHelpBlueprint,
            _bWindowCatalog.BLocalizationTextRead("Window.Help.Title"),
            "dialog",
            [],
            [close],
            0,
            help,
            BKeymapFocus.BKeymapFocusHelp);
        _ = BWindowParleyConsult(parley);
    }

    private BKeymapRow? BWindowKeymapFind(BKeymapFocus focus, ConsoleKeyInfo key)
    {
        return BWindowKeymap.BKeymapFind(focus, key)
            ?? (_bWindowParleys is [.., BSParley top] ? top.BSParleyKeymap.BKeymapFind(focus, key) : null);
    }

    private void BWindowQuitConfirm()
    {
        if (_bWindowAtelier is not CAtelier atelier || atelier.CAtelierQuitConfirm(_bWindowEnvoy))
        {
            _bWindowExit.TrySetResult();
        }
    }

    private void BWindowParleyObserve(ConsoleKeyInfo key)
    {
        BSParley parley = _bWindowParleys[^1];
        parley.BSParleyKeyObserve(key);
        if (parley.BSParleyAnswer.IsCompleted)
        {
            _bWindowParleys.Remove(parley);
        }

        _bWindowChanged = true;
    }

    private IEnumerable<BCanvasText> BWindowFrameBuild(int width, int height)
    {
        BBlueprint.BBlueprintPlace(_bWindowBlueprint, 0, 0, width, height);
        foreach (BCanvasText scaffold in BBlueprint.BBlueprintDraw(_bWindowBlueprint, _bWindowGlyphs))
        {
            yield return scaffold;
        }

        BBlueprintPart establishment = _bWindowEstablishment;
        yield return new BCanvasText(
            establishment.BBlueprintPartRow,
            establishment.BBlueprintPartColumn,
            establishment.BBlueprintPartWidth,
            _bWindowCatalog.BLocalizationTextRead("Window.Opening"),
            establishment.BBlueprintPartTheme);
        BBlueprintPart legend = _bWindowLegend;
        foreach (BCanvasText key in BWindowLegendBuild(
            legend.BBlueprintPartRow, legend.BBlueprintPartColumn, BWindowFocusRead()))
        {
            yield return key;
        }

        foreach (BSParley parley in _bWindowParleys)
        {
            foreach (BCanvasText text in parley.BSParleyDraw(_bWindowGlyphs, width, height))
            {
                yield return text;
            }
        }
    }

    private IEnumerable<BCanvasText> BWindowLegendBuild(int row, int column, BKeymapFocus focus)
    {
        string theme = _bWindowLegend.BBlueprintPartTheme;
        int packed = column;
        for (ConsoleKey key = ConsoleKey.F1; key <= ConsoleKey.F10; key++)
        {
            BKeymapRow? found = BWindowKeymapFind(focus, new ConsoleKeyInfo('\0', key, false, false, false));
            BKeymapLive live = found?.BKeymapRowLive() ?? BKeymapLive.BKeymapLiveHidden;
            if (found is null || live == BKeymapLive.BKeymapLiveHidden)
            {
                continue;
            }

            int slot = key - ConsoleKey.F1;
            int left = focus == BKeymapFocus.BKeymapFocusNotice ? packed : column + (slot * BWindowLegendWidth);
            packed += BWindowLegendWidth;
            string number = (slot + 1).ToString(CultureInfo.InvariantCulture);
            string label = _bWindowCatalog.BLocalizationTextRead(found.BKeymapRowLabel);
            string role = live == BKeymapLive.BKeymapLiveDim ? "disabled" : $"{theme}-label";
            yield return new BCanvasText(row, left, number.Length, number, $"{theme}-number");
            yield return new BCanvasText(row, left + number.Length, BWindowLegendWidth - number.Length, label, role);
        }
    }

    private IEnumerable<BCanvasText> BWindowNoticeBuild(int width, int height)
    {
        string notice = string.Format(
            CultureInfo.CurrentCulture,
            _bWindowCatalog.BLocalizationTextRead("Window.Small"),
            BWindowWidthMinimum,
            BWindowHeightMinimum,
            width,
            height);
        BCanvasWidth measure = new(false);
        if (width < BWindowNoticeWidth || height < BWindowNoticeHeight)
        {
            IReadOnlyList<string> bare = measure.BCanvasWidthDivide(notice, width);
            for (int row = 0; row < bare.Count; row++)
            {
                yield return new BCanvasText(row, 0, width, bare[row], "text");
            }

            yield break;
        }

        BBlueprintPart text = _bWindowNoticeText;
        string theme = text.BBlueprintPartTheme;
        int across = BWindowNoticeWidth - 2;
        BBlueprint.BBlueprintPlace(_bWindowNotice, 0, 0, across, height - 1);
        IReadOnlyList<string> lines = measure.BCanvasWidthDivide(notice, text.BBlueprintPartWidth);
        int down = Math.Min(lines.Count + (height - 1 - text.BBlueprintPartHeight), height - 1);
        (int top, int left) = ((height - 1 - down) / 2, (width - across) / 2);
        BBlueprint.BBlueprintPlace(_bWindowNotice, top, left, across, down);
        foreach (BCanvasText scaffold in BBlueprint.BBlueprintDraw(_bWindowNotice, _bWindowGlyphs))
        {
            yield return scaffold;
        }

        const string title = " Llyn ";
        yield return new BCanvasText(top, left + ((across - title.Length) / 2), title.Length, title, theme);
        for (int line = 0; line < Math.Min(lines.Count, text.BBlueprintPartHeight); line++)
        {
            yield return new BCanvasText(
                text.BBlueprintPartRow + line, text.BBlueprintPartColumn, text.BBlueprintPartWidth, lines[line], theme);
        }

        foreach (BCanvasText key in BWindowLegendBuild(height - 1, 0, BKeymapFocus.BKeymapFocusNotice))
        {
            yield return key;
        }
    }
}
