using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LEntryDraft LEngineListApply(LEntryDraft content, LRequest request)
    {
        return request switch
        {
            LRequestTagPick { LRequestCardId: 0 } sent => LEngineCardResolve(
                content, id => sent with { LRequestCardId = id }),
            LRequestRegisterPick { LRequestCardId: 0 } sent => LEngineCardResolve(
                content, id => sent with { LRequestCardId = id }),
            LRequestSituationPick { LRequestCardId: 0 } sent => LEngineCardResolve(
                content, id => sent with { LRequestCardId = id }),
            LRequestSentenceExample { LRequestCardId: 0 } sent => LEngineCardResolve(
                content, id => sent with { LRequestCardId = id }),
            LRequestSentenceReference { LRequestCardId: 0 } sent => LEngineCardResolve(
                content, id => sent with { LRequestCardId = id }),
            LRequestSentenceExample { LRequestSentenceId: 0 } sent => LEngineSentenceResolve(
                content, sent.LRequestCardId, id => sent with { LRequestSentenceId = id }),
            LRequestSentenceReference { LRequestSentenceId: 0 } sent => LEngineSentenceResolve(
                content, sent.LRequestCardId, id => sent with { LRequestSentenceId = id }),
            LRequestSentenceAddition sent => LEngineSentenceAdd(content, sent),
            LRequestSentenceRemoval sent => LEngineSentenceRemove(content, sent),
            LRequestSentenceShift sent => LEngineSentenceMove(content, sent),
            LRequestSentenceExample sent => LEngineSentenceSelect(content, sent),
            LRequestSentenceText sent => LEngineExampleChange(
                content,
                sent.LRequestCardId,
                sent.LRequestSentenceId,
                example => LEngineMentionUpdate(example, LEngineValueRead(sent.LRequestValue))),
            LRequestSentenceParticle sent => LEngineSentenceChange(
                content,
                sent.LRequestCardId,
                sent.LRequestSentenceId,
                sentence => sentence with { LSentenceDraftParticle = LEngineValueRead(sent.LRequestValue) }),
            LRequestSentenceDependence sent => LEngineSentenceChange(
                content,
                sent.LRequestCardId,
                sent.LRequestSentenceId,
                sentence => sentence with { LSentenceDraftDependence = LEngineValueRead(sent.LRequestValue) }),
            LRequestSentenceReference sent => LEngineExampleChange(
                content,
                sent.LRequestCardId,
                sent.LRequestSentenceId,
                example => example with
                {
                    LExampleDraftReference = LStateAnchor.LStateAnchorRead(sent.LRequestReferenceId),
                }),
            LRequestSituationAddition sent => LEngineSituationAdd(content, sent),
            LRequestSituationPick sent => LEngineSituationInsert(content, sent),
            LRequestSituationRemoval sent => LEngineSituationRemove(content, sent),
            LRequestSituationShift sent => LEngineSituationMove(content, sent),
            LRequestRegisterAddition sent => LEngineRegisterAdd(content, sent),
            LRequestRegisterPick sent => LEngineRegisterInsert(content, sent),
            LRequestRegisterRemoval sent => LEngineRegisterRemove(content, sent),
            LRequestRegisterShift sent => LEngineRegisterMove(content, sent),
            LRequestRegisterName sent => LEngineRegisterChange(content, sent),
            LRequestTagAddition sent => LEngineTagAdd(content, sent),
            LRequestTagPick sent => LEngineTagInsert(content, sent),
            LRequestTagRemoval sent => LEngineTagRemove(content, sent),
            LRequestTagShift sent => LEngineTagMove(content, sent),
            LRequestTagText sent => LEngineTagChange(content, sent),
            LRequestTranslationPick sent => LEngineTranslationInsert(content, sent),
            LRequestTranslationRemoval sent => LEngineTranslationRemove(content, sent),
            LRequestTranslationShift sent => LEngineTranslationMove(content, sent),
            LRequestImageAddition sent => LEngineImageApply(content, sent.LRequestCardId, sent),
            LRequestImagePick sent => LEngineImageApply(content, sent.LRequestCardId, sent),
            LRequestImageRemoval sent => LEngineImageApply(content, sent.LRequestCardId, sent),
            LRequestImageShift sent => LEngineImageApply(content, sent.LRequestCardId, sent),
            LRequestImageLocation sent => LEngineImageChange(content, sent),
            LRequestVideoAddition sent => LEngineVideoApply(content, sent.LRequestCardId, sent),
            LRequestVideoPick sent => LEngineVideoApply(content, sent.LRequestCardId, sent),
            LRequestVideoRemoval sent => LEngineVideoApply(content, sent.LRequestCardId, sent),
            LRequestVideoShift sent => LEngineVideoApply(content, sent.LRequestCardId, sent),
            LRequestVideoLocation sent => LEngineVideoChange(
                content,
                sent.LRequestVideoId,
                video => video with { LVideoDraftLocation = LEngineValueRead(sent.LRequestValue) }),
            LRequestVideoSpan sent => LEngineVideoChange(
                content,
                sent.LRequestVideoId,
                video => video with { LVideoDraftSpan = LEngineValueRead(sent.LRequestValue) }),
            _ => throw new ArgumentException("The request kind is not one the engine applies.", nameof(request)),
        };
    }

    private LEntryDraft LEngineCardResolve(LEntryDraft content, Func<long, LRequest> retarget)
    {
        if (content.LEntryDraftMeanings.Count == 0)
        {
            content = LEngineCardInsert(
                content, new LRequestCardAddition(0, LCardKind.LCardKindMeaning, 0, 0));
        }

        return LEngineListApply(content, retarget(content.LEntryDraftMeanings[0].LCardDraftId));
    }

    private LEntryDraft LEngineSentenceResolve(LEntryDraft content, long cardId, Func<long, LRequest> retarget)
    {
        LCardDraft card = LEngineCardFind(content, cardId) ?? throw new LRefusal(LRefusal.LRefusalCard);
        if (card.LCardDraftSentence.Count == 0)
        {
            content = LEngineSentenceAdd(content, new LRequestSentenceAddition(0, cardId, 0));
            card = LEngineCardFind(content, cardId) ?? throw new LRefusal(LRefusal.LRefusalCard);
        }

        return LEngineListApply(content, retarget(card.LCardDraftSentence[0].LSentenceDraftId));
    }

    private static LCardDraft? LEngineCardFind(LEntryDraft content, long id)
    {
        return LEngineCardFind(content.LEntryDraftMeanings, id)
            ?? LEngineCardFind(content.LEntryDraftCollocations, id);
    }

    private static LCardDraft? LEngineCardFind(IReadOnlyList<LCardDraft> cards, long id)
    {
        foreach (LCardDraft card in cards)
        {
            if (card.LCardDraftId == id)
            {
                return card;
            }

            if (LEngineCardFind(card.LCardDraftChild, id) is LCardDraft nested)
            {
                return nested;
            }
        }

        return null;
    }

    private static IReadOnlyList<LEngineItem> LEngineListAdd<LEngineItem>(
        IReadOnlyList<LEngineItem> items, LEngineItem item, int position)
    {
        List<LEngineItem> written = new(items);
        written.Insert(Math.Clamp(position, 0, written.Count), item);
        return written;
    }

    private static IReadOnlyList<LEngineItem> LEngineListInsert<LEngineItem>(
        IReadOnlyList<LEngineItem> items, LEngineItem item, long id, int position, Func<LEngineItem, long> key)
    {
        return LEngineListFind(items, id, key) < 0 ? LEngineListAdd(items, item, position) : items;
    }

    private static IReadOnlyList<LEngineItem> LEngineListRemove<LEngineItem>(
        IReadOnlyList<LEngineItem> items, long id, Func<LEngineItem, long> key)
    {
        int index = LEngineListFind(items, id, key);
        if (index < 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        List<LEngineItem> written = new(items);
        written.RemoveAt(index);
        return written;
    }

    private static IReadOnlyList<LEngineItem> LEngineListMove<LEngineItem>(
        IReadOnlyList<LEngineItem> items, long id, int position, Func<LEngineItem, long> key)
    {
        int index = LEngineListFind(items, id, key);
        if (index < 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        List<LEngineItem> written = new(items);
        LEngineItem moved = written[index];
        written.RemoveAt(index);
        written.Insert(Math.Clamp(position, 0, written.Count), moved);
        return written;
    }

    private static IReadOnlyList<LEngineItem>? LEngineListChange<LEngineItem>(
        IReadOnlyList<LEngineItem> items, long id, Func<LEngineItem, long> key, Func<LEngineItem, LEngineItem> change)
    {
        int index = LEngineListFind(items, id, key);
        if (index < 0)
        {
            return null;
        }

        List<LEngineItem> written = new(items);
        written[index] = change(written[index]);
        return written;
    }

    private static int LEngineListFind<LEngineItem>(
        IReadOnlyList<LEngineItem> items, long id, Func<LEngineItem, long> key)
    {
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

    private static LEntryDraft LEngineRowChange(LEntryDraft content, Func<LCardDraft, LCardDraft?> change)
    {
        bool found = false;
        IReadOnlyList<LCardDraft> meanings = LEngineRowChange(content.LEntryDraftMeanings, change, ref found);
        IReadOnlyList<LCardDraft> collocations = LEngineRowChange(content.LEntryDraftCollocations, change, ref found);

        if (!found)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        return content with { LEntryDraftMeanings = meanings, LEntryDraftCollocations = collocations };
    }

    private static IReadOnlyList<LCardDraft> LEngineRowChange(
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

            IReadOnlyList<LCardDraft> children = LEngineRowChange(card.LCardDraftChild, change, ref found);
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
