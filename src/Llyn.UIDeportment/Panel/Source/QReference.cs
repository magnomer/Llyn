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

    private LShelf _lShelf = null!;

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
        LEditor editor = host.PWindowDeportment.LWindowForge.QForgeEditorCreate(host.PWindowUnreadableConfirm);
        LLectern lectern = new(editor.LEditorDisplay);
        _lShelf = host.PWindowDeportment.LWindowForge.QForgeShelfCreate(
            editor,
            lectern,
            QReferenceShownCheck,
            QReferenceDiscardConfirm,
            QReferenceRemovalConfirm,
            host.PWindowUnreadableConfirm);
        _lShelf.LShelfChanged += QReferenceModeUpdate;
        _lShelf.LShelfPanel.LPanelChanged += QReferenceModeUpdate;
        _lShelf.LShelfPanel.LPanelRowsChanged += QShelfUpdate;
        _lShelf.LShelfPanel.LPanelCleared += _qImprint.QImprintClear;
        _lShelf.LShelfPanel.LPanelCleared += _qColophon.QColophonClear;
        _lShelf.LShelfColophonChanged += _qColophon.QColophonShow;
        _lShelf.LShelfPanel.LPanelFailed += host.PWindowFailureShow;
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelChanged += QReferenceModeUpdate;
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelRowsChanged += QFootnoteUpdate;
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelFailed += host.PWindowFailureShow;

        QShelf.ItemsSource = _qShelfList;
        QFootnote.ItemsSource = _qFootnoteList;

        _qImprint.QImprintAttach(host, _lShelf.LShelfImprint);
        QReferenceDisplay.PDisplayAttach(host, lectern);
        QReferenceEditor.PEditorAttach(host, _lShelf.LShelfEditor, lectern);

        _qReferenceSurface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QReferencePressHandle, QReferencePressCheck));
        _qReferenceSurface.CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, QReferencePortraitHandle,
            QReferencePortraitCheck));
    }

    internal async void QReferenceVistaRestore()
    {
        LPanel footnote = _lShelf.LShelfFootnote.LFootnotePanel;
        LPanel shelf = _lShelf.LShelfPanel;
        shelf.LPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.LPanelRowsUpdate));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, _lShelf.LShelfClear));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectAuthor,
            LObserver.LObserverCreate<CBulletin>(
                _qReferenceSurface, _lShelf.LShelfImprint.LImprintDesk.LDeskDraftUpdate));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectAuthor, LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.LPanelRowsUpdate));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectReference,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.LPanelRowsUpdate));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectExample, LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.LPanelRowsUpdate));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.LPanelRowsUpdate));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectSettings,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.LPanelRowsUpdate));
        footnote.LPanelObserverAttach(
            CSubject.CSubjectEntry,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, footnote.LPanelEntryHandle));
        footnote.LPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, shelf.LPanelRowsUpdate));
        footnote.LPanelChosenAttach(
            CSubject.CSubjectEntry,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, _lShelf.LShelfEntryUpdate));
        footnote.LPanelObserverAttach(
            CSubject.CSubjectVista,
            LObserver.LObserverCreate<CBulletin>(_qReferenceSurface, footnote.LPanelRowsUpdate));
        QReferenceDisplay.PDisplayObserverAttach();
        QReferenceEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(QGradeList, "Grade", QGradeHandle, LShelf.LShelfOrderRead());
        QChoice.QChoiceOrderApply(QGradeDropdown, _lShelf.LShelfPanel.LPanelOrder);
        QTrellisUpdate();

        await LEnsignImage.LEnsignLoad(_qReferenceHost.PWindowDeportment);

        QChoice.QChoiceFilterBuild(
            QTrellisList,
            _qReferenceHost.PWindowDeportment.LWindowWorkspace.QWorkspaceLanguageRead(),
            _lShelf.LShelfPanel.LPanelFilter,
            QTrellisHandle);
        _lShelf.LShelfQuerySet(QSurvey.Text);
        _lShelf.LShelfFootnote.LFootnoteQuerySet(QRummage.Text);
        _lShelf.LShelfPanel.LPanelRowsUpdate();
    }

    internal bool QReferenceChangeCheck()
    {
        return _lShelf.LShelfChangeCheck();
    }

    internal bool QReferenceDraftFinish(bool store)
    {
        return _lShelf.LShelfDraftFinish(store);
    }

    internal bool QReferenceLeaveConfirm()
    {
        return _lShelf.LShelfLeaveConfirm();
    }

    internal void QReferenceScribeRestore(bool editing)
    {
        _lShelf.LShelfScribeRestore(editing);
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

    private bool QReferenceDiscardConfirm()
    {
        return _qReferenceHost.PWindowDiscardConfirm(true, QReferenceDraftFinish);
    }

    private bool QReferenceRemovalConfirm(int usage)
    {
        return _qReferenceHost.PWindowRemovalConfirm(usage, "Source");
    }

    private void QShelfUpdate()
    {
        LSplice.LSpliceApply(
            _qShelfList,
            QShelfItem.QShelfItemBuild(_lShelf.LShelfRowsRead()),
            QShelfItem.QShelfItemMatch,
            QShelfItem.QShelfItemSync);
        QShelfEmpty.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfEmpty);
        _qColophon.QColophonTallyShow(_lShelf.LShelfTallyRead());
        _qImprint.QImprintTallyShow();
    }

    private void QFootnoteUpdate()
    {
        LSplice.LSpliceApply(
            _qFootnoteList,
            QFootnoteItem.QFootnoteItemBuild(_lShelf.LShelfFootnote.LFootnoteRowsRead()),
            QFootnoteItem.QFootnoteItemMatch,
            QFootnoteItem.QFootnoteItemSync);
        QFootnoteEmpty.SetResourceReference(TextBlock.TextProperty, _lShelf.LShelfFootnote.LFootnoteEmptyKey);
        QFootnoteEmpty.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfFootnote.LFootnoteEmpty);
    }

    private void QReferenceModeUpdate()
    {
        QReferenceEditor.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfEditorShown);
        QReferenceDisplay.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfDisplayShown);
        QReferenceImprint.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfImprintShown);
        QReferenceColophon.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfColophonShown);
        QReferenceViewer.IsChecked = QLook.QLookCheckedRead(_lShelf.LShelfViewerChecked);
        QReferenceScribe.IsChecked = QLook.QLookCheckedRead(_lShelf.LShelfScribeChecked);
        QReferenceVoyage.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfViewerChecked);
        QReferenceChronicle.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfScribeChecked);
        QReferenceMode.IsEnabled = _lShelf.LShelfModeEnabled;
        QReferenceBin.IsEnabled = _lShelf.LShelfBinEnabled;
        QReferenceStore.IsEnabled = _lShelf.LShelfStoreEnabled;
        QReferenceChronicleUpdate();
    }

    private void QReferenceChronicleUpdate()
    {
        (bool undo, bool redo) = _lShelf.LShelfChronicleRead();
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
        _qReferenceHost.PVoyageRetreatRun();
    }

    private void QReferenceAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _qReferenceHost.PVoyageAdvanceRun();
    }

    private void QTrellisUpdate()
    {
        QTrellisMark.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfSieveActive);
    }

    private void QSurveyHandle(object sender, TextChangedEventArgs e)
    {
        _lShelf.LShelfQuerySet(QSurvey.Text);
    }

    private void QRummageHandle(object sender, TextChangedEventArgs e)
    {
        _lShelf.LShelfFootnote.LFootnoteQuerySet(QRummage.Text);
    }

    private void QTrellisHandle(object sender, RoutedEventArgs e)
    {
        _lShelf.LShelfSieveSet(QChoice.QChoiceFilterRead(sender));
        QTrellisUpdate();
    }

    private void QGradeHandle(object sender, RoutedEventArgs e)
    {
        QGradeDropper.IsChecked = false;
        _lShelf.LShelfOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    private void QShelfHandle(object sender, RoutedEventArgs e)
    {
        _qReferenceHost.PVoyageRecord();
        _lShelf.LShelfRowSelect(QSender.QSenderSourceRead<QShelfItem>(e)?.QShelfItemId);
    }

    internal long QReferenceVoyageRead()
    {
        return _lShelf.LShelfPanel.LPanelVoyageRead();
    }

    internal void QShelfSourceShow(long id)
    {
        _lShelf.LShelfRowShow(id);
    }

    private void QFootnoteHandle(object sender, RoutedEventArgs e)
    {
        _lShelf.LShelfEntrySelect(QSender.QSenderSourceRead<QFootnoteItem>(e)?.QFootnoteItemId);
    }

    private void QReferenceFreshHandle(object sender, RoutedEventArgs e)
    {
        _lShelf.LShelfFreshStart();
    }

    private void QReferenceScribeHandle(object sender, RoutedEventArgs e)
    {
        _lShelf.LShelfScribeSet(ReferenceEquals(sender, QReferenceScribe));
    }

    private void QReferenceStoreHandle(object sender, RoutedEventArgs e)
    {
        _lShelf.LShelfStoreRun();
    }

    private void QReferenceUndoHandle(object sender, RoutedEventArgs e)
    {
        QChronicle.QChronicleRun(_lShelf.LShelfUndo);
    }

    private void QReferenceRedoHandle(object sender, RoutedEventArgs e)
    {
        QChronicle.QChronicleRun(_lShelf.LShelfRedo);
    }

    private void QReferenceBinHandle(object sender, RoutedEventArgs e)
    {
        _lShelf.LShelfDelete();
    }

    private void QReferencePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lShelf.LShelfPressAllowed;
    }

    private async void QReferencePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _qReferenceHost.PWindowPressRun("Source", _lShelf.LShelfPortraitPrint);
    }

    private void QReferencePortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lShelf.LShelfPortraitAllowed;
    }

    private async void QReferencePortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _qReferenceHost.PWindowPortraitExport(_lShelf.LShelfFileRead(), _lShelf.LShelfPortraitExport);
    }
}
