using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LEntry LEngineEntryUpdate(string id, LEntryDraft draft)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentNullException.ThrowIfNull(draft);

            if (string.IsNullOrWhiteSpace(draft.LEntryDraftHeadword))
            {
                throw new LRefusal(LRefusal.LRefusalHeadword);
            }

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

            LEntryArchive entries = new(_lEngineDatabase);
            LEntry stored = entries.LEntryRead(id) ?? throw new LRefusal(LRefusal.LRefusalEntry);

            List<LRevisionChange> changes = [];

            entries.LEntryUpdate(stored with
            {
                LEntryHeadword = draft.LEntryDraftHeadword,
                LEntryLanguage = draft.LEntryDraftLanguage,
            });

            if (!string.Equals(stored.LEntryHeadword, draft.LEntryDraftHeadword, StringComparison.Ordinal) ||
                !string.Equals(stored.LEntryLanguage, draft.LEntryDraftLanguage, StringComparison.Ordinal))
            {
                changes.Add(new LRevisionChange(0, id, "entry", "update", draft.LEntryDraftHeadword));
            }

            LEngineCardUpdate(
                session.LDatabaseSessionConnection,
                id,
                draft.LEntryDraftMeanings,
                draft.LEntryDraftLanguage,
                collocation: false,
                changes);
            LEngineCardUpdate(
                session.LDatabaseSessionConnection,
                id,
                draft.LEntryDraftCollocations,
                draft.LEntryDraftLanguage,
                collocation: true,
                changes);

            LEngineSpeechUpdate(entries, id, draft, changes);
            LEngineNoteUpdate(id, draft, changes);
            LEnginePronunciationUpdate(id, draft, changes);

            LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord(changes);

            LWorkspaceArchive workspace = new(_lEngineDatabase);
            LWorkspaceState state = workspace.LWorkspaceStateRead();
            workspace.LWorkspaceStateSave(state with
            {
                LWorkspaceStateLeft = id,
                LWorkspaceStateRevision = revision.LRevisionId,
            });

            LEntry updated = entries.LEntryRead(id) ?? stored;
            session.LDatabaseSessionCommit();
            return updated;
        }
    }

    private void LEngineSpeechUpdate(
        LEntryArchive entries, string entryId, LEntryDraft draft, List<LRevisionChange> changes)
    {
        IReadOnlyList<LSpeech> stored = entries.LEntrySpeechRead(entryId);
        IReadOnlyList<LSpeech> current = LEngineSpeechResolve(
            entryId, draft.LEntryDraftLanguage, draft.LEntryDraftSpeeches);

        if (LEngineSpeechMatch(stored, current))
        {
            return;
        }

        entries.LEntrySpeechSet(entryId, current);
        changes.Add(new LRevisionChange(
            0,
            entryId,
            "speech",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            LEngineSpeechFormat(current)));
    }

    private static string LEngineSpeechFormat(IReadOnlyList<LSpeech> speeches)
    {
        List<string> names = new(speeches.Count);
        foreach (LSpeech speech in speeches)
        {
            names.Add(speech.LSpeechCustom ?? speech.LSpeechValueId ?? string.Empty);
        }

        return string.Join(", ", names);
    }

    private static bool LEngineSpeechMatch(IReadOnlyList<LSpeech> one, IReadOnlyList<LSpeech> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (!string.Equals(one[index].LSpeechValueId, other[index].LSpeechValueId, StringComparison.Ordinal) ||
                !string.Equals(one[index].LSpeechCustom, other[index].LSpeechCustom, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private void LEngineNoteUpdate(string entryId, LEntryDraft draft, List<LRevisionChange> changes)
    {
        LNoteArchive notes = new(_lEngineDatabase);
        LNote? stored = notes.LNoteRead(entryId);

        if (string.IsNullOrWhiteSpace(draft.LEntryDraftNote))
        {
            if (stored is not null)
            {
                notes.LNoteDelete(entryId);
                changes.Add(new LRevisionChange(0, entryId, "note", "delete", null));
            }

            return;
        }

        if (string.Equals(stored?.LNoteText, draft.LEntryDraftNote, StringComparison.Ordinal))
        {
            return;
        }

        notes.LNoteSave(new LNote(entryId, draft.LEntryDraftNote));
        changes.Add(new LRevisionChange(
            0, entryId, "note", stored is null ? "create" : "update", null));
    }

    private void LEnginePronunciationUpdate(
        string entryId, LEntryDraft draft, List<LRevisionChange> changes)
    {
        LPronunciationArchive pronunciations = new(_lEngineDatabase);
        LPronunciation? stored = pronunciations.LPronunciationRead(entryId);

        if (string.IsNullOrWhiteSpace(draft.LEntryDraftPronunciation) &&
            string.IsNullOrWhiteSpace(draft.LEntryDraftAudio))
        {
            if (stored is not null)
            {
                pronunciations.LPronunciationDelete(stored.LPronunciationId);
                changes.Add(new LRevisionChange(
                    0, stored.LPronunciationId, "pronunciation", "delete", stored.LPronunciationIpa));
            }

            return;
        }

        string pronunciationId;
        if (stored is null)
        {
            LPronunciation created = pronunciations.LPronunciationCreate(new LPronunciation(
                string.Empty, entryId, null, draft.LEntryDraftPronunciation, [], []));
            pronunciationId = created.LPronunciationId;
            changes.Add(new LRevisionChange(
                0, pronunciationId, "pronunciation", "create", draft.LEntryDraftPronunciation));
        }
        else
        {
            pronunciationId = stored.LPronunciationId;
            if (!string.Equals(
                    stored.LPronunciationIpa, draft.LEntryDraftPronunciation, StringComparison.Ordinal))
            {
                pronunciations.LPronunciationUpdate(stored with
                {
                    LPronunciationIpa = draft.LEntryDraftPronunciation,
                });
                changes.Add(new LRevisionChange(
                    0, pronunciationId, "pronunciation", "update", draft.LEntryDraftPronunciation));
            }
        }

        if (string.IsNullOrWhiteSpace(draft.LEntryDraftAudio))
        {
            return;
        }

        string file = LEngineRecordingFormat(draft.LEntryDraftAudio);
        LPronunciationAudio? audio = pronunciations.LPronunciationAudioRead(pronunciationId);
        if (string.Equals(audio?.LPronunciationAudioFile, file, StringComparison.Ordinal))
        {
            return;
        }

        pronunciations.LPronunciationAudioSave(pronunciationId, file, draft.LEntryDraftSource);
        changes.Add(new LRevisionChange(0, pronunciationId, "pronunciation", "update", file));
    }
}
