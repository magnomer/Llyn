using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PCorpus : UserControl, PChronicleHost
{
    private readonly PCorpusTranscript _pCorpusTranscript;

    private PWindow _pCorpusHost = null!;

    private LCorpus _lCorpus = null!;

    public PCorpus()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Corpus/PCorpus.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));
        _pCorpusTranscript = new PCorpusTranscript(this);
        Resources.MergedDictionaries.Add(_pCorpusTranscript);
        PLook.PLookStyleAttach(surface.Resources);
        PLook.PLookStyleAttach(Resources);

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PCorpusPressHandle, PCorpusPressCheck));
        CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, PCorpusPortraitHandle, PCorpusPortraitCheck));
        PCorpusPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        PCorpusPress.Command = ApplicationCommands.Print;

        PTranscript.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandLink, PTranscriptLinkHandle, PTranscriptLinkCheck));
        PTranscript.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandChoose, PTranscriptSenseHandle, PTranscriptSenseCheck));
        PTranscript.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandSilence, PTranscriptSilenceHandle, PTranscriptLinkCheck));
        PTranscript.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandUnlink, PTranscriptUnlinkHandle, PTranscriptUnlinkCheck));

        PChoice.PChoiceDropperAttach(PRankDropper, PRankDropdown, PRank);
        PChoice.PChoiceDropperAttach(PGauzeDropper, PGauzeDropdown, PGauzeDropper);
        PChoice.PChoiceDropperAttach(PSpeaker, PLanguage, PSpeaker);

        PRankIcon.PIconSource = PIcon.PIconResolve("sort", 24);
        PGauzeIcon.PIconSource = PIcon.PIconResolve("filter", 24);
        PSpeakerIcon.PIconSource = PIcon.PIconResolve("expand", 12);
        PCorpusBinIcon.PIconSource = PIcon.PIconResolve("delete", 24);
        PCorpusFresh.Tag = PIcon.PIconResolve("new", 24);
        PCorpusStore.Tag = PIcon.PIconResolve("save", 24);
        PCorpusEarlier.Tag = PIcon.PIconResolve("retreat", 24);
        PCorpusLater.Tag = PIcon.PIconResolve("advance", 24);
        PCorpusBackward.Tag = PIcon.PIconResolve("undo", 24);
        PCorpusForward.Tag = PIcon.PIconResolve("redo", 24);
        PCorpusPortrait.Tag = PIcon.PIconResolve("export", 24);
        PCorpusPress.Tag = PIcon.PIconResolve("print", 24);
        PCorpusViewer.Tag = PIcon.PIconResolve("view", 24);
        PCorpusScribe.Tag = PIcon.PIconResolve("edit", 24);
        if (PTranscriptAddition.Content is PIconImage addition)
        {
            addition.PIconSource = PIcon.PIconResolve("add", 12);
        }

        PQuery.TextChanged += PQueryHandle;
        PDredge.TextChanged += PDredgeHandle;
        PTranscriptText.TextChanged += PTranscriptTextHandle;
        PCitationField.TextChanged += PCitationTextHandle;
        PCitationField.PreviewKeyDown += PCitationKeyHandle;
        PCitationField.LostKeyboardFocus += PCitationLeaveHandle;
        PTranscriptSeedField.GotKeyboardFocus += PTranscriptSeedHandle;
        PExcerptText.PMentionClick += PExcerptMentionHandle;
        PCorpusFresh.Click += PCorpusFreshHandle;
        PCorpusStore.Click += PCorpusStoreHandle;
        PCorpusEarlier.Click += PCorpusRetreatHandle;
        PCorpusLater.Click += PCorpusAdvanceHandle;
        PCorpusBackward.Click += PCorpusUndoHandle;
        PCorpusForward.Click += PCorpusRedoHandle;
        PCorpusViewer.Click += PCorpusScribeHandle;
        PCorpusScribe.Click += PCorpusScribeHandle;
        PCorpusBin.Click += PCorpusBinHandle;
        PTranscriptAddition.Click += PGlossAddHandle;

        PLookItem.PLookItemAttach(PAnthology, PAnthologyApply);
        PLookItem.PLookItemAttach(PQuotation, PQuotationApply);
        PLookItem.PLookItemAttach(PCitationList, PCitationApply);
        PLookItem.PLookItemAttach(PLanguageList, PSpeakerApply);
    }

    private Border PRank => (Border)FindName(nameof(PRank));

    private ToggleButton PRankDropper => (ToggleButton)FindName(nameof(PRankDropper));

    private PIconImage PRankIcon => (PIconImage)FindName(nameof(PRankIcon));

    private TextBox PQuery => (TextBox)FindName(nameof(PQuery));

    private Popup PRankDropdown => (Popup)FindName(nameof(PRankDropdown));

    private StackPanel PRankList => (StackPanel)FindName(nameof(PRankList));

    private TextBox PDredge => (TextBox)FindName(nameof(PDredge));

    private ToggleButton PGauzeDropper => (ToggleButton)FindName(nameof(PGauzeDropper));

    private PIconImage PGauzeIcon => (PIconImage)FindName(nameof(PGauzeIcon));

    private FrameworkElement PGauzeMark => (FrameworkElement)FindName(nameof(PGauzeMark));

    private Popup PGauzeDropdown => (Popup)FindName(nameof(PGauzeDropdown));

    private StackPanel PGauzeList => (StackPanel)FindName(nameof(PGauzeList));

    private Button PCorpusFresh => (Button)FindName(nameof(PCorpusFresh));

    private Button PCorpusStore => (Button)FindName(nameof(PCorpusStore));

    private StackPanel PCorpusVoyage => (StackPanel)FindName(nameof(PCorpusVoyage));

    private Button PCorpusEarlier => (Button)FindName(nameof(PCorpusEarlier));

    private Button PCorpusLater => (Button)FindName(nameof(PCorpusLater));

    private StackPanel PCorpusChronicle => (StackPanel)FindName(nameof(PCorpusChronicle));

    private Button PCorpusBackward => (Button)FindName(nameof(PCorpusBackward));

    private Button PCorpusForward => (Button)FindName(nameof(PCorpusForward));

    private Button PCorpusPortrait => (Button)FindName(nameof(PCorpusPortrait));

    private Button PCorpusPress => (Button)FindName(nameof(PCorpusPress));

    private Border PCorpusMode => (Border)FindName(nameof(PCorpusMode));

    private RadioButton PCorpusViewer => (RadioButton)FindName(nameof(PCorpusViewer));

    private RadioButton PCorpusScribe => (RadioButton)FindName(nameof(PCorpusScribe));

    private ItemsControl PAnthology => (ItemsControl)FindName(nameof(PAnthology));

    private TextBlock PAnthologyEmpty => (TextBlock)FindName(nameof(PAnthologyEmpty));

    private ItemsControl PQuotation => (ItemsControl)FindName(nameof(PQuotation));

    private TextBlock PQuotationEmpty => (TextBlock)FindName(nameof(PQuotationEmpty));

    private PDisplay PDisplay => (PDisplay)FindName(nameof(PDisplay));

    private PEditor PEditor => (PEditor)FindName(nameof(PEditor));

    private Grid PExcerpt => (Grid)FindName(nameof(PExcerpt));

    private StackPanel PExcerptBody => (StackPanel)FindName(nameof(PExcerptBody));

    private PMention PExcerptText => (PMention)FindName(nameof(PExcerptText));

    private Image PExcerptFlag => (Image)FindName(nameof(PExcerptFlag));

    private TextBlock PExcerptLanguage => (TextBlock)FindName(nameof(PExcerptLanguage));

    private TextBlock PExcerptTally => (TextBlock)FindName(nameof(PExcerptTally));

    private StackPanel PExcerptGlossSection => (StackPanel)FindName(nameof(PExcerptGlossSection));

    private ItemsControl PExcerptGloss => (ItemsControl)FindName(nameof(PExcerptGloss));

    private StackPanel PExcerptCitationSection => (StackPanel)FindName(nameof(PExcerptCitationSection));

    private TextBlock PExcerptCitation => (TextBlock)FindName(nameof(PExcerptCitation));

    private TextBlock PExcerptUnselected => (TextBlock)FindName(nameof(PExcerptUnselected));

    private Grid PTranscript => (Grid)FindName(nameof(PTranscript));

    private TextBox PTranscriptText => (TextBox)FindName(nameof(PTranscriptText));

    private ItemsControl PTranscriptMentionLine => (ItemsControl)FindName(nameof(PTranscriptMentionLine));

    private ToggleButton PSpeaker => (ToggleButton)FindName(nameof(PSpeaker));

    private Image PSpeakerFlag => (Image)FindName(nameof(PSpeakerFlag));

    private TextBlock PSpeakerName => (TextBlock)FindName(nameof(PSpeakerName));

    private PIconImage PSpeakerIcon => (PIconImage)FindName(nameof(PSpeakerIcon));

    private Popup PLanguage => (Popup)FindName(nameof(PLanguage));

    private ItemsControl PLanguageList => (ItemsControl)FindName(nameof(PLanguageList));

    private TextBlock PTranscriptTally => (TextBlock)FindName(nameof(PTranscriptTally));

    private ItemsControl PTranscriptGlossLine => (ItemsControl)FindName(nameof(PTranscriptGlossLine));

    private Border PTranscriptSeed => (Border)FindName(nameof(PTranscriptSeed));

    private TextBox PTranscriptSeedField => (TextBox)FindName(nameof(PTranscriptSeedField));

    private Button PTranscriptAddition => (Button)FindName(nameof(PTranscriptAddition));

    private TextBox PCitationField => (TextBox)FindName(nameof(PCitationField));

    private Popup PCitationDrawer => (Popup)FindName(nameof(PCitationDrawer));

    private Border PCitationSheet => (Border)FindName(nameof(PCitationSheet));

    private ListBox PCitationList => (ListBox)FindName(nameof(PCitationList));

    private Button PCorpusBin => (Button)FindName(nameof(PCorpusBin));

    private PIconImage PCorpusBinIcon => (PIconImage)FindName(nameof(PCorpusBinIcon));

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
        PLookItem.PLookItemAttach(PTranscriptMentionLine, PMentionChip.PMentionChipApply);
        PLookItem.PLookItemAttach(PTranscriptGlossLine, PTranscriptGlossApply);
        PLookItem.PLookItemAttach(PExcerptGloss, PGloss.PGlossRowApply);

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
        PCorpusVoyage.Visibility = PLook.PLookVisibleRead(_lCorpus.LCorpusViewerChecked);
        PCorpusChronicle.Visibility = PLook.PLookVisibleRead(_lCorpus.LCorpusScribeChecked);
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
        e.CanExecute = _lCorpus?.LCorpusPressAllowed ?? false;
    }

    private async void PCorpusPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pCorpusHost.PWindowPressRun(
            (label, ticket) => _lCorpus.LCorpusPortraitPrint(
                label, _pCorpusHost.PWindowLegendRead("Example"), ticket));
    }

    private void PCorpusPortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lCorpus?.LCorpusPortraitAllowed ?? false;
    }

    private async void PCorpusPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pCorpusHost.PWindowPortraitExport(
            _lCorpus.LCorpusQuotation.LQuotationFileRead(), _lCorpus.LCorpusPortraitExport);
    }
}
