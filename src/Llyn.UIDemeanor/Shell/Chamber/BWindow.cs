using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.UIDemeanor.Capsule;

namespace Llyn.UIDemeanor;

internal sealed class BWindow
{
    internal const int BWindowWidthMinimum = 80;
    internal const int BWindowHeightMinimum = 24;

    private readonly BConsole _bWindowConsole;
    private readonly BLocalizationCatalog _bWindowCatalog;
    private readonly CAtelier? _bWindowAtelier;
    private readonly CEnvoy _bWindowEnvoy;
    private readonly BWardrobe _bWindowWardrobe;
    private readonly BBlueprintPart _bWindowBlueprint;
    private readonly BBlueprintPart _bWindowHelpBlueprint;
    private readonly BNavigation _bWindowTabs;
    private readonly BMural _bWindowMural;
    private readonly CCourier? _bWindowCourier;
    private readonly BEstablishment _bWindowEstablishment;
    private readonly TaskCompletionSource _bWindowExit = new();
    private readonly BDocket _bWindowDocket;
    private readonly BMirror _bWindowMirror;
    private (int BConsoleWidth, int BConsoleHeight) _bWindowSize;
    private BCanvas? _bWindowCanvas;
    private bool _bWindowChanged;
    private BLibrary? _bWindowPanel;
    private BInput? _bWindowInput;

    internal BWindow(
        BConsole console,
        BLoop loop,
        BLocalizationCatalog catalog,
        CAtelier? atelier,
        BEnvoy envoy,
        BBlueprint blueprint,
        (string?, BTheme) theme,
        Func<string, BTheme?> themes,
        LCapsuleConsole capsule)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(loop);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(theme.Item2);
        ArgumentNullException.ThrowIfNull(themes);
        ArgumentNullException.ThrowIfNull(capsule);

        _bWindowConsole = console;
        _bWindowCatalog = catalog;
        _bWindowAtelier = atelier;
        _bWindowEnvoy = envoy;
        Action changed = () => _bWindowChanged = true;
        _bWindowDocket = new BDocket(loop, _bWindowExit.Task, changed);
        _bWindowBlueprint = BContract.BContractBlueprintFind(blueprint, "AWindow");
        _bWindowHelpBlueprint = BContract.BContractBlueprintFind(blueprint, "ASParley");
        string glyphs = _bWindowHelpBlueprint.BBlueprintPartChildren is [{ BBlueprintPartGlyphs: string set }]
            ? set : string.Empty;
        _bWindowWardrobe = new BWardrobe(
            theme, themes, envoy.BEnvoyThemeShow, blueprint.BBlueprintGlyphs, glyphs, capsule, atelier);
        _bWindowTabs = new BNavigation(blueprint, atelier);
        _bWindowEstablishment = new BEstablishment(_bWindowBlueprint, catalog);
        _bWindowMural = new BMural(blueprint, _bWindowBlueprint, catalog, _bWindowTabs, _bWindowEstablishment);
        CCourier? courier = atelier is null ? null : CCourier.CCourierCreate(atelier);
        _bWindowCourier = courier;
        _bWindowMirror = new BMirror(
            catalog,
            atelier,
            envoy,
            _bWindowWardrobe,
            _bWindowBlueprint,
            _bWindowTabs,
            heard => BHeadquarter.BHeadquarterBuild(
                catalog,
                courier,
                atelier?.CAtelierWorkspace,
                envoy,
                heard,
                () => BHeadquarter.BHeadquarterAboutShow(
                    catalog, _bWindowHelpBlueprint, parley => BWindowParleyConsult(parley)),
                () => BWindowHelpShow(BKeymapFocus.BKeymapFocusWindow),
                () => BWindowHelpShow(BKeymapFocus.BKeymapFocusHelp),
                BWindowQuitConfirm),
            () => _bWindowPanel?.BLibraryMenuBuild() ?? [],
            BWindowNavigationShow,
            () =>
            {
                _bWindowCanvas = null;
                BWindowSizeObserve(_bWindowSize.BConsoleWidth, _bWindowSize.BConsoleHeight);
            },
            changed);
        BWindowKeymap = BKeymapWindow.BKeymapWindowBuild(
            BWindowHelpShow,
            BWindowQuitConfirm,
            BWindowNavigationShow,
            () => _bWindowMirror.BMirrorRoof.BRoofClose(),
            _bWindowTabs.BNavigationRetreat,
            _bWindowTabs.BNavigationAdvance,
            () => _bWindowMirror.BMirrorNavigation);
    }

    internal Task BWindowExit => _bWindowExit.Task;

    internal BKeymap BWindowKeymap { get; }

    internal void BWindowOpen()
    {
        (int width, int height) = _bWindowConsole.BConsoleSizeRead();
        BWindowSizeObserve(width, height);
        if (_bWindowAtelier is CAtelier atelier)
        {
            Dictionary<string, BLibrary> panels = [];
            Func<string, Func<bool>> shown = tab => () =>
                _bWindowPanel is BLibrary panel && panel == panels.GetValueOrDefault(tab);
            panels["Library"] = new BLibrary(
                _bWindowCatalog,
                atelier,
                _bWindowEnvoy,
                _bWindowHelpBlueprint,
                (parley, until) => BWindowParleyConsult(parley, until: until),
                shown("Library"),
                () => _bWindowChanged = true,
                () => _bWindowWardrobe.BWardrobeWidth);
            panels["Favorite"] = new BLibrary(
                _bWindowCatalog,
                atelier,
                _bWindowEnvoy,
                _bWindowHelpBlueprint,
                (parley, until) => BWindowParleyConsult(parley, until: until),
                shown("Favorite"),
                () => _bWindowChanged = true,
                () => _bWindowWardrobe.BWardrobeWidth,
                CFavorite.CFavoriteCreate);
            bool typing = false;
            _bWindowInput = new BInput(
                _bWindowCatalog, atelier, _bWindowEnvoy, () => typing, () => _bWindowChanged = true);
            atelier.CAtelierNavigation.CNavigationChanged += BObserver.BObserverCreate<CNavigationState>(state =>
            {
                if (state.CNavigationStateTab is string mode)
                {
                    _bWindowPanel = panels.GetValueOrDefault(mode);
                    typing = mode == "Input";
                }
            });
            atelier.CAtelierNavigation.CNavigationArrived += () =>
            {
                _bWindowMirror.BMirrorRoof.BRoofClose();
                _bWindowChanged = true;
            };
            _bWindowMirror.BMirrorOpen(atelier, _bWindowCourier);
            atelier.CAtelierWorkspace.CWorkspaceEstablishmentChanged +=
                BObserver.BObserverCreate<CEstablishment>(establishment =>
                {
                    _bWindowEstablishment.BEstablishmentObserve(establishment);
                    _bWindowChanged = true;
                });
        }
    }

    internal void BWindowAtelierOpen()
    {
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
        BBlueprint.BBlueprintPlace(_bWindowBlueprint, 0, 0, width, height);
        (_, BTheme theme) = _bWindowWardrobe.BWardrobeTheme;
        _bWindowCanvas = new BCanvas(_bWindowConsole, _bWindowWardrobe.BWardrobeWidth, theme, width, height);
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
        BKeymapFocus focus = BWindowFocusRead();
        BContract.BContractFind(_bWindowBlueprint, "AIndex").BBlueprintPartFolded =
            _bWindowPanel?.BLibraryWhole ?? _bWindowInput?.BInputWhole ?? false;
        bool focused = focus is BKeymapFocus.BKeymapFocusWindow or BKeymapFocus.BKeymapFocusLectern
            or BKeymapFocus.BKeymapFocusEditor;
        IEnumerable<BCanvasText> panel = _bWindowPanel is BLibrary library
            ? library.BLibraryDraw(BWindowMarksRead(), _bWindowBlueprint, focused, _bWindowWardrobe.BWardrobeWidth)
            : _bWindowInput?.BInputDraw(
                BWindowMarksRead(), _bWindowBlueprint, focused, _bWindowWardrobe.BWardrobeWidth) ?? [];
        canvas.BCanvasFrameDraw([.. _bWindowMural.BMuralDraw(
            width,
            height,
            focus,
            _bWindowMirror.BMirrorRoof,
            _bWindowMirror.BMirrorNavigation,
            _bWindowMirror.BMirrorCourier,
            _bWindowDocket.BDocketParleys,
            panel,
            _bWindowWardrobe.BWardrobeGlyphSets,
            BWindowMarksRead(),
            BWindowKeymapFind,
            _bWindowWardrobe.BWardrobeWidth,
            _bWindowPanel?.BLibraryChronicle)]);
        canvas.BCanvasSync();
    }

    internal void BWindowKeyObserve(ConsoleKeyInfo key)
    {
        BKeymapFocus focus = BWindowFocusRead();
        const BKeymapFocus window = BKeymapFocus.BKeymapFocusWindow;
        BKeymapFocus beneath = focus is BKeymapFocus.BKeymapFocusLectern or BKeymapFocus.BKeymapFocusEditor
            ? window
            : focus;
        if ((BWindowKeymap.BKeymapFind(focus, key)
                ?? _bWindowPanel?.BLibraryKeymap.BKeymapFind(focus, key)
                ?? _bWindowInput?.BInputKeymap.BKeymapFind(focus, key)
                ?? BWindowKeymap.BKeymapFind(beneath, key)
                ?? _bWindowPanel?.BLibraryKeymap.BKeymapFind(beneath, key)
                ?? _bWindowInput?.BInputKeymap.BKeymapFind(beneath, key))
            is BKeymapRow row)
        {
            BField.BFieldDeferredApply();
            row.BKeymapRowAction(key);
        }
        else if (focus is BKeymapFocus.BKeymapFocusParley or BKeymapFocus.BKeymapFocusHelp)
        {
            _bWindowDocket.BDocketKeyObserve(key);
        }
        else if (focus is BKeymapFocus.BKeymapFocusWindow or BKeymapFocus.BKeymapFocusRoof
                or BKeymapFocus.BKeymapFocusLectern or BKeymapFocus.BKeymapFocusEditor
            && _bWindowMirror.BMirrorRoof.BRoofKeyObserve(key))
        {
            _bWindowChanged = true;
        }
        else if (focus is BKeymapFocus.BKeymapFocusWindow or BKeymapFocus.BKeymapFocusEditor)
        {
            if (_bWindowPanel is BLibrary panel)
            {
                panel.BLibraryObserve(focus, key);
            }
            else if (_bWindowInput is BInput input)
            {
                input.BInputObserve(key);
            }
        }
    }

    internal int? BWindowParleyConsult(BSParley parley, Action<Exception>? fault = null, Task? until = null)
    {
        return _bWindowDocket.BDocketConsult(parley, fault, until);
    }

    internal IReadOnlyDictionary<string, IReadOnlyList<string>> BWindowMarksRead()
    {
        return _bWindowWardrobe.BWardrobeMarks;
    }

    private BKeymapFocus BWindowFocusRead()
    {
        (int width, int height) = _bWindowSize;
        return width < BWindowWidthMinimum || height < BWindowHeightMinimum ? BKeymapFocus.BKeymapFocusNotice
            : _bWindowDocket.BDocketParleys is [.., BSParley top] ? top.BSParleyKeymapFocus
            : _bWindowMirror.BMirrorRoof.BRoofKeymapFocus is BKeymapFocus.BKeymapFocusWindow
                ? _bWindowPanel?.BLibraryKeymapFocus ?? _bWindowInput?.BInputKeymapFocus
                    ?? BKeymapFocus.BKeymapFocusWindow
            : _bWindowMirror.BMirrorRoof.BRoofKeymapFocus;
    }

    private void BWindowHelpShow(BKeymapFocus focus)
    {
        BSHelp help = new(
            _bWindowCatalog,
            kind =>
            {
                IReadOnlyList<BSParley> parleys = _bWindowDocket.BDocketParleys;
                BSParley? box = kind == BKeymapFocus.BKeymapFocusWindow ? null
                    : parleys.LastOrDefault(parley => parley.BSParleyKeymapFocus == kind) ?? parleys.LastOrDefault();
                return
                [
                    .. BWindowKeymap.BKeymapRead(kind),
                    .. _bWindowPanel?.BLibraryKeymap.BKeymapRead(kind)
                        ?? _bWindowInput?.BInputKeymap.BKeymapRead(kind) ?? [],
                    .. kind == BKeymapFocus.BKeymapFocusWindow
                        ? (BKeymapRow[])
                        [
                            .. _bWindowPanel?.BLibraryKeymap.BKeymapRead(BKeymapFocus.BKeymapFocusLectern) ?? [],
                            .. _bWindowPanel?.BLibraryKeymap.BKeymapRead(BKeymapFocus.BKeymapFocusEditor)
                                ?? _bWindowInput?.BInputKeymap.BKeymapRead(BKeymapFocus.BKeymapFocusEditor) ?? [],
                        ]
                        : [],
                    .. _bWindowMirror.BMirrorRoof.BRoofKeymap.BKeymapRead(kind),
                    .. box?.BSParleyKeymapRead() ?? [],
                ];
            },
            focus,
            _bWindowWardrobe.BWardrobeWidth);
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

    private void BWindowNavigationShow()
    {
        if (_bWindowAtelier is not CAtelier atelier || _bWindowMirror.BMirrorNavigation is not CNavigationState state)
        {
            return;
        }

        TaskCompletionSource<string> chosen = new();
        BSNavigation body = new(
            _bWindowCatalog,
            _bWindowTabs.BNavigationTabs,
            state,
            tab => chosen.TrySetResult(tab),
            _bWindowWardrobe.BWardrobeWidth);
        BSParley parley = new(
            _bWindowHelpBlueprint, _bWindowCatalog.BLocalizationTextRead("Window.Navigation.Title"), "dialog", [],
            [new BSParleyButton(_bWindowCatalog.BLocalizationTextRead("Window.Navigation.Close"))], 0, body);
        _ = BWindowParleyConsult(parley, until: chosen.Task);
        if (chosen.Task.IsCompleted && !_bWindowExit.Task.IsCompleted)
        {
            _ = atelier.CAtelierNavigation.CNavigationTabSelect(chosen.Task.Result);
        }
    }

    private BKeymapRow? BWindowKeymapFind(BKeymapFocus focus, ConsoleKeyInfo key)
    {
        if (focus is BKeymapFocus.BKeymapFocusLectern or BKeymapFocus.BKeymapFocusEditor)
        {
            return _bWindowPanel?.BLibraryKeymap.BKeymapFind(focus, key)
                ?? _bWindowInput?.BInputKeymap.BKeymapFind(focus, key)
                ?? BWindowKeymapFind(BKeymapFocus.BKeymapFocusWindow, key);
        }

        return BWindowKeymap.BKeymapFind(focus, key)
            ?? _bWindowPanel?.BLibraryKeymap.BKeymapFind(focus, key)
            ?? _bWindowInput?.BInputKeymap.BKeymapFind(focus, key)
            ?? _bWindowMirror.BMirrorRoof.BRoofKeymap.BKeymapFind(focus, key)
            ?? (_bWindowDocket.BDocketParleys is [.., BSParley top]
                ? top.BSParleyKeymap.BKeymapFind(focus, key) : null);
    }

    private void BWindowQuitConfirm()
    {
        if (_bWindowAtelier is not CAtelier atelier || atelier.CAtelierQuitConfirm(_bWindowEnvoy))
        {
            _bWindowExit.TrySetResult();
        }
    }
}
