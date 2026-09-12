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
                card = PCardCreate(prefix, draft);
                cards.Add(card);
            }

            PCardTextShow(card, draft);
            PCardListShow(card, draft, targets);
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
            && !card.PCardTitleRead().LStateWrittenMatch(draft.LCardDraftTitle))
        {
            card.PCardTitleShow(draft.LCardDraftTitle);
        }

        if (!PEditorRequestCheck(PEditorRequestFormat(card, nameof(PCard.PCardExpression)))
            && !card.PCardExpressionRead().LStateWrittenMatch(draft.LCardDraftExpression))
        {
            card.PCardExpressionShow(draft.LCardDraftExpression);
        }

        if (!PEditorRequestCheck(PEditorRequestFormat(card, nameof(PCard.PCardDefinition)))
            && !card.PCardDefinitionRead().LStateWrittenMatch(draft.LCardDraftMeaning))
        {
            card.PCardDefinitionShow(draft.LCardDraftMeaning);
        }
    }

    private PCard PCardCreate(string prefix, LCardDraft draft)
    {
        PCard card = new(
            _lEngine, prefix, draft.LCardDraftPosition, _pEditorCitation, _pEditorParticle, _pEditorDependence)
        {
            PCardId = draft.LCardDraftId,
        };

        card.PCardSentenceApply(_pEditorSentenceOrder);
        PSentenceAttach(card);
        PLinkAttach(card);
        PContextAttach(card);
        PRegisterAttach(card);
        PLabelAttach(card);
        PImageAttach(card);
        PVideoAttach(card);
        PEditorChangeAttach(card);
        return card;
    }

    private void PCardListShow(
        PCard card, LCardDraft draft, IReadOnlyDictionary<long, LTranslationTarget> targets)
    {
        card.PCardSentenceShow(draft.LCardDraftSentence, (row, field) => PSentencePendingCheck(card, row, field));
        PSentenceMentionShow(card);
        card.PCardContextShow(draft.LCardDraftSituation);
        card.PCardRegisterShow(draft.LCardDraftRegister);
        card.PCardLinkShow(PCardTargetRead(targets, draft.LCardDraftTranslation));
        card.PCardLabelShow(draft.LCardDraftTag);
        card.PCardImageShow(draft.LCardDraftImage, row => PImagePendingCheck(card, row));
        card.PCardVideoShow(draft.LCardDraftVideo, (row, field) => PVideoPendingCheck(card, row, field));
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
}
