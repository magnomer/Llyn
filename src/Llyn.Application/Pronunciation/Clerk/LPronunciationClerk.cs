using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LPronunciationClerk
{
    private readonly LEntryVault _lPronunciationClerkEntries;
    private readonly LPronunciationVault _lPronunciationClerkPronunciations;
    private readonly LTrail _lPronunciationClerkTrail;
    private readonly string _lPronunciationClerkWorkspace;

    public LPronunciationClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lPronunciationClerkEntries = rig.LRigEntries;
        _lPronunciationClerkPronunciations = rig.LRigPronunciations;
        _lPronunciationClerkTrail = rig.LRigTrail;
        _lPronunciationClerkWorkspace = rig.LRigWorkspace;
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

    public void LPronunciationClerkSync(
        long entryId,
        IReadOnlyList<LPronunciationDraft> drafts,
        List<LRevisionDelta>? changes,
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
                changes?.Add(new LRevisionDelta(dropped, "pronunciation", "delete", row.LPronunciationIpa));
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
        LPronunciation stored, LPronunciationDraft draft, List<LRevisionDelta>? changes)
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
            changes?.Add(new LRevisionDelta(
                stored.LPronunciationId, "pronunciation", "update", current.LPronunciationIpa));
        }

        return stored.LPronunciationId;
    }

    private long LPronunciationClerkInsert(
        long entryId, LPronunciationDraft draft, List<LRevisionDelta>? changes, Dictionary<long, long> identity)
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
        changes?.Add(new LRevisionDelta(
            created.LPronunciationId, "pronunciation", "create", created.LPronunciationIpa));
        return created.LPronunciationId;
    }

    private void LAudioSync(long pronunciationId, LPronunciationDraft draft, List<LRevisionDelta>? changes)
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
        changes?.Add(new LRevisionDelta(pronunciationId, "pronunciation", "update", file));
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
