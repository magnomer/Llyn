using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BLibrary
{
    private readonly BLocalizationCatalog _bLibraryCatalog;
    private readonly BBlueprintPart _bLibraryParley;
    private readonly Func<BSParley, Task?, int?> _bLibraryDocket;
    private readonly Func<bool> _bLibraryShown;
    private readonly Action _bLibraryChanged;
    private readonly Func<BCanvasWidth> _bLibraryMeasure;
    private readonly CPanel _bLibraryPanel;
    private readonly CEditor _bLibraryEditor;
    private readonly IReadOnlyList<CCatalogOrder> _bLibraryOrders;
    private readonly string _bLibraryOrderKey;
    private readonly Func<Task> _bLibraryPortrait;
    private readonly Func<Task>? _bLibraryMarkup;
    private readonly BIndex<CVistaRow> _bIndex;
    private readonly BLectern _bLibraryLectern;
    private bool _bLibrarySide;
    private bool _bLibraryWhole;
    private IReadOnlyList<string> _bLibraryLanguages = [];

    internal BLibrary(
        BLocalizationCatalog catalog, CAtelier atelier, CEnvoy envoy, BBlueprintPart parley,
        Func<BSParley, Task?, int?> docket, Func<bool> shown, Action changed, Func<BCanvasWidth> measure)
        : this(catalog, atelier, parley, docket, shown, changed, measure, CLibrary.CLibraryCreate(
            atelier ?? throw new ArgumentNullException(nameof(atelier)),
            shown,
            envoy ?? throw new ArgumentNullException(nameof(envoy)),
            BObserver.BObserverCreate<Action>(static run => run())))
    {
    }

    private BLibrary(
        BLocalizationCatalog catalog, CAtelier atelier, BBlueprintPart parley, Func<BSParley, Task?, int?> docket,
        Func<bool> shown, Action changed, Func<BCanvasWidth> measure, CLibrary library)
        : this(
            catalog, atelier, parley, docket, shown, changed, measure,
            CLibrary.CLibraryTitleKey, library.CLibrarySearchKey, library.CLibrarySeriesKey,
            library.CLibraryPanel,
            library.CLibraryEditor,
            CLibrary.CLibraryOrderRead(),
            library.CLibraryRowsRead,
            library.CLibraryRowsLoad,
            library.CLibraryPortraitExport,
            library.CLibraryImportExists ? library.CLibraryMarkupImport : null,
            library.CLibraryNewExists ? library.CLibraryPanel.CPanelEntryCreate : null)
    {
    }

    private BLibrary(
        BLocalizationCatalog catalog, CAtelier atelier, BBlueprintPart parley, Func<BSParley, Task?, int?> docket,
        Func<bool> shown, Action changed, Func<BCanvasWidth> measure,
        string title, string search, string series,
        CPanel panel,
        CEditor editor,
        IReadOnlyList<CCatalogOrder> orders,
        Func<IReadOnlyList<CVistaRow>> listed,
        Func<Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action>,
            Task<CEnsignSheet<IReadOnlyList<CVistaRow>>>> load,
        Func<Task> portrait,
        Func<Task>? markup,
        Action? fresh)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(parley);
        ArgumentNullException.ThrowIfNull(docket);
        ArgumentNullException.ThrowIfNull(shown);
        ArgumentNullException.ThrowIfNull(changed);
        ArgumentNullException.ThrowIfNull(measure);

        _bLibraryCatalog = catalog;
        _bLibraryParley = parley;
        _bLibraryDocket = docket;
        _bLibraryShown = shown;
        _bLibraryChanged = changed;
        _bLibraryMeasure = measure;
        _bLibraryPanel = panel;
        _bLibraryEditor = editor;
        _bLibraryOrders = orders;
        _bLibraryOrderKey = series;
        _bLibraryPortrait = portrait;
        _bLibraryMarkup = markup;
        CAperture aperture = panel.CPanelAperture;
        _bIndex = new BIndex<CVistaRow>(
            catalog,
            title,
            search,
            static row => (row.CVistaRowName, row.CVistaRowEpithet, row.CVistaRowLanguage, row.CVistaRowChosen),
            static row => row.CVistaRowId,
            aperture.CApertureQuerySet,
            BLibraryRowSelect);
        CDisplay display = editor.CEditorDisplay;
        _bLibraryLectern = new BLectern(
            catalog,
            display,
            atelier,
            changed,
            () => shown() && _bLibrarySide && panel.CPanelViewerChecked,
            docket,
            parley,
            measure);

        const BKeymapFocus window = BKeymapFocus.BKeymapFocusWindow;
        const BKeymapFocus lectern = BKeymapFocus.BKeymapFocusLectern;
        Func<BKeymapLive> live = () => shown() ? BKeymapLive.BKeymapLiveShown : BKeymapLive.BKeymapLiveHidden;
        Func<bool, Func<bool>, Func<BKeymapLive>> gated = (visible, enabled) => () =>
            !shown() || !visible ? BKeymapLive.BKeymapLiveHidden
            : enabled() ? BKeymapLive.BKeymapLiveShown
            : BKeymapLive.BKeymapLiveDim;
        Func<Action, Action<ConsoleKeyInfo>> pressed = action => _ =>
        {
            action();
            changed();
        };
        CDeskDraft draft = editor.CEditorDesk.CDeskDraft;
        Func<BKeymapLive> storable = gated(true, () => draft.CDeskDraftStorable);
        Func<BKeymapLive> scribe = () => gated(panel.CPanelViewerChecked, () => panel.CPanelModeEnabled)();
        Func<BKeymapLive> viewer = () => gated(panel.CPanelScribeChecked, static () => true)();
        Func<BKeymapLive> bin = gated(true, () => panel.CPanelBinEnabled);
        Func<BKeymapLive> favorite = gated(true, () => panel.CPanelViewerChecked && !display.CDisplayBlank);
        Func<BKeymapLive> reachable = () => gated(panel.CPanelViewerChecked, static () => true)();
        Func<BKeymapLive> reading = () =>
            gated(_bLibrarySide && panel.CPanelViewerChecked && !_bLibraryWhole, static () => true)();
        Func<BKeymapLive> whole = () => gated(BLibraryWhole, static () => true)();
        Action<ConsoleKeyInfo> widen = pressed(() => (_bLibraryWhole, _bLibrarySide) = (true, true));
        Action<ConsoleKeyInfo> narrow = pressed(() => _bLibraryWhole = false);
        List<BKeymapRow> rows =
        [
            .. orders.Take(4).Select((order, index) => new BKeymapRow(
                ConsoleKey.F3 + index,
                ConsoleModifiers.Control,
                window,
                $"{series}.{CCatalog.CCatalogOrderFormat(order)}",
                pressed(() => aperture.CApertureOrderSet(order)),
                live)),
            new(ConsoleKey.F7, ConsoleModifiers.Control, window, $"{series}.Title", pressed(BLibraryOrderShow), live),
            new(ConsoleKey.F8, ConsoleModifiers.Control, window, "Sieve.Title", pressed(BLibraryFilterShow), live),
            new(ConsoleKey.F2, 0, window, "Scribe.Save", pressed(BLibraryStoreObserve), storable),
            new(ConsoleKey.F4, 0, window, "Scribe.Edit", pressed(BLibraryScribeObserve), scribe),
            new(ConsoleKey.F4, 0, window, "Scribe.View", pressed(BLibraryViewerObserve), viewer),
            .. (BKeymapRow[])(fresh is Action create
                ? [new(ConsoleKey.F7, 0, window, "Scribe.New", pressed(create), live)]
                : []),
            new(ConsoleKey.F8, 0, window, "Scribe.Delete", pressed(BLibraryBinObserve), bin),
            new(ConsoleKey.F6, 0, window, "Favorite.Mark", BLibraryFavoriteToggle, favorite),
            new(ConsoleKey.Tab, 0, window, "Window.Help.Part", pressed(() => _bLibrarySide = true), reachable),
            new(ConsoleKey.Tab, 0, lectern, "Window.Help.Part", pressed(() => _bLibrarySide = false), reading),
            new(ConsoleKey.F3, 0, window, "Roof.View", widen, reachable),
            new(ConsoleKey.F3, 0, lectern, "Roof.View", widen, reading),
            .. _bLibraryLectern.BLecternKeymap.BKeymapRead(lectern),
            new(ConsoleKey.F3, 0, lectern, "Display.Retreat", narrow, whole),
            new(ConsoleKey.Escape, 0, lectern, "Display.Retreat", narrow, whole),
        ];
        BLibraryKeymap = new BKeymap(rows);

        aperture.CApertureRowsChanged += () => BLibraryRowsRefine(listed());
        panel.CPanelChanged += changed;
        editor.CEditorDesk.CDeskStateChanged += changed;
        atelier.CAtelierWorkspace.CWorkspaceOpened +=
            async () => BLibraryVistaRefine(await load(static (_, _) => static () => { }));
    }

    internal BLibrary(
        BLocalizationCatalog catalog, CAtelier atelier, CEnvoy envoy, BBlueprintPart parley,
        Func<BSParley, Task?, int?> docket, Func<bool> shown, Action changed, Func<BCanvasWidth> measure,
        Func<CAtelier, Func<bool>, CEnvoy, Action<Action>, CFavorite> favorite)
        : this(catalog, atelier, parley, docket, shown, changed, measure, (favorite
            ?? throw new ArgumentNullException(nameof(favorite)))(
            atelier ?? throw new ArgumentNullException(nameof(atelier)),
            shown,
            envoy ?? throw new ArgumentNullException(nameof(envoy)),
            BObserver.BObserverCreate<Action>(static run => run())))
    {
        atelier.CAtelierWorkspace.CWorkspaceOpened += _bIndex.BIndexLitClear;
    }

    private BLibrary(
        BLocalizationCatalog catalog, CAtelier atelier, BBlueprintPart parley, Func<BSParley, Task?, int?> docket,
        Func<bool> shown, Action changed, Func<BCanvasWidth> measure, CFavorite favorite)
        : this(
            catalog, atelier, parley, docket, shown, changed, measure,
            CFavorite.CFavoriteTitleKey, favorite.CFavoriteSearchKey, favorite.CFavoriteSeriesKey,
            favorite.CFavoritePanel,
            favorite.CFavoriteEditor,
            CFavorite.CFavoriteOrderRead(),
            favorite.CFavoriteRowsRead,
            favorite.CFavoriteRowsLoad,
            favorite.CFavoritePortraitExport,
            null,
            favorite.CFavoriteNewExists ? favorite.CFavoritePanel.CPanelEntryCreate : null)
    {
    }

    internal BKeymap BLibraryKeymap { get; }

    internal bool BLibraryWhole => _bLibraryShown() && _bLibraryWhole && _bLibraryPanel.CPanelViewerChecked;

    internal BKeymapFocus BLibraryKeymapFocus =>
        _bLibraryShown() && _bLibrarySide && _bLibraryPanel.CPanelViewerChecked
            ? BKeymapFocus.BKeymapFocusLectern
            : BKeymapFocus.BKeymapFocusWindow;

    internal void BLibraryObserve(ConsoleKeyInfo key)
    {
        _ = _bIndex.BIndexObserve(key);
        _bLibraryChanged();
    }

    internal IEnumerable<BCanvasText> BLibraryDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BBlueprintPart window,
        bool focused,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(measure);

        if (!_bLibraryShown())
        {
            yield break;
        }

        BBlueprintPart index = BContract.BContractFind(window, "AIndex");
        foreach (BCanvasText text in BLibraryWhole ? Enumerable.Empty<BCanvasText>() : _bIndex.BIndexDraw(
            glyphs,
            index.BBlueprintPartRow,
            index.BBlueprintPartColumn,
            index.BBlueprintPartWidth,
            index.BBlueprintPartHeight,
            focused && !(_bLibrarySide && _bLibraryPanel.CPanelViewerChecked),
            measure))
        {
            yield return text;
        }

        BBlueprintPart display = BContract.BContractFind(window, "ADisplay");
        if (!_bLibraryPanel.CPanelViewerChecked)
        {
            yield break;
        }

        foreach (BCanvasText text in _bLibraryLectern.BLecternDraw(
            glyphs,
            display.BBlueprintPartRow,
            display.BBlueprintPartColumn,
            display.BBlueprintPartWidth,
            display.BBlueprintPartHeight,
            focused && _bLibrarySide,
            measure))
        {
            yield return text;
        }
    }

    internal IReadOnlyList<BRoofMenu> BLibraryMenuBuild()
    {
        if (!_bLibraryShown())
        {
            return [];
        }

        BLocalizationCatalog catalog = _bLibraryCatalog;
        CPanel panel = _bLibraryPanel;
        Func<string, ConsoleKey?> letter = key => catalog.BLocalizationTextRead(key) is [char single]
            && char.IsAsciiLetter(single)
                ? ConsoleKey.A + (char.ToUpperInvariant(single) - 'A')
                : null;
        Func<string, Action, Func<bool>, BRoofItem> item = (key, action, live) =>
            new(catalog.BLocalizationTextRead(key), null, letter($"{key}.Hotkey"), action, live);
        return
        [
            new BRoofMenu(
                catalog.BLocalizationTextRead("Roof.Entry"),
                letter("Roof.Entry.Hotkey"),
                [
                    .. (BRoofItem[])(_bLibraryMarkup is Func<Task> markup
                        ? [item("Roof.Entry.Import", async () => await markup(), static () => true)]
                        : []),
                    item("Roof.Entry.Export", BLibraryPortraitObserve, () => panel.CPanelPressAllowed),
                ]),
            new BRoofMenu(
                catalog.BLocalizationTextRead("Roof.View"),
                letter("Roof.View.Hotkey"),
                [
                    item("Roof.View.Order", BLibraryOrderShow, static () => true),
                    item("Roof.View.Sieve", BLibraryFilterShow, static () => true),
                ]),
        ];
    }

    private void BLibraryRowsRefine(IReadOnlyList<CVistaRow> rows)
    {
        CAperture aperture = _bLibraryPanel.CPanelAperture;
        _bIndex.BIndexRefine(
            rows,
            $"{_bLibraryOrderKey}.{aperture.CApertureOrderWord}",
            aperture.CApertureFiltered,
            aperture.CApertureEmpty ? aperture.CApertureKey : string.Empty);
        _bLibraryChanged();
    }

    private void BLibraryVistaRefine(CEnsignSheet<IReadOnlyList<CVistaRow>> sheet)
    {
        _bLibraryLanguages = sheet.CEnsignSheetLanguages;
        BLibraryRowsRefine(sheet.CEnsignSheetRows);
    }

    private void BLibraryRowSelect(CVistaRow row)
    {
        CPanelOutcome outcome = _bLibraryPanel.CPanelRowSelect(row.CVistaRowId);
        if (outcome == CPanelOutcome.CPanelOutcomeStayed)
        {
            _bIndex.BIndexLitClear();
        }

        if (outcome == CPanelOutcome.CPanelOutcomeOpened)
        {
            _bLibraryLectern.BLecternReset();
            _bLibrarySide = true;
        }

        _bLibraryChanged();
    }

    private void BLibraryScribeObserve() => _bLibraryPanel.CPanelScribeToggle(true);

    private void BLibraryViewerObserve() => _bLibraryPanel.CPanelScribeToggle(false);

    private void BLibraryStoreObserve() => _bLibraryEditor.CEditorEntrySave();

    private void BLibraryBinObserve() => _bLibraryPanel.CPanelBin.CPanelBinDelete();

    private void BLibraryFavoriteToggle(ConsoleKeyInfo key)
    {
        _bIndex.BIndexLitSettle(() => _bLibraryLectern.BLecternFavoriteToggle(key));
    }

    private async void BLibraryPortraitObserve()
    {
        await _bLibraryPortrait();
    }

    private void BLibraryOrderShow()
    {
        CAperture aperture = _bLibraryPanel.CPanelAperture;
        TaskCompletionSource chosen = new();
        BSChoiceOrder body = new(
            _bLibraryCatalog,
            _bLibraryOrderKey,
            _bLibraryOrders,
            () => aperture.CApertureOrderWord,
            order =>
            {
                aperture.CApertureOrderSet(order);
                chosen.TrySetResult();
            },
            _bLibraryMeasure());
        BSParley parley = new(
            _bLibraryParley, _bLibraryCatalog.BLocalizationTextRead($"{_bLibraryOrderKey}.Title"), "dialog", [],
            [new BSParleyButton(_bLibraryCatalog.BLocalizationTextRead("Order.Close"))], 0, body);
        _ = _bLibraryDocket(parley, chosen.Task);
    }

    private void BLibraryFilterShow()
    {
        CAperture aperture = _bLibraryPanel.CPanelAperture;
        BSChoiceFilter body = new(
            _bLibraryLanguages,
            aperture.CApertureFilter,
            hidden => aperture.CApertureFilterSet(new CCatalogFilter(hidden)),
            _bLibraryMeasure());
        BSParley parley = new(
            _bLibraryParley, _bLibraryCatalog.BLocalizationTextRead("Sieve.Title"), "dialog", [],
            [new BSParleyButton(_bLibraryCatalog.BLocalizationTextRead("Sieve.Close"))], 0, body);
        _ = _bLibraryDocket(parley, null);
    }
}
