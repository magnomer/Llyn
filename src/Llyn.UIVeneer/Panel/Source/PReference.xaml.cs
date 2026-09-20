using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PReference : UserControl
{
    private readonly ObservableCollection<PShelfItem> _pShelfList = [];

    private readonly ObservableCollection<PFootnoteItem> _pFootnoteList = [];

    private PWindow _pReferenceHost = null!;

    private LShelf _lShelf = null!;

    public PReference()
    {
        InitializeComponent();
    }

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
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelCleared += PDisplay.PDisplayClear;
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelDraftChanged += PReferenceEntryUpdate;
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelFailed += host.PWindowFailureShow;

        PShelf.ItemsSource = _pShelfList;
        PFootnote.ItemsSource = _pFootnoteList;

        PColophon.PColophonAttach(host);
        PImprint.PImprintAttach(host, _lShelf.LShelfImprint);
        PDisplay.PDisplayAttach(host, _lShelf.LShelfEditor.LEditorDisplay);
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
            LSubject.LSubjectVista, PObserver.PObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, _lShelf.LShelfClear));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectAuthor,
            PObserver.PObserverCreate(this, _lShelf.LShelfImprint.LImprintDesk.LDeskDraftUpdate));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectAuthor, PObserver.PObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectReference, PObserver.PObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectExample, PObserver.PObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectReflex, PObserver.PObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfPanel.LPanelObserverAttach(
            LSubject.LSubjectSettings, PObserver.PObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelObserverAttach(
            LSubject.LSubjectEntry,
            PObserver.PObserverCreate(this, _lShelf.LShelfFootnote.LFootnotePanel.LPanelEntryHandle));
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelObserverAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelChosenAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, _lShelf.LShelfEntryUpdate));
        _lShelf.LShelfFootnote.LFootnotePanel.LPanelObserverAttach(
            LSubject.LSubjectVista,
            PObserver.PObserverCreate(this, _lShelf.LShelfFootnote.LFootnotePanel.LPanelRowsUpdate));
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

        await PEnsign.PEnsignLoad(_pReferenceHost.PWindowDeportment);

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
        PSplice.PSpliceApply(
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
        PSplice.PSpliceApply(
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

    private void PReferenceEntryUpdate(LDraft draft)
    {
        PDisplay.PDisplayShow(draft.LDraftContent);
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
        _lShelf.LShelfSieveSet(PChoice.PChoiceFilterRead(PTrellisList));
        PTrellisUpdate();
    }

    private void PGradeHandle(object sender, RoutedEventArgs e)
    {
        PGradeDropper.IsChecked = false;
        _lShelf.LShelfOrderSet(PSender.PSenderOrderRead(sender));
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
        _lShelf.LShelfRowSelect(id);
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
