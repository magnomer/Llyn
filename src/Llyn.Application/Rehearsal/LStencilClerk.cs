using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LStencilClerk
{
    private readonly LDeckVault _lStencilClerkDecks;
    private readonly LStencilVault _lStencilClerkStencils;
    private readonly LCueVault _lStencilClerkCues;
    private readonly LEntryClerk _lStencilClerkEntries;
    private readonly LTranslationClerk _lStencilClerkTranslations;
    private readonly LReferenceClerk _lStencilClerkReferences;
    private readonly LTrailClerk _lStencilClerkTrail;

    public LStencilClerk(
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
        _lStencilClerkDecks = rig.LRigRehearsal.LRigRehearsalDecks;
        _lStencilClerkStencils = rig.LRigRehearsal.LRigRehearsalStencils;
        _lStencilClerkCues = rig.LRigRehearsal.LRigRehearsalCues;
        _lStencilClerkEntries = entries;
        _lStencilClerkTranslations = translations;
        _lStencilClerkReferences = references;
        _lStencilClerkTrail = trail;
    }

    public LCueFace LStencilClerkResolve(LCue cue, LStencil stencil, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(cue);
        ArgumentNullException.ThrowIfNull(stencil);
        ArgumentNullException.ThrowIfNull(label);

        LEntryDraft? draft =
            cue.LCueGrain != LGrain.LGrainFree && _lStencilClerkCues.LCueHolderRead(cue) is long entryId
                ? _lStencilClerkEntries.LEntryClerkLoad(entryId)
                : null;
        if (draft is null)
        {
            return new LCueFace([]);
        }

        LStencilReader reader = new(_lStencilClerkTranslations, _lStencilClerkReferences, _lStencilClerkTrail, label);
        List<LCueCell> cells = new(stencil.LStencilSlots.Count);
        foreach (LStencilSlot slot in stencil.LStencilSlots)
        {
            cells.Add(new LCueCell(
                slot.LStencilSlotColumn,
                slot.LStencilSlotIndex,
                LStencilSlotResolve(reader, draft, cue, slot)));
        }

        return new LCueFace(cells);
    }

    public LCueFace LStencilClerkResolve(long cueId, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(label);

        foreach (LDeck deck in _lStencilClerkDecks.LDeckRead())
        {
            if (_lStencilClerkCues.LCueRead(deck.LDeckId).FirstOrDefault(cue => cue.LCueId == cueId) is LCue cue)
            {
                return LStencilClerkResolve(cue, LStencilOwnedRead(deck), label);
            }
        }

        throw new LRefusal(LRefusal.LRefusalCue);
    }

    public LStencil LStencilClerkRead(long deckId)
    {
        LDeck deck = _lStencilClerkDecks.LDeckRead(deckId) ?? throw new LRefusal(LRefusal.LRefusalDeck);
        return LStencilOwnedRead(deck);
    }

    public void LStencilSlotSet(long deckId, LStencilSlot slot)
    {
        ArgumentNullException.ThrowIfNull(slot);
        ArgumentNullException.ThrowIfNull(slot.LStencilSlotBody);

        LStencil owned = LStencilClerkRead(deckId);
        List<LStencilSlot> slots = [.. owned.LStencilSlots];
        int taken = slots.FindIndex(kept =>
            kept.LStencilSlotColumn == slot.LStencilSlotColumn && kept.LStencilSlotIndex == slot.LStencilSlotIndex);
        if (taken < 0)
        {
            slots.Add(slot);
        }
        else
        {
            slots[taken] = slot;
        }

        _lStencilClerkStencils.LStencilUpdate(owned with { LStencilSlots = slots });
    }

    public void LStencilClerkSave(long deckId, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        LStencil owned = LStencilClerkRead(deckId);
        _lStencilClerkStencils.LStencilCreate(owned with
        {
            LStencilId = 0,
            LStencilName = name.Trim(),
            LStencilSaved = true,
        });
    }

    public void LStencilClerkApply(long deckId, long stencilId)
    {
        LStencil owned = LStencilClerkRead(deckId);
        LStencil saved = _lStencilClerkStencils.LStencilRead(stencilId) is { LStencilSaved: true } stored
            ? stored
            : throw new InvalidOperationException($"No saved stencil carries the id '{stencilId}'.");
        _lStencilClerkStencils.LStencilUpdate(owned with { LStencilSlots = saved.LStencilSlots });
    }

    private LStencil LStencilOwnedRead(LDeck deck)
    {
        return _lStencilClerkStencils.LStencilRead(deck.LDeckStencilId)
            ?? throw new InvalidOperationException($"No stencil carries the id '{deck.LDeckStencilId}'.");
    }

    private static string LStencilSlotResolve(LStencilReader reader, LEntryDraft draft, LCue cue, LStencilSlot slot)
    {
        bool media = slot.LStencilSlotColumn is LCueColumn.LCueColumnPicture or LCueColumn.LCueColumnSound;
        StringBuilder text = new();
        foreach (LStencilToken token in LStencilToken.LStencilTokenParse(slot.LStencilSlotBody))
        {
            if (token.LStencilTokenField is not LStencilField field)
            {
                text.Append(token.LStencilTokenLiteral);
                continue;
            }

            IReadOnlyList<string> values = reader.LStencilReaderRead(draft, cue, field);
            if (!media)
            {
                values = values.Select(LCueMarkup.LCueMarkupFormat).ToList();
            }

            text.Append(token.LStencilTokenAll
                ? string.Join('|', values)
                : values.ElementAtOrDefault((token.LStencilTokenIndex ?? 1) - 1) ?? string.Empty);
        }

        return text.ToString();
    }
}
