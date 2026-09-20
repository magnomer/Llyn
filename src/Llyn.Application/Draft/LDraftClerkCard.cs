using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public static class LDraftClerkCard
{
    public static LEntryDraft LCardInsert(
        LEntryDraft content, LCardKind kind, long parentId, int position, LCardDraft card)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (kind == LCardKind.LCardKindCollocation)
        {
            if (parentId != 0)
            {
                throw new LRefusal(LRefusal.LRefusalCollocation);
            }

            return content with
            {
                LEntryDraftCollocations = LCardInsert(content.LEntryDraftCollocations, card, position),
            };
        }

        if (parentId == 0)
        {
            return content with
            {
                LEntryDraftMeanings = LCardInsert(content.LEntryDraftMeanings, card, position),
            };
        }

        IReadOnlyList<LCardDraft> meanings = LCardChange(
            content.LEntryDraftMeanings,
            parentId,
            parent => parent with { LCardDraftChild = LCardInsert(parent.LCardDraftChild, card, position) })
            ?? throw new LRefusal(LRefusal.LRefusalCard);

        return content with { LEntryDraftMeanings = meanings };
    }

    private static IReadOnlyList<LCardDraft> LCardInsert(IReadOnlyList<LCardDraft> cards, LCardDraft card, int position)
    {
        List<LCardDraft> written = new(cards);
        written.Insert(Math.Clamp(position, 0, written.Count), card);
        return LCardPositionUpdate(written);
    }

    public static LEntryDraft LCardRemove(LEntryDraft content, long id)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (id == 0)
        {
            throw new LRefusal(LRefusal.LRefusalCard);
        }

        IReadOnlyList<LCardDraft>? meanings = LCardRemove(content.LEntryDraftMeanings, id, out _);
        if (meanings is not null)
        {
            return content with { LEntryDraftMeanings = meanings };
        }

        IReadOnlyList<LCardDraft> collocations = LCardRemove(content.LEntryDraftCollocations, id, out _)
            ?? throw new LRefusal(LRefusal.LRefusalCard);

        return content with { LEntryDraftCollocations = collocations };
    }

    public static LEntryDraft LCardMove(LEntryDraft content, LRequestCardShift request)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(request);

        if (request.LRequestCardId == 0)
        {
            throw new LRefusal(LRefusal.LRefusalCard);
        }

        LCardDraft? moved;
        LCardKind kind = LCardKind.LCardKindMeaning;
        IReadOnlyList<LCardDraft>? cards = LCardRemove(content.LEntryDraftMeanings, request.LRequestCardId, out moved);
        if (cards is not null)
        {
            content = content with { LEntryDraftMeanings = cards };
        }
        else
        {
            kind = LCardKind.LCardKindCollocation;
            cards = LCardRemove(content.LEntryDraftCollocations, request.LRequestCardId, out moved)
                ?? throw new LRefusal(LRefusal.LRefusalCard);
            content = content with { LEntryDraftCollocations = cards };
        }

        return LCardInsert(content, kind, request.LRequestParentId, request.LRequestPosition, moved!);
    }

    private static IReadOnlyList<LCardDraft>? LCardRemove(
        IReadOnlyList<LCardDraft> cards, long id, out LCardDraft? removed)
    {
        for (int index = 0; index < cards.Count; index++)
        {
            LCardDraft card = cards[index];
            if (card.LCardDraftId == id)
            {
                removed = card;
                List<LCardDraft> written = new(cards);
                written.RemoveAt(index);
                return LCardPositionUpdate(written);
            }

            IReadOnlyList<LCardDraft>? children = LCardRemove(card.LCardDraftChild, id, out removed);
            if (children is null)
            {
                continue;
            }

            List<LCardDraft> kept = new(cards);
            kept[index] = card with { LCardDraftChild = children };
            return kept;
        }

        removed = null;
        return null;
    }

    public static LEntryDraft LCardChange(LEntryDraft content, long id, Func<LCardDraft, LCardDraft> change)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (id == 0)
        {
            throw new LRefusal(LRefusal.LRefusalCard);
        }

        IReadOnlyList<LCardDraft>? meanings = LCardChange(content.LEntryDraftMeanings, id, change);
        if (meanings is not null)
        {
            return content with { LEntryDraftMeanings = meanings };
        }

        IReadOnlyList<LCardDraft> collocations = LCardChange(content.LEntryDraftCollocations, id, change)
            ?? throw new LRefusal(LRefusal.LRefusalCard);

        return content with { LEntryDraftCollocations = collocations };
    }

    private static IReadOnlyList<LCardDraft>? LCardChange(
        IReadOnlyList<LCardDraft> cards, long id, Func<LCardDraft, LCardDraft> change)
    {
        for (int index = 0; index < cards.Count; index++)
        {
            LCardDraft card = cards[index];
            LCardDraft? written = null;

            if (card.LCardDraftId == id)
            {
                written = change(card);
            }
            else if (LCardChange(card.LCardDraftChild, id, change) is IReadOnlyList<LCardDraft> children)
            {
                written = card with { LCardDraftChild = children };
            }

            if (written is null)
            {
                continue;
            }

            List<LCardDraft> kept = new(cards);
            kept[index] = written;
            return kept;
        }

        return null;
    }

    public static LCardDraft? LCardFind(LEntryDraft content, long id)
    {
        ArgumentNullException.ThrowIfNull(content);

        return LCardFind(content.LEntryDraftMeanings, id)
            ?? LCardFind(content.LEntryDraftCollocations, id);
    }

    private static LCardDraft? LCardFind(IReadOnlyList<LCardDraft> cards, long id)
    {
        foreach (LCardDraft card in cards)
        {
            if (card.LCardDraftId == id)
            {
                return card;
            }

            if (LCardFind(card.LCardDraftChild, id) is LCardDraft nested)
            {
                return nested;
            }
        }

        return null;
    }

    private static IReadOnlyList<LCardDraft> LCardPositionUpdate(List<LCardDraft> cards)
    {
        for (int index = 0; index < cards.Count; index++)
        {
            if (cards[index].LCardDraftPosition != index + 1)
            {
                cards[index] = cards[index] with { LCardDraftPosition = index + 1 };
            }
        }

        return cards;
    }
}
