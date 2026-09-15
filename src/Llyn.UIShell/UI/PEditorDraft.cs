using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private void PEditorDraftShow(LEntryDraft draft)
    {
        bool filling = _pEditorFill;
        _pEditorFill = true;
        try
        {
            PEditorTextShow(PHeadword, PEditorRequestHeadword, draft.LEntryDraftHeadword);
            PEditorTextShow(PPronunciationField, PEditorRequestIpa, draft.LEntryDraftIpa);
            PAccentShow(draft);
            PGlyphShow(draft);
            PTranscriptionShow(draft);
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

        PTranscriptionPrepare(draft);
        PGlyphPrepare(draft);
        PSentencePrepare();
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

        PEditorLanguageSend();
        PCardPrepare();
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
