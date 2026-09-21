using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PEditor : UserControl, PImageHost, PVideoHost, PChronicleHost
{
    private readonly ObservableCollection<PLanguageItem> _pLanguageItem = [];

    private PWindow _pEditorHost = null!;

    private LEditor _lEditor = null!;

    public PEditor()
    {
        InitializeComponent();

        Resources.MergedDictionaries.Add(new PSentenceTemplate(this));
        Resources.MergedDictionaries.Add(new PContextTemplate(this));
        Resources.MergedDictionaries.Add(new PRegisterTemplate(this));
        Resources.MergedDictionaries.Add(new PImageTemplate(this));
        Resources.MergedDictionaries.Add(new PVideoTemplate(this));
        Resources.MergedDictionaries.Add(new PLabelTemplate(this));
        Resources.MergedDictionaries.Add(new PLinkTemplate(this));
        Resources.MergedDictionaries.Add(new PProspectTemplate(this));
        Resources.MergedDictionaries.Add(new PCandidateTemplate(this));
        Resources.MergedDictionaries.Add(new PSlateTemplate(this));
        Resources.MergedDictionaries.Add(new PMeaningTemplate(this));
        Resources.MergedDictionaries.Add(new PCollocationTemplate(this));
        Resources.MergedDictionaries.Add(new PLanguageTemplate(this));
        Resources.MergedDictionaries.Add(new PMarkerTemplate(this));
        Resources.MergedDictionaries.Add(new PCategoryTemplate(this));
        Resources.MergedDictionaries.Add(new PNotationTemplate(this));
        Resources.MergedDictionaries.Add(new PClipTemplate(this));

        PMeaningList.ItemsSource = _pMeaningList;
        PCollocationList.ItemsSource = _pCollocationList;
        PNotationList.ItemsSource = _pNotationItem;
        PAccent.ItemsSource = _pAccentItem;
        PField.PFieldCellAttach(PAccent);
        PAccentControl.PAccentControlAttach(PAccent);
        PTranscription.ItemsSource = _pTranscriptionItem;
        PGlyph.ItemsSource = _pGlyphItem;
        PReflex.ItemsSource = _pReflexItem;
        PField.PFieldCellAttach(PReflex);
        PAccentControl.PAccentControlAttach(PReflex);
        PClipList.ItemsSource = _pClipItem;
        PLanguageList.ItemsSource = _pLanguageItem;
        PProspectList.ItemsSource = _pProspectItem;
        PCandidateList.ItemsSource = _pCandidateItem;
        PCandidate.CustomPopupPlacementCallback = PCandidatePlace;
        PSlateList.ItemsSource = _pSlateItem;
        PSlate.CustomPopupPlacementCallback = PSlatePlace;
        PCategoryList.ItemsSource = _pCategoryItem;
        PMarkerList.ItemsSource = _pMarkerChip;
        PMarkerField.KeyDown += PMarkerFieldHandle;
        PMarkerField.TextChanged += PMarkerTextHandle;
        AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(PEditorTextHandle));
        PHeadword.TextChanged += PHeadwordHandle;
        PPronunciationField.TextChanged += PPronunciationHandle;
        PNoteContents.TextChanged += PNoteHandle;
        AddHandler(LostFocusEvent, new RoutedEventHandler(PEditorFocusHandle));
        PVolumeAttach();
        _pDownloaderPlayer.MediaEnded += PClipEndHandle;
        _pDownloaderPlayer.MediaFailed += PClipEndHandle;
    }

    private long PEditorDraft => _lEditor.LEditorDesk.LDeskId;

    internal void PEditorAttach(PWindow host, LEditor editor)
    {
        _pEditorHost = host;
        _lEditor = editor;
        _lEditor.LEditorDesk.LDeskStarted += PEditorStartUpdate;
        PEditorObserverAttach(_lEditor.LEditorDesk);
        _lEditor.LEditorDesk.LDeskDraftChanged += PEditorDraftUpdate;
        _lEditor.LEditorDesk.LDeskFailed += host.PWindowFailureShow;
        _lEditor.LEditorStateChanged += PEditorStateUpdate;
        _lEditor.LEditorStopped += PEditorStopUpdate;
        _lEditor.LEditorFavoriteChanged += PEditorFavoriteUpdate;
        _lEditor.LEditorGraspChanged += PEditorGraspUpdate;
        PEditorGrasp.PGraspLimit = _lEditor.LEditorGraspStep;
        _lEditor.LEditorSounding.LSoundingChanged += PEditorFanqieUpdate;
        _lEditor.LEditorFailed += host.PWindowFailureShow;
        _lEditor.LEditorClip.LClipSourceStarted += PClipSourceHandle;
        _lEditor.LEditorClip.LClipRecordingAdded += PClipRecordingHandle;
        _lEditor.LEditorClip.LClipFinished += PClipFinishHandle;
        _lEditor.LEditorNotation.LNotationSourceStarted += PNotationSourceHandle;
        _lEditor.LEditorNotation.LNotationCandidateAdded += PNotationCandidateHandle;
        _lEditor.LEditorNotation.LNotationFinished += PNotationFinishHandle;

        PSentenceLoad();
        PSpeakerLoad();
        PCategoryLoad();
        PVolumeLoad();
    }

    internal void PEditorVistaRestore()
    {
        PEditorCommand.Visibility = PLook.PLookVisibleRead(_lEditor.LEditorOwned);
        _lEditor.LEditorOpen(null);
    }

    internal void PEditorClose()
    {
        _lEditor.LEditorClose();

        PNotationCancel();
        PClipCancel();
        _pDownloaderPlayer.Close();
    }

    internal void PEditorEntryShow(long id)
    {
        _lEditor.LEditorOpen(id);
    }

    internal void PEditorReset()
    {
        _lEditor.LEditorOpen(null);
    }

    internal void PEditorEntrySave()
    {
        _lEditor.LEditorSave();
    }

    internal bool PEditorDraftFinish(bool store)
    {
        return _lEditor.LEditorFinish(store);
    }

    internal bool PEditorChangeCheck()
    {
        return _lEditor.LEditorDesk.LDeskChangeCheck();
    }

    internal event Action? PEditorChronicleChanged;

    internal (bool PEditorPast, bool PEditorFuture) PEditorChronicleRead()
    {
        return _lEditor.LEditorDesk.LDeskChronicleRead();
    }

    public void PChronicleUndo()
    {
        PChronicle.PChronicleRun(_lEditor.LEditorDesk.LDeskUndo);
    }

    public void PChronicleRedo()
    {
        PChronicle.PChronicleRun(_lEditor.LEditorDesk.LDeskRedo);
    }

    public void PChronicleUpdate()
    {
        _lEditor.LEditorDesk.LDeskStateUpdate();
    }

    private void PEditorRequestDefer(LRequest request)
    {
        _lEditor.LEditorDesk.LDeskDefer(request);
    }

    private void PEditorRequestSend(LRequest request)
    {
        _lEditor.LEditorDesk.LDeskSend(request);
    }

    private void PEditorSpeechSend()
    {
        PEditorRequestSend(new LRequestSpeech(PEditorDraft, PMarkerRead()));
    }

    private void PEditorObserverAttach(LDesk desk)
    {
        desk.LDeskEntryAttach(LSubject.LSubjectFrequency, PObserver.PObserverCreate(this, PEditorFrequencyUpdate));
        desk.LDeskEntryAttach(LSubject.LSubjectGrasp, PObserver.PObserverCreate(this, PEditorGraspUpdate));
        desk.LDeskEntryAttach(LSubject.LSubjectInflection, PObserver.PObserverCreate(this, PEditorParadigmUpdate));
        desk.LDeskEntryAttach(LSubject.LSubjectReflex, PObserver.PObserverCreate(this, PReflexPendingShow));
        desk.LDeskObserverAttach(LSubject.LSubjectScript, PObserver.PObserverCreate(this, PEditorScriptUpdate));
        desk.LDeskObserverAttach(LSubject.LSubjectFanqie, PObserver.PObserverCreate(this, PEditorFanqieUpdate));
        desk.LDeskObserverAttach(LSubject.LSubjectReference, PObserver.PObserverCreate(this, PSentenceLoad));
        desk.LDeskObserverAttach(LSubject.LSubjectSettings, PObserver.PObserverCreate(this, desk.LDeskDraftUpdate));
        desk.LDeskDraftAttach(LSubject.LSubjectTenure, PObserver.PObserverCreate(this, desk.LDeskStateUpdate));
        desk.LDeskDraftAttach(LSubject.LSubjectDraft, PObserver.PObserverCreate(this, desk.LDeskDraftUpdate));
    }

    private void PEditorStartUpdate()
    {
        _pMeaningList.Clear();
        _pCollocationList.Clear();
        PEditorFavoriteUpdate();
        PEditorGraspUpdate();
        PEditorFrequencyUpdate();
        PEditorParadigmUpdate();
        PEditorScriptUpdate();
        PEditorFanqieUpdate();
    }

    private void PEditorDraftUpdate(LDraft held)
    {
        PEditorDraftShow(held.LDraftContent);
    }

    private void PEditorDraftShow(LEntryDraft draft)
    {
        PField.PFieldTextShow(PHeadword, draft.LEntryDraftHeadword);
        PPronunciationOpener.Text = PLook.PLookFirstRead(_lEditor.LEditorPhonemic, "/", "[");
        PPronunciationCloser.Text = PLook.PLookFirstRead(_lEditor.LEditorPhonemic, "/", "]");
        PField.PFieldTextShow(PPronunciationField, _lEditor.LEditorPronunciationRead());
        PAccentShow(draft);
        PGlyphShow(draft);
        PTranscriptionShow(draft);
        PReflexShow(draft);
        PMarkerShow(draft.LEntryDraftSpeeches);
        PEditorLanguageUpdate();

        IReadOnlyDictionary<long, LTranslationTarget> targets = _lEditor.LEditorTargetRead();
        PCardShow(_pMeaningList, "Meaning", draft.LEntryDraftMeanings, targets, draft.LEntryDraftLanguage);
        PCardShow(_pCollocationList, "Collocation", draft.LEntryDraftCollocations, targets, draft.LEntryDraftLanguage);

        PField.PFieldNoteShow(PNoteContents, draft.LEntryDraftNote);
        PEditorRecordingShow(draft);
        PPlaybackTrayShow();
        PReflexPrepare(draft);
    }

    private void PEditorLanguageUpdate()
    {
        PSpeakerName.Text = _lEditor.LEditorLanguage;
        PSpeakerFlagUpdate();
        PFont.PFontApply(
            _pEditorHost.PWindowDeportment, _lEditor.LEditorLanguage, PHeadword, PHeadwordHint, PHeadwordGhost);
        PFont.PFontPlace(PHeadword, PHeadwordHint, PHeadwordGhost);
        PFont.PFontExampleApply(Resources, _pEditorHost.PWindowDeportment, _lEditor.LEditorLanguage);
        PContour.PContourTonal = _lEditor.LEditorTonal;
        PPronunciation.Visibility = PLook.PLookVisibleRead(_lEditor.LEditorSpoken);
        PAccent.Visibility = PLook.PLookVisibleRead(_lEditor.LEditorSpoken);
        PSentenceFrameLoad(_lEditor.LEditorLanguage);
        PCategoryLoad();
    }

    internal async void PSpeakerLoad()
    {
        await PEnsign.PEnsignLoad(_pEditorHost.PWindowDeportment);
        PLanguageItem.PLanguageItemReset(_pLanguageItem, _pEditorHost.PWindowDeportment.LWindowLanguageRead());
        PEditorLanguageUpdate();
        PLinkFlagUpdate();
    }

    internal void PSpeakerHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorLanguageSet(PLanguageItem.PLanguageNameRead(sender));
        PSpeaker.IsChecked = false;
    }

    private async void PSpeakerFlagUpdate()
    {
        await PEnsign.PEnsignLoad(_pEditorHost.PWindowDeportment);
        PEnsign.PEnsignFlagShow(PSpeakerFlag, PSpeakerGlobe, _lEditor.LEditorLanguage);
    }

    private void PHeadwordHandle(object sender, TextChangedEventArgs e)
    {
        _lEditor.LEditorHeadwordSet(PHeadword.Text);
    }

    private void PPronunciationHandle(object sender, TextChangedEventArgs e)
    {
        _lEditor.LEditorPronunciationSet(PPronunciationField.Text);
    }

    private void PNoteHandle(object sender, TextChangedEventArgs e)
    {
        _lEditor.LEditorNoteSet(PNoteContents.Text);
    }

    private void PEditorTextHandle(object sender, TextChangedEventArgs e)
    {
        if (e.OriginalSource is TextBox { DataContext: PCard or PSentence or PGloss or PImage or PVideo } box)
        {
            PEditorFieldHandle(box);
        }
    }

    private void PEditorFocusHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorPersist();
    }

    private void PEditorStateUpdate()
    {
        IsEnabled = _lEditor.LEditorRunning;
        PEditorDiscard.IsEnabled = _lEditor.LEditorChanged;
        PEditorStore.IsEnabled = _lEditor.LEditorStorable;
        (bool undo, bool redo) = _lEditor.LEditorDesk.LDeskChronicleRead();
        PEditorBackward.IsEnabled = undo;
        PEditorForward.IsEnabled = redo;
        PEditorChronicleChanged?.Invoke();
    }

    private void PEditorStopUpdate()
    {
        _pEditorHost.PWindowFailureShow("Input.HoldFailed");
    }

    private void PEditorUndoHandle(object sender, RoutedEventArgs e)
    {
        PChronicleUndo();
    }

    private void PEditorRedoHandle(object sender, RoutedEventArgs e)
    {
        PChronicleRedo();
    }

    private void PEditorStoreHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorSave();
    }

    private void PEditorDiscardHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorReset();
    }

    private void PEditorFavoriteUpdate()
    {
        PEditorFavorite.IsEnabled = _lEditor.LEditorDesk.LDeskStored;
        PEditorFavorite.IsChecked = _lEditor.LEditorFavorite;
    }

    private void PEditorFavoriteHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorFavoriteSet(PLook.PLookCheckedRead(PEditorFavorite.IsChecked));
    }

    private void PEditorGraspUpdate()
    {
        PEditorGrasp.IsEnabled = _lEditor.LEditorDesk.LDeskStored;
        PEditorGrasp.PGraspStep = _lEditor.LEditorGrasp;
        PEditorGraspLabel.Text = _lEditor.LEditorGraspFormat(PEditorGrasp.PGraspStep);
    }

    private void PEditorHoverHandle(object sender, RoutedEventArgs e)
    {
        PEditorGraspLabel.Text = _lEditor.LEditorGraspFormat(PEditorGrasp.PGraspPointed);
    }

    private void PEditorGraspHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorGraspSet(PEditorGrasp.PGraspStep);
    }

    private void PEditorFrequencyUpdate()
    {
        PFrequencyLabel.PFrequencyChipShow(
            PEditorFrequencySection,
            PEditorFrequencyChip,
            PEditorFrequency,
            PEditorFrequencyBand,
            _lEditor.LEditorFrequencyRead());
    }

    private void PEditorParadigmUpdate()
    {
        PFont.PFontApply(_pEditorHost.PWindowDeportment, _lEditor.LEditorParadigmLanguage, PEditorParadigm);
        PEditorParadigm.PParadigmItems = PParadigmItem.PParadigmItemScan(
            _lEditor.LEditorParadigmRead(), _lEditor.LEditorParadigmPending, _lEditor.LEditorMorphology, true);
    }

    private void PEditorScriptUpdate()
    {
        PFont.PFontApply(
            _pEditorHost.PWindowDeportment, _lEditor.LEditorLanguage, LFontRole.LFontRoleGlyph, PEditorScript);
        PEditorScript.PScriptItems = PScriptItem.PScriptItemScan(_lEditor.LEditorScriptRead());
        PEditorScript.PScriptPending = _lEditor.LEditorScriptPending;
    }

    private void PEditorFanqieUpdate()
    {
        PReflexAnchorShow();
        PFont.PFontApply(
            _pEditorHost.PWindowDeportment, _lEditor.LEditorLanguage, LFontRole.LFontRoleGlyph, PEditorFanqie);
        PEditorFanqie.PFanqieItems = PFanqieItem.PFanqieItemScan(_lEditor.LEditorFanqieRead());
        PEditorFanqie.PFanqiePending = _lEditor.LEditorFanqiePending;
        PEditorFanqie.PFanqieRebuildNotice =
            PLook.PLookFirstRead<Action?>(_lEditor.LEditorRebuildable, _lEditor.LEditorFanqieRebuild, null);
        PEditorFanqie.PFanqieDiweiNotice =
            (kind, key) => _pEditorHost.PWindowDiweiShow(_lEditor.LEditorLanguage, kind, key);
        PEditorFanqie.PFanqieRepresentativeNotice =
            (fanqieId, rank) => _lEditor.LEditorFanqieSet(fanqieId, rank);
        PEditorReading.Text = _lEditor.LEditorReadingRead(PHeadword.Text);
    }
}
