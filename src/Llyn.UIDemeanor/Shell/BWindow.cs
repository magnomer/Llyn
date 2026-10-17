using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BWindow
{
    private const int BWindowWidthMinimum = 80;
    private const int BWindowHeightMinimum = 24;
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
    }

    internal Task BWindowExit => _bWindowExit.Task;

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
        bool small = width < BWindowWidthMinimum || height < BWindowHeightMinimum;
        canvas.BCanvasFrameDraw([.. small ? BWindowNoticeBuild(width, height) : BWindowFrameBuild(width, height)]);
        canvas.BCanvasSync();
    }

    internal void BWindowKeyObserve(ConsoleKeyInfo key)
    {
        bool interrupt = (key.Key == ConsoleKey.C && key.Modifiers == ConsoleModifiers.Control)
            || key.KeyChar == '\u0003';
        (int width, int height) = _bWindowSize;
        bool small = width < BWindowWidthMinimum || height < BWindowHeightMinimum;
        if (!interrupt && !small && _bWindowParleys.Count > 0)
        {
            BSParley parley = _bWindowParleys[^1];
            parley.BSParleyKeyObserve(key);
            if (parley.BSParleyAnswer.IsCompleted)
            {
                _bWindowParleys.Remove(parley);
            }

            _bWindowChanged = true;
            return;
        }

        if ((interrupt || key.Key == ConsoleKey.F10)
            && (_bWindowAtelier is not CAtelier atelier || atelier.CAtelierQuitConfirm(_bWindowEnvoy)))
        {
            _bWindowExit.TrySetResult();
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
            legend.BBlueprintPartRow,
            legend.BBlueprintPartColumn + (9 * BWindowLegendWidth),
            _bWindowParleys.Count > 0 ? "Legend.Close" : "Legend.Quit"))
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

    private IEnumerable<BCanvasText> BWindowLegendBuild(int row, int column, string key)
    {
        string theme = _bWindowLegend.BBlueprintPartTheme;
        string label = _bWindowCatalog.BLocalizationTextRead(key);
        yield return new BCanvasText(row, column, 2, "10", $"{theme}-number");
        yield return new BCanvasText(row, column + 2, BWindowLegendWidth - 2, label, $"{theme}-label");
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

        foreach (BCanvasText key in BWindowLegendBuild(height - 1, 0, "Legend.Quit"))
        {
            yield return key;
        }
    }
}
