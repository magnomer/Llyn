using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private void PEditorDraftShow(LEntryDraft draft)
    {
        draft = PEditorDraftPrepare(draft);

        bool filling = _pEditorFill;
        _pEditorFill = true;
        try
        {
            PEditorTextShow(PHeadword, draft.LEntryDraftHeadword);
            _pEditorRespelling = PRespelling.PRespellingRead(_lEngine, draft.LEntryDraftLanguage);
            PPronunciationOpener.Text = _pEditorRespelling.PRespellingOpener;
            PPronunciationCloser.Text = _pEditorRespelling.PRespellingCloser;
            PEditorTextShow(
                PPronunciationField,
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
            PCardShow(_pMeaningList, "Meaning", draft.LEntryDraftMeanings, targets, draft.LEntryDraftLanguage);
            PCardShow(
                _pCollocationList, "Collocation", draft.LEntryDraftCollocations, targets, draft.LEntryDraftLanguage);

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
        if (_pEditorFill || _pEditorTenure is null)
        {
            return draft;
        }

        return _pEditorTenure.LTenurePrepare(() =>
        {
            PCardPrepare(draft);
            PTranscriptionPrepare(draft);
            PGlyphPrepare(draft);
            PSentencePrepare(_pEditorTenure.LTenureRead()?.LDraftContent ?? draft);
        })?.LDraftContent ?? draft;
    }

    private static void PEditorTextShow(TextBox box, string text)
    {
        if (string.Equals(box.Text, text, StringComparison.Ordinal))
        {
            return;
        }

        box.Text = text;
    }

    private void PEditorSpeechShow(IReadOnlyList<LSpeechDraft> speeches)
    {
        if (PMarkerMatch(speeches))
        {
            return;
        }

        PMarkerShow(speeches);
    }

    private void PEditorNoteShow(string note)
    {
        if (string.Equals(PEditorNoteRead(), note, StringComparison.Ordinal))
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
        string language = PSpeakerLanguageRead();
        PHeadwordFontApply(language);
        PEditorContourApply(language);
        PEditorSilentApply(language);
        PEditorExampleShow(language);
        PSentenceFrameLoad(language);

        PNoteContents.Text = string.Empty;

        _pEditorFill = false;

        _pEditorTenure?.LTenurePrepare(PEditorLanguageSend);

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
