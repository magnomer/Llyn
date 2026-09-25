using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PCorpus : UserControl, PChronicleHost
{
    private PWindow _pCorpusHost = null!;

    private LCorpus _lCorpus = null!;

    public PCorpus()
    {
        InitializeComponent();
        Resources.MergedDictionaries.Add(new PCorpusTranscript(this));
    }

    internal void PCorpusAttach(PWindow host)
    {
        _pCorpusHost = host;
        LEditor editor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        _lCorpus = host.PWindowDeportment.LWindowCorpusCreate(
            editor,
            PCorpusShownCheck,
            PCorpusDiscardConfirm,
            PCorpusRemovalConfirm,
            host.PWindowUnreadableConfirm);
        PTranscriptDeskAttach();

        PAnthology.ItemsSource = _pAnthologyList;
        PQuotation.ItemsSource = _pQuotationList;
        PCitationList.ItemsSource = _pCitationItem;
        PCitationDrawer.CustomPopupPlacementCallback = PCitationPlace;
        PLanguageList.ItemsSource = _pLanguageItem;
        PTranscriptMentionLine.ItemsSource = _pTranscriptChip.PMentionLineChip;
        PTranscriptGlossLine.ItemsSource = _pTranscriptGloss;
        PExcerptGloss.ItemsSource = _pExcerptGloss;

        PDisplay.PDisplayAttach(host, editor.LEditorLectern);
        PEditor.PEditorAttach(host, editor);

        LPanel anthology = _lCorpus.LCorpusAnthology.LAnthologyPanel;
        LPanel quotation = _lCorpus.LCorpusQuotation.LQuotationPanel;
        _lCorpus.LCorpusChanged += PCorpusModeUpdate;
        _lCorpus.LCorpusTranscriptChanged += PTranscriptApply;
        _lCorpus.LCorpusExampleChanged += PExcerptShow;
        _lCorpus.LCorpusFailed += host.PWindowFailureShow;
        _lCorpus.LCorpusQueryCleared += PQueryClear;
        anthology.LPanelChanged += PCorpusModeUpdate;
        anthology.LPanelRowsChanged += PAnthologyFind;
        anthology.LPanelFailed += host.PWindowFailureShow;
        quotation.LPanelChanged += PCorpusModeUpdate;
        quotation.LPanelRowsChanged += PQuotationFind;
        quotation.LPanelFailed += host.PWindowFailureShow;

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PCorpusPressHandle, PCorpusPressCheck));
        CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, PCorpusPortraitHandle, PCorpusPortraitCheck));
    }

    private bool PCorpusShownCheck()
    {
        return IsVisible;
    }

    private bool PCorpusDiscardConfirm(Func<bool, bool> finish)
    {
        return _pCorpusHost.PWindowDiscardConfirm(true, finish);
    }

    private bool PCorpusRemovalConfirm(int usage)
    {
        return _pCorpusHost.PWindowRemovalConfirm(usage, "Example");
    }

    private void PCorpusModeUpdate()
    {
        PTranscript.Visibility = PLook.PLookVisibleRead(_lCorpus.LCorpusTranscriptShown);
        PExcerpt.Visibility = PLook.PLookVisibleRead(_lCorpus.LCorpusExcerptShown);
        PDisplay.Visibility = PLook.PLookVisibleRead(_lCorpus.LCorpusDisplayShown);
        PEditor.Visibility = PLook.PLookVisibleRead(_lCorpus.LCorpusEditorShown);
        PExcerptBody.Visibility = PLook.PLookVisibleRead(_lCorpus.LCorpusExcerptHeld);
        PExcerptUnselected.Visibility = PLook.PLookVisibleRead(_lCorpus.LCorpusExcerptBlank);
        PCorpusViewer.IsChecked = PLook.PLookCheckedRead(_lCorpus.LCorpusViewerChecked);
        PCorpusScribe.IsChecked = PLook.PLookCheckedRead(_lCorpus.LCorpusScribeChecked);
        PCorpusMode.IsEnabled = _lCorpus.LCorpusModeEnabled;
        PCorpusBin.IsEnabled = _lCorpus.LCorpusBinEnabled;
        PCorpusStore.IsEnabled = _lCorpus.LCorpusStoreEnabled;
        PTranscript.IsEnabled = _lCorpus.LCorpusDesk.LDeskRunning;
        PChronicleUpdate();
    }

    internal void PCorpusReset()
    {
        _lCorpus.LCorpusClear();
    }

    internal bool PCorpusChangeCheck()
    {
        return _lCorpus.LCorpusChangeCheck();
    }

    internal bool PCorpusDraftFinish(bool store)
    {
        return _lCorpus.LCorpusDraftFinish(store);
    }

    internal void PCorpusClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
        PCitationDrawer.IsOpen = false;
        PLanguage.IsOpen = false;
        PRankDropdown.IsOpen = false;
        PGauzeDropdown.IsOpen = false;
    }

    private void PCorpusPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lCorpus.LCorpusPressAllowed;
    }

    private async void PCorpusPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pCorpusHost.PWindowPressRun(
            (label, ticket) => _lCorpus.LCorpusPortraitPrint(
                label, _pCorpusHost.PWindowLegendRead("Example"), ticket));
    }

    private void PCorpusPortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lCorpus.LCorpusPortraitAllowed;
    }

    private async void PCorpusPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pCorpusHost.PWindowPortraitExport(
            _lCorpus.LCorpusQuotation.LQuotationFileRead(), _lCorpus.LCorpusPortraitExport);
    }
}
