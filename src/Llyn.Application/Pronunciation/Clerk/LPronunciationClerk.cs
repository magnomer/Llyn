using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LPronunciationClerk
{
    private readonly LEntryVault _lPronunciationClerkEntries;
    private readonly LNoteVault _lPronunciationClerkNotes;
    private readonly LPronunciationVault _lPronunciationClerkPronunciations;
    private readonly LTrail _lPronunciationClerkTrail;
    private readonly string _lPronunciationClerkWorkspace;

    public LPronunciationClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lPronunciationClerkEntries = rig.LRigEntries;
        _lPronunciationClerkNotes = rig.LRigNotes;
        _lPronunciationClerkPronunciations = rig.LRigPronunciations;
        _lPronunciationClerkTrail = rig.LRigTrail;
        _lPronunciationClerkWorkspace = rig.LRigWorkspace;
    }

    public LPronunciation LPronunciationClerkCreate(LPronunciation pronunciation)
    {
        ArgumentNullException.ThrowIfNull(pronunciation);
        LPronunciation created = _lPronunciationClerkPronunciations.LPronunciationCreate(pronunciation);
        _lPronunciationClerkEntries.LEntryUpdatedSet(created.LPronunciationEntryId);
        return created;
    }

    public IReadOnlyList<LPronunciation> LPronunciationClerkRead(long entryId)
    {
        return _lPronunciationClerkPronunciations.LPronunciationRead(entryId);
    }

    public IReadOnlyList<LCatalogPronunciation> LPronunciationClerkFind(string query, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(query);

        LPronunciationVault pronunciations = _lPronunciationClerkPronunciations;
        List<LCatalogPronunciation> rows = [];
        foreach (LEntry entry in _lPronunciationClerkEntries.LEntryFind(query))
        {
            IReadOnlyList<LPronunciation> spoken = pronunciations.LPronunciationRead(entry.LEntryId);
            rows.Add(LCatalogPronunciation.LCatalogPronunciationCreate(
                entry,
                spoken.Count == 0 ? null : spoken[0].LPronunciationIpa));
        }

        return LCatalogPronunciation.LCatalogPronunciationSort(rows, order);
    }

    public void LPronunciationClerkUpdate(LPronunciation pronunciation)
    {
        ArgumentNullException.ThrowIfNull(pronunciation);
        _lPronunciationClerkPronunciations.LPronunciationUpdate(pronunciation);
        _lPronunciationClerkEntries.LEntryUpdatedSet(pronunciation.LPronunciationEntryId);
    }

    public void LPronunciationClerkDelete(long id)
    {
        LPronunciationVault pronunciations = _lPronunciationClerkPronunciations;
        long? entryId = pronunciations.LPronunciationHolderRead(id);
        pronunciations.LPronunciationDelete(id);
        if (entryId is long held)
        {
            _lPronunciationClerkEntries.LEntryUpdatedSet(held);
        }
    }

    public void LAudioSave(long pronunciationId, string file, string? source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        LPronunciationVault pronunciations = _lPronunciationClerkPronunciations;
        pronunciations.LPronunciationAudioSave(pronunciationId, LRecordingFormat(file), source);
        if (pronunciations.LPronunciationHolderRead(pronunciationId) is long held)
        {
            _lPronunciationClerkEntries.LEntryUpdatedSet(held);
        }
    }

    public LPronunciationAudio? LAudioRead(long pronunciationId)
    {
        return _lPronunciationClerkPronunciations.LPronunciationAudioRead(pronunciationId);
    }

    public void LNoteSave(LNote note)
    {
        ArgumentNullException.ThrowIfNull(note);
        _lPronunciationClerkNotes.LNoteSave(note with { LNoteText = LMarkdown.LMarkdownNormalize(note.LNoteText) });
        _lPronunciationClerkEntries.LEntryUpdatedSet(note.LNoteEntryId);
    }

    public LNote? LNoteRead(long entryId)
    {
        return _lPronunciationClerkNotes.LNoteRead(entryId);
    }

    public void LNoteDelete(long entryId)
    {
        _lPronunciationClerkNotes.LNoteDelete(entryId);
        _lPronunciationClerkEntries.LEntryUpdatedSet(entryId);
    }

    public void LPronunciationClerkSync(
        long entryId,
        IReadOnlyList<LPronunciationDraft> drafts,
        List<LRevisionChange>? changes,
        Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(identity);

        LPronunciationVault pronunciations = _lPronunciationClerkPronunciations;
        Dictionary<long, LPronunciation> stored = [];
        foreach (LPronunciation row in pronunciations.LPronunciationRead(entryId))
        {
            stored[row.LPronunciationId] = row;
        }

        IReadOnlyList<LPronunciationDraft> written = LPronunciationClerkScan(drafts);
        HashSet<long> named = [];
        foreach (LPronunciationDraft draft in written)
        {
            if (draft.LPronunciationDraftId > 0)
            {
                if (!stored.ContainsKey(draft.LPronunciationDraftId))
                {
                    throw new LRefusal(LRefusal.LRefusalLink);
                }

                named.Add(draft.LPronunciationDraftId);
            }
        }

        foreach ((long dropped, LPronunciation row) in stored)
        {
            if (!named.Contains(dropped))
            {
                pronunciations.LPronunciationDelete(dropped);
                changes?.Add(new LRevisionChange(0, dropped, "pronunciation", "delete", row.LPronunciationIpa));
            }
        }

        List<long> order = new(written.Count);
        bool moved = false;
        foreach (LPronunciationDraft draft in written)
        {
            long rowId = draft.LPronunciationDraftId > 0
                ? LPronunciationClerkSave(stored[draft.LPronunciationDraftId], draft, changes)
                : LPronunciationClerkInsert(entryId, draft, changes, identity);
            LAudioSync(rowId, draft, changes);

            moved |= draft.LPronunciationDraftId <= 0
                || stored[draft.LPronunciationDraftId].LPronunciationPosition != order.Count;
            order.Add(rowId);
        }

        if (moved || stored.Count != order.Count)
        {
            pronunciations.LPronunciationOrderSet(entryId, order);
        }
    }

    public static IReadOnlyList<LPronunciationDraft> LPronunciationClerkScan(IReadOnlyList<LPronunciationDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        List<LPronunciationDraft> filled = [];
        foreach (LPronunciationDraft draft in drafts)
        {
            if (!draft.LPronunciationDraftSeeded || !draft.LPronunciationDraftEmpty)
            {
                filled.Add(draft with
                {
                    LPronunciationDraftIpa = draft.LPronunciationDraftIpa.Trim(),
                    LPronunciationDraftRespelling = draft.LPronunciationDraftRespelling.Trim(),
                });
            }
        }

        return filled;
    }

    private long LPronunciationClerkSave(
        LPronunciation stored, LPronunciationDraft draft, List<LRevisionChange>? changes)
    {
        LPronunciation current = stored with
        {
            LPronunciationVariety = draft.LPronunciationDraftVariety.Trim().Length == 0
                ? null
                : draft.LPronunciationDraftVariety.Trim(),
            LPronunciationIpa = draft.LPronunciationDraftIpa.Length == 0 ? null : draft.LPronunciationDraftIpa,
            LPronunciationSyllables = draft.LPronunciationDraftSyllables,
            LPronunciationRespelling = draft.LPronunciationDraftRespelling.Length == 0
                ? null
                : draft.LPronunciationDraftRespelling,
        };

        if (!LPronunciationClerkMatch(stored, current))
        {
            _lPronunciationClerkPronunciations.LPronunciationUpdate(current);
            changes?.Add(new LRevisionChange(
                0, stored.LPronunciationId, "pronunciation", "update", current.LPronunciationIpa));
        }

        return stored.LPronunciationId;
    }

    private long LPronunciationClerkInsert(
        long entryId, LPronunciationDraft draft, List<LRevisionChange>? changes, Dictionary<long, long> identity)
    {
        LPronunciation created = _lPronunciationClerkPronunciations.LPronunciationCreate(new LPronunciation(
            0,
            entryId,
            0,
            draft.LPronunciationDraftVariety,
            draft.LPronunciationDraftIpa.Length == 0 ? null : draft.LPronunciationDraftIpa,
            draft.LPronunciationDraftSyllables,
            draft.LPronunciationDraftRespelling.Length == 0 ? null : draft.LPronunciationDraftRespelling));

        LIdentity.LIdentityRecord(identity, draft.LPronunciationDraftId, created.LPronunciationId);
        changes?.Add(new LRevisionChange(
            0, created.LPronunciationId, "pronunciation", "create", created.LPronunciationIpa));
        return created.LPronunciationId;
    }

    private void LAudioSync(long pronunciationId, LPronunciationDraft draft, List<LRevisionChange>? changes)
    {
        if (draft.LPronunciationDraftAudio.Length == 0)
        {
            return;
        }

        LPronunciationVault pronunciations = _lPronunciationClerkPronunciations;
        string file = LRecordingFormat(draft.LPronunciationDraftAudio);
        LPronunciationAudio? audio = pronunciations.LPronunciationAudioRead(pronunciationId);
        if (string.Equals(audio?.LPronunciationAudioFile, file, StringComparison.Ordinal)
            && string.Equals(
                audio?.LPronunciationAudioSource, draft.LPronunciationDraftSource, StringComparison.Ordinal))
        {
            return;
        }

        pronunciations.LPronunciationAudioSave(pronunciationId, file, draft.LPronunciationDraftSource);
        changes?.Add(new LRevisionChange(0, pronunciationId, "pronunciation", "update", file));
    }

    private string LRecordingFormat(string path)
    {
        return _lPronunciationClerkTrail.LTrailRelativeResolve(_lPronunciationClerkWorkspace, path) ?? path;
    }

    private static bool LPronunciationClerkMatch(LPronunciation stored, LPronunciation current)
    {
        if (!string.Equals(stored.LPronunciationIpa, current.LPronunciationIpa, StringComparison.Ordinal)
            || !string.Equals(
                stored.LPronunciationRespelling, current.LPronunciationRespelling, StringComparison.Ordinal)
            || !string.Equals(stored.LPronunciationVariety, current.LPronunciationVariety, StringComparison.Ordinal)
            || stored.LPronunciationSyllables.Count != current.LPronunciationSyllables.Count)
        {
            return false;
        }

        for (int index = 0; index < stored.LPronunciationSyllables.Count; index++)
        {
            if (stored.LPronunciationSyllables[index] with { LSyllablePronunciationId = 0 }
                != current.LPronunciationSyllables[index] with { LSyllablePronunciationId = 0 })
            {
                return false;
            }
        }

        return true;
    }
}
