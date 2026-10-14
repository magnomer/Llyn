using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LCueClerk
{
    private readonly LVault _lCueClerkVault;
    private readonly LDeckVault _lCueClerkDecks;
    private readonly LCueVault _lCueClerkCues;
    private readonly LEntryClerk _lCueClerkEntries;

    public LCueClerk(LRig rig, LEntryClerk entries)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(entries);
        _lCueClerkVault = rig.LRigVault;
        _lCueClerkDecks = rig.LRigRehearsal.LRigRehearsalDecks;
        _lCueClerkCues = rig.LRigRehearsal.LRigRehearsalCues;
        _lCueClerkEntries = entries;
    }

    public IReadOnlyList<LCue> LCueClerkRead(long deckId)
    {
        return _lCueClerkCues.LCueRead(deckId);
    }

    public IReadOnlyList<LCue> LCueClerkAdd(long deckId, IReadOnlyList<long> entryIds)
    {
        ArgumentNullException.ThrowIfNull(entryIds);

        LDeck deck = _lCueClerkDecks.LDeckRead(deckId) ?? throw new LRefusal(LRefusal.LRefusalDeck);
        var bound = _lCueClerkCues.LCueRead(deckId)
            .Select(static cue => (cue.LCueGrain, cue.LCueEntryId, cue.LCueMeaningId, cue.LCueCollocationId,
                cue.LCueExampleId, cue.LCueTargetId))
            .ToHashSet();

        List<LCue> added = [];
        foreach (long entryId in entryIds.Distinct())
        {
            if (_lCueClerkEntries.LEntryClerkLoad(entryId) is not LEntryDraft draft)
            {
                continue;
            }

            foreach (LCue row in LCueClerkScan(draft, deck.LDeckGrain))
            {
                LCue cue = row.LCueGrain == LGrain.LGrainEntry ? row with { LCueEntryId = entryId } : row;
                if (bound.Add((cue.LCueGrain, cue.LCueEntryId, cue.LCueMeaningId, cue.LCueCollocationId,
                    cue.LCueExampleId, cue.LCueTargetId)))
                {
                    added.Add(cue);
                }
            }
        }

        return added.Count == 0 ? [] : _lCueClerkCues.LCueCreate(deckId, added);
    }

    public static IReadOnlyList<LCue> LCueClerkScan(LEntryDraft draft, LGrain grain)
    {
        ArgumentNullException.ThrowIfNull(draft);

        List<LCardDraft> meanings = [];
        Stack<LCardDraft> pending = new(draft.LEntryDraftMeanings.Reverse());
        while (pending.Count > 0)
        {
            LCardDraft card = pending.Pop();
            meanings.Add(card);
            foreach (LCardDraft child in card.LCardDraftChild.Reverse())
            {
                pending.Push(child);
            }
        }

        IReadOnlyList<LCardDraft> collocations = draft.LEntryDraftCollocations;
        return grain switch
        {
            LGrain.LGrainEntry => [LCueRowCreate(grain)],
            LGrain.LGrainMeaning => meanings
                .Select(card => LCueRowCreate(grain) with { LCueMeaningId = card.LCardDraftId })
                .ToList(),
            LGrain.LGrainCollocation => collocations
                .Select(card => LCueRowCreate(grain) with { LCueCollocationId = card.LCardDraftId })
                .ToList(),
            LGrain.LGrainExample => meanings.Concat(collocations)
                .SelectMany(static card => card.LCardDraftSentence)
                .Select(static sentence => sentence.LSentenceDraftExample?.LExampleDraftId ?? 0)
                .Where(static id => id > 0)
                .Distinct()
                .Select(id => LCueRowCreate(grain) with { LCueExampleId = id })
                .ToList(),
            LGrain.LGrainPair =>
            [
                .. meanings.SelectMany(card => card.LCardDraftTranslation.Where(static id => id > 0).Distinct()
                    .Select(id => LCueRowCreate(grain) with { LCueMeaningId = card.LCardDraftId, LCueTargetId = id })),
                .. collocations.SelectMany(card => card.LCardDraftTranslation.Where(static id => id > 0).Distinct()
                    .Select(id => LCueRowCreate(grain) with
                    {
                        LCueCollocationId = card.LCardDraftId,
                        LCueTargetId = id,
                    })),
            ],
            _ => [],
        };
    }

    public LCue LCueClerkCreate(long deckId, IReadOnlyList<LCueCell> cells)
    {
        ArgumentNullException.ThrowIfNull(cells);

        if (_lCueClerkDecks.LDeckRead(deckId) is null)
        {
            throw new LRefusal(LRefusal.LRefusalDeck);
        }

        using LVaultSession session = _lCueClerkVault.LVaultSessionStart();

        LCue created = _lCueClerkCues.LCueCreate(deckId, [LCueRowCreate(LGrain.LGrainFree)])[0];
        _lCueClerkCues.LCueCellSave(created.LCueId, cells);

        session.LVaultSessionCommit();
        return created;
    }

    public IReadOnlyList<LCueCell> LCueCellRead(long cueId)
    {
        return _lCueClerkCues.LCueCellRead(cueId);
    }

    public void LCueCellSave(long cueId, IReadOnlyList<LCueCell> cells)
    {
        ArgumentNullException.ThrowIfNull(cells);

        LCue cue = LCueClerkFind(cueId) ?? throw new LRefusal(LRefusal.LRefusalCue);
        if (cue.LCueGrain != LGrain.LGrainFree)
        {
            throw new LRefusal(LRefusal.LRefusalBound);
        }

        _lCueClerkCues.LCueCellSave(cueId, cells);
    }

    public void LCueClerkRemove(IReadOnlyList<long> cueIds)
    {
        ArgumentNullException.ThrowIfNull(cueIds);
        _lCueClerkCues.LCueDelete(cueIds);
    }

    public void LCueClerkMove(long deckId, long cueId, int position)
    {
        List<long> order = _lCueClerkCues.LCueRead(deckId).Select(static cue => cue.LCueId).ToList();
        if (!order.Remove(cueId))
        {
            return;
        }

        order.Insert(Math.Clamp(position, 0, order.Count), cueId);
        _lCueClerkCues.LCueOrderSet(deckId, order);
    }

    public void LCueClerkSet(long cueId, int flag, bool excluded)
    {
        LCue cue = LCueClerkFind(cueId) ?? throw new LRefusal(LRefusal.LRefusalCue);
        _lCueClerkCues.LCueUpdate(cue with { LCueFlag = flag, LCueExcluded = excluded });
    }

    private LCue? LCueClerkFind(long cueId)
    {
        foreach (LDeck deck in _lCueClerkDecks.LDeckRead())
        {
            if (_lCueClerkCues.LCueRead(deck.LDeckId).FirstOrDefault(cue => cue.LCueId == cueId) is LCue cue)
            {
                return cue;
            }
        }

        return null;
    }

    private static LCue LCueRowCreate(LGrain grain)
    {
        return new LCue(
            0, 0, 0, grain, null, null, null, null, null, 0, false, string.Empty, string.Empty, string.Empty);
    }
}
