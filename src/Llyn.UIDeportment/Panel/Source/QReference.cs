using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QReference
{
    private readonly ObservableCollection<QShelfItem> _qShelfList = [];

    private readonly ObservableCollection<QFootnoteItem> _qFootnoteList = [];

    private readonly UserControl _qReferenceSurface;

    private readonly QEditor _qReferenceEditor;

    private readonly QDisplay _qReferenceDisplay;

    private readonly QImprint _qImprint;

    private readonly QColophon _qColophon;

    private QWindow _qReferenceHost = null!;

    private CShelf _cShelf = null!;

    internal QReference(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qReferenceSurface = surface;
        _qReferenceEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qReferenceDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));
        _qImprint = new QImprint(QReferenceImprint);
        _qColophon = new QColophon(QReferenceColophon);

        QReferencePortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QReferencePress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(QGradeDropper, QGradeDropdown, QGrade);
        QChoice.QChoiceDropperAttach(QTrellisDropper, QTrellisDropdown, QTrellisDropper);

        QGradeIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QTrellisIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        QReferenceBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QSurvey.SetResourceReference(QField.QFieldHintProperty, "Source.Search");
        QRummage.SetResourceReference(QField.QFieldHintProperty, "Rummage.Search");
        QReferenceFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QReferenceStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QReferenceEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QReferenceLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QReferenceBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QReferenceForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QReferencePortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QReferencePress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QReferenceViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QReferenceScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QSurvey.TextChanged += QSurveyObserve;
        QRummage.TextChanged += QRummageObserve;
        QReferenceFresh.Click += QReferenceFreshObserve;
        QReferenceStore.Click += QReferenceStoreObserve;
        QReferenceEarlier.Click += QReferenceRetreatObserve;
        QReferenceLater.Click += QReferenceAdvanceObserve;
        QReferenceBackward.Click += QReferenceUndoObserve;
        QReferenceForward.Click += QReferenceRedoObserve;
        QReferenceViewer.Click += QReferenceViewerObserve;
        QReferenceScribe.Click += QReferenceScribeObserve;
        QReferenceBin.Click += QReferenceBinObserve;

        QShelf.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QShelfObserve));
        QFootnote.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QFootnoteObserve));

        QLookItem.QLookItemAttach(QShelf, QShelfItem.QShelfItemRefine);
        QLookItem.QLookItemAttach(QFootnote, QFootnoteItem.QFootnoteItemRefine);
    }

    private Border QGrade => QContract.QContractFind<Border>(_qReferenceSurface, "PGrade");

    private ToggleButton QGradeDropper => QContract.QContractFind<ToggleButton>(_qReferenceSurface, "PGradeDropper");

    private QIconImage QGradeIcon => QContract.QContractFind<QIconImage>(_qReferenceSurface, "PGradeIcon");

    private TextBox QSurvey => QContract.QContractFind<TextBox>(_qReferenceSurface, "PSurvey");

    private Popup QGradeDropdown => QContract.QContractFind<Popup>(_qReferenceSurface, "PGradeDropdown");

    private StackPanel QGradeList => QContract.QContractFind<StackPanel>(_qReferenceSurface, "PGradeList");

    private TextBox QRummage => QContract.QContractFind<TextBox>(_qReferenceSurface, "PRummage");

    private ToggleButton QTrellisDropper =>
        QContract.QContractFind<ToggleButton>(_qReferenceSurface, "PTrellisDropper");

    private QIconImage QTrellisIcon => QContract.QContractFind<QIconImage>(_qReferenceSurface, "PTrellisIcon");

    private FrameworkElement QTrellisMark =>
        QContract.QContractFind<FrameworkElement>(_qReferenceSurface, "PTrellisMark");

    private Popup QTrellisDropdown => QContract.QContractFind<Popup>(_qReferenceSurface, "PTrellisDropdown");

    private StackPanel QTrellisList => QContract.QContractFind<StackPanel>(_qReferenceSurface, "PTrellisList");

    private Button QReferenceFresh => QContract.QContractFind<Button>(_qReferenceSurface, "PReferenceFresh");

    private Button QReferenceStore => QContract.QContractFind<Button>(_qReferenceSurface, "PReferenceStore");

    private StackPanel QReferenceVoyage => QContract.QContractFind<StackPanel>(_qReferenceSurface, "PReferenceVoyage");

    private Button QReferenceEarlier => QContract.QContractFind<Button>(_qReferenceSurface, "PReferenceEarlier");

    private Button QReferenceLater => QContract.QContractFind<Button>(_qReferenceSurface, "PReferenceLater");

    private StackPanel QReferenceChronicle =>
        QContract.QContractFind<StackPanel>(_qReferenceSurface, "PReferenceChronicle");

    private Button QReferenceBackward => QContract.QContractFind<Button>(_qReferenceSurface, "PReferenceBackward");

    private Button QReferenceForward => QContract.QContractFind<Button>(_qReferenceSurface, "PReferenceForward");

    private Button QReferencePortrait => QContract.QContractFind<Button>(_qReferenceSurface, "PReferencePortrait");

    private Button QReferencePress => QContract.QContractFind<Button>(_qReferenceSurface, "PReferencePress");

    private Border QReferenceMode => QContract.QContractFind<Border>(_qReferenceSurface, "PReferenceMode");

    private RadioButton QReferenceViewer =>
        QContract.QContractFind<RadioButton>(_qReferenceSurface, "PReferenceViewer");

    private RadioButton QReferenceScribe =>
        QContract.QContractFind<RadioButton>(_qReferenceSurface, "PReferenceScribe");

    private ItemsControl QShelf => QContract.QContractFind<ItemsControl>(_qReferenceSurface, "PShelf");

    private TextBlock QShelfEmpty => QContract.QContractFind<TextBlock>(_qReferenceSurface, "PShelfEmpty");

    private ItemsControl QFootnote => QContract.QContractFind<ItemsControl>(_qReferenceSurface, "PFootnote");

    private TextBlock QFootnoteEmpty => QContract.QContractFind<TextBlock>(_qReferenceSurface, "PFootnoteEmpty");

    private UserControl QReferenceColophon => QContract.QContractFind<UserControl>(_qReferenceSurface, "PColophon");

    private UserControl QReferenceImprint => QContract.QContractFind<UserControl>(_qReferenceSurface, "PImprint");

    private Button QReferenceBin => QContract.QContractFind<Button>(_qReferenceSurface, "PReferenceBin");

    private QIconImage QReferenceBinIcon =>
        QContract.QContractFind<QIconImage>(_qReferenceSurface, "PReferenceBinIcon");

    internal void QReferenceIntroduce(QWindow host)
    {
        _qReferenceHost = host;
        _cShelf = CShelf.CShelfCreate(
            host.QWindowAtelier,
            QReferenceShownCheck,
            host.QWindowEnvoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        CPanel footnote = _cShelf.CShelfFootnote.CFootnotePanel;
        QLectern lectern = new(_cShelf.CShelfEditor.CEditorDisplay, footnote);
        _cShelf.CShelfChanged += QReferenceModeRefine;
        _cShelf.CShelfPanel.CPanelChanged += QReferenceModeRefine;
        _cShelf.CShelfPanel.CPanelAperture.CApertureRowsChanged += QShelfRefine;
        _cShelf.CShelfPanel.CPanelAperture.CApertureRowsChanged += _qImprint.QImprintTallyRefine;
        _cShelf.CShelfPanel.CPanelCleared += _qImprint.QImprintClearRefine;
        _cShelf.CShelfPanel.CPanelCleared += _qColophon.QColophonClearRefine;
        _cShelf.CShelfColophonChanged += _qColophon.QColophonRefine;
        _cShelf.CShelfFootnote.CFootnotePanel.CPanelChanged += QReferenceModeRefine;
        _cShelf.CShelfFootnote.CFootnotePanel.CPanelAperture.CApertureRowsChanged += QFootnoteRefine;

        QChoice.QChoiceOrderBuild(QGradeList, "Grade", QGradeObserve, CShelf.CShelfOrderRead());
        QShelf.ItemsSource = _qShelfList;
        QFootnote.ItemsSource = _qFootnoteList;

        _qImprint.QImprintIntroduce(_cShelf.CShelfImprint);
        _qReferenceDisplay.QDisplayIntroduce(host, lectern);
        _qReferenceEditor.QEditorIntroduce(host, _cShelf.CShelfEditor);

        _qReferenceSurface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QReferencePressObserve, QReferencePressRefine));
        _qReferenceSurface.CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, QReferencePortraitObserve,
            QReferencePortraitRefine));
    }

    internal async void QReferenceVistaRefine()
    {
        QChoice.QChoiceOrderApply(QGradeDropdown, _cShelf.CShelfPanel.CPanelAperture.CApertureOrder);
        QTrellisRefine();
        CEnsignSheet<CShelfRoll> sheet =
            await _cShelf.CShelfRollLoad(QEnsignImage.QEnsignDraw);
        QTrellisListRefine(sheet.CEnsignSheetLanguages);
        QShelfRefine(sheet.CEnsignSheetRows);
    }

    internal async void QFootnoteVistaRefine()
    {
        QFootnoteRefine(
            (await _cShelf.CShelfFootnote.CFootnoteRowsLoad(QEnsignImage.QEnsignDraw)).CEnsignSheetRows);
    }

    internal void QReferenceExitRefine()
    {
        _qImprint.QImprintCloseRefine();
        _qReferenceEditor.QEditorPlayerRefine();
        QGradeDropdown.IsOpen = false;
        QTrellisDropdown.IsOpen = false;
    }

    private bool QReferenceShownCheck()
    {
        return _qReferenceSurface.IsVisible;
    }

    private void QShelfRefine()
    {
        QShelfRefine(_cShelf.CShelfRollRead());
    }

    private void QShelfRefine(CShelfRoll roll)
    {
        QSplice.QSpliceRefine(
            _qShelfList,
            QShelfItem.QShelfItemBuild(roll.CShelfRollRows),
            QShelfItem.QShelfItemMatch,
            QShelfItem.QShelfItemSync);
        QShelfEmpty.Visibility = QLook.QLookVisibleRead(roll.CShelfRollEmpty);
        _qColophon.QColophonTallyRefine(roll.CShelfRollTally);
    }

    private void QFootnoteRefine()
    {
        QFootnoteRefine(_cShelf.CShelfFootnote.CFootnoteRowsRead());
    }

    private void QFootnoteRefine(IReadOnlyList<CVistaRow> rows)
    {
        QSplice.QSpliceRefine(
            _qFootnoteList,
            QFootnoteItem.QFootnoteItemBuild(rows),
            QFootnoteItem.QFootnoteItemMatch,
            QFootnoteItem.QFootnoteItemSync);
        QFootnoteEmpty.SetResourceReference(
            TextBlock.TextProperty, _cShelf.CShelfFootnote.CFootnotePanel.CPanelAperture.CApertureKey);
        QFootnoteEmpty.Visibility = QLook.QLookVisibleRead(_qFootnoteList.Count == 0);
    }

    private void QReferenceModeRefine()
    {
        _qReferenceEditor.QEditorVisibleRefine(QLook.QLookVisibleRead(_cShelf.CShelfDiptych.CDiptychChildEditing));
        _qReferenceDisplay.QDisplayVisibleRefine(QLook.QLookVisibleRead(_cShelf.CShelfDiptych.CDiptychChildShown));
        QReferenceImprint.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfDiptych.CDiptychParentEditing);
        QReferenceColophon.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfDiptych.CDiptychParentShown);
        QReferenceViewer.IsChecked = _cShelf.CShelfDiptych.CDiptychViewerChecked;
        QReferenceScribe.IsChecked = _cShelf.CShelfDiptych.CDiptychScribeChecked;
        QReferenceVoyage.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfDiptych.CDiptychViewerChecked);
        QReferenceChronicle.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfDiptych.CDiptychScribeChecked);
        QReferenceMode.IsEnabled = _cShelf.CShelfDiptych.CDiptychModeEnabled;
        QReferenceBin.IsEnabled = _cShelf.CShelfDiptych.CDiptychBinEnabled;
        QReferenceStore.IsEnabled = _cShelf.CShelfStoreEnabled;
        QReferenceChronicleRefine();
    }

    private void QReferenceChronicleRefine()
    {
        (bool undo, bool redo) = _cShelf.CShelfSession.CSessionChronicleRead();
        QReferenceBackward.IsEnabled = undo;
        QReferenceForward.IsEnabled = redo;
    }

    internal void QReferenceVoyageRefine(bool past, bool future)
    {
        QReferenceEarlier.IsEnabled = past;
        QReferenceLater.IsEnabled = future;
    }

    private void QReferenceRetreatObserve(object sender, RoutedEventArgs e)
    {
        _qReferenceHost.QWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void QReferenceAdvanceObserve(object sender, RoutedEventArgs e)
    {
        _qReferenceHost.QWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }

    private void QTrellisRefine()
    {
        QTrellisMark.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfPanel.CPanelAperture.CApertureFiltered);
    }

    private void QTrellisListRefine(IReadOnlyList<string> languages)
    {
        QChoice.QChoiceFilterBuild(
            QTrellisList, languages, _cShelf.CShelfPanel.CPanelAperture.CApertureFilter, QTrellisObserve);
    }

    private void QSurveyObserve(object sender, TextChangedEventArgs e)
    {
        _cShelf.CShelfPanel.CPanelAperture.CApertureQuerySet(QSurvey.Text);
    }

    private void QRummageObserve(object sender, TextChangedEventArgs e)
    {
        _cShelf.CShelfFootnote.CFootnotePanel.CPanelAperture.CApertureQuerySet(QRummage.Text);
    }

    private void QTrellisObserve(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfPanel.CPanelAperture.CApertureFilterSet(QChoice.QChoiceFilterRead(sender));
        QTrellisRefine();
    }

    private void QGradeObserve(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfPanel.CPanelAperture.CApertureOrderSet(QChoice.QChoiceOrderRead(sender));
        QGradeRefine();
    }

    private void QGradeRefine()
    {
        QGradeDropper.IsChecked = false;
    }

    private void QShelfObserve(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfReferenceSelect(QSender.QSenderSourceRead<QShelfItem>(e)?.QShelfItemId);
    }

    private void QFootnoteObserve(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfEntrySelect(QSender.QSenderSourceRead<QFootnoteItem>(e)?.QFootnoteItemId);
    }

    private void QReferenceFreshObserve(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfDiptych.CDiptychEntryCreate();
    }

    private void QReferenceViewerObserve(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfScribeToggle(false);
    }

    private void QReferenceScribeObserve(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfScribeToggle(true);
    }

    private void QReferenceStoreObserve(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfSession.CSessionSave();
    }

    private void QReferenceUndoObserve(object sender, RoutedEventArgs e)
    {
        QChronicle.QChronicleCaretRefine(_cShelf.CShelfSession.CSessionUndo);
    }

    private void QReferenceRedoObserve(object sender, RoutedEventArgs e)
    {
        QChronicle.QChronicleCaretRefine(_cShelf.CShelfSession.CSessionRedo);
    }

    private void QReferenceBinObserve(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfDiptych.CDiptychEntryDelete();
    }

    private void QReferencePressRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cShelf.CShelfPressAllowed;
    }

    private async void QReferencePressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cShelf.CShelfPortraitPrint();
    }

    private void QReferencePortraitRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cShelf.CShelfPortraitAllowed;
    }

    private async void QReferencePortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cShelf.CShelfPortraitExport();
    }
}
