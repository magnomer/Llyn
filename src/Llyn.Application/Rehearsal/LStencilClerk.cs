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

    public IReadOnlyList<LStencil> LStencilClerkRead()
    {
        return _lStencilClerkStencils.LStencilRead();
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

        LStencilSlotSet(LStencilClerkRead(deckId), slot);
    }

    public static bool LStencilClerkCheck(string body)
    {
        return LStencilBody.LStencilBodyCheck(body);
    }

    public static IReadOnlyList<LStencilToken> LStencilClerkParse(string body)
    {
        return LStencilToken.LStencilTokenParse(body);
    }

    public void LStencilFieldAppend(long deckId, int index, LStencilField field) =>
        LStencilSlotSet(deckId, index, body => LStencilBody.LStencilBodyAppend(body, field));

    public void LStencilFieldRemove(long deckId, int index, int place) =>
        LStencilSlotSet(deckId, index, body => LStencilBody.LStencilBodyRemove(body, place));

    public void LStencilJoinSet(long deckId, int index, int place, bool all) =>
        LStencilSlotSet(deckId, index, body => LStencilBody.LStencilBodySet(body, place, all));

    public void LStencilMediaSet(long deckId, LCueColumn column, int index)
    {
        if (column is not (LCueColumn.LCueColumnPicture or LCueColumn.LCueColumnSound))
        {
            throw new ArgumentOutOfRangeException(nameof(column), column, "Only a picture or sound slot moves sides.");
        }

        ArgumentOutOfRangeException.ThrowIfNegative(index);

        LStencil owned = LStencilClerkRead(deckId);
        List<LStencilSlot> slots = [.. owned.LStencilSlots];
        int first = slots.FindIndex(kept => kept.LStencilSlotColumn == column);
        string body = first < 0 ? LStencilDefault.LStencilDefaultRead(column) : slots[first].LStencilSlotBody;
        slots.RemoveAll(kept => kept.LStencilSlotColumn == column);
        if (index > 0)
        {
            slots.Insert(first < 0 ? slots.Count : first, new LStencilSlot(column, index, body));
        }

        _lStencilClerkStencils.LStencilUpdate(owned with { LStencilSlots = slots });
    }

    private void LStencilSlotSet(long deckId, int index, Func<string, string> rule)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(index);

        LStencil owned = LStencilClerkRead(deckId);
        string body = owned.LStencilSlots.FirstOrDefault(kept =>
            kept.LStencilSlotColumn == LCueColumn.LCueColumnText && kept.LStencilSlotIndex == index)
            ?.LStencilSlotBody ?? string.Empty;
        LStencilSlotSet(owned, new LStencilSlot(LCueColumn.LCueColumnText, index, rule(body)));
    }

    private void LStencilSlotSet(LStencil owned, LStencilSlot slot)
    {
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
