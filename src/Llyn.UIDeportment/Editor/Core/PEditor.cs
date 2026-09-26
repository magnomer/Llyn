using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public partial class PEditor : UserControl, PImageHost, PVideoHost, PChronicleHost
{
    private PWindow _pEditorHost = null!;

    private LEditor _lEditor = null!;

    public PEditor()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Core/PEditor.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        _pSentenceTemplate = new PSentenceTemplate(this);
        Resources.MergedDictionaries.Add(_pSentenceTemplate);
        _pContextTemplate = new PContextTemplate(this);
        Resources.MergedDictionaries.Add(_pContextTemplate);
        _pRegisterTemplate = new PRegisterTemplate(this);
        Resources.MergedDictionaries.Add(_pRegisterTemplate);
        _pImageTemplate = new PImageTemplate(this);
        Resources.MergedDictionaries.Add(_pImageTemplate);
        _pVideoTemplate = new PVideoTemplate(this);
        Resources.MergedDictionaries.Add(_pVideoTemplate);
        _pLabelTemplate = new PLabelTemplate(this);
        Resources.MergedDictionaries.Add(_pLabelTemplate);
        _pLinkTemplate = new PLinkTemplate(this);
        Resources.MergedDictionaries.Add(_pLinkTemplate);
        _pProspectTemplate = new PProspectTemplate(this);
        Resources.MergedDictionaries.Add(_pProspectTemplate);
        _pCandidateTemplate = new PCandidateTemplate(this);
        Resources.MergedDictionaries.Add(_pCandidateTemplate);
        _pSlateTemplate = new PSlateTemplate(this);
        Resources.MergedDictionaries.Add(_pSlateTemplate);
        _pMeaningTemplate = new PMeaningTemplate(this);
        Resources.MergedDictionaries.Add(_pMeaningTemplate);
        _pCollocationTemplate = new PCollocationTemplate(this);
        Resources.MergedDictionaries.Add(_pCollocationTemplate);
        _pLanguageTemplate = new PLanguageTemplate(this);
        Resources.MergedDictionaries.Add(_pLanguageTemplate);
        _pMarkerTemplate = new PMarkerTemplate(this);
        Resources.MergedDictionaries.Add(_pMarkerTemplate);
        _pCategoryTemplate = new PCategoryTemplate(this);
        Resources.MergedDictionaries.Add(_pCategoryTemplate);
        _pNotationTemplate = new PNotationTemplate(this);
        Resources.MergedDictionaries.Add(_pNotationTemplate);
        _pClipTemplate = new PClipTemplate(this);
        Resources.MergedDictionaries.Add(_pClipTemplate);
        PLook.PLookStyleAttach(_pClipTemplate);
        PLook.PLookStyleAttach(surface.Resources);

        PCardListAttach();
        PNotationAttach();
        PAccentAttach();
        PTranscriptionAttach();
        PGlyphAttach();
        PReflexAttach();
        PAnchorAttach();
        PClipAttach();
        PSpeakerAttach();
        PProspectList.ItemsSource = _pProspectItem;
        PLookItem.PLookItemAttach(PProspectList, PProspectApply);
        PCandidateAttach();
        PSlateAttach();
        PCategoryAttach();
        PMarkerAttach();
        PStackAttach();
        AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(PEditorTextHandle));
        PHeadword.TextChanged += PHeadwordHandle;
        PPronunciationField.TextChanged += PPronunciationHandle;
        DependencyPropertyDescriptor.FromProperty(TagProperty, typeof(TextBox)).AddValueChanged(
            PHeadword, (_, _) => PField.PFieldGhostShow(PHeadwordGhost, PHeadword));
        DependencyPropertyDescriptor.FromProperty(TagProperty, typeof(TextBox)).AddValueChanged(
            PPronunciationField, (_, _) => PField.PFieldGhostShow(PPronunciationMeasure, PPronunciationField));
        PNoteContents.TextChanged += PNoteHandle;
        PEditorFavorite.Click += PEditorFavoriteHandle;
        PEditorGrasp.PGraspChanged += PEditorGraspHandle;
        PEditorGrasp.PGraspHovered += PEditorHoverHandle;
        PEditorBackward.Click += PEditorUndoHandle;
        PEditorBackward.Tag = PIcon.PIconResolve("undo", 24);
        PEditorForward.Click += PEditorRedoHandle;
        PEditorForward.Tag = PIcon.PIconResolve("redo", 24);
        PEditorDiscard.Click += PEditorDiscardHandle;
        PEditorDiscard.Tag = PIcon.PIconResolve("new", 24);
        PEditorStore.Click += PEditorStoreHandle;
        PEditorStore.Tag = PIcon.PIconResolve("save", 24);
        PPlaybackAction.Click += PPlaybackActionHandle;
        PPlaybackAction.Tag = PIcon.PIconResolve("play", 24);
        PEtymologyAttach();
        AddHandler(LostFocusEvent, new RoutedEventHandler(PEditorFocusHandle));
        _pDownloaderPlayer.MediaEnded += PClipEndHandle;
        _pDownloaderPlayer.MediaFailed += PClipEndHandle;
    }

    private TextBlock PHeadwordGhost => (TextBlock)FindName(nameof(PHeadwordGhost));

    private TextBox PHeadword => (TextBox)FindName(nameof(PHeadword));

    private TextBlock PEditorReading => (TextBlock)FindName(nameof(PEditorReading));

    private ToggleButton PEditorFavorite => (ToggleButton)FindName(nameof(PEditorFavorite));

    private PGrasp PEditorGrasp => (PGrasp)FindName(nameof(PEditorGrasp));

    private TextBlock PEditorGraspLabel => (TextBlock)FindName(nameof(PEditorGraspLabel));

    private Border PEditorCommand => (Border)FindName(nameof(PEditorCommand));

    private Button PEditorBackward => (Button)FindName(nameof(PEditorBackward));

    private Button PEditorForward => (Button)FindName(nameof(PEditorForward));

    private Button PEditorDiscard => (Button)FindName(nameof(PEditorDiscard));

    private Button PEditorStore => (Button)FindName(nameof(PEditorStore));

    private StackPanel PEditorSound => (StackPanel)FindName(nameof(PEditorSound));

    private Border PPronunciation => (Border)FindName(nameof(PPronunciation));

    private TextBlock PPronunciationOpener => (TextBlock)FindName(nameof(PPronunciationOpener));

    private TextBlock PPronunciationMeasure => (TextBlock)FindName(nameof(PPronunciationMeasure));

    internal TextBox PPronunciationField => (TextBox)FindName(nameof(PPronunciationField));

    private TextBlock PPronunciationCloser => (TextBlock)FindName(nameof(PPronunciationCloser));

    private PContour PContour => (PContour)FindName(nameof(PContour));

    private StackPanel PEditorFrequencySection => (StackPanel)FindName(nameof(PEditorFrequencySection));

    private Border PEditorFrequencyChip => (Border)FindName(nameof(PEditorFrequencyChip));

    private TextBlock PEditorFrequency => (TextBlock)FindName(nameof(PEditorFrequency));

    private TextBlock PEditorFrequencyBand => (TextBlock)FindName(nameof(PEditorFrequencyBand));

    private PParadigm PEditorParadigm => (PParadigm)FindName(nameof(PEditorParadigm));

    private PFanqie PEditorFanqie => (PFanqie)FindName(nameof(PEditorFanqie));

    private PScript PEditorScript => (PScript)FindName(nameof(PEditorScript));

    private Border PContents => (Border)FindName(nameof(PContents));

    private TextBox PNoteContents => (TextBox)FindName(nameof(PNoteContents));

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
        PVolumeAttach();
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
        desk.LDeskEntryAttach(LSubject.LSubjectFrequency, LObserver.LObserverCreate(this, PEditorFrequencyUpdate));
        desk.LDeskEntryAttach(LSubject.LSubjectGrasp, LObserver.LObserverCreate(this, PEditorGraspUpdate));
        desk.LDeskEntryAttach(LSubject.LSubjectInflection, LObserver.LObserverCreate(this, PEditorParadigmUpdate));
        desk.LDeskEntryAttach(LSubject.LSubjectReflex, LObserver.LObserverCreate(this, PReflexPendingShow));
        desk.LDeskObserverAttach(LSubject.LSubjectScript, LObserver.LObserverCreate(this, PEditorScriptUpdate));
        desk.LDeskObserverAttach(LSubject.LSubjectFanqie, LObserver.LObserverCreate(this, PEditorFanqieUpdate));
        desk.LDeskObserverAttach(LSubject.LSubjectReference, LObserver.LObserverCreate(this, PSentenceLoad));
        desk.LDeskObserverAttach(LSubject.LSubjectSettings, LObserver.LObserverCreate(this, desk.LDeskDraftUpdate));
        desk.LDeskDraftAttach(LSubject.LSubjectTenure, LObserver.LObserverCreate(this, desk.LDeskStateUpdate));
        desk.LDeskDraftAttach(LSubject.LSubjectDraft, LObserver.LObserverCreate(this, desk.LDeskDraftUpdate));
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
        PEditorReadingShow();

        IReadOnlyDictionary<long, LTranslationTarget> targets = _lEditor.LEditorTargetRead();
        PCardShow(_pMeaningList, "Meaning", draft.LEntryDraftMeanings, targets, draft.LEntryDraftLanguage);
        PCardShow(_pCollocationList, "Collocation", draft.LEntryDraftCollocations, targets, draft.LEntryDraftLanguage);

        PEtymologyShow(draft);
        PField.PFieldNoteShow(PNoteContents, draft.LEntryDraftNote);
        PEditorRecordingShow(draft);
        PPlaybackTrayShow();
        PReflexPrepare(draft);
    }

    private void PEditorLanguageUpdate()
    {
        PSpeakerName.Text = _lEditor.LEditorLanguage;
        PSpeakerFlagUpdate();
        LFontFace.LFontApply(_pEditorHost.PWindowDeportment, _lEditor.LEditorLanguage, PHeadword, PHeadwordGhost);
        LFontFace.LFontPlace(PHeadword, PHeadwordGhost);
        LFontFace.LFontExampleApply(Resources, _pEditorHost.PWindowDeportment, _lEditor.LEditorLanguage);
        PContour.PContourTonal = _lEditor.LEditorTonal;
        PPronunciation.Visibility = PLook.PLookVisibleRead(_lEditor.LEditorSpoken);
        PAccent.Visibility = PLook.PLookVisibleRead(_lEditor.LEditorSpoken);
        PSentenceFrameLoad(_lEditor.LEditorLanguage);
        PCategoryLoad();
    }

    private void PHeadwordHandle(object sender, TextChangedEventArgs e)
    {
        PField.PFieldGhostShow(PHeadwordGhost, PHeadword);
        _lEditor.LEditorHeadwordSet(PHeadword.Text);
    }

    private void PPronunciationHandle(object sender, TextChangedEventArgs e)
    {
        PField.PFieldGhostShow(PPronunciationMeasure, PPronunciationField);
        PContour.PContourIpa = PPronunciationField.Text;
        _lEditor.LEditorPronunciationSet(PPronunciationField.Text);
    }

    private void PNoteHandle(object sender, TextChangedEventArgs e)
    {
        _lEditor.LEditorNoteSet(PNoteContents.Text);
    }

    private void PEditorTextHandle(object sender, TextChangedEventArgs e)
    {
        switch (e.OriginalSource)
        {
            case TextBox { DataContext: PCard or PSentence or PGloss or PImage or PVideo } box:
                PEditorFieldHandle(box);
                break;
            case TextBox { DataContext: PContextCaret caret } box:
                caret.PContextCaretText = box.Text;
                break;
            case TextBox { DataContext: PRegisterCaret caret } box:
                caret.PRegisterCaretText = box.Text;
                break;
            case TextBox { DataContext: PLinkCaret caret } box:
                caret.PLinkCaretText = box.Text;
                break;
            case TextBox { DataContext: PLabelCaret caret } box:
                caret.PLabelCaretText = box.Text;
                break;
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
        LFrequencyLabel.LFrequencyChipShow(
            PEditorFrequencySection,
            PEditorFrequencyChip,
            PEditorFrequency,
            PEditorFrequencyBand,
            _lEditor.LEditorFrequencyRead());
    }

    private void PEditorParadigmUpdate()
    {
        LFontFace.LFontApply(_pEditorHost.PWindowDeportment, _lEditor.LEditorParadigmLanguage, PEditorParadigm);
        PEditorParadigm.PParadigmItems = PParadigmItem.PParadigmItemScan(
            _lEditor.LEditorParadigmRead(), _lEditor.LEditorParadigmPending, _lEditor.LEditorMorphology, true);
    }

    private void PEditorScriptUpdate()
    {
        LFontFace.LFontApply(
            _pEditorHost.PWindowDeportment, _lEditor.LEditorLanguage, LFontRole.LFontRoleGlyph, PEditorScript);
        PEditorScript.PScriptItems = PScriptItem.PScriptItemScan(_lEditor.LEditorScriptRead());
        PEditorScript.PScriptPending = _lEditor.LEditorScriptPending;
        PEditorScript.PScriptRenewal =
            PLook.PLookFirstRead<Action?>(_lEditor.LEditorScriptRebuildable, _lEditor.LEditorScriptRebuild, null);
    }

    private void PEditorFanqieUpdate()
    {
        PReflexAnchorShow();
        LFontFace.LFontApply(
            _pEditorHost.PWindowDeportment, _lEditor.LEditorLanguage, LFontRole.LFontRoleGlyph, PEditorFanqie);
        PEditorFanqie.PFanqieItems = PFanqieItem.PFanqieItemScan(_lEditor.LEditorFanqieRead());
        PEditorFanqie.PFanqiePending = _lEditor.LEditorFanqiePending;
        PEditorFanqie.PFanqieRenewal =
            PLook.PLookFirstRead<Action?>(_lEditor.LEditorFanqieRebuildable, _lEditor.LEditorFanqieRebuild, null);
        PEditorFanqie.PFanqieDiweiNotice =
            (kind, key) => _pEditorHost.PWindowDiweiShow(_lEditor.LEditorLanguage, kind, key);
        PEditorFanqie.PFanqieRepresentativeNotice =
            (fanqieId, rank) => _lEditor.LEditorFanqieSet(fanqieId, rank);
        PEditorReadingShow();
    }

    private void PEditorReadingShow()
    {
        PEditorReading.Text = _lEditor.LEditorReadingRead(PHeadword.Text);
    }
}
