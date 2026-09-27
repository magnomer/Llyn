using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor : UserControl, PImageHost, PVideoHost, QChronicleHost
{
    private PWindow _pEditorHost = null!;

    private LEditor _lEditor = null!;

    private LLectern _pEditorLectern = null!;

    private readonly QRegard _qRegard;

    private readonly QCadence _qCadence;

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
        QLook.QLookStyleAttach(_pClipTemplate);
        QLook.QLookStyleAttach(surface.Resources);

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
        QLookItem.QLookItemAttach(PProspectList, PProspectApply);
        PCandidateAttach();
        PSlateAttach();
        PCategoryAttach();
        PMarkerAttach();
        PStackAttach();
        AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(PEditorTextHandle));
        PHeadword.TextChanged += PHeadwordHandle;
        PPronunciationField.TextChanged += PPronunciationHandle;
        QField.QFieldGhostAttach(PHeadwordGhost, PHeadword);
        QField.QFieldGhostAttach(PPronunciationMeasure, PPronunciationField);
        PNoteContents.TextChanged += PNoteHandle;
        _qRegard = new QRegard(this);
        _qCadence = new QCadence(this);
        PEditorBackward.Click += PEditorUndoHandle;
        PHeadword.SetResourceReference(QField.QFieldHintProperty, "Input.Headword");
        PPronunciationField.SetResourceReference(QField.QFieldHintProperty, "Input.Pronunciation");
        PNoteContents.SetResourceReference(QField.QFieldHintProperty, "Input.NoteHint");
        PEditorBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        PEditorForward.Click += PEditorRedoHandle;
        PEditorForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        PEditorDiscard.Click += PEditorDiscardHandle;
        PEditorDiscard.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        PEditorStore.Click += PEditorStoreHandle;
        PEditorStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        PPlaybackAction.Click += PPlaybackActionHandle;
        PPlaybackAction.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("play", 24));
        PEtymologyAttach();
        AddHandler(LostFocusEvent, new RoutedEventHandler(PEditorFocusHandle));
        _pDownloaderPlayer.MediaEnded += PClipEndHandle;
        _pDownloaderPlayer.MediaFailed += PClipEndHandle;
    }

    private TextBlock PHeadwordGhost => (TextBlock)FindName(nameof(PHeadwordGhost));

    private TextBox PHeadword => (TextBox)FindName(nameof(PHeadword));

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

    private Border PContents => (Border)FindName(nameof(PContents));

    private TextBox PNoteContents => (TextBox)FindName(nameof(PNoteContents));

    private long PEditorDraft => _lEditor.LEditorDesk.LDeskId;

    internal void PEditorAttach(PWindow host, LEditor editor, LLectern lectern)
    {
        _pEditorHost = host;
        _lEditor = editor;
        _pEditorLectern = lectern;
        _qRegard.QRegardAttach(editor);
        _qCadence.QCadenceAttach(host, editor);
        _lEditor.LEditorDesk.LDeskStarted += PEditorStartUpdate;
        PEditorObserverAttach(_lEditor.LEditorDesk);
        _lEditor.LEditorDraftChanged += PEditorDraftShow;
        _lEditor.LEditorDesk.LDeskFailed += host.PWindowFailureShow;
        _lEditor.LEditorStateChanged += PEditorStateUpdate;
        _lEditor.LEditorStopped += PEditorStopUpdate;
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
        PEditorCommand.Visibility = QLook.QLookVisibleRead(_lEditor.LEditorOwned);
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

    public void QChronicleUndo()
    {
        QChronicle.QChronicleRun(_lEditor.LEditorDesk.LDeskUndo);
    }

    public void QChronicleRedo()
    {
        QChronicle.QChronicleRun(_lEditor.LEditorDesk.LDeskRedo);
    }

    public void QChronicleUpdate()
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
        desk.LDeskVigil.QVigilEntryAttach(
            CSubject.CSubjectFrequency, LObserver.LObserverCreate<CBulletin>(this, _qRegard.QRegardFrequencyUpdate));
        desk.LDeskVigil.QVigilEntryAttach(
            CSubject.CSubjectGrasp, LObserver.LObserverCreate<CBulletin>(this, _qRegard.QRegardGraspUpdate));
        desk.LDeskVigil.QVigilEntryAttach(
            CSubject.CSubjectInflection, LObserver.LObserverCreate<CBulletin>(this, _qCadence.QCadenceParadigmUpdate));
        desk.LDeskVigil.QVigilEntryAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(this, PReflexPendingShow));
        desk.LDeskVigil.QVigilObserverAttach(
            CSubject.CSubjectScript, LObserver.LObserverCreate<CBulletin>(this, _qCadence.QCadenceScriptUpdate));
        desk.LDeskVigil.QVigilObserverAttach(
            CSubject.CSubjectFanqie, LObserver.LObserverCreate<CBulletin>(this, PEditorFanqieUpdate));
        desk.LDeskVigil.QVigilObserverAttach(
            CSubject.CSubjectReference, LObserver.LObserverCreate<CBulletin>(this, PSentenceLoad));
        desk.LDeskVigil.QVigilObserverAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(this, desk.LDeskDraftUpdate));
        desk.LDeskVigil.QVigilDraftAttach(
            CSubject.CSubjectTenure, LObserver.LObserverCreate<CBulletin>(this, desk.LDeskStateUpdate));
        desk.LDeskVigil.QVigilDraftAttach(
            CSubject.CSubjectDraft, LObserver.LObserverCreate<CBulletin>(this, desk.LDeskDraftUpdate));
    }

    private void PEditorStartUpdate()
    {
        _pMeaningList.Clear();
        _pCollocationList.Clear();
        _qRegard.QRegardUpdate();
        _qCadence.QCadenceParadigmUpdate();
        _qCadence.QCadenceScriptUpdate();
        PEditorFanqieUpdate();
    }

    private void PEditorDraftShow(CEntryDraft draft)
    {
        QField.QFieldTextShow(PHeadword, draft.CEntryDraftHeadword);
        PPronunciationOpener.Text = QLook.QLookFirstRead(_lEditor.LEditorTimbre.QTimbrePhonemic, "/", "[");
        PPronunciationCloser.Text = QLook.QLookFirstRead(_lEditor.LEditorTimbre.QTimbrePhonemic, "/", "]");
        QField.QFieldTextShow(PPronunciationField, _lEditor.LEditorPronunciationRead());
        PAccentShow(draft);
        PGlyphShow(draft);
        PTranscriptionShow(draft);
        PReflexShow(draft);
        PMarkerShow(draft.CEntryDraftSpeeches);
        PEditorLanguageUpdate();
        _qCadence.QCadenceReadingShow(PHeadword.Text);

        IReadOnlyDictionary<long, CTranslationTarget> targets = _lEditor.LEditorTargetRead();
        PCardShow(_pMeaningList, "Meaning", draft.CEntryDraftMeanings, targets, draft.CEntryDraftLanguage);
        PCardShow(_pCollocationList, "Collocation", draft.CEntryDraftCollocations, targets, draft.CEntryDraftLanguage);

        PEtymologyShow(draft);
        QField.QFieldNoteShow(PNoteContents, draft.CEntryDraftNote);
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
        PContour.PContourTonal = _lEditor.LEditorTimbre.QTimbreTonal;
        PPronunciation.Visibility = QLook.QLookVisibleRead(_lEditor.LEditorTimbre.QTimbreSpoken);
        PAccent.Visibility = QLook.QLookVisibleRead(_lEditor.LEditorTimbre.QTimbreSpoken);
        PSentenceFrameLoad(_lEditor.LEditorLanguage);
        PCategoryLoad();
    }

    private void PHeadwordHandle(object sender, TextChangedEventArgs e)
    {
        _lEditor.LEditorHeadwordSet(PHeadword.Text);
    }

    private void PPronunciationHandle(object sender, TextChangedEventArgs e)
    {
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
        _pEditorHost.PWindowEnvoy.CEnvoyFailureShow("Input.HoldFailed");
    }

    private void PEditorUndoHandle(object sender, RoutedEventArgs e)
    {
        QChronicleUndo();
    }

    private void PEditorRedoHandle(object sender, RoutedEventArgs e)
    {
        QChronicleRedo();
    }

    private void PEditorStoreHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorSave();
    }

    private void PEditorDiscardHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorReset();
    }

    private void PEditorFanqieUpdate()
    {
        PReflexAnchorShow();
        _qCadence.QCadenceFanqieUpdate();
        _qCadence.QCadenceReadingShow(PHeadword.Text);
    }
}
