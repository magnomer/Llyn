using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private bool _pEditorPreparing;

    private bool _pEditorPrepareStale;

    private void PEditorDraftShow(LEntryDraft draft)
    {
        draft = PEditorDraftPrepare(draft);

        bool filling = _pEditorFill;
        _pEditorFill = true;
        try
        {
            PEditorTextShow(PHeadword, PEditorRequestHeadword, draft.LEntryDraftHeadword);
            _pEditorRespelling = PRespelling.PRespellingRead(_lEngine, draft.LEntryDraftLanguage);
            PPronunciationOpener.Text = _pEditorRespelling.PRespellingOpener;
            PPronunciationCloser.Text = _pEditorRespelling.PRespellingCloser;
            PEditorTextShow(
                PPronunciationField,
                PEditorRequestIpa,
                draft.LEntryDraftPronunciation is LPronunciationDraft primary
                    ? _pEditorRespelling.PRespellingTextRead(primary)
                    : string.Empty);
            PAccentShow(draft);
            PGlyphShow(draft);
            PTranscriptionShow(draft);
            PReflexShow(draft);
            PEditorSpeechShow(draft.LEntryDraftSpeeches);
            PEditorLanguageShow(draft.LEntryDraftLanguage);

            IReadOnlyDictionary<long, LTranslationTarget> targets = PEditorTargetRead(draft);
            PCardShow(_pMeaningList, "Meaning", draft.LEntryDraftMeanings, targets);
            PCardShow(_pCollocationList, "Collocation", draft.LEntryDraftCollocations, targets);

            PEditorNoteShow(draft.LEntryDraftNote);
            PEditorRecordingShow(draft);
            PPlaybackTrayShow();
        }
        finally
        {
            _pEditorFill = filling;
        }

        PReflexPrepare(draft);
    }

    private LEntryDraft PEditorDraftPrepare(LEntryDraft draft)
    {
        if (_pEditorFill || _pEditorHalted || _pEditorDraft == 0)
        {
            return draft;
        }

        _pEditorPreparing = true;
        _pEditorPrepareStale = false;
        try
        {
            PCardPrepare(draft);
            PTranscriptionPrepare(draft);
            PGlyphPrepare(draft);
            draft = PEditorDraftRead(draft);
            PSentencePrepare(draft);
            return PEditorDraftRead(draft);
        }
        finally
        {
            _pEditorPreparing = false;
            _pEditorPrepareStale = false;
        }
    }

    private LEntryDraft PEditorDraftRead(LEntryDraft draft)
    {
        if (!_pEditorPrepareStale || _pEditorHalted || _pEditorDraft == 0)
        {
            return draft;
        }

        _pEditorPrepareStale = false;
        try
        {
            return _lEngine.LEngineDraftRead(_pEditorDraft)?.LDraftContent ?? draft;
        }
        catch (Exception exception)
        {
            PEditorHoldSuspend(exception);
            return draft;
        }
    }

    private void PEditorTextShow(TextBox box, string key, string text)
    {
        if (PEditorRequestCheck(key) || string.Equals(box.Text, text, StringComparison.Ordinal))
        {
            return;
        }

        box.Text = text;
    }

    private void PEditorSpeechShow(IReadOnlyList<LSpeechDraft> speeches)
    {
        if (PEditorRequestCheck(PEditorRequestSpeech) || PMarkerMatch(speeches))
        {
            return;
        }

        PMarkerShow(speeches);
    }

    private void PEditorNoteShow(string note)
    {
        if (PEditorRequestCheck(PEditorRequestNote)
            || string.Equals(PEditorNoteRead(), note, StringComparison.Ordinal))
        {
            return;
        }

        PNoteContents.Text = note;
    }

    private void PEditorRecordingShow(LEntryDraft draft)
    {
        string audio = draft.LEntryDraftAudio;
        if (string.Equals(_pRecording ?? string.Empty, audio, StringComparison.Ordinal))
        {
            return;
        }

        PRecordingClear();
        if (!_lEngine.LEngineRecordingExist(audio))
        {
            return;
        }

        _pRecording = audio;
        _pRecordingSource = draft.LEntryDraftPronunciation?.LPronunciationDraftSource;
        _pRecordingStored = true;
        PPlaybackAction.Visibility = Visibility.Visible;
        PVolumeLoad();
    }

    internal void PEditorReset()
    {
        PEditorDraftStart(null);

        _pEditorFill = true;

        PHeadword.Text = string.Empty;
        PPronunciationField.Text = string.Empty;
        PAccentClear();
        PTranscriptionClear();
        PGlyphClear();
        PReflexClear();
        PMarkerShow(null);
        PRecordingClear();
        _pSpeakerEntry = false;
        PHeadwordFontApply(_pSpeakerChoice);
        PEditorContourApply(_pSpeakerChoice);
        PEditorSilentApply(_pSpeakerChoice);
        PEditorExampleShow(_pSpeakerChoice);
        PSentenceFrameLoad(_pSpeakerChoice);

        PNoteContents.Text = string.Empty;

        _pEditorFill = false;

        _pEditorPreparing = true;
        try
        {
            PEditorLanguageSend();
        }
        finally
        {
            _pEditorPreparing = false;
            _pEditorPrepareStale = false;
        }

        PEditorDraftRestore();
        PEditorChangeUpdate();
        PEditorFavoriteShow();
        PEditorGraspShow();
        PEditorFrequencyShow();
        PEditorParadigmShow();
        PEditorScriptShow();
        PEditorFanqieShow();
    }

    private string PEditorNoteRead()
    {
        return (PNoteContents.Text ?? string.Empty).TrimEnd('\r', '\n');
    }
}
