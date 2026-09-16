using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LEntry LEngineEntryUpdate(long id, LEntryDraft draft)
    {
        return LEngineEntryUpdate(id, draft, []);
    }

    private LEntry LEngineEntryUpdate(long id, LEntryDraft draft, Dictionary<long, long> identity)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
            ArgumentNullException.ThrowIfNull(draft);

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

            LEntry stored = new LEntryArchive(_lEngineDatabase).LEntryRead(id)
                ?? throw new LRefusal(LRefusal.LRefusalEntry);

            List<LRevisionChange> changes = [];
            LEntry updated = LEngineEntryUpdate(id, draft, identity, changes);

            if (changes.Count > 0)
            {
                LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord(changes);

                LWorkspaceArchive workspace = new(_lEngineDatabase);
                LWorkspaceState state = workspace.LWorkspaceStateRead();
                workspace.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });
            }

            session.LDatabaseSessionCommit();
            bool renamed =
                !string.Equals(stored.LEntryHeadword, updated.LEntryHeadword, StringComparison.Ordinal) ||
                !string.Equals(stored.LEntryLanguage, updated.LEntryLanguage, StringComparison.Ordinal);
            if (renamed)
            {
                LEngineFrequencyStart(id);
            }

            return updated;
        }
    }

    private LEntry LEngineEntryUpdate(
        long id, LEntryDraft draft, Dictionary<long, long> identity, List<LRevisionChange> changes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(draft);

        if (string.IsNullOrWhiteSpace(draft.LEntryDraftHeadword))
        {
            throw new LRefusal(LRefusal.LRefusalHeadword);
        }

        draft = draft with { LEntryDraftHeadword = draft.LEntryDraftHeadword.Trim() };

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        LEntryArchive entries = new(_lEngineDatabase);
        LEntry stored = entries.LEntryRead(id) ?? throw new LRefusal(LRefusal.LRefusalEntry);
        LEntryDraft? origin = LEngineEntryLoad(id);

        if (origin is null || !LEngineDraftMatch(origin, draft))
        {
            entries.LEntryUpdate(stored with
            {
                LEntryHeadword = draft.LEntryDraftHeadword,
                LEntryLanguage = draft.LEntryDraftLanguage,
            });
        }

        bool renamed =
            !string.Equals(stored.LEntryHeadword, draft.LEntryDraftHeadword, StringComparison.Ordinal) ||
            !string.Equals(stored.LEntryLanguage, draft.LEntryDraftLanguage, StringComparison.Ordinal);
        if (renamed)
        {
            entries.LEntryFrequencySet(id, null);
            changes.Add(new LRevisionChange(0, id, "entry", "update", draft.LEntryDraftHeadword));
        }

        LEngineCardUpdate(
            session.LDatabaseSessionConnection,
            id,
            draft.LEntryDraftMeanings,
            draft.LEntryDraftLanguage,
            false,
            changes,
            identity);
        LEngineCardUpdate(
            session.LDatabaseSessionConnection,
            id,
            draft.LEntryDraftCollocations,
            draft.LEntryDraftLanguage,
            true,
            changes,
            identity);

        LEngineSpeechUpdate(entries, id, draft, changes);
        LEngineFormUpdate(entries, id, draft, changes);
        LEngineInflectionUpdate(id, draft, changes);
        LEngineInflectionReset(id);
        LEngineNoteUpdate(id, draft, changes);
        LEnginePronunciationSync(id, draft.LEntryDraftPronunciations, changes, identity);
        LEngineTranscriptionSync(id, draft.LEntryDraftTranscriptions, changes, identity);
        LEngineReflexSync(id, draft.LEntryDraftReflexes, changes, identity);

        LEntry updated = entries.LEntryRead(id) ?? stored;
        LEngineParadigmUpdate(updated);
        session.LDatabaseSessionCommit();
        return updated;
    }

    private void LEngineRevisionRecord(long target, string subject, string kind, string? summary)
    {
        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        LRevisionChange change = new(0, target, subject, kind, summary);
        LRevision revision = new LRevisionArchive(_lEngineDatabase).LRevisionRecord([change]);

        LWorkspaceArchive workspace = new(_lEngineDatabase);
        LWorkspaceState state = workspace.LWorkspaceStateRead();
        workspace.LWorkspaceStateSave(state with { LWorkspaceStateRevision = revision.LRevisionId });

        session.LDatabaseSessionCommit();
    }

    private void LEngineUpdatedSet(long entryId)
    {
        new LEntryArchive(_lEngineDatabase).LEntryUpdatedSet(entryId);
    }

    private void LEngineUpdatedSet(long ownerId, bool collocation)
    {
        long? entryId = collocation
            ? new LCollocationArchive(_lEngineDatabase).LCollocationHolderRead(ownerId)
            : new LMeaningArchive(_lEngineDatabase).LMeaningHolderRead(ownerId);

        if (entryId is long held)
        {
            LEngineUpdatedSet(held);
        }
    }

    private void LEngineSpeechUpdate(
        LEntryArchive entries, long entryId, LEntryDraft draft, List<LRevisionChange> changes)
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
        LEntryArchive entries, long entryId, LEntryDraft draft, List<LRevisionChange> changes)
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
        long entryId, LEntryDraft draft, List<LRevisionChange> changes)
    {
        LInflectionArchive inflections = new(_lEngineDatabase);
        IReadOnlyList<LInflection> stored = inflections.LInflectionRead(entryId);
        IReadOnlyList<LInflection> current = draft.LEntryDraftInflections;

        if (LEngineInflectionMatch(stored, current))
        {
            return;
        }

        LEngineInflectionValidate(current);
        inflections.LInflectionSet(entryId, current);
        changes.Add(new LRevisionChange(
            0,
            entryId,
            "inflection",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            null));
    }

    private string LEngineSpeechFormat(IReadOnlyList<LSpeech> speeches)
    {
        LSpeechArchive values = new(_lEngineDatabase);
        List<string> names = new(speeches.Count);
        foreach (LSpeech speech in speeches)
        {
            names.Add(speech.LSpeechCustom
                ?? values.LSpeechValueRead(speech.LSpeechValueId ?? 0)?.LSpeechValueName
                ?? string.Empty);
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
            if (one[index].LSpeechValueId != other[index].LSpeechValueId ||
                !string.Equals(one[index].LSpeechCustom, other[index].LSpeechCustom, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private void LEngineNoteUpdate(long entryId, LEntryDraft draft, List<LRevisionChange> changes)
    {
        LNoteArchive notes = new(_lEngineDatabase);
        LNote? stored = notes.LNoteRead(entryId);
        string text = LMarkdown.LMarkdownNormalize(draft.LEntryDraftNote);

        if (text.Length == 0)
        {
            if (stored is not null)
            {
                notes.LNoteDelete(entryId);
                changes.Add(new LRevisionChange(0, entryId, "note", "delete", null));
            }

            return;
        }

        if (string.Equals(stored?.LNoteText, text, StringComparison.Ordinal))
        {
            return;
        }

        notes.LNoteSave(new LNote(entryId, text));
        changes.Add(new LRevisionChange(
            0, entryId, "note", stored is null ? "create" : "update", null));
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
                || stored[index].LInflectionSpeechId != current[index].LInflectionSpeechId
                || stored[index].LInflectionMorphology.Count != current[index].LInflectionMorphology.Count)
            {
                return false;
            }

            for (int place = 0; place < stored[index].LInflectionMorphology.Count; place++)
            {
                if (stored[index].LInflectionMorphology[place] != current[index].LInflectionMorphology[place])
                {
                    return false;
                }
            }
        }

        return true;
    }
}
