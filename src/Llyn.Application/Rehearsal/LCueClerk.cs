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
    private readonly LTranslationClerk _lCueClerkTranslations;
    private readonly LReferenceClerk _lCueClerkReferences;
    private readonly LTrailClerk _lCueClerkTrail;

    public LCueClerk(
        LRig rig,
        LEntryClerk entries,
        LTranslationClerk translations,
        LReferenceClerk references,
        LTrailClerk trail)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(translations);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(trail);
        _lCueClerkVault = rig.LRigVault;
        _lCueClerkDecks = rig.LRigRehearsal.LRigRehearsalDecks;
        _lCueClerkCues = rig.LRigRehearsal.LRigRehearsalCues;
        _lCueClerkEntries = entries;
        _lCueClerkTranslations = translations;
        _lCueClerkReferences = references;
        _lCueClerkTrail = trail;
    }

    public IReadOnlyList<LCue> LCueClerkRead(long deckId)
    {
        return _lCueClerkCues.LCueRead(deckId);
    }

    public IReadOnlyList<(LCue LCueStored, string LCueLine)> LCueLineRead(long deckId, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(label);

        LStencilReader reader = new(_lCueClerkTranslations, _lCueClerkReferences, _lCueClerkTrail, label);
        Dictionary<long, LEntryDraft?> drafts = [];
        List<(LCue LCueStored, string LCueLine)> lines = [];
        foreach (LCue cue in _lCueClerkCues.LCueRead(deckId))
        {
            lines.Add((cue, LCueLineResolve(reader, drafts, cue)));
        }

        return lines;
    }

    public IReadOnlySet<long> LCueEntryRead(long deckId)
    {
        HashSet<long> entries = [];
        foreach (LCue cue in _lCueClerkCues.LCueRead(deckId))
        {
            if (cue.LCueGrain != LGrain.LGrainFree && !cue.LCueOrphaned
                && _lCueClerkCues.LCueHolderRead(cue) is long entryId)
            {
                entries.Add(entryId);
            }
        }

        return entries;
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

    private string LCueLineResolve(LStencilReader reader, Dictionary<long, LEntryDraft?> drafts, LCue cue)
    {
        if (cue.LCueGrain == LGrain.LGrainFree)
        {
            return _lCueClerkCues.LCueCellRead(cue.LCueId)
                .Where(static cell => cell.LCueCellColumn == LCueColumn.LCueColumnText)
                .OrderBy(static cell => cell.LCueCellIndex)
                .Select(static cell => cell.LCueCellValue)
                .FirstOrDefault(static value => value.Length > 0) ?? string.Empty;
        }

        if (cue.LCueOrphaned || _lCueClerkCues.LCueHolderRead(cue) is not long entryId)
        {
            return string.Empty;
        }

        if (!drafts.TryGetValue(entryId, out LEntryDraft? draft))
        {
            draft = _lCueClerkEntries.LEntryClerkLoad(entryId);
            drafts[entryId] = draft;
        }

        LStencilField field = cue.LCueGrain switch
        {
            LGrain.LGrainMeaning => LStencilField.LStencilFieldDefinition,
            LGrain.LGrainCollocation => LStencilField.LStencilFieldExpression,
            LGrain.LGrainExample => LStencilField.LStencilFieldSentence,
            LGrain.LGrainPair => LStencilField.LStencilFieldTarget,
            _ => LStencilField.LStencilFieldHeadword,
        };
        return draft is null
            ? string.Empty
            : reader.LStencilReaderRead(draft, cue, field).FirstOrDefault() ?? string.Empty;
    }

    private static LCue LCueRowCreate(LGrain grain)
    {
        return new LCue(
            0, 0, 0, grain, null, null, null, null, null, 0, false, string.Empty, string.Empty, string.Empty);
    }
}
