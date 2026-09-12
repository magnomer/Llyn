using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private void PCardShow(
        ObservableCollection<PCard> cards,
        string prefix,
        IReadOnlyList<LCardDraft> drafts,
        IReadOnlyDictionary<long, LTranslationTarget> targets)
    {
        List<PCard> shown = new(drafts.Count);
        foreach (LCardDraft draft in drafts)
        {
            PCard? card = PCardFind(cards, draft.LCardDraftId);
            if (card is null || shown.Contains(card))
            {
                card = PCardCreate(prefix, draft, targets);
                cards.Add(card);
            }
            else
            {
                PCardTextShow(card, draft);
            }

            card.PCardPosition = draft.LCardDraftPosition;
            shown.Add(card);
        }

        for (int index = cards.Count - 1; index >= 0; index--)
        {
            if (!shown.Contains(cards[index]))
            {
                cards.RemoveAt(index);
            }
        }

        for (int index = 0; index < shown.Count; index++)
        {
            int current = cards.IndexOf(shown[index]);
            if (current != index)
            {
                cards.Move(current, index);
            }
        }
    }

    private void PCardTextShow(PCard card, LCardDraft draft)
    {
        if (!PEditorRequestCheck(PEditorRequestFormat(card, nameof(PCard.PTitle)))
            && card.PCardTitleRead() != draft.LCardDraftTitle)
        {
            card.PCardTitleShow(draft.LCardDraftTitle);
        }

        if (!PEditorRequestCheck(PEditorRequestFormat(card, nameof(PCard.PCardExpression)))
            && card.PCardExpressionRead() != draft.LCardDraftExpression)
        {
            card.PCardExpressionShow(draft.LCardDraftExpression);
        }

        if (!PEditorRequestCheck(PEditorRequestFormat(card, nameof(PCard.PCardDefinition)))
            && card.PCardDefinitionRead() != draft.LCardDraftMeaning)
        {
            card.PCardDefinitionShow(draft.LCardDraftMeaning);
        }
    }

    private PCard PCardCreate(
        string prefix, LCardDraft draft, IReadOnlyDictionary<long, LTranslationTarget> targets)
    {
        PCard card = new(
            _lEngine, prefix, draft.LCardDraftPosition, _pEditorCitation, _pEditorParticle, _pEditorDependence)
        {
            PCardId = draft.LCardDraftId,
        };

        card.PCardSentenceApply(_pEditorSentenceOrder);
        card.PCardTitleShow(draft.LCardDraftTitle);
        card.PCardExpressionShow(draft.LCardDraftExpression);
        card.PCardDefinitionShow(draft.LCardDraftMeaning);
        card.PCardSentenceShow(draft.LCardDraftSentence);
        card.PCardContextShow(draft.LCardDraftSituation);
        card.PCardRegisterShow(draft.LCardDraftRegister);
        card.PCardLinkShow(PCardTargetRead(targets, draft.LCardDraftTranslation));
        PLinkAttach(card);
        PContextAttach(card);
        PRegisterAttach(card);
        PLabelAttach(card);
        PEditorChangeAttach(card);
        card.PCardLabelShow(draft.LCardDraftTag);
        card.PCardImageShow(draft.LCardDraftImage);
        card.PCardVideoShow(draft.LCardDraftVideo);
        return card;
    }

    private void PCardPrepare()
    {
        if (_pMeaningList.Count == 0)
        {
            PEditorRequestSend(new LRequestCardAddition(_pEditorDraft, LCardKind.LCardKindMeaning, 0, 0));
        }

        if (_pCollocationList.Count == 0)
        {
            PEditorRequestSend(new LRequestCardAddition(_pEditorDraft, LCardKind.LCardKindCollocation, 0, 0));
        }
    }

    private static PCard? PCardFind(IReadOnlyList<PCard> cards, long id)
    {
        foreach (PCard card in cards)
        {
            if (card.PCardId == id)
            {
                return card;
            }
        }

        return null;
    }

    private static IReadOnlyList<LTranslationTarget> PCardTargetRead(
        IReadOnlyDictionary<long, LTranslationTarget> targets, IReadOnlyList<long> ids)
    {
        List<LTranslationTarget> found = [];
        foreach (long id in ids)
        {
            if (targets.TryGetValue(id, out LTranslationTarget? target))
            {
                found.Add(target);
            }
        }

        return found;
    }

    private static IReadOnlyList<LCardDraft> PCardRead(IReadOnlyList<PCard> cards, IReadOnlyList<LCardDraft> held)
    {
        List<LCardDraft> drafts = new(held.Count);
        foreach (LCardDraft draft in held)
        {
            PCard? card = PCardFind(cards, draft.LCardDraftId);
            drafts.Add(card is null
                ? draft
                : draft with
                {
                    LCardDraftSentence = card.PCardSentenceRead(),
                    LCardDraftSituation = card.PCardContextRead(),
                    LCardDraftRegister = card.PCardRegisterRead(),
                    LCardDraftTranslation = card.PCardLinkRead(),
                    LCardDraftTag = card.PCardLabelRead(),
                    LCardDraftImage = card.PCardImageRead(),
                    LCardDraftVideo = card.PCardVideoRead(),
                });
        }

        return drafts;
    }
}
