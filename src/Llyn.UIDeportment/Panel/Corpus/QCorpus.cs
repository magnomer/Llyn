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

    private readonly QEditor _qCorpusEditor;

    private readonly QDisplay _qCorpusDisplay;

    private readonly QDrawer _qDrawer;

    private QWindow _qCorpusHost = null!;

    private CCorpus _cCorpus = null!;

    private CTranscript _cTranscript = null!;

    internal QCorpus(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qCorpusSurface = surface;
        _qCorpusEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qCorpusDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));
        QLook.QLookStyleAttach(surface);
        QChronicle.QChronicleIntroduce(surface, this);
        _qDrawer = new QDrawer(QCitationDrawer, QCitationSheet, QCitationList, QCitationPressObserve);

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QCorpusPressObserve, QCorpusPressCheck));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QCorpusPortraitObserve, QCorpusPortraitCheck));
        QCorpusPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QCorpusPress.Command = ApplicationCommands.Print;

        QTranscript.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandLink, QTranscriptLinkObserve, QTranscriptSpanRefine));
        QTranscript.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandChoose, QTranscriptMeaningRefine, QTranscriptSenseRefine));
        QTranscript.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandSilence, QTranscriptSilenceObserve, QTranscriptSpanRefine));
        QTranscript.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandUnlink, QTranscriptUnlinkObserve, QTranscriptUnlinkRefine));
        QTranscript.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandPick, QTranscriptPickObserve));

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

        QQuery.TextChanged += QQueryObserve;
        QDredge.TextChanged += QDredgeObserve;
        QCitationField.TextChanged += QCitationTextObserve;
        QCitationField.PreviewKeyDown += QCitationKeyRefine;
        QCitationField.PreviewKeyDown += QCitationPickObserve;
        QCitationField.PreviewKeyDown += QCitationCommitObserve;
        QCitationField.PreviewKeyDown += QCitationEscapeRefine;
        QCitationField.LostKeyboardFocus += QCitationLeaveRefine;
        QTranscriptSeedField.GotKeyboardFocus += QTranscriptSeedObserve;
        QExcerptText.PMentionClick += QExcerptMentionObserve;
        QCorpusFresh.Click += QCorpusFreshObserve;
        QCorpusStore.Click += QCorpusStoreObserve;
        QCorpusEarlier.Click += QCorpusRetreatObserve;
        QCorpusLater.Click += QCorpusAdvanceObserve;
        QCorpusBackward.Click += QCorpusUndoObserve;
        QCorpusForward.Click += QCorpusRedoObserve;
        QCorpusViewer.Click += QCorpusViewerObserve;
        QCorpusScribe.Click += QCorpusScribeObserve;
        QCorpusBin.Click += QCorpusBinObserve;
        QTranscriptAddition.Click += QGlossAddObserve;
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

    internal void QCorpusIntroduce(QWindow host)
    {
        _qCorpusHost = host;
        _cCorpus = CCorpus.CCorpusCreate(
            host.QWindowAtelier,
            QCorpusShownCheck,
            host.QWindowEnvoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        _cTranscript = _cCorpus.CCorpusTranscript;
        QLectern lectern = new(_cCorpus.CCorpusEditor.CEditorDisplay, _cCorpus.CCorpusQuotation.CQuotationPanel);
        QTranscriptDeskIntroduce();

        QAnthology.ItemsSource = _qAnthologyList;
        QQuotation.ItemsSource = _qQuotationList;
        QLanguageList.ItemsSource = _qLanguageItem;
        QTranscriptMentionLine.ItemsSource = _qTranscriptChip.PMentionLineChip;
        QTranscriptGlossLine.ItemsSource = _qTranscriptGloss;
        QExcerptGloss.ItemsSource = _qExcerptGloss;
        QLookItem.QLookItemAttach(QTranscriptMentionLine, PMentionChip.PMentionChipRefine);
        QLookItem.QLookItemAttach(QTranscriptGlossLine, QTranscriptGlossApply);
        QLookItem.QLookItemAttach(QExcerptGloss, PGloss.PGlossRowApply);
        QChoice.QChoiceOrderBuild(QRankList, "Rank", QRankObserve, CAnthology.CAnthologyOrderRead());

        _qCorpusDisplay.QDisplayIntroduce(host, lectern);
        _qCorpusEditor.QEditorIntroduce(host, _cCorpus.CCorpusEditor);
        _cTranscript.CTranscriptMentionOffered += _qCorpusEditor.QEditorProspect.QProspectOpenRefine;

        CPanel anthology = _cCorpus.CCorpusAnthology.CAnthologyPanel;
        CPanel quotation = _cCorpus.CCorpusQuotation.CQuotationPanel;
        _cCorpus.CCorpusChanged += QCorpusModeUpdate;
        _cCorpus.CCorpusTranscriptChanged += QTranscriptRefine;
        _cCorpus.CCorpusExampleChanged += QExcerptRefine;
        _cCorpus.CCorpusQueryCleared += QQueryClear;
        _cCorpus.CCorpusWorkspaceChanged += QCorpusWorkspaceRefine;
        anthology.CPanelChanged += QCorpusModeUpdate;
        anthology.CPanelRowsChanged += QAnthologyRefine;
        anthology.CPanelRowsChanged += QCorpusTallyRefine;
        quotation.CPanelChanged += QCorpusModeUpdate;
        quotation.CPanelRowsChanged += QQuotationRefine;
    }

    private bool QCorpusShownCheck()
    {
        return _qCorpusSurface.IsVisible;
    }

    private void QCorpusModeUpdate()
    {
        QTranscript.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusTranscriptShown);
        QExcerpt.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusExcerptShown);
        _qCorpusDisplay.QDisplayVisibleRefine(QLook.QLookVisibleRead(_cCorpus.CCorpusDisplayShown));
        _qCorpusEditor.QEditorVisibleRefine(QLook.QLookVisibleRead(_cCorpus.CCorpusEditorShown));
        QExcerptBody.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusExcerptHeld);
        QExcerptUnselected.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusExcerptBlank);
        QCorpusViewer.IsChecked = _cCorpus.CCorpusViewerChecked;
        QCorpusScribe.IsChecked = _cCorpus.CCorpusScribeChecked;
        QCorpusVoyage.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusViewerChecked);
        QCorpusChronicle.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusScribeChecked);
        QCorpusMode.IsEnabled = _cCorpus.CCorpusModeEnabled;
        QCorpusBin.IsEnabled = _cCorpus.CCorpusBinEnabled;
        QCorpusStore.IsEnabled = _cCorpus.CCorpusStoreEnabled;
        QTranscript.IsEnabled = _cCorpus.CCorpusTranscriptEnabled;
        QCorpusChronicleRefine();
    }

    internal void QCorpusExitRefine()
    {
        _qCorpusEditor.QEditorPlayerRefine();
        _qDrawer.QDrawerHide();
        QLanguage.IsOpen = false;
        QRankDropdown.IsOpen = false;
        QGauzeDropdown.IsOpen = false;
    }

    private void QCorpusPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cCorpus?.CCorpusPressAllowed ?? false;
    }

    private async void QCorpusPressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cCorpus.CCorpusPortraitPrint();
    }

    private void QCorpusPortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cCorpus?.CCorpusPortraitAllowed ?? false;
    }

    private async void QCorpusPortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cCorpus.CCorpusQuotation.CQuotationPortraitExport();
    }
}
