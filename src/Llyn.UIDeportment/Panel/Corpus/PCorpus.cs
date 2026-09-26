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
        QLook.QLookStyleAttach(surface.Resources);
        QLook.QLookStyleAttach(Resources);

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

        QChoice.QChoiceDropperAttach(PRankDropper, PRankDropdown, PRank);
        QChoice.QChoiceDropperAttach(PGauzeDropper, PGauzeDropdown, PGauzeDropper);
        QChoice.QChoiceDropperAttach(PSpeaker, PLanguage, PSpeaker);

        PRankIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        PGauzeIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        PSpeakerIcon.QIconSource = QIcon.QIconResolve("expand", 12);
        PCorpusBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        PQuery.SetResourceReference(QField.QFieldHintProperty, "Example.Search");
        PDredge.SetResourceReference(QField.QFieldHintProperty, "Dredge.Search");
        PTranscriptSeedField.SetResourceReference(QField.QFieldHintProperty, "Example.Translation");
        PCorpusFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        PCorpusStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        PCorpusEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        PCorpusLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        PCorpusBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        PCorpusForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        PCorpusPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        PCorpusPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        PCorpusViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        PCorpusScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));
        if (PTranscriptAddition.Content is QIconImage addition)
        {
            addition.QIconSource = QIcon.QIconResolve("add", 12);
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

        QLookItem.QLookItemAttach(PAnthology, PAnthologyApply);
        QLookItem.QLookItemAttach(PQuotation, PQuotationApply);
        QLookItem.QLookItemAttach(PCitationList, PCitationApply);
        QLookItem.QLookItemAttach(PLanguageList, PSpeakerApply);
    }

    private Border PRank => (Border)FindName(nameof(PRank));

    private ToggleButton PRankDropper => (ToggleButton)FindName(nameof(PRankDropper));

    private QIconImage PRankIcon => (QIconImage)FindName(nameof(PRankIcon));

    private TextBox PQuery => (TextBox)FindName(nameof(PQuery));

    private Popup PRankDropdown => (Popup)FindName(nameof(PRankDropdown));

    private StackPanel PRankList => (StackPanel)FindName(nameof(PRankList));

    private TextBox PDredge => (TextBox)FindName(nameof(PDredge));

    private ToggleButton PGauzeDropper => (ToggleButton)FindName(nameof(PGauzeDropper));

    private QIconImage PGauzeIcon => (QIconImage)FindName(nameof(PGauzeIcon));

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

    private QIconImage PSpeakerIcon => (QIconImage)FindName(nameof(PSpeakerIcon));

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

    private QIconImage PCorpusBinIcon => (QIconImage)FindName(nameof(PCorpusBinIcon));

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
        QLookItem.QLookItemAttach(PTranscriptMentionLine, PMentionChip.PMentionChipApply);
        QLookItem.QLookItemAttach(PTranscriptGlossLine, PTranscriptGlossApply);
        QLookItem.QLookItemAttach(PExcerptGloss, PGloss.PGlossRowApply);

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
        PTranscript.Visibility = QLook.QLookVisibleRead(_lCorpus.LCorpusTranscriptShown);
        PExcerpt.Visibility = QLook.QLookVisibleRead(_lCorpus.LCorpusExcerptShown);
        PDisplay.Visibility = QLook.QLookVisibleRead(_lCorpus.LCorpusDisplayShown);
        PEditor.Visibility = QLook.QLookVisibleRead(_lCorpus.LCorpusEditorShown);
        PExcerptBody.Visibility = QLook.QLookVisibleRead(_lCorpus.LCorpusExcerptHeld);
        PExcerptUnselected.Visibility = QLook.QLookVisibleRead(_lCorpus.LCorpusExcerptBlank);
        PCorpusViewer.IsChecked = QLook.QLookCheckedRead(_lCorpus.LCorpusViewerChecked);
        PCorpusScribe.IsChecked = QLook.QLookCheckedRead(_lCorpus.LCorpusScribeChecked);
        PCorpusVoyage.Visibility = QLook.QLookVisibleRead(_lCorpus.LCorpusViewerChecked);
        PCorpusChronicle.Visibility = QLook.QLookVisibleRead(_lCorpus.LCorpusScribeChecked);
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
