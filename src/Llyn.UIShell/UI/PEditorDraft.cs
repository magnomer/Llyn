using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private LEntryDraft PEditorDraftRead(LEntryDraft held)
    {
        return held with
        {
            LEntryDraftMeanings = PCardRead(_pMeaningList, held.LEntryDraftMeanings),
            LEntryDraftCollocations = PCardRead(_pCollocationList, held.LEntryDraftCollocations),
        };
    }

    private void PEditorDraftShow(LEntryDraft draft)
    {
        _pEditorFill = true;
        try
        {
            PEditorTextShow(PHeadword, PEditorRequestHeadword, draft.LEntryDraftHeadword);
            PEditorTextShow(PPronunciation, PEditorRequestIpa, draft.LEntryDraftIpa);
            PEditorSpeechShow(draft.LEntryDraftSpeeches);
            PEditorLanguageShow(draft.LEntryDraftLanguage);

            IReadOnlyDictionary<long, LTranslationTarget> targets = PEditorTargetRead(draft);
            PCardShow(_pMeaningList, "Meaning", draft.LEntryDraftMeanings, targets);
            PCardShow(_pCollocationList, "Collocation", draft.LEntryDraftCollocations, targets);

            PEditorNoteShow(draft.LEntryDraftNote);
            PEditorRecordingShow(draft);
        }
        finally
        {
            _pEditorFill = false;
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

    private void PEditorIdentityApply(LEntryDraft stored)
    {
        PCardIdentityApply(_pMeaningList, stored.LEntryDraftMeanings);
        PCardIdentityApply(_pCollocationList, stored.LEntryDraftCollocations);
    }

    private static void PCardIdentityApply(IReadOnlyList<PCard> cards, IReadOnlyList<LCardDraft> stored)
    {
        foreach (LCardDraft draft in stored)
        {
            PCardFind(cards, draft.LCardDraftId)?.PCardIdentityApply(draft);
        }
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
        if (audio.Length == 0 || !File.Exists(audio))
        {
            return;
        }

        _pRecording = audio;
        _pRecordingSource = draft.LEntryDraftPronunciation?.LPronunciationDraftSource;
        _pRecordingStored = true;
        PPlayback.Visibility = Visibility.Visible;
        PVolumeLoad();
    }

    internal void PEditorReset()
    {
        PEditorDraftStart(null);

        _pEditorFill = true;

        PHeadword.Text = string.Empty;
        PPronunciation.Text = string.Empty;
        PMarkerShow(null);
        PRecordingClear();
        _pSpeakerEntry = false;
        PHeadwordFontApply(_pSpeakerChoice);
        PEditorExampleShow(_pSpeakerChoice);
        PSentenceFrameLoad(_pSpeakerChoice);

        PNoteContents.Text = string.Empty;

        _pEditorFill = false;

        PEditorLanguageSend();
        PCardPrepare();
        PEditorChangeUpdate();
        PEditorFavoriteShow();
    }

    private string PEditorNoteRead()
    {
        return (PNoteContents.Text ?? string.Empty).TrimEnd('\r', '\n');
    }
}
