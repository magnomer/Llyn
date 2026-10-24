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
    private readonly CLibrary _cLibrary;
    private readonly BIndex<CVistaRow> _bIndex;
    private IReadOnlyList<string> _bLibraryLanguages = [];

    internal BLibrary(
        BLocalizationCatalog catalog,
        CAtelier atelier,
        CEnvoy envoy,
        BBlueprintPart parley,
        Func<BSParley, Task?, int?> docket,
        Func<bool> shown,
        Action changed,
        Func<BCanvasWidth> measure)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);
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
        _cLibrary = CLibrary.CLibraryCreate(
            atelier, shown, envoy, BObserver.BObserverCreate<Action>(static run => run()));
        CPanel panel = _cLibrary.CLibraryPanel;
        CAperture aperture = panel.CPanelAperture;
        _bIndex = new BIndex<CVistaRow>(
            catalog,
            "Navigation.Library",
            "List.Search",
            static row => (row.CVistaRowName, row.CVistaRowEpithet, row.CVistaRowLanguage, row.CVistaRowChosen),
            static row => row.CVistaRowId,
            aperture.CApertureQuerySet,
            BLibraryRowSelect);

        const BKeymapFocus window = BKeymapFocus.BKeymapFocusWindow;
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
        CDeskDraft draft = _cLibrary.CLibraryEditor.CEditorDesk.CDeskDraft;
        Func<BKeymapLive> storable = gated(true, () => draft.CDeskDraftStorable);
        Func<BKeymapLive> scribe = () => gated(panel.CPanelViewerChecked, () => panel.CPanelModeEnabled)();
        Func<BKeymapLive> viewer = () => gated(panel.CPanelScribeChecked, static () => true)();
        Func<BKeymapLive> bin = gated(true, () => panel.CPanelBinEnabled);
        List<BKeymapRow> rows =
        [
            .. CLibrary.CLibraryOrderRead().Select((order, index) => new BKeymapRow(
                ConsoleKey.F3 + index,
                ConsoleModifiers.Control,
                window,
                "Order." + CCatalog.CCatalogOrderFormat(order),
                pressed(() => aperture.CApertureOrderSet(order)),
                live)),
            new(ConsoleKey.F7, ConsoleModifiers.Control, window, "Order.Title", pressed(BLibraryOrderShow), live),
            new(ConsoleKey.F8, ConsoleModifiers.Control, window, "Sieve.Title", pressed(BLibraryFilterShow), live),
            new(ConsoleKey.F2, 0, window, "Scribe.Save", pressed(BLibraryStoreObserve), storable),
            new(ConsoleKey.F4, 0, window, "Scribe.Edit", pressed(BLibraryScribeObserve), scribe),
            new(ConsoleKey.F4, 0, window, "Scribe.View", pressed(BLibraryViewerObserve), viewer),
            new(ConsoleKey.F7, 0, window, "Scribe.New", pressed(BLibraryFreshObserve), live),
            new(ConsoleKey.F8, 0, window, "Scribe.Delete", pressed(BLibraryBinObserve), bin),
        ];
        BLibraryKeymap = new BKeymap(rows);

        aperture.CApertureRowsChanged += BLibraryRowsRefine;
        panel.CPanelChanged += changed;
        _cLibrary.CLibraryEditor.CEditorDesk.CDeskStateChanged += changed;
        atelier.CAtelierWorkspace.CWorkspaceOpened += BLibraryVistaRefine;
    }

    internal BKeymap BLibraryKeymap { get; }

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
        foreach (BCanvasText text in _bIndex.BIndexDraw(
            glyphs,
            index.BBlueprintPartRow,
            index.BBlueprintPartColumn,
            index.BBlueprintPartWidth,
            index.BBlueprintPartHeight,
            focused,
            measure))
        {
            yield return text;
        }

        BBlueprintPart display = BContract.BContractFind(window, "ADisplay");
        CPanel panel = _cLibrary.CLibraryPanel;
        if (!panel.CPanelViewerChecked || !_cLibrary.CLibraryEditor.CEditorDisplay.CDisplayBlank
            || display.BBlueprintPartWidth <= 0)
        {
            yield break;
        }

        string prompt = measure.BCanvasWidthClamp(
            _bLibraryCatalog.BLocalizationTextRead("Display.Unselected"), display.BBlueprintPartWidth);
        int cells = measure.BCanvasWidthResolve(prompt);
        yield return new BCanvasText(
            display.BBlueprintPartRow + (display.BBlueprintPartHeight / 2),
            display.BBlueprintPartColumn + ((display.BBlueprintPartWidth - cells) / 2),
            cells,
            prompt,
            "muted");
    }

    internal IReadOnlyList<BRoofMenu> BLibraryMenuBuild()
    {
        if (!_bLibraryShown())
        {
            return [];
        }

        BLocalizationCatalog catalog = _bLibraryCatalog;
        CPanel panel = _cLibrary.CLibraryPanel;
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
                    item("Roof.Entry.Import", BLibraryMarkupObserve, static () => true),
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

    private void BLibraryRowsRefine()
    {
        BLibraryRowsRefine(_cLibrary.CLibraryRowsRead());
    }

    private void BLibraryRowsRefine(IReadOnlyList<CVistaRow> rows)
    {
        CAperture aperture = _cLibrary.CLibraryPanel.CPanelAperture;
        _bIndex.BIndexRefine(
            rows,
            aperture.CApertureOrderWord,
            aperture.CApertureFiltered,
            aperture.CApertureEmpty ? aperture.CApertureKey : string.Empty);
        _bLibraryChanged();
    }

    private async void BLibraryVistaRefine()
    {
        CEnsignSheet<IReadOnlyList<CVistaRow>> sheet =
            await _cLibrary.CLibraryRowsLoad(static (_, _) => static () => { });
        _bLibraryLanguages = sheet.CEnsignSheetLanguages;
        BLibraryRowsRefine(sheet.CEnsignSheetRows);
    }

    private void BLibraryRowSelect(CVistaRow row)
    {
        if (_cLibrary.CLibraryPanel.CPanelRowSelect(row.CVistaRowId) == CPanelOutcome.CPanelOutcomeStayed)
        {
            _bIndex.BIndexLitClear();
        }

        _bLibraryChanged();
    }

    private void BLibraryFreshObserve()
    {
        _cLibrary.CLibraryPanel.CPanelEntryCreate();
    }

    private void BLibraryScribeObserve()
    {
        _cLibrary.CLibraryPanel.CPanelScribeToggle(true);
    }

    private void BLibraryViewerObserve()
    {
        _cLibrary.CLibraryPanel.CPanelScribeToggle(false);
    }

    private void BLibraryStoreObserve()
    {
        _cLibrary.CLibraryEditor.CEditorEntrySave();
    }

    private void BLibraryBinObserve()
    {
        _cLibrary.CLibraryPanel.CPanelBin.CPanelBinDelete();
    }

    private async void BLibraryMarkupObserve()
    {
        await _cLibrary.CLibraryMarkupImport();
    }

    private async void BLibraryPortraitObserve()
    {
        await _cLibrary.CLibraryPortraitExport();
    }

    private void BLibraryOrderShow()
    {
        CAperture aperture = _cLibrary.CLibraryPanel.CPanelAperture;
        TaskCompletionSource chosen = new();
        BSChoiceOrder body = new(
            _bLibraryCatalog,
            "Order",
            CLibrary.CLibraryOrderRead(),
            () => aperture.CApertureOrderWord,
            order =>
            {
                aperture.CApertureOrderSet(order);
                chosen.TrySetResult();
            },
            _bLibraryMeasure());
        BSParley parley = new(
            _bLibraryParley, _bLibraryCatalog.BLocalizationTextRead("Order.Title"), "dialog", [],
            [new BSParleyButton(_bLibraryCatalog.BLocalizationTextRead("Order.Close"))], 0, body);
        _ = _bLibraryDocket(parley, chosen.Task);
    }

    private void BLibraryFilterShow()
    {
        CAperture aperture = _cLibrary.CLibraryPanel.CPanelAperture;
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
