using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public static class LDraftClerkList
{
    public static IReadOnlyList<LDraftItem> LDraftListAdd<LDraftItem>(
        IReadOnlyList<LDraftItem> items, LDraftItem item, int position)
    {
        ArgumentNullException.ThrowIfNull(items);

        List<LDraftItem> written = new(items);
        written.Insert(Math.Clamp(position, 0, written.Count), item);
        return written;
    }

    public static IReadOnlyList<LDraftItem> LDraftListInsert<LDraftItem>(
        IReadOnlyList<LDraftItem> items, LDraftItem item, long id, int position, Func<LDraftItem, long> key)
    {
        return LDraftListFind(items, id, key) < 0 ? LDraftListAdd(items, item, position) : items;
    }

    public static IReadOnlyList<LDraftItem> LDraftListChange<LDraftItem>(
        IReadOnlyList<LDraftItem> items,
        LDraftItem item,
        long id,
        int position,
        long former,
        Func<LDraftItem, long> key)
    {
        if (id == former)
        {
            return items;
        }

        IReadOnlyList<LDraftItem> written = LDraftListInsert(items, item, id, position, key);
        if (former == 0 || LDraftListFind(written, former, key) < 0)
        {
            return written;
        }

        return LDraftListRemove(written, former, key);
    }

    public static IReadOnlyList<LDraftItem> LDraftListRemove<LDraftItem>(
        IReadOnlyList<LDraftItem> items, long id, Func<LDraftItem, long> key)
    {
        int index = LDraftListFind(items, id, key);
        if (index < 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        List<LDraftItem> written = new(items);
        written.RemoveAt(index);
        return written;
    }

    public static IReadOnlyList<LDraftItem> LDraftListMove<LDraftItem>(
        IReadOnlyList<LDraftItem> items, long id, int position, Func<LDraftItem, long> key)
    {
        int index = LDraftListFind(items, id, key);
        if (index < 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        List<LDraftItem> written = new(items);
        LDraftItem moved = written[index];
        written.RemoveAt(index);
        written.Insert(Math.Clamp(position, 0, written.Count), moved);
        return written;
    }

    public static IReadOnlyList<LDraftItem>? LDraftListChange<LDraftItem>(
        IReadOnlyList<LDraftItem> items, long id, Func<LDraftItem, long> key, Func<LDraftItem, LDraftItem> change)
    {
        ArgumentNullException.ThrowIfNull(change);

        int index = LDraftListFind(items, id, key);
        if (index < 0)
        {
            return null;
        }

        List<LDraftItem> written = new(items);
        written[index] = change(written[index]);
        return written;
    }

    public static int LDraftListFind<LDraftItem>(
        IReadOnlyList<LDraftItem> items, long id, Func<LDraftItem, long> key)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(key);

        if (id == 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        for (int index = 0; index < items.Count; index++)
        {
            if (key(items[index]) == id)
            {
                return index;
            }
        }

        return -1;
    }

    public static LEntryDraft LDraftRowChange(LEntryDraft content, Func<LCardDraft, LCardDraft?> change)
    {
        ArgumentNullException.ThrowIfNull(content);

        bool found = false;
        IReadOnlyList<LCardDraft> meanings = LDraftRowChange(content.LEntryDraftMeanings, change, ref found);
        IReadOnlyList<LCardDraft> collocations = LDraftRowChange(content.LEntryDraftCollocations, change, ref found);

        if (!found)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        return content with { LEntryDraftMeanings = meanings, LEntryDraftCollocations = collocations };
    }

    private static IReadOnlyList<LCardDraft> LDraftRowChange(
        IReadOnlyList<LCardDraft> cards, Func<LCardDraft, LCardDraft?> change, ref bool found)
    {
        List<LCardDraft>? written = null;
        for (int index = 0; index < cards.Count; index++)
        {
            LCardDraft card = cards[index];
            LCardDraft? changed = change(card);
            if (changed is not null)
            {
                found = true;
            }

            IReadOnlyList<LCardDraft> children = LDraftRowChange(card.LCardDraftChild, change, ref found);
            if (!ReferenceEquals(children, card.LCardDraftChild))
            {
                changed = (changed ?? card) with { LCardDraftChild = children };
            }

            if (changed is null)
            {
                continue;
            }

            written ??= new List<LCardDraft>(cards);
            written[index] = changed;
        }

        return written ?? cards;
    }
}
