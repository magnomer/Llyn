using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PRepertoire : UserControl, PImageHost, PVideoHost, PChronicleHost
{
    private PWindow _pRepertoireHost = null!;

    private LRepertoire _lRepertoire = null!;

    public PRepertoire()
    {
        InitializeComponent();

        Resources.MergedDictionaries.Add(new PImageTemplate(this));
        Resources.MergedDictionaries.Add(new PVideoTemplate(this));
    }

    internal void PRepertoireAttach(PWindow host)
    {
        _pRepertoireHost = host;
        LEditor editor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        _lRepertoire = host.PWindowDeportment.LWindowRepertoireCreate(
            editor,
            PRepertoireShownCheck,
            PRepertoireDiscardConfirm,
            PRepertoireRemovalConfirm,
            host.PWindowUnreadableConfirm);
        PScenarioDeskAttach();

        PAtlas.ItemsSource = _pAtlasList;
        POccurrence.ItemsSource = _pOccurrenceList;
        PScenarioImage.ItemsSource = _pScenarioImage;
        PScenarioVideo.ItemsSource = _pScenarioVideo;

        PMedia.PMediaAttach(this, host.PWindowDeportment);
        PDisplay.PDisplayAttach(host, editor.LEditorDisplay);
        PEditor.PEditorAttach(host, editor);

        LPanel atlas = _lRepertoire.LRepertoireAtlas.LAtlasPanel;
        LPanel occurrence = _lRepertoire.LRepertoireOccurrence.LOccurrencePanel;
        _lRepertoire.LRepertoireChanged += PRepertoireModeUpdate;
        _lRepertoire.LRepertoireScenarioChanged += PScenarioApply;
        _lRepertoire.LRepertoireSituationChanged += PVignetteShow;
        _lRepertoire.LRepertoireFailed += host.PWindowFailureShow;
        _lRepertoire.LRepertoireInquestCleared += PInquestClear;
        atlas.LPanelChanged += PRepertoireModeUpdate;
        atlas.LPanelRowsChanged += PAtlasFind;
        atlas.LPanelCleared += PVignetteClear;
        atlas.LPanelFailed += host.PWindowFailureShow;
        occurrence.LPanelChanged += PRepertoireModeUpdate;
        occurrence.LPanelRowsChanged += POccurrenceFind;
        occurrence.LPanelCleared += PDisplay.PDisplayClear;
        occurrence.LPanelDraftChanged += POccurrenceDraftShow;
        occurrence.LPanelFailed += host.PWindowFailureShow;

        CommandBindings.Add(new CommandBinding(
            ApplicationCommands.Print, PRepertoirePressHandle, PRepertoirePressCheck));
        CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, PRepertoirePortraitHandle, PRepertoirePortraitCheck));
    }

    private bool PRepertoireShownCheck()
    {
        return IsVisible;
    }

    private bool PRepertoireDiscardConfirm(Func<bool, bool> finish)
    {
        return _pRepertoireHost.PWindowDiscardConfirm(true, finish);
    }

    private bool PRepertoireRemovalConfirm(int usage)
    {
        return _pRepertoireHost.PWindowRemovalConfirm(usage);
    }

    private void POccurrenceDraftShow(LDraft draft)
    {
        PDisplay.PDisplayShow(draft.LDraftContent);
    }

    private void PRepertoireModeUpdate()
    {
        PScenario.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireScenarioShown);
        PVignette.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireVignetteShown);
        PDisplay.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireDisplayShown);
        PEditor.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireEditorShown);
        PVignetteBody.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireVignetteHeld);
        PVignetteUnselected.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireVignetteBlank);
        PRepertoireViewer.IsChecked = PLook.PLookCheckedRead(_lRepertoire.LRepertoireViewerChecked);
        PRepertoireScribe.IsChecked = PLook.PLookCheckedRead(_lRepertoire.LRepertoireScribeChecked);
        PRepertoireMode.IsEnabled = _lRepertoire.LRepertoireModeEnabled;
        PRepertoireBin.IsEnabled = _lRepertoire.LRepertoireBinEnabled;
        PRepertoireStore.IsEnabled = _lRepertoire.LRepertoireStoreEnabled;
        PScenario.IsEnabled = _lRepertoire.LRepertoireDesk.LDeskRunning;
        PChronicleUpdate();
    }

    internal void PRepertoireReset()
    {
        _lRepertoire.LRepertoireClear();
    }

    internal bool PRepertoireChangeCheck()
    {
        return _lRepertoire.LRepertoireChangeCheck();
    }

    internal bool PRepertoireDraftFinish(bool store)
    {
        return _lRepertoire.LRepertoireDraftFinish(store);
    }

    internal void PRepertoireClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
        PTierDropdown.IsOpen = false;
        PMeshDropdown.IsOpen = false;
    }

    private void PRepertoirePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lRepertoire.LRepertoirePressAllowed;
    }

    private async void PRepertoirePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pRepertoireHost.PWindowPressRun(
            (label, ticket) => _lRepertoire.LRepertoirePortraitPrint(
                label, _pRepertoireHost.PWindowLegendRead("Situation"), ticket));
    }

    private void PRepertoirePortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lRepertoire.LRepertoirePortraitAllowed;
    }

    private async void PRepertoirePortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pRepertoireHost.PWindowPortraitExport(
            _lRepertoire.LRepertoireOccurrence.LOccurrenceFileRead(), _lRepertoire.LRepertoirePortraitExport);
    }
}
