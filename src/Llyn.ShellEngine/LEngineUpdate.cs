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
            LEngineFormUpdate(entries, id, draft, changes);
            LEngineInflectionUpdate(id, draft, changes);
            LEngineNoteUpdate(id, draft, changes);
            LEnginePronunciationUpdate(id, draft, changes);

            LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord(changes);

            LWorkspaceArchive workspace = new(_lEngineDatabase);
            LWorkspaceState state = workspace.LWorkspaceStateRead();
            workspace.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

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

    private static void LEngineFormUpdate(
        LEntryArchive entries, string entryId, LEntryDraft draft, List<LRevisionChange> changes)
    {
        IReadOnlyList<LForm> stored = entries.LEntryFormRead(entryId);
        IReadOnlyList<LForm> current = draft.LEntryDraftForms;

        if (LEngineFormMatch(stored, current))
        {
            return;
        }

        entries.LEntryFormSet(entryId, current);
        changes.Add(new LRevisionChange(
            0,
            entryId,
            "form",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            null));
    }

    private static bool LEngineFormMatch(IReadOnlyList<LForm> stored, IReadOnlyList<LForm> current)
    {
        if (stored.Count != current.Count)
        {
            return false;
        }

        for (int index = 0; index < stored.Count; index++)
        {
            if (!string.Equals(stored[index].LFormText, current[index].LFormText, StringComparison.Ordinal)
                || !string.Equals(stored[index].LFormRole, current[index].LFormRole, StringComparison.Ordinal)
                || !string.Equals(
                    stored[index].LFormLocal, current[index].LFormLocal, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private void LEngineInflectionUpdate(
        string entryId, LEntryDraft draft, List<LRevisionChange> changes)
    {
        LInflectionArchive inflections = new(_lEngineDatabase);
        IReadOnlyList<LInflection> stored = inflections.LInflectionRead(entryId);
        IReadOnlyList<LInflection> current = draft.LEntryDraftInflections;

        if (LEngineInflectionMatch(stored, current))
        {
            return;
        }

        inflections.LInflectionSet(entryId, current);
        changes.Add(new LRevisionChange(
            0,
            entryId,
            "inflection",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            null));
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
        LPronunciationDraft? written = draft.LEntryDraftPronunciation;

        if (written is null || written.LPronunciationDraftEmpty)
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
                string.Empty,
                entryId,
                written.LPronunciationDraftLevel,
                written.LPronunciationDraftIpa,
                written.LPronunciationDraftSyllables,
                written.LPronunciationDraftRepresentations));
            pronunciationId = created.LPronunciationId;
            changes.Add(new LRevisionChange(
                0, pronunciationId, "pronunciation", "create", written.LPronunciationDraftIpa));
        }
        else
        {
            pronunciationId = stored.LPronunciationId;
            LPronunciation current = stored with
            {
                LPronunciationLevel = written.LPronunciationDraftLevel,
                LPronunciationIpa = written.LPronunciationDraftIpa,
                LPronunciationSyllables = written.LPronunciationDraftSyllables,
                LPronunciationRepresentations = written.LPronunciationDraftRepresentations,
            };

            if (!LEngineSoundMatch(stored, current))
            {
                pronunciations.LPronunciationUpdate(current);
                changes.Add(new LRevisionChange(
                    0, pronunciationId, "pronunciation", "update", written.LPronunciationDraftIpa));
            }
        }

        if (written.LPronunciationDraftAudio.Length == 0)
        {
            return;
        }

        string file = LEngineRecordingFormat(written.LPronunciationDraftAudio);
        LPronunciationAudio? audio = pronunciations.LPronunciationAudioRead(pronunciationId);
        if (string.Equals(audio?.LPronunciationAudioFile, file, StringComparison.Ordinal)
            && string.Equals(
                audio?.LPronunciationAudioSource,
                written.LPronunciationDraftSource,
                StringComparison.Ordinal))
        {
            return;
        }

        pronunciations.LPronunciationAudioSave(
            pronunciationId, file, written.LPronunciationDraftSource);
        changes.Add(new LRevisionChange(0, pronunciationId, "pronunciation", "update", file));
    }

    private static bool LEngineSoundMatch(LPronunciation stored, LPronunciation current)
    {
        if (!string.Equals(stored.LPronunciationIpa, current.LPronunciationIpa, StringComparison.Ordinal)
            || !string.Equals(
                stored.LPronunciationLevel, current.LPronunciationLevel, StringComparison.Ordinal)
            || stored.LPronunciationSyllables.Count != current.LPronunciationSyllables.Count
            || stored.LPronunciationRepresentations.Count != current.LPronunciationRepresentations.Count)
        {
            return false;
        }

        for (int index = 0; index < stored.LPronunciationSyllables.Count; index++)
        {
            if (stored.LPronunciationSyllables[index] with { LSyllablePronunciationId = string.Empty }
                != current.LPronunciationSyllables[index] with { LSyllablePronunciationId = string.Empty })
            {
                return false;
            }
        }

        for (int index = 0; index < stored.LPronunciationRepresentations.Count; index++)
        {
            if (stored.LPronunciationRepresentations[index]
                    with { LRepresentationPronunciationId = string.Empty }
                != current.LPronunciationRepresentations[index]
                    with { LRepresentationPronunciationId = string.Empty })
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineInflectionMatch(
        IReadOnlyList<LInflection> stored, IReadOnlyList<LInflection> current)
    {
        if (stored.Count != current.Count)
        {
            return false;
        }

        for (int index = 0; index < stored.Count; index++)
        {
            if (!string.Equals(
                    stored[index].LInflectionText, current[index].LInflectionText, StringComparison.Ordinal)
                || !string.Equals(
                    stored[index].LInflectionLocal,
                    current[index].LInflectionLocal,
                    StringComparison.Ordinal)
                || !string.Equals(
                    stored[index].LInflectionSpeechId,
                    current[index].LInflectionSpeechId,
                    StringComparison.Ordinal)
                || stored[index].LInflectionFeatures.Count != current[index].LInflectionFeatures.Count)
            {
                return false;
            }

            for (int place = 0; place < stored[index].LInflectionFeatures.Count; place++)
            {
                if (stored[index].LInflectionFeatures[place] != current[index].LInflectionFeatures[place])
                {
                    return false;
                }
            }
        }

        return true;
    }
}
