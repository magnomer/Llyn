using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Core;

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

        PChoice.PChoiceDropperAttach(PGradeDropper, PGradeDropdown, PGrade);
        PChoice.PChoiceDropperAttach(PTrellisDropper, PTrellisDropdown, PTrellisDropper);

        PGradeIcon.PIconSource = PIcon.PIconResolve("sort", 24);
        PTrellisIcon.PIconSource = PIcon.PIconResolve("filter", 24);
        PReferenceBinIcon.PIconSource = PIcon.PIconResolve("delete", 24);
        PReferenceFresh.Tag = PIcon.PIconResolve("new", 24);
        PReferenceStore.Tag = PIcon.PIconResolve("save", 24);
        PReferenceEarlier.Tag = PIcon.PIconResolve("retreat", 24);
        PReferenceLater.Tag = PIcon.PIconResolve("advance", 24);
        PReferenceBackward.Tag = PIcon.PIconResolve("undo", 24);
        PReferenceForward.Tag = PIcon.PIconResolve("redo", 24);
        PReferencePortrait.Tag = PIcon.PIconResolve("export", 24);
        PReferencePress.Tag = PIcon.PIconResolve("print", 24);
        PReferenceViewer.Tag = PIcon.PIconResolve("view", 24);
        PReferenceScribe.Tag = PIcon.PIconResolve("edit", 24);

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

        PLookItem.PLookItemAttach(PShelf, PShelfItem.PShelfItemApply);
        PLookItem.PLookItemAttach(PFootnote, PFootnoteItem.PFootnoteItemApply);
    }

    private Border PGrade => (Border)FindName(nameof(PGrade));

    private ToggleButton PGradeDropper => (ToggleButton)FindName(nameof(PGradeDropper));

    private PIconImage PGradeIcon => (PIconImage)FindName(nameof(PGradeIcon));

    private TextBox PSurvey => (TextBox)FindName(nameof(PSurvey));

    private Popup PGradeDropdown => (Popup)FindName(nameof(PGradeDropdown));

    private StackPanel PGradeList => (StackPanel)FindName(nameof(PGradeList));

    private TextBox PRummage => (TextBox)FindName(nameof(PRummage));

    private ToggleButton PTrellisDropper => (ToggleButton)FindName(nameof(PTrellisDropper));

    private PIconImage PTrellisIcon => (PIconImage)FindName(nameof(PTrellisIcon));

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

    private PIconImage PReferenceBinIcon => (PIconImage)FindName(nameof(PReferenceBinIcon));

    internal void PReferenceAttach(PWindow host)
    {
        _pReferenceHost = host;
        _lShelf = host.PWindowDeportment.LWindowShelfCreate(
            host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm),
            PReferenceShownCheck,
            PReferenceDiscardConfirm,
            PReferenceRemovalConfirm,
            host.PWindowUnreadableConfirm);
        _lShelf.LShelfChanged += PReferenceModeUpdate;
        _lShelf.LShelfPanel.LPanelChanged += PReferenceModeUpdate;
        _lShelf.LShelfPanel.LPanelRowsChanged += PShelfUpdate;
        _lShelf.LShelfPanel.LPanelCleared += PImprint.PImprintClear;
        _lShelf.LShelfPanel.LPanelCleared += PColophon.PColophonClear;
        _lShelf.LShelfPanel.LPanelDraftChanged += PReferenceSourceUpdate;
        _lShelf.LShelfPanel.LPanelFailed += host.PWindowFailureShow;
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelChanged += PReferenceModeUpdate;
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelRowsChanged += PFootnoteUpdate;
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelFailed += host.PWindowFailureShow;

        PShelf.ItemsSource = _pShelfList;
        PFootnote.ItemsSource = _pFootnoteList;

        PImprint.PImprintAttach(host, _lShelf.LShelfImprint);
        PDisplay.PDisplayAttach(host, _lShelf.LShelfEditor.LEditorLectern);
        PEditor.PEditorAttach(host, _lShelf.LShelfEditor);

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PReferencePressHandle, PReferencePressCheck));
        CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, PReferencePortraitHandle,
            PReferencePortraitCheck));
    }

    internal async void PReferenceVistaRestore()
    {
        _lShelf.LShelfVistaRestore(_pReferenceHost.PWindowDeportment);
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectVista, LObserver.LObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectWorkspace, LObserver.LObserverCreate(this, _lShelf.LShelfClear));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectAuthor,
            LObserver.LObserverCreate(this, _lShelf.LShelfImprint.LImprintDesk.LDeskDraftUpdate));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectAuthor, LObserver.LObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectReference, LObserver.LObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectExample, LObserver.LObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectReflex, LObserver.LObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectSettings, LObserver.LObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelObserverAttach(
            LSubject.LSubjectEntry,
            LObserver.LObserverCreate(this, _lShelf.LShelfFootnote.LFootnotePanel.LPanelEntryHandle));
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelObserverAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelChosenAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, _lShelf.LShelfEntryUpdate));
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelObserverAttach(
            LSubject.LSubjectVista,
            LObserver.LObserverCreate(this, _lShelf.LShelfFootnote.LFootnotePanel.LPanelRowsUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderBuild(
            PGradeList,
            "Grade",
            PGradeHandle,
            [
                LCatalogOrder.LCatalogOrderName,
                LCatalogOrder.LCatalogOrderYear,
                LCatalogOrder.LCatalogOrderAuthor,
                LCatalogOrder.LCatalogOrderUsage,
            ]);
        PChoice.PChoiceOrderApply(PGradeDropdown, _lShelf.LShelfPanel.LPanelOrder);
        PTrellisUpdate();

        await LEnsignImage.LEnsignLoad(_pReferenceHost.PWindowDeportment);

        PChoice.PChoiceFilterBuild(
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
        PShelfEmpty.Visibility = PLook.PLookVisibleRead(_lShelf.LShelfEmpty);
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
        PFootnoteEmpty.Visibility = PLook.PLookVisibleRead(_lShelf.LShelfFootnote.LFootnoteEmpty);
    }

    private void PReferenceModeUpdate()
    {
        PEditor.Visibility = PLook.PLookVisibleRead(_lShelf.LShelfEditorShown);
        PDisplay.Visibility = PLook.PLookVisibleRead(_lShelf.LShelfDisplayShown);
        PImprint.Visibility = PLook.PLookVisibleRead(_lShelf.LShelfImprintShown);
        PColophon.Visibility = PLook.PLookVisibleRead(_lShelf.LShelfColophonShown);
        PReferenceViewer.IsChecked = PLook.PLookCheckedRead(_lShelf.LShelfViewerChecked);
        PReferenceScribe.IsChecked = PLook.PLookCheckedRead(_lShelf.LShelfScribeChecked);
        PReferenceVoyage.Visibility = PLook.PLookVisibleRead(_lShelf.LShelfViewerChecked);
        PReferenceChronicle.Visibility = PLook.PLookVisibleRead(_lShelf.LShelfScribeChecked);
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

    private void PReferenceSourceUpdate(LDraft draft)
    {
        PColophon.PColophonShow(_lShelf.LShelfColophonRead(draft));
    }

    private void PTrellisUpdate()
    {
        PTrellisMark.Visibility = PLook.PLookVisibleRead(_lShelf.LShelfSieveActive);
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
        _lShelf.LShelfSieveSet(LChoice.LChoiceFilterRead(PTrellisList));
        PTrellisUpdate();
    }

    private void PGradeHandle(object sender, RoutedEventArgs e)
    {
        PGradeDropper.IsChecked = false;
        _lShelf.LShelfOrderSet(LChoice.LChoiceOrderRead(sender));
    }

    private void PShelfHandle(object sender, RoutedEventArgs e)
    {
        _pReferenceHost.PVoyageRecord();
        _lShelf.LShelfRowSelect(PSender.PSenderSourceRead<PShelfItem>(e)?.PShelfItemId);
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
        _lShelf.LShelfEntrySelect(PSender.PSenderSourceRead<PFootnoteItem>(e)?.PFootnoteItemId);
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
