using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public static class LDraftClerkCard
{
    public static LEntryDraft? LCardApply(LEntryDraft content, LRequest request, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(content);

        return request switch
        {
            LRequestCardAddition sent => LCardInsert(content, sent, identity),
            LRequestCardRemoval sent => LCardRemove(content, sent.LRequestKind, sent.LRequestCardId),
            LRequestCardShift sent => LCardMove(content, sent),
            LRequestCardTitle sent => LCardChange(
                content,
                sent.LRequestKind, sent.LRequestCardId,
                card => card with { LCardDraftTitle = LStateValue.LStateValueRead(sent.LRequestValue) }),
            LRequestCardExpression sent => LCardExpressionChange(content, sent),
            LRequestCardMeaning sent => LCardChange(
                content,
                sent.LRequestKind, sent.LRequestCardId,
                card => card with { LCardDraftMeaning = LStateValue.LStateValueRead(sent.LRequestValue) }),
            LRequestSenseAddition sent => LDraftClerkSense.LCardSenseAttach(
                content, sent.LRequestCardId, sent.LRequestSenseId),
            LRequestSenseRemoval sent => LDraftClerkSense.LCardSenseDetach(
                content, sent.LRequestCardId, sent.LRequestSenseId),
            _ => null,
        };
    }

    private static LEntryDraft LCardExpressionChange(LEntryDraft content, LRequestCardExpression request)
    {
        LStateValue expression = LStateValue.LStateValueRead(request.LRequestValue);
        (LCardKind kind, long id) = (request.LRequestKind, request.LRequestCardId);
        if (kind == LCardKind.LCardKindCollocation)
        {
            return LCardChange(content, kind, id, card => card with { LCardDraftExpression = expression });
        }

        return expression.LStateValueEmpty && LCardFind(content, kind, id) is not null
            ? content
            : throw new LRefusal(LRefusal.LRefusalCard);
    }

    public static LEntryDraft LCardInsert(LEntryDraft content, LRequestCardAddition request, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(identity);

        LCardDraft card = new(
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            0,
            identity.LIdentityCreate());

        return LCardInsert(
            content, request.LRequestKind, request.LRequestParentId, request.LRequestPosition, card);
    }

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

    public static LEntryDraft LCardRemove(LEntryDraft content, LCardKind kind, long id)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (id == 0)
        {
            throw new LRefusal(LRefusal.LRefusalCard);
        }

        if (kind == LCardKind.LCardKindCollocation)
        {
            IReadOnlyList<LCardDraft> collocations = LCardRemove(content.LEntryDraftCollocations, id, out _)
                ?? throw new LRefusal(LRefusal.LRefusalCard);

            return content with { LEntryDraftCollocations = collocations };
        }

        IReadOnlyList<LCardDraft> meanings = LCardRemove(content.LEntryDraftMeanings, id, out LCardDraft? removed)
            ?? throw new LRefusal(LRefusal.LRefusalCard);

        return LDraftClerkSense.LCardSenseSweep(content with { LEntryDraftMeanings = meanings }, removed!);
    }

    public static LEntryDraft LCardMove(LEntryDraft content, LRequestCardShift request)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(request);

        if (request.LRequestCardId == 0)
        {
            throw new LRefusal(LRefusal.LRefusalCard);
        }

        LCardKind kind = request.LRequestKind;
        IReadOnlyList<LCardDraft> cards = LCardRemove(
            LCardListRead(content, kind), request.LRequestCardId, out LCardDraft? moved)
            ?? throw new LRefusal(LRefusal.LRefusalCard);
        content = kind == LCardKind.LCardKindCollocation
            ? content with { LEntryDraftCollocations = cards }
            : content with { LEntryDraftMeanings = cards };

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

    public static LEntryDraft LCardChange(
        LEntryDraft content, LCardKind kind, long id, Func<LCardDraft, LCardDraft> change)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (id == 0)
        {
            throw new LRefusal(LRefusal.LRefusalCard);
        }

        if (kind == LCardKind.LCardKindCollocation)
        {
            return content with
            {
                LEntryDraftCollocations = LCardChange(content.LEntryDraftCollocations, id, change)
                    ?? throw new LRefusal(LRefusal.LRefusalCard),
            };
        }

        return content with
        {
            LEntryDraftMeanings = LCardChange(content.LEntryDraftMeanings, id, change)
                ?? throw new LRefusal(LRefusal.LRefusalCard),
        };
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

    public static int LCardEndRead(LEntryDraft content, LCardKind kind)
    {
        ArgumentNullException.ThrowIfNull(content);

        return kind == LCardKind.LCardKindCollocation
            ? content.LEntryDraftCollocations.Count
            : content.LEntryDraftMeanings.Count;
    }

    public static bool LCardLoneCheck(LEntryDraft content, LCardKind kind, long id)
    {
        ArgumentNullException.ThrowIfNull(content);

        return LCardLoneCheck(
            kind == LCardKind.LCardKindCollocation ? content.LEntryDraftCollocations : content.LEntryDraftMeanings,
            id);
    }

    private static bool LCardLoneCheck(IReadOnlyList<LCardDraft> cards, long id)
    {
        return cards.Count == 1 && cards[0].LCardDraftId == id;
    }

    public static int? LCardShiftRead(LEntryDraft content, LCardKind kind, long id, int place)
    {
        ArgumentNullException.ThrowIfNull(content);

        IReadOnlyList<LCardDraft> cards = LCardListRead(content, kind);
        int current = LCardPlaceRead(cards, id);
        return current < 0 || place < 0 || place >= cards.Count || place == current ? null : place;
    }

    private static IReadOnlyList<LCardDraft> LCardListRead(LEntryDraft content, LCardKind kind)
    {
        return kind == LCardKind.LCardKindCollocation
            ? content.LEntryDraftCollocations
            : content.LEntryDraftMeanings;
    }

    private static int LCardPlaceRead(IReadOnlyList<LCardDraft> cards, long id)
    {
        for (int index = 0; index < cards.Count; index++)
        {
            if (cards[index].LCardDraftId == id)
            {
                return index;
            }
        }

        return -1;
    }

    public static LCardDraft? LCardFind(LEntryDraft content, LCardKind kind, long id)
    {
        ArgumentNullException.ThrowIfNull(content);

        return LCardFind(
            kind == LCardKind.LCardKindCollocation ? content.LEntryDraftCollocations : content.LEntryDraftMeanings,
            card => card.LCardDraftId == id);
    }

    public static (LOwner, int)? LCardOwnerFind(LEntryDraft draft, LCardKind kind, long id)
    {
        ArgumentNullException.ThrowIfNull(draft);

        int index = LCardPlaceRead(LCardListRead(draft, kind), id);
        LOwner owner = kind == LCardKind.LCardKindCollocation ? LOwner.LOwnerCollocation : LOwner.LOwnerMeaning;
        return index < 0 ? null : (owner, index);
    }

    private static LCardDraft? LCardFind(IReadOnlyList<LCardDraft> cards, Func<LCardDraft, bool> match)
    {
        foreach (LCardDraft card in cards)
        {
            if (match(card))
            {
                return card;
            }

            if (LCardFind(card.LCardDraftChild, match) is LCardDraft nested)
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
