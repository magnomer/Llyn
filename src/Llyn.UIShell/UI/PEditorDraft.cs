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
        IReadOnlyList<LSpeechDraft> held = _pEditorDetail?.LEntryDraftSpeeches ?? [];
        List<LSpeechDraft> written = [];

        foreach (string name in PMarkerRead())
        {
            written.Add(PEditorSpeechFind(held, name));
        }

        return written;
    }

    private static LSpeechDraft PEditorSpeechFind(IReadOnlyList<LSpeechDraft> held, string name)
    {
        foreach (LSpeechDraft speech in held)
        {
            if (string.Equals(speech.LSpeechDraftName, name, StringComparison.OrdinalIgnoreCase))
            {
                return speech;
            }
        }

        return LSpeechDraft.LSpeechDraftCreate(name);
    }

    private static IReadOnlyList<string> PEditorSpeechShow(IReadOnlyList<LSpeechDraft> speeches)
    {
        List<string> named = new(speeches.Count);
        foreach (LSpeechDraft speech in speeches)
        {
            if (speech.LSpeechDraftName.Length > 0)
            {
                named.Add(speech.LSpeechDraftName);
            }
        }

        return named;
    }

    private void PEditorDraftShow(LEntryDraft draft)
    {
        _pEditorFill = true;
        _pEditorDetail = draft;
        PSentenceFrameLoad(draft.LEntryDraftLanguage);

        PHeadword.Text = draft.LEntryDraftHeadword;
        PPronunciation.Text = draft.LEntryDraftIpa;
        PMarkerShow(PEditorSpeechShow(draft.LEntryDraftSpeeches));
        PEditorLanguageShow(draft.LEntryDraftLanguage);

        IReadOnlyDictionary<string, LTranslationTarget> targets = PEditorTargetRead(draft);
        PCardShow(_pMeaningList, "Meaning", draft.LEntryDraftMeanings, targets);
        PCardShow(_pCollocationList, "Collocation", draft.LEntryDraftCollocations, targets);

        PEditorNoteShow(draft.LEntryDraftNote);
        PEditorRecordingShow(draft);

        _pEditorFill = false;
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
