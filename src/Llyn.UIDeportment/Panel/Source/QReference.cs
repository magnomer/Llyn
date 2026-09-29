using System;
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

    private readonly QImprint _qImprint;

    private readonly QColophon _qColophon;

    private PWindow _qReferenceHost = null!;

    private CShelf _cShelf = null!;

    internal QReference(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qReferenceSurface = surface;
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

        QSurvey.TextChanged += QSurveyHandle;
        QRummage.TextChanged += QRummageHandle;
        QReferenceFresh.Click += QReferenceFreshHandle;
        QReferenceStore.Click += QReferenceStoreHandle;
        QReferenceEarlier.Click += QReferenceRetreatHandle;
        QReferenceLater.Click += QReferenceAdvanceHandle;
        QReferenceBackward.Click += QReferenceUndoHandle;
        QReferenceForward.Click += QReferenceRedoHandle;
        QReferenceViewer.Click += QReferenceScribeHandle;
        QReferenceScribe.Click += QReferenceScribeHandle;
        QReferenceBin.Click += QReferenceBinHandle;

        QShelf.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QShelfHandle));
        QFootnote.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QFootnoteHandle));

        QLookItem.QLookItemAttach(QShelf, QShelfItem.QShelfItemApply);
        QLookItem.QLookItemAttach(QFootnote, QFootnoteItem.QFootnoteItemApply);
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

    private PDisplay QReferenceDisplay => QContract.QContractFind<PDisplay>(_qReferenceSurface, "PDisplay");

    private PEditor QReferenceEditor => QContract.QContractFind<PEditor>(_qReferenceSurface, "PEditor");

    private UserControl QReferenceColophon => QContract.QContractFind<UserControl>(_qReferenceSurface, "PColophon");

    private UserControl QReferenceImprint => QContract.QContractFind<UserControl>(_qReferenceSurface, "PImprint");

    private Button QReferenceBin => QContract.QContractFind<Button>(_qReferenceSurface, "PReferenceBin");

    private QIconImage QReferenceBinIcon =>
        QContract.QContractFind<QIconImage>(_qReferenceSurface, "PReferenceBinIcon");

    internal void QReferenceAttach(PWindow host)
    {
        _qReferenceHost = host;
        _cShelf = host.PWindowForge.QForgeShelfCreate(QReferenceShownCheck, host.PWindowEnvoy);
        CPanel footnote = _cShelf.CShelfFootnote.CFootnotePanel;
        QLectern lectern = new(_cShelf.CShelfEditor.CEditorDisplay, footnote);
        _cShelf.CShelfChanged += QReferenceModeUpdate;
        _cShelf.CShelfPanel.CPanelChanged += QReferenceModeUpdate;
        _cShelf.CShelfPanel.CPanelRowsChanged += QShelfUpdate;
        _cShelf.CShelfPanel.CPanelCleared += _qImprint.QImprintClear;
        _cShelf.CShelfPanel.CPanelCleared += _qColophon.QColophonClear;
        _cShelf.CShelfColophonChanged += _qColophon.QColophonShow;
        _cShelf.CShelfFootnote.CFootnotePanel.CPanelChanged += QReferenceModeUpdate;
        _cShelf.CShelfFootnote.CFootnotePanel.CPanelRowsChanged += QFootnoteUpdate;

        QShelf.ItemsSource = _qShelfList;
        QFootnote.ItemsSource = _qFootnoteList;

        _qImprint.QImprintAttach(host, _cShelf.CShelfImprint);
        QReferenceDisplay.PDisplayAttach(host, lectern);
        QReferenceEditor.PEditorIntroduce(host, new QEditor(_cShelf.CShelfEditor));

        _qReferenceSurface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QReferencePressHandle, QReferencePressCheck));
        _qReferenceSurface.CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, QReferencePortraitHandle,
            QReferencePortraitCheck));
    }

    internal async void QReferenceVistaRestore()
    {
        CPanel footnote = _cShelf.CShelfFootnote.CFootnotePanel;
        CPanel shelf = _cShelf.CShelfPanel;
        shelf.CPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.CPanelRowsResonate));
        shelf.CPanelObserverAttach(
            CSubject.CSubjectWorkspace,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, _cShelf.CShelfReferenceClose));
        shelf.CPanelObserverAttach(
            CSubject.CSubjectAuthor,
            LObserver.LObserverCreate<CBulletin>(
                _qReferenceSurface, _cShelf.CShelfImprint.CImprintDesk.CDeskDraftResonate));
        shelf.CPanelObserverAttach(
            CSubject.CSubjectAuthor,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.CPanelRowsResonate));
        shelf.CPanelObserverAttach(
            CSubject.CSubjectReference,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.CPanelRowsResonate));
        shelf.CPanelObserverAttach(
            CSubject.CSubjectExample,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.CPanelRowsResonate));
        shelf.CPanelObserverAttach(
            CSubject.CSubjectReflex,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.CPanelRowsResonate));
        shelf.CPanelObserverAttach(
            CSubject.CSubjectSettings,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.CPanelRowsResonate));
        footnote.CPanelObserverAttach(
            CSubject.CSubjectEntry,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, footnote.CPanelEntryResonate));
        footnote.CPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.CPanelRowsResonate));
        footnote.CPanelChosenAttach(
            CSubject.CSubjectEntry,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, _cShelf.CShelfEntryResonate));
        footnote.CPanelObserverAttach(
            CSubject.CSubjectVista,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, footnote.CPanelRowsResonate));
        QChoice.QChoiceOrderBuild(QGradeList, "Grade", QGradeHandle, CShelf.CShelfOrderRead());
        QChoice.QChoiceOrderApply(QGradeDropdown, _cShelf.CShelfPanel.CPanelOrder);
        QTrellisUpdate();

        await LEnsignImage.LEnsignLoad(_qReferenceHost.PWindowAtelier);

        QChoice.QChoiceFilterBuild(
            QTrellisList,
            _qReferenceHost.PWindowAtelier.CAtelierCatalog.CCatalogLanguageRead(),
            _cShelf.CShelfPanel.CPanelFilter,
            QTrellisHandle);
        _cShelf.CShelfQuerySet(QSurvey.Text);
        _cShelf.CShelfFootnote.CFootnoteQuerySet(QRummage.Text);
        _cShelf.CShelfPanel.CPanelRowsResonate();
    }

    internal void QReferenceClose()
    {
        _qImprint.QImprintClose();
        QReferenceEditor.PEditorClose();
        QReferenceDisplay.PDisplayClose();
        QGradeDropdown.IsOpen = false;
        QTrellisDropdown.IsOpen = false;
    }

    private bool QReferenceShownCheck()
    {
        return _qReferenceSurface.IsVisible;
    }

    private void QShelfUpdate()
    {
        LSplice.LSpliceApply(
            _qShelfList,
            QShelfItem.QShelfItemBuild(_cShelf.CShelfRowsRead()),
            QShelfItem.QShelfItemMatch,
            QShelfItem.QShelfItemSync);
        QShelfEmpty.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfEmpty);
        _qColophon.QColophonTallyShow(_cShelf.CShelfTallyRead());
        _qImprint.QImprintTallyShow();
    }

    private void QFootnoteUpdate()
    {
        LSplice.LSpliceApply(
            _qFootnoteList,
            QFootnoteItem.QFootnoteItemBuild(_cShelf.CShelfFootnote.CFootnoteRowsRead()),
            QFootnoteItem.QFootnoteItemMatch,
            QFootnoteItem.QFootnoteItemSync);
        QFootnoteEmpty.SetResourceReference(TextBlock.TextProperty, _cShelf.CShelfFootnote.CFootnoteEmptyKey);
        QFootnoteEmpty.Visibility = QLook.QLookVisibleRead(_qFootnoteList.Count == 0);
    }

    private void QReferenceModeUpdate()
    {
        QReferenceEditor.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfEditorShown);
        QReferenceDisplay.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfDisplayShown);
        QReferenceImprint.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfImprintShown);
        QReferenceColophon.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfColophonShown);
        QReferenceViewer.IsChecked = QLook.QLookCheckedRead(_cShelf.CShelfViewerChecked);
        QReferenceScribe.IsChecked = QLook.QLookCheckedRead(_cShelf.CShelfScribeChecked);
        QReferenceVoyage.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfViewerChecked);
        QReferenceChronicle.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfScribeChecked);
        QReferenceMode.IsEnabled = _cShelf.CShelfModeEnabled;
        QReferenceBin.IsEnabled = _cShelf.CShelfBinEnabled;
        QReferenceStore.IsEnabled = _cShelf.CShelfStoreEnabled;
        QReferenceChronicleUpdate();
    }

    private void QReferenceChronicleUpdate()
    {
        (bool undo, bool redo) = _cShelf.CShelfChronicleRead();
        QReferenceBackward.IsEnabled = undo;
        QReferenceForward.IsEnabled = redo;
    }

    internal void QReferenceVoyageShow(bool past, bool future)
    {
        QReferenceEarlier.IsEnabled = past;
        QReferenceLater.IsEnabled = future;
    }

    private void QReferenceRetreatHandle(object sender, RoutedEventArgs e)
    {
        _qReferenceHost.PWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void QReferenceAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _qReferenceHost.PWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }

    private void QTrellisUpdate()
    {
        QTrellisMark.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfFiltered);
    }

    private void QSurveyHandle(object sender, TextChangedEventArgs e)
    {
        _cShelf.CShelfQuerySet(QSurvey.Text);
    }

    private void QRummageHandle(object sender, TextChangedEventArgs e)
    {
        _cShelf.CShelfFootnote.CFootnoteQuerySet(QRummage.Text);
    }

    private void QTrellisHandle(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfFilterSet(QChoice.QChoiceFilterRead(sender));
        QTrellisUpdate();
    }

    private void QGradeHandle(object sender, RoutedEventArgs e)
    {
        QGradeDropper.IsChecked = false;
        _cShelf.CShelfOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    private void QShelfHandle(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfReferenceSelect(QSender.QSenderSourceRead<QShelfItem>(e)?.QShelfItemId);
    }

    private void QFootnoteHandle(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfEntrySelect(QSender.QSenderSourceRead<QFootnoteItem>(e)?.QFootnoteItemId);
    }

    private void QReferenceFreshHandle(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfReferenceCreate();
    }

    private void QReferenceScribeHandle(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfScribeToggle(ReferenceEquals(sender, QReferenceScribe));
    }

    private void QReferenceStoreHandle(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfDraftSave();
    }

    private void QReferenceUndoHandle(object sender, RoutedEventArgs e)
    {
        QChronicle.QChronicleRun(_cShelf.CShelfDraftUndo);
    }

    private void QReferenceRedoHandle(object sender, RoutedEventArgs e)
    {
        QChronicle.QChronicleRun(_cShelf.CShelfDraftRedo);
    }

    private void QReferenceBinHandle(object sender, RoutedEventArgs e)
    {
        _cShelf.CShelfReferenceDelete();
    }

    private void QReferencePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cShelf.CShelfPressAllowed;
    }

    private async void QReferencePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _cShelf.CShelfPortraitPrint();
    }

    private void QReferencePortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cShelf.CShelfPortraitAllowed;
    }

    private async void QReferencePortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _cShelf.CShelfPortraitExport();
    }
}
