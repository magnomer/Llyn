using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BWindow
{
    internal const int BWindowWidthMinimum = 80;
    internal const int BWindowHeightMinimum = 24;

    private readonly BConsole _bWindowConsole;
    private readonly BLoop _bWindowLoop;
    private readonly BLocalizationCatalog _bWindowCatalog;
    private readonly CAtelier? _bWindowAtelier;
    private readonly CEnvoy _bWindowEnvoy;
    private readonly BTheme _bWindowTheme;
    private readonly BBlueprintPart _bWindowBlueprint;
    private readonly BBlueprintPart _bWindowHelpBlueprint;
    private readonly IReadOnlyDictionary<string, IReadOnlyList<string>> _bWindowMarks;
    private readonly BNavigation _bWindowTabs;
    private readonly BMural _bWindowMural;
    private readonly BRoofMenu _bWindowHeadquarter;
    private readonly TaskCompletionSource _bWindowExit = new();
    private readonly List<BSParley> _bWindowParleys = [];
    private BRoof _bWindowRoof;
    private CNavigationState? _bWindowNavigation;
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
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> glyphs =
            blueprint.BBlueprintGlyphs;
        _bWindowBlueprint = BContract.BContractBlueprintFind(blueprint, "AWindow");
        _bWindowHelpBlueprint = BContract.BContractBlueprintFind(blueprint, "ASParley");
        _bWindowMarks = _bWindowHelpBlueprint.BBlueprintPartChildren is [{ BBlueprintPartGlyphs: string set }]
            && glyphs.TryGetValue(set, out IReadOnlyDictionary<string, IReadOnlyList<string>>? marks)
                ? marks
                : new Dictionary<string, IReadOnlyList<string>>();
        _bWindowTabs = new BNavigation(blueprint, atelier);
        _bWindowMural = new BMural(blueprint, _bWindowBlueprint, catalog, _bWindowMarks, _bWindowTabs);
        _bWindowHeadquarter = BHeadquarter.BHeadquarterBuild(
            catalog,
            () => BHeadquarter.BHeadquarterAboutShow(
                catalog, _bWindowHelpBlueprint, parley => BWindowParleyConsult(parley)),
            () => BWindowHelpShow(BKeymapFocus.BKeymapFocusWindow),
            () => BWindowHelpShow(BKeymapFocus.BKeymapFocusHelp),
            BWindowQuitConfirm);
        _bWindowRoof = BWindowRoofBuild();
        Func<BKeymapLive> shown = static () => BKeymapLive.BKeymapLiveShown;
        Func<Action, Action<ConsoleKeyInfo>> pressed = action => _ => action();
        Action<ConsoleKeyInfo> quit = pressed(BWindowQuitConfirm);
        Func<Action, Action<ConsoleKeyInfo>> closing = action => _ =>
        {
            _bWindowRoof.BRoofClose();
            action();
        };
        Action back = _bWindowTabs.BNavigationRetreat;
        Action forward = _bWindowTabs.BNavigationAdvance;
        Func<Func<CVoyageState, bool>, Func<BKeymapLive>> voyage = can => () =>
            _bWindowNavigation is not CNavigationState state ? BKeymapLive.BKeymapLiveHidden
            : can(state.CNavigationStateVoyage) ? BKeymapLive.BKeymapLiveShown
            : BKeymapLive.BKeymapLiveDim;
        Func<BKeymapLive> past = voyage(static state => state.CVoyageStatePast);
        Func<BKeymapLive> future = voyage(static state => state.CVoyageStateFuture);
        Func<BKeymapLive> open = voyage(static _ => true);
        const ConsoleModifiers alt = ConsoleModifiers.Alt;
        const BKeymapFocus window = BKeymapFocus.BKeymapFocusWindow;
        const BKeymapFocus parley = BKeymapFocus.BKeymapFocusParley;
        const BKeymapFocus notice = BKeymapFocus.BKeymapFocusNotice;
        const BKeymapFocus help = BKeymapFocus.BKeymapFocusHelp;
        const BKeymapFocus roof = BKeymapFocus.BKeymapFocusRoof;
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
            new(ConsoleKey.F1, 0, roof, "Legend.Help", closing(() => BWindowHelpShow(roof)), shown),
            new(ConsoleKey.F10, 0, roof, "Legend.Quit", closing(BWindowQuitConfirm), shown),
            new(ConsoleKey.C, ConsoleModifiers.Control, roof, "Legend.Quit", closing(BWindowQuitConfirm), shown),
            new(ConsoleKey.F1, alt, window, "Window.Help.Choose", pressed(BWindowNavigationShow), open),
            new(ConsoleKey.LeftArrow, alt, window, "Voyage.Retreat", pressed(back), past),
            new(ConsoleKey.RightArrow, alt, window, "Voyage.Advance", pressed(forward), future),
            new(ConsoleKey.F1, alt, roof, "Window.Help.Choose", closing(BWindowNavigationShow), open),
            new(ConsoleKey.LeftArrow, alt, roof, "Voyage.Retreat", closing(back), past),
            new(ConsoleKey.RightArrow, alt, roof, "Voyage.Advance", closing(forward), future),
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
            atelier.CAtelierNavigation.CNavigationArrived += () =>
            {
                _bWindowRoof.BRoofClose();
                _bWindowChanged = true;
            };
            atelier.CAtelierNavigation.CNavigationChanged +=
                BObserver.BObserverCreate<CNavigationState>(BWindowNavigationObserve);
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
        BBlueprint.BBlueprintPlace(_bWindowBlueprint, 0, 0, width, height);
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
        canvas.BCanvasFrameDraw([.. _bWindowMural.BMuralDraw(
            width, height, BWindowFocusRead(), _bWindowRoof, _bWindowNavigation, _bWindowParleys, BWindowKeymapFind)]);
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
        else if (focus is BKeymapFocus.BKeymapFocusWindow or BKeymapFocus.BKeymapFocusRoof
            && _bWindowRoof.BRoofKeyObserve(key))
        {
            _bWindowChanged = true;
        }
    }

    internal int? BWindowParleyConsult(BSParley parley, Action<Exception>? fault = null, Task? until = null)
    {
        ArgumentNullException.ThrowIfNull(parley);

        if (_bWindowExit.Task.IsCompleted)
        {
            return null;
        }

        _bWindowParleys.Add(parley);
        _bWindowChanged = true;
        _bWindowLoop.BLoopRun(
            until is null
                ? Task.WhenAny(parley.BSParleyAnswer, _bWindowExit.Task)
                : Task.WhenAny(parley.BSParleyAnswer, until, _bWindowExit.Task),
            fault);
        if (until is not null)
        {
            _bWindowParleys.Remove(parley);
            _bWindowChanged = true;
        }

        parley.BSParleyKeyObserve(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        return parley.BSParleyAnswer.Result;
    }

    private BKeymapFocus BWindowFocusRead()
    {
        (int width, int height) = _bWindowSize;
        return width < BWindowWidthMinimum || height < BWindowHeightMinimum ? BKeymapFocus.BKeymapFocusNotice
            : _bWindowParleys is [.., BSParley top] ? top.BSParleyKeymapFocus
            : _bWindowRoof.BRoofKeymapFocus;
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
                    .. _bWindowRoof.BRoofKeymap.BKeymapRead(kind),
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

    private void BWindowNavigationObserve(CNavigationState state)
    {
        _bWindowNavigation = state;
        _bWindowRoof = BWindowRoofBuild();
        _bWindowChanged = true;
    }

    private void BWindowNavigationShow()
    {
        if (_bWindowAtelier is not CAtelier atelier || _bWindowNavigation is not CNavigationState state)
        {
            return;
        }

        TaskCompletionSource<string> chosen = new();
        BSNavigation body = new(_bWindowCatalog, _bWindowTabs.BNavigationTabs, state, tab => chosen.TrySetResult(tab));
        BSParley parley = new(
            _bWindowHelpBlueprint, _bWindowCatalog.BLocalizationTextRead("Window.Navigation.Title"), "dialog", [],
            [new BSParleyButton(_bWindowCatalog.BLocalizationTextRead("Window.Navigation.Close"))], 0, body);
        _ = BWindowParleyConsult(parley, until: chosen.Task);
        if (chosen.Task.IsCompleted && !_bWindowExit.Task.IsCompleted)
        {
            _ = atelier.CAtelierNavigation.CNavigationTabSelect(chosen.Task.Result);
        }
    }

    private BRoof BWindowRoofBuild()
    {
        BRoofMenu panel = _bWindowTabs.BNavigationBuild(
            _bWindowCatalog, _bWindowNavigation, _bWindowMarks, BWindowNavigationShow);
        return new BRoof(_bWindowBlueprint, [_bWindowHeadquarter, panel]);
    }

    private BKeymapRow? BWindowKeymapFind(BKeymapFocus focus, ConsoleKeyInfo key)
    {
        return BWindowKeymap.BKeymapFind(focus, key)
            ?? _bWindowRoof.BRoofKeymap.BKeymapFind(focus, key)
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
}
