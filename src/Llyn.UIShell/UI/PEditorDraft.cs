using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private LEntryDraft? _pEditorDetail;

    private LEntryDraft PEditorDraftRead()
    {
        return new LEntryDraft(
            PHeadword.Text ?? string.Empty,
            _pSpeakerChoice,
            PEditorSoundRead(),
            PEditorNoteRead(),
            PCardRead(_pMeaningList),
            PCardRead(_pCollocationList),
            PEditorSpeechRead(),
            _pEditorDetail?.LEntryDraftForms ?? [],
            _pEditorDetail?.LEntryDraftInflections ?? []);
    }

    private LPronunciationDraft? PEditorSoundRead()
    {
        LPronunciationDraft held = _pEditorDetail?.LEntryDraftPronunciation
            ?? new LPronunciationDraft(string.Empty);

        LPronunciationDraft written = held with
        {
            LPronunciationDraftIpa = PPronunciation.Text ?? string.Empty,
            LPronunciationDraftAudio = _pRecording ?? string.Empty,
            LPronunciationDraftSource = _pRecordingSource,
        };

        return written.LPronunciationDraftEmpty ? null : written;
    }

    private IReadOnlyList<LSpeechDraft> PEditorSpeechRead()
    {
        return PMarkerRead();
    }

    private void PEditorDraftShow(LEntryDraft draft)
    {
        _pEditorFill = true;
        _pEditorDetail = draft;
        PSentenceFrameLoad(draft.LEntryDraftLanguage);

        PHeadword.Text = draft.LEntryDraftHeadword;
        PPronunciation.Text = draft.LEntryDraftIpa;
        PMarkerShow(draft.LEntryDraftSpeeches);
        PEditorLanguageShow(draft.LEntryDraftLanguage);

        IReadOnlyDictionary<long, LTranslationTarget> targets = PEditorTargetRead(draft);
        PCardShow(_pMeaningList, "Meaning", draft.LEntryDraftMeanings, targets);
        PCardShow(_pCollocationList, "Collocation", draft.LEntryDraftCollocations, targets);

        PEditorNoteShow(draft.LEntryDraftNote);
        PEditorRecordingShow(draft);

        _pEditorFill = false;
    }

    private void PEditorIdentityApply(LEntryDraft stored)
    {
        _pEditorDetail = stored;
        PCardIdentityApply(_pMeaningList, stored.LEntryDraftMeanings);
        PCardIdentityApply(_pCollocationList, stored.LEntryDraftCollocations);
    }

    private static void PCardIdentityApply(IReadOnlyList<PCard> cards, IReadOnlyList<LCardDraft> stored)
    {
        for (int index = 0; index < cards.Count && index < stored.Count; index++)
        {
            cards[index].PCardIdentityApply(stored[index]);
        }
    }

    private void PEditorNoteShow(string note)
    {
        PNoteContents.Text = note;
    }

    private void PEditorRecordingShow(LEntryDraft draft)
    {
        PRecordingClear();
        if (draft.LEntryDraftAudio.Length == 0 || !File.Exists(draft.LEntryDraftAudio))
        {
            return;
        }

        _pRecording = draft.LEntryDraftAudio;
        _pRecordingSource = draft.LEntryDraftPronunciation?.LPronunciationDraftSource;
        _pRecordingStored = true;
        PPlayback.Visibility = Visibility.Visible;
        PVolumeLoad();
    }

    internal void PEditorReset()
    {
        PEditorDraftStart(null);

        _pEditorFill = true;
        _pEditorDetail = null;

        PHeadword.Text = string.Empty;
        PPronunciation.Text = string.Empty;
        PMarkerShow(null);
        PRecordingClear();
        _pSpeakerEntry = false;
        PHeadwordFontApply(_pSpeakerChoice);
        PSentenceFrameLoad(_pSpeakerChoice);

        PCardShow(_pMeaningList, "Meaning", [], PEditorTargetEmpty);
        PCardShow(_pCollocationList, "Collocation", [], PEditorTargetEmpty);

        PNoteContents.Text = string.Empty;

        _pEditorFill = false;

        PEditorChangeUpdate();
        PEditorFavoriteShow();
    }

    private string PEditorNoteRead()
    {
        return (PNoteContents.Text ?? string.Empty).TrimEnd('\r', '\n');
    }
}
