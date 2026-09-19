using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PReference : UserControl
{
    private readonly ObservableCollection<PShelfItem> _pShelfList = [];

    private readonly ObservableCollection<PFootnoteItem> _pFootnoteList = [];

    private PWindow _pReferenceHost = null!;

    private LEngine _lEngine = null!;

    private LShelf _lShelf = null!;

    public PReference()
    {
        InitializeComponent();
    }

    internal void PReferenceAttach(PWindow host, LEngine engine)
    {
        _pReferenceHost = host;
        _lEngine = engine;
        _lShelf = new LShelf(
            engine,
            new LEditor(engine, host.PWindowUnreadableConfirm),
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
        PDisplay.PDisplayAttach(host, engine);
        PEditor.PEditorAttach(host, engine, _lShelf.LShelfEditor);
    }

    internal async void PReferenceVistaRestore(LVista vista, LVista footnote)
    {
        _lShelf.LShelfVistaRestore(vista, footnote);
        vista.LVistaObserverAttach(
            LSubject.LSubjectVista, new PObserver(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, _lShelf.LShelfClear));
        vista.LVistaObserverAttach(
            LSubject.LSubjectAuthor, new PObserver(this, _lShelf.LShelfImprint.LImprintDesk.LDeskDraftUpdate));
        vista.LVistaObserverAttach(
            LSubject.LSubjectAuthor, new PObserver(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        vista.LVistaObserverAttach(
            LSubject.LSubjectReference, new PObserver(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        vista.LVistaObserverAttach(
            LSubject.LSubjectExample, new PObserver(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        vista.LVistaObserverAttach(
            LSubject.LSubjectReflex, new PObserver(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        vista.LVistaObserverAttach(
            LSubject.LSubjectSettings, new PObserver(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        footnote.LVistaObserverAttach(
            LSubject.LSubjectEntry, new PObserver(this, _lShelf.LShelfFootnote.LFootnotePanel.LPanelEntryHandle));
        footnote.LVistaObserverAttach(
            LSubject.LSubjectEntry, new PObserver(this, _lShelf.LShelfPanel.LPanelRowsUpdate));
        footnote.LVistaChosenAttach(LSubject.LSubjectEntry, new PObserver(this, _lShelf.LShelfEntryUpdate));
        footnote.LVistaObserverAttach(
            LSubject.LSubjectVista, new PObserver(this, _lShelf.LShelfFootnote.LFootnotePanel.LPanelRowsUpdate));
        PDisplay.PDisplayVistaRestore(footnote);
        PEditor.PEditorVistaRestore(footnote);
        PChoice.PChoiceOrderApply(PGradeDropdown, vista.LVistaOrder);
        PTrellisUpdate();

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PTrellisList, _lEngine.LEngineLanguageRead(), vista.LVistaFilter, PTrellisHandle);
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
        _lShelf.LShelfOrderSet(PSender.PSenderTagRead(sender));
    }

    private void PShelfHandle(object sender, RoutedEventArgs e)
    {
        _lShelf.LShelfRowSelect(PSender.PSenderSourceRead<PShelfItem>(e)?.PShelfItemId);
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
        await _pReferenceHost.PWindowPortraitExport(_lShelf.LShelfFootnote.LFootnotePanel.LPanelVista);
    }
}
