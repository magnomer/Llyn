using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus : QChronicleHost
{
    private readonly UserControl _qCorpusSurface;

    private readonly QDrawer _qDrawer;

    private PWindow _qCorpusHost = null!;

    private CCorpus _cCorpus = null!;

    private CDesk _qCorpusDesk = null!;

    internal QCorpus(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qCorpusSurface = surface;
        QLook.QLookStyleAttach(surface.Resources);
        QChronicle.QChronicleAttach(surface, this);
        _qDrawer = new QDrawer(QCitationDrawer, QCitationSheet, QCitationList, QCitationPressHandle);

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QCorpusPressHandle, QCorpusPressCheck));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QCorpusPortraitHandle, QCorpusPortraitCheck));
        QCorpusPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QCorpusPress.Command = ApplicationCommands.Print;

        QTranscript.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandLink, QTranscriptLinkHandle, QTranscriptLinkCheck));
        QTranscript.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandChoose, QTranscriptSenseHandle, QTranscriptSenseCheck));
        QTranscript.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandSilence, QTranscriptSilenceHandle, QTranscriptLinkCheck));
        QTranscript.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandUnlink, QTranscriptUnlinkHandle, QTranscriptUnlinkCheck));

        QChoice.QChoiceDropperAttach(QRankDropper, QRankDropdown, QRank);
        QChoice.QChoiceDropperAttach(QGauzeDropper, QGauzeDropdown, QGauzeDropper);
        QChoice.QChoiceDropperAttach(QSpeaker, QLanguage, QSpeaker);

        QRankIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QGauzeIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        QSpeakerIcon.QIconSource = QIcon.QIconResolve("expand", 12);
        QCorpusBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QTranscriptAdditionIcon.QIconSource = QIcon.QIconResolve("add", 12);
        QQuery.SetResourceReference(QField.QFieldHintProperty, "Example.Search");
        QDredge.SetResourceReference(QField.QFieldHintProperty, "Dredge.Search");
        QTranscriptSeedField.SetResourceReference(QField.QFieldHintProperty, "Example.Translation");
        QCorpusFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QCorpusStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QCorpusEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QCorpusLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QCorpusBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QCorpusForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QCorpusPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QCorpusPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QCorpusViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QCorpusScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QQuery.TextChanged += QQueryHandle;
        QDredge.TextChanged += QDredgeHandle;
        QCitationField.TextChanged += QCitationTextHandle;
        QCitationField.PreviewKeyDown += QCitationKeyHandle;
        QCitationField.LostKeyboardFocus += QCitationLeaveHandle;
        QTranscriptSeedField.GotKeyboardFocus += QTranscriptSeedHandle;
        QExcerptText.PMentionClick += QExcerptMentionHandle;
        QCorpusFresh.Click += QCorpusFreshHandle;
        QCorpusStore.Click += QCorpusStoreHandle;
        QCorpusEarlier.Click += QCorpusRetreatHandle;
        QCorpusLater.Click += QCorpusAdvanceHandle;
        QCorpusBackward.Click += QCorpusUndoHandle;
        QCorpusForward.Click += QCorpusRedoHandle;
        QCorpusViewer.Click += QCorpusViewerHandle;
        QCorpusScribe.Click += QCorpusScribeHandle;
        QCorpusBin.Click += QCorpusBinHandle;
        QTranscriptAddition.Click += QGlossAddHandle;
        QTranscriptAttach();

        QLookItem.QLookItemAttach(QAnthology, QAnthologyApply);
        QLookItem.QLookItemAttach(QQuotation, QQuotationApply);
        QLookItem.QLookItemAttach(QLanguageList, QSpeakerApply);
    }

    private Border QRank => QContract.QContractFind<Border>(_qCorpusSurface, "PRank");

    private ToggleButton QRankDropper => QContract.QContractFind<ToggleButton>(_qCorpusSurface, "PRankDropper");

    private QIconImage QRankIcon => QContract.QContractFind<QIconImage>(_qCorpusSurface, "PRankIcon");

    private TextBox QQuery => QContract.QContractFind<TextBox>(_qCorpusSurface, "PQuery");

    private Popup QRankDropdown => QContract.QContractFind<Popup>(_qCorpusSurface, "PRankDropdown");

    private StackPanel QRankList => QContract.QContractFind<StackPanel>(_qCorpusSurface, "PRankList");

    private TextBox QDredge => QContract.QContractFind<TextBox>(_qCorpusSurface, "PDredge");

    private ToggleButton QGauzeDropper => QContract.QContractFind<ToggleButton>(_qCorpusSurface, "PGauzeDropper");

    private QIconImage QGauzeIcon => QContract.QContractFind<QIconImage>(_qCorpusSurface, "PGauzeIcon");

    private FrameworkElement QGauzeMark => QContract.QContractFind<FrameworkElement>(_qCorpusSurface, "PGauzeMark");

    private Popup QGauzeDropdown => QContract.QContractFind<Popup>(_qCorpusSurface, "PGauzeDropdown");

    private StackPanel QGauzeList => QContract.QContractFind<StackPanel>(_qCorpusSurface, "PGauzeList");

    private Button QCorpusFresh => QContract.QContractFind<Button>(_qCorpusSurface, "PCorpusFresh");

    private Button QCorpusStore => QContract.QContractFind<Button>(_qCorpusSurface, "PCorpusStore");

    private StackPanel QCorpusVoyage => QContract.QContractFind<StackPanel>(_qCorpusSurface, "PCorpusVoyage");

    private Button QCorpusEarlier => QContract.QContractFind<Button>(_qCorpusSurface, "PCorpusEarlier");

    private Button QCorpusLater => QContract.QContractFind<Button>(_qCorpusSurface, "PCorpusLater");

    private StackPanel QCorpusChronicle => QContract.QContractFind<StackPanel>(_qCorpusSurface, "PCorpusChronicle");

    private Button QCorpusBackward => QContract.QContractFind<Button>(_qCorpusSurface, "PCorpusBackward");

    private Button QCorpusForward => QContract.QContractFind<Button>(_qCorpusSurface, "PCorpusForward");

    private Button QCorpusPortrait => QContract.QContractFind<Button>(_qCorpusSurface, "PCorpusPortrait");

    private Button QCorpusPress => QContract.QContractFind<Button>(_qCorpusSurface, "PCorpusPress");

    private Border QCorpusMode => QContract.QContractFind<Border>(_qCorpusSurface, "PCorpusMode");

    private RadioButton QCorpusViewer => QContract.QContractFind<RadioButton>(_qCorpusSurface, "PCorpusViewer");

    private RadioButton QCorpusScribe => QContract.QContractFind<RadioButton>(_qCorpusSurface, "PCorpusScribe");

    private ItemsControl QAnthology => QContract.QContractFind<ItemsControl>(_qCorpusSurface, "PAnthology");

    private TextBlock QAnthologyEmpty => QContract.QContractFind<TextBlock>(_qCorpusSurface, "PAnthologyEmpty");

    private ItemsControl QQuotation => QContract.QContractFind<ItemsControl>(_qCorpusSurface, "PQuotation");

    private TextBlock QQuotationEmpty => QContract.QContractFind<TextBlock>(_qCorpusSurface, "PQuotationEmpty");

    private PDisplay QCorpusDisplay => QContract.QContractFind<PDisplay>(_qCorpusSurface, "PDisplay");

    private PEditor QCorpusEditor => QContract.QContractFind<PEditor>(_qCorpusSurface, "PEditor");

    private Grid QExcerpt => QContract.QContractFind<Grid>(_qCorpusSurface, "PExcerpt");

    private StackPanel QExcerptBody => QContract.QContractFind<StackPanel>(_qCorpusSurface, "PExcerptBody");

    private PMention QExcerptText => QContract.QContractFind<PMention>(_qCorpusSurface, "PExcerptText");

    private Image QExcerptFlag => QContract.QContractFind<Image>(_qCorpusSurface, "PExcerptFlag");

    private TextBlock QExcerptLanguage => QContract.QContractFind<TextBlock>(_qCorpusSurface, "PExcerptLanguage");

    private TextBlock QExcerptTally => QContract.QContractFind<TextBlock>(_qCorpusSurface, "PExcerptTally");

    private StackPanel QExcerptGlossSection =>
        QContract.QContractFind<StackPanel>(_qCorpusSurface, "PExcerptGlossSection");

    private ItemsControl QExcerptGloss => QContract.QContractFind<ItemsControl>(_qCorpusSurface, "PExcerptGloss");

    private StackPanel QExcerptCitationSection =>
        QContract.QContractFind<StackPanel>(_qCorpusSurface, "PExcerptCitationSection");

    private TextBlock QExcerptCitation => QContract.QContractFind<TextBlock>(_qCorpusSurface, "PExcerptCitation");

    private TextBlock QExcerptUnselected =>
        QContract.QContractFind<TextBlock>(_qCorpusSurface, "PExcerptUnselected");

    private Grid QTranscript => QContract.QContractFind<Grid>(_qCorpusSurface, "PTranscript");

    private TextBox QTranscriptText => QContract.QContractFind<TextBox>(_qCorpusSurface, "PTranscriptText");

    private ItemsControl QTranscriptMentionLine =>
        QContract.QContractFind<ItemsControl>(_qCorpusSurface, "PTranscriptMentionLine");

    private ToggleButton QSpeaker => QContract.QContractFind<ToggleButton>(_qCorpusSurface, "PSpeaker");

    private Image QSpeakerFlag => QContract.QContractFind<Image>(_qCorpusSurface, "PSpeakerFlag");

    private TextBlock QSpeakerName => QContract.QContractFind<TextBlock>(_qCorpusSurface, "PSpeakerName");

    private QIconImage QSpeakerIcon => QContract.QContractFind<QIconImage>(_qCorpusSurface, "PSpeakerIcon");

    private Popup QLanguage => QContract.QContractFind<Popup>(_qCorpusSurface, "PLanguage");

    private ItemsControl QLanguageList => QContract.QContractFind<ItemsControl>(_qCorpusSurface, "PLanguageList");

    private TextBlock QTranscriptTally => QContract.QContractFind<TextBlock>(_qCorpusSurface, "PTranscriptTally");

    private ItemsControl QTranscriptGlossLine =>
        QContract.QContractFind<ItemsControl>(_qCorpusSurface, "PTranscriptGlossLine");

    private Border QTranscriptSeed => QContract.QContractFind<Border>(_qCorpusSurface, "PTranscriptSeed");

    private TextBox QTranscriptSeedField => QContract.QContractFind<TextBox>(_qCorpusSurface, "PTranscriptSeedField");

    private Button QTranscriptAddition => QContract.QContractFind<Button>(_qCorpusSurface, "PTranscriptAddition");

    private QIconImage QTranscriptAdditionIcon =>
        QContract.QContractFind<QIconImage>(_qCorpusSurface, "PTranscriptAdditionIcon");

    private TextBox QCitationField => QContract.QContractFind<TextBox>(_qCorpusSurface, "PCitationField");

    private Popup QCitationDrawer => QContract.QContractFind<Popup>(_qCorpusSurface, "PCitationDrawer");

    private Border QCitationSheet => QContract.QContractFind<Border>(_qCorpusSurface, "PCitationSheet");

    private ListBox QCitationList => QContract.QContractFind<ListBox>(_qCorpusSurface, "PCitationList");

    private Button QCorpusBin => QContract.QContractFind<Button>(_qCorpusSurface, "PCorpusBin");

    private QIconImage QCorpusBinIcon => QContract.QContractFind<QIconImage>(_qCorpusSurface, "PCorpusBinIcon");

    internal void QCorpusAttach(PWindow host)
    {
        _qCorpusHost = host;
        _cCorpus = host.PWindowForge.QForgeCorpusCreate(QCorpusShownCheck, host.PWindowEnvoy);
        QLectern lectern = new(_cCorpus.CCorpusEditor.CEditorDisplay, _cCorpus.CCorpusQuotation.CQuotationPanel);
        _qCorpusDesk = _cCorpus.CCorpusDesk;
        QTranscriptDeskAttach();

        QAnthology.ItemsSource = _qAnthologyList;
        QQuotation.ItemsSource = _qQuotationList;
        QLanguageList.ItemsSource = _qLanguageItem;
        QTranscriptMentionLine.ItemsSource = _qTranscriptChip.PMentionLineChip;
        QTranscriptGlossLine.ItemsSource = _qTranscriptGloss;
        QExcerptGloss.ItemsSource = _qExcerptGloss;
        QLookItem.QLookItemAttach(QTranscriptMentionLine, PMentionChip.PMentionChipApply);
        QLookItem.QLookItemAttach(QTranscriptGlossLine, QTranscriptGlossApply);
        QLookItem.QLookItemAttach(QExcerptGloss, PGloss.PGlossRowApply);

        QCorpusDisplay.PDisplayAttach(host, lectern);
        QCorpusEditor.PEditorIntroduce(host, new QEditor(_cCorpus.CCorpusEditor));

        CPanel anthology = _cCorpus.CCorpusAnthology.CAnthologyPanel;
        CPanel quotation = _cCorpus.CCorpusQuotation.CQuotationPanel;
        _cCorpus.CCorpusChanged += QCorpusModeUpdate;
        _cCorpus.CCorpusTranscriptChanged += QTranscriptApply;
        _cCorpus.CCorpusExampleChanged += QExcerptShow;
        _cCorpus.CCorpusQueryCleared += QQueryClear;
        anthology.CPanelChanged += QCorpusModeUpdate;
        anthology.CPanelRowsChanged += QAnthologyFind;
        quotation.CPanelChanged += QCorpusModeUpdate;
        quotation.CPanelRowsChanged += QQuotationFind;
    }

    private bool QCorpusShownCheck()
    {
        return _qCorpusSurface.IsVisible;
    }

    private void QCorpusModeUpdate()
    {
        QTranscript.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusTranscriptShown);
        QExcerpt.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusExcerptShown);
        QCorpusDisplay.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusDisplayShown);
        QCorpusEditor.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusEditorShown);
        QExcerptBody.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusExcerptHeld);
        QExcerptUnselected.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusExcerptBlank);
        QCorpusViewer.IsChecked = QLook.QLookCheckedRead(_cCorpus.CCorpusViewerChecked);
        QCorpusScribe.IsChecked = QLook.QLookCheckedRead(_cCorpus.CCorpusScribeChecked);
        QCorpusVoyage.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusViewerChecked);
        QCorpusChronicle.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusScribeChecked);
        QCorpusMode.IsEnabled = _cCorpus.CCorpusModeEnabled;
        QCorpusBin.IsEnabled = _cCorpus.CCorpusBinEnabled;
        QCorpusStore.IsEnabled = _cCorpus.CCorpusStoreEnabled;
        QTranscript.IsEnabled = _cCorpus.CCorpusDesk.CDeskRunning;
        QChronicleUpdate();
    }

    internal void QCorpusReset()
    {
        _cCorpus.CCorpusExampleClose();
    }

    internal void QCorpusClose()
    {
        QCorpusEditor.PEditorClose();
        QCorpusDisplay.PDisplayClose();
        _qDrawer.QDrawerHide();
        QLanguage.IsOpen = false;
        QRankDropdown.IsOpen = false;
        QGauzeDropdown.IsOpen = false;
    }

    private void QCorpusPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cCorpus?.CCorpusPressAllowed ?? false;
    }

    private async void QCorpusPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _cCorpus.CCorpusPortraitPrint();
    }

    private void QCorpusPortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cCorpus?.CCorpusPortraitAllowed ?? false;
    }

    private async void QCorpusPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _cCorpus.CCorpusPortraitExport();
    }
}
