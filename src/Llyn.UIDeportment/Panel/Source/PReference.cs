using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public class PReference : UserControl
{
    private readonly ObservableCollection<PShelfItem> _pShelfList = [];

    private readonly ObservableCollection<PFootnoteItem> _pFootnoteList = [];

    private PWindow _pReferenceHost = null!;

    private LShelf _lShelf = null!;

    public PReference()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Source/PReference.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        PReferencePortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        PReferencePress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(PGradeDropper, PGradeDropdown, PGrade);
        QChoice.QChoiceDropperAttach(PTrellisDropper, PTrellisDropdown, PTrellisDropper);

        PGradeIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        PTrellisIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        PReferenceBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        PSurvey.SetResourceReference(QField.QFieldHintProperty, "Source.Search");
        PRummage.SetResourceReference(QField.QFieldHintProperty, "Rummage.Search");
        PReferenceFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        PReferenceStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        PReferenceEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        PReferenceLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        PReferenceBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        PReferenceForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        PReferencePortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        PReferencePress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        PReferenceViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        PReferenceScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        PSurvey.TextChanged += PSurveyHandle;
        PRummage.TextChanged += PRummageHandle;
        PReferenceFresh.Click += PReferenceFreshHandle;
        PReferenceStore.Click += PReferenceStoreHandle;
        PReferenceEarlier.Click += PReferenceRetreatHandle;
        PReferenceLater.Click += PReferenceAdvanceHandle;
        PReferenceBackward.Click += PReferenceUndoHandle;
        PReferenceForward.Click += PReferenceRedoHandle;
        PReferenceViewer.Click += PReferenceScribeHandle;
        PReferenceScribe.Click += PReferenceScribeHandle;
        PReferenceBin.Click += PReferenceBinHandle;

        PShelf.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PShelfHandle));
        PFootnote.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PFootnoteHandle));

        QLookItem.QLookItemAttach(PShelf, PShelfItem.PShelfItemApply);
        QLookItem.QLookItemAttach(PFootnote, PFootnoteItem.PFootnoteItemApply);
    }

    private Border PGrade => (Border)FindName(nameof(PGrade));

    private ToggleButton PGradeDropper => (ToggleButton)FindName(nameof(PGradeDropper));

    private QIconImage PGradeIcon => (QIconImage)FindName(nameof(PGradeIcon));

    private TextBox PSurvey => (TextBox)FindName(nameof(PSurvey));

    private Popup PGradeDropdown => (Popup)FindName(nameof(PGradeDropdown));

    private StackPanel PGradeList => (StackPanel)FindName(nameof(PGradeList));

    private TextBox PRummage => (TextBox)FindName(nameof(PRummage));

    private ToggleButton PTrellisDropper => (ToggleButton)FindName(nameof(PTrellisDropper));

    private QIconImage PTrellisIcon => (QIconImage)FindName(nameof(PTrellisIcon));

    private FrameworkElement PTrellisMark => (FrameworkElement)FindName(nameof(PTrellisMark));

    private Popup PTrellisDropdown => (Popup)FindName(nameof(PTrellisDropdown));

    private StackPanel PTrellisList => (StackPanel)FindName(nameof(PTrellisList));

    private Button PReferenceFresh => (Button)FindName(nameof(PReferenceFresh));

    private Button PReferenceStore => (Button)FindName(nameof(PReferenceStore));

    private StackPanel PReferenceVoyage => (StackPanel)FindName(nameof(PReferenceVoyage));

    private Button PReferenceEarlier => (Button)FindName(nameof(PReferenceEarlier));

    private Button PReferenceLater => (Button)FindName(nameof(PReferenceLater));

    private StackPanel PReferenceChronicle => (StackPanel)FindName(nameof(PReferenceChronicle));

    private Button PReferenceBackward => (Button)FindName(nameof(PReferenceBackward));

    private Button PReferenceForward => (Button)FindName(nameof(PReferenceForward));

    private Button PReferencePortrait => (Button)FindName(nameof(PReferencePortrait));

    private Button PReferencePress => (Button)FindName(nameof(PReferencePress));

    private Border PReferenceMode => (Border)FindName(nameof(PReferenceMode));

    private RadioButton PReferenceViewer => (RadioButton)FindName(nameof(PReferenceViewer));

    private RadioButton PReferenceScribe => (RadioButton)FindName(nameof(PReferenceScribe));

    private ItemsControl PShelf => (ItemsControl)FindName(nameof(PShelf));

    private TextBlock PShelfEmpty => (TextBlock)FindName(nameof(PShelfEmpty));

    private ItemsControl PFootnote => (ItemsControl)FindName(nameof(PFootnote));

    private TextBlock PFootnoteEmpty => (TextBlock)FindName(nameof(PFootnoteEmpty));

    private PDisplay PDisplay => (PDisplay)FindName(nameof(PDisplay));

    private PEditor PEditor => (PEditor)FindName(nameof(PEditor));

    private PColophon PColophon => (PColophon)FindName(nameof(PColophon));

    private PImprint PImprint => (PImprint)FindName(nameof(PImprint));

    private Button PReferenceBin => (Button)FindName(nameof(PReferenceBin));

    private QIconImage PReferenceBinIcon => (QIconImage)FindName(nameof(PReferenceBinIcon));

    internal void PReferenceAttach(PWindow host)
    {
        _pReferenceHost = host;
        LEditor editor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        LLectern lectern = new(editor.LEditorDisplay);
        _lShelf = host.PWindowDeportment.LWindowShelfCreate(
            editor,
            lectern,
            PReferenceShownCheck,
            PReferenceDiscardConfirm,
            PReferenceRemovalConfirm,
            host.PWindowUnreadableConfirm);
        _lShelf.LShelfChanged += PReferenceModeUpdate;
        _lShelf.LShelfPanel.LPanelChanged += PReferenceModeUpdate;
        _lShelf.LShelfPanel.LPanelRowsChanged += PShelfUpdate;
        _lShelf.LShelfPanel.LPanelCleared += PImprint.PImprintClear;
        _lShelf.LShelfPanel.LPanelCleared += PColophon.PColophonClear;
        _lShelf.LShelfColophonChanged += PColophon.PColophonShow;
        _lShelf.LShelfPanel.LPanelFailed += host.PWindowFailureShow;
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelChanged += PReferenceModeUpdate;
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelRowsChanged += PFootnoteUpdate;
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelFailed += host.PWindowFailureShow;

        PShelf.ItemsSource = _pShelfList;
        PFootnote.ItemsSource = _pFootnoteList;

        PImprint.PImprintAttach(host, _lShelf.LShelfImprint);
        PDisplay.PDisplayAttach(host, lectern);
        PEditor.PEditorAttach(host, _lShelf.LShelfEditor, lectern);

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PReferencePressHandle, PReferencePressCheck));
        CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, PReferencePortraitHandle,
            PReferencePortraitCheck));
    }

    internal async void PReferenceVistaRestore()
    {
        _lShelf.LShelfVistaRestore(_pReferenceHost.PWindowDeportment);
        LPanel footnote = _lShelf.LShelfFootnote.LFootnotePanel;
        LPanel shelf = _lShelf.LShelfPanel;
        shelf.LPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(this, shelf.LPanelRowsUpdate));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(this, _lShelf.LShelfClear));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectAuthor,
            LObserver.LObserverCreate<CBulletin>(this, _lShelf.LShelfImprint.LImprintDesk.LDeskDraftUpdate));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectAuthor, LObserver.LObserverCreate<CBulletin>(this, shelf.LPanelRowsUpdate));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectReference, LObserver.LObserverCreate<CBulletin>(this, shelf.LPanelRowsUpdate));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectExample, LObserver.LObserverCreate<CBulletin>(this, shelf.LPanelRowsUpdate));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(this, shelf.LPanelRowsUpdate));
        shelf.LPanelObserverAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(this, shelf.LPanelRowsUpdate));
        footnote.LPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, footnote.LPanelEntryHandle));
        footnote.LPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, shelf.LPanelRowsUpdate));
        footnote.LPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, _lShelf.LShelfEntryUpdate));
        footnote.LPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(this, footnote.LPanelRowsUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(
            PGradeList,
            "Grade",
            PGradeHandle,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderYear,
                CCatalogOrder.CCatalogOrderAuthor,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        QChoice.QChoiceOrderApply(PGradeDropdown, _lShelf.LShelfPanel.LPanelOrder);
        PTrellisUpdate();

        await LEnsignImage.LEnsignLoad(_pReferenceHost.PWindowDeportment);

        QChoice.QChoiceFilterBuild(
            PTrellisList,
            _pReferenceHost.PWindowDeportment.LWindowLanguageRead(),
            _lShelf.LShelfPanel.LPanelFilter,
            PTrellisHandle);
        _lShelf.LShelfQuerySet(PSurvey.Text);
        _lShelf.LShelfFootnote.LFootnoteQuerySet(PRummage.Text);
        _lShelf.LShelfPanel.LPanelRowsUpdate();
    }

    internal bool PReferenceChangeCheck()
    {
        return _lShelf.LShelfChangeCheck();
    }

    internal bool PReferenceDraftFinish(bool store)
    {
        return _lShelf.LShelfDraftFinish(store);
    }

    internal bool PReferenceLeaveConfirm()
    {
        return _lShelf.LShelfLeaveConfirm();
    }

    internal void PReferenceScribeRestore(bool editing)
    {
        _lShelf.LShelfScribeRestore(editing);
    }

    internal void PReferenceClose()
    {
        PImprint.PImprintClose();
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
        PGradeDropdown.IsOpen = false;
        PTrellisDropdown.IsOpen = false;
    }

    private bool PReferenceShownCheck()
    {
        return IsVisible;
    }

    private bool PReferenceDiscardConfirm()
    {
        return _pReferenceHost.PWindowDiscardConfirm(true, PReferenceDraftFinish);
    }

    private bool PReferenceRemovalConfirm(int usage)
    {
        return _pReferenceHost.PWindowRemovalConfirm(usage, "Source");
    }

    private void PShelfUpdate()
    {
        LSplice.LSpliceApply(
            _pShelfList,
            PShelfItem.PShelfItemBuild(_lShelf.LShelfRowsRead()),
            PShelfItem.PShelfItemMatch,
            PShelfItem.PShelfItemSync);
        PShelfEmpty.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfEmpty);
        PColophon.PColophonTallyShow(_lShelf.LShelfTallyRead());
        PImprint.PImprintTallyShow();
    }

    private void PFootnoteUpdate()
    {
        LSplice.LSpliceApply(
            _pFootnoteList,
            PFootnoteItem.PFootnoteItemBuild(_lShelf.LShelfFootnote.LFootnoteRowsRead()),
            PFootnoteItem.PFootnoteItemMatch,
            PFootnoteItem.PFootnoteItemSync);
        PFootnoteEmpty.SetResourceReference(TextBlock.TextProperty, _lShelf.LShelfFootnote.LFootnoteEmptyKey);
        PFootnoteEmpty.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfFootnote.LFootnoteEmpty);
    }

    private void PReferenceModeUpdate()
    {
        PEditor.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfEditorShown);
        PDisplay.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfDisplayShown);
        PImprint.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfImprintShown);
        PColophon.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfColophonShown);
        PReferenceViewer.IsChecked = QLook.QLookCheckedRead(_lShelf.LShelfViewerChecked);
        PReferenceScribe.IsChecked = QLook.QLookCheckedRead(_lShelf.LShelfScribeChecked);
        PReferenceVoyage.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfViewerChecked);
        PReferenceChronicle.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfScribeChecked);
        PReferenceMode.IsEnabled = _lShelf.LShelfModeEnabled;
        PReferenceBin.IsEnabled = _lShelf.LShelfBinEnabled;
        PReferenceStore.IsEnabled = _lShelf.LShelfStoreEnabled;
        PReferenceChronicleUpdate();
    }

    private void PReferenceChronicleUpdate()
    {
        (bool undo, bool redo) = _lShelf.LShelfChronicleRead();
        PReferenceBackward.IsEnabled = undo;
        PReferenceForward.IsEnabled = redo;
    }

    internal void PReferenceVoyageShow(bool past, bool future)
    {
        PReferenceEarlier.IsEnabled = past;
        PReferenceLater.IsEnabled = future;
    }

    private void PReferenceRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pReferenceHost.PVoyageRetreatRun();
    }

    private void PReferenceAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pReferenceHost.PVoyageAdvanceRun();
    }

    private void PTrellisUpdate()
    {
        PTrellisMark.Visibility = QLook.QLookVisibleRead(_lShelf.LShelfSieveActive);
    }

    private void PSurveyHandle(object sender, TextChangedEventArgs e)
    {
        _lShelf.LShelfQuerySet(PSurvey.Text);
    }

    private void PRummageHandle(object sender, TextChangedEventArgs e)
    {
        _lShelf.LShelfFootnote.LFootnoteQuerySet(PRummage.Text);
    }

    private void PTrellisHandle(object sender, RoutedEventArgs e)
    {
        _lShelf.LShelfSieveSet(QChoice.QChoiceFilterRead(PTrellisList));
        PTrellisUpdate();
    }

    private void PGradeHandle(object sender, RoutedEventArgs e)
    {
        PGradeDropper.IsChecked = false;
        _lShelf.LShelfOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    private void PShelfHandle(object sender, RoutedEventArgs e)
    {
        _pReferenceHost.PVoyageRecord();
        _lShelf.LShelfRowSelect(QSender.QSenderSourceRead<PShelfItem>(e)?.PShelfItemId);
    }

    internal long PReferenceVoyageRead()
    {
        return _lShelf.LShelfPanel.LPanelVoyageRead();
    }

    internal void PShelfSourceShow(long id)
    {
        _lShelf.LShelfRowShow(id);
    }

    private void PFootnoteHandle(object sender, RoutedEventArgs e)
    {
        _lShelf.LShelfEntrySelect(QSender.QSenderSourceRead<PFootnoteItem>(e)?.PFootnoteItemId);
    }

    private void PReferenceFreshHandle(object sender, RoutedEventArgs e)
    {
        _lShelf.LShelfFreshStart();
    }

    private void PReferenceScribeHandle(object sender, RoutedEventArgs e)
    {
        _lShelf.LShelfScribeSet(ReferenceEquals(sender, PReferenceScribe));
    }

    private void PReferenceStoreHandle(object sender, RoutedEventArgs e)
    {
        _lShelf.LShelfStoreRun();
    }

    private void PReferenceUndoHandle(object sender, RoutedEventArgs e)
    {
        PChronicle.PChronicleRun(_lShelf.LShelfUndo);
    }

    private void PReferenceRedoHandle(object sender, RoutedEventArgs e)
    {
        PChronicle.PChronicleRun(_lShelf.LShelfRedo);
    }

    private void PReferenceBinHandle(object sender, RoutedEventArgs e)
    {
        _lShelf.LShelfDelete();
    }

    private void PReferencePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lShelf.LShelfPressAllowed;
    }

    private async void PReferencePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pReferenceHost.PWindowPressRun(
            ticket => _lShelf.LShelfPortraitPrint(
                _pReferenceHost.PWindowLabelRead(), _pReferenceHost.PWindowLegendRead("Source"), ticket));
    }

    private void PReferencePortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lShelf.LShelfPortraitAllowed;
    }

    private async void PReferencePortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pReferenceHost.PWindowPortraitExport(_lShelf.LShelfFileRead(), _lShelf.LShelfPortraitExport);
    }
}
