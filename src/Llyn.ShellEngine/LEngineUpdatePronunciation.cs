using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private void LEnginePronunciationSync(
        long entryId,
        IReadOnlyList<LPronunciationDraft> drafts,
        List<LRevisionChange>? changes,
        Dictionary<long, long> identity)
    {
        LPronunciationArchive pronunciations = new(_lEngineDatabase);
        Dictionary<long, LPronunciation> stored = [];
        foreach (LPronunciation row in pronunciations.LPronunciationRead(entryId))
        {
            stored[row.LPronunciationId] = row;
        }

        IReadOnlyList<LPronunciationDraft> written = LEnginePronunciationScan(drafts);
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
                ? LEnginePronunciationSave(pronunciations, stored[draft.LPronunciationDraftId], draft, changes)
                : LEnginePronunciationInsert(pronunciations, entryId, draft, changes, identity);
            LEngineAudioSync(pronunciations, rowId, draft, changes);

            moved |= draft.LPronunciationDraftId <= 0
                || stored[draft.LPronunciationDraftId].LPronunciationPosition != order.Count;
            order.Add(rowId);
        }

        if (moved || stored.Count != order.Count)
        {
            pronunciations.LPronunciationOrderSet(entryId, order);
        }
    }

    private static IReadOnlyList<LPronunciationDraft> LEnginePronunciationScan(IReadOnlyList<LPronunciationDraft> drafts)
    {
        List<LPronunciationDraft> filled = [];
        foreach (LPronunciationDraft draft in drafts)
        {
            if (!draft.LPronunciationDraftSeeded || !draft.LPronunciationDraftEmpty)
            {
                filled.Add(draft with { LPronunciationDraftIpa = draft.LPronunciationDraftIpa.Trim() });
            }
        }

        return filled;
    }

    private static long LEnginePronunciationSave(
        LPronunciationArchive pronunciations,
        LPronunciation stored,
        LPronunciationDraft draft,
        List<LRevisionChange>? changes)
    {
        LPronunciation current = stored with
        {
            LPronunciationVariety = draft.LPronunciationDraftVariety.Trim().Length == 0
                ? null
                : draft.LPronunciationDraftVariety.Trim(),
            LPronunciationIpa = draft.LPronunciationDraftIpa.Length == 0 ? null : draft.LPronunciationDraftIpa,
            LPronunciationSyllables = draft.LPronunciationDraftSyllables,
        };

        if (!LEngineSoundMatch(stored, current))
        {
            pronunciations.LPronunciationUpdate(current);
            changes?.Add(new LRevisionChange(
                0, stored.LPronunciationId, "pronunciation", "update", current.LPronunciationIpa));
        }

        return stored.LPronunciationId;
    }

    private static long LEnginePronunciationInsert(
        LPronunciationArchive pronunciations,
        long entryId,
        LPronunciationDraft draft,
        List<LRevisionChange>? changes,
        Dictionary<long, long> identity)
    {
        LPronunciation created = pronunciations.LPronunciationCreate(new LPronunciation(
            0,
            entryId,
            0,
            draft.LPronunciationDraftVariety,
            draft.LPronunciationDraftIpa.Length == 0 ? null : draft.LPronunciationDraftIpa,
            draft.LPronunciationDraftSyllables));

        LEngineIdentityRecord(identity, draft.LPronunciationDraftId, created.LPronunciationId);
        changes?.Add(new LRevisionChange(
            0, created.LPronunciationId, "pronunciation", "create", created.LPronunciationIpa));
        return created.LPronunciationId;
    }

    private void LEngineAudioSync(
        LPronunciationArchive pronunciations,
        long pronunciationId,
        LPronunciationDraft draft,
        List<LRevisionChange>? changes)
    {
        if (draft.LPronunciationDraftAudio.Length == 0)
        {
            return;
        }

        string file = LEngineRecordingFormat(draft.LPronunciationDraftAudio);
        LPronunciationAudio? audio = pronunciations.LPronunciationAudioRead(pronunciationId);
        if (string.Equals(audio?.LPronunciationAudioFile, file, StringComparison.Ordinal)
            && string.Equals(audio?.LPronunciationAudioSource, draft.LPronunciationDraftSource, StringComparison.Ordinal))
        {
            return;
        }

        pronunciations.LPronunciationAudioSave(pronunciationId, file, draft.LPronunciationDraftSource);
        changes?.Add(new LRevisionChange(0, pronunciationId, "pronunciation", "update", file));
    }

    private static bool LEngineSoundMatch(LPronunciation stored, LPronunciation current)
    {
        if (!string.Equals(stored.LPronunciationIpa, current.LPronunciationIpa, StringComparison.Ordinal)
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
