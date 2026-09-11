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
        cards.Clear();
        foreach (LCardDraft draft in drafts)
        {
            PCard card = new(prefix, draft.LCardDraftPosition, _pEditorCitation, _pEditorParticle, _pEditorDependence)
            {
                PCardId = draft.LCardDraftId,
                PCardDraft = draft,
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
            cards.Add(card);
        }

        if (cards.Count == 0)
        {
            cards.Add(PCardCreate(prefix, 1));
        }
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

    private IReadOnlyList<LCardDraft> PCardRead(IReadOnlyList<PCard> cards)
    {
        List<LCardDraft> drafts = new(cards.Count);
        foreach (PCard card in cards)
        {
            drafts.Add((card.PCardDraft ?? PCardDraftCreate()) with
            {
                LCardDraftTitle = card.PCardTitleRead(),
                LCardDraftExpression = card.PCardExpressionRead(),
                LCardDraftMeaning = card.PCardDefinitionRead(),
                LCardDraftSentence = card.PCardSentenceRead(),
                LCardDraftSituation = card.PCardContextRead(),
                LCardDraftRegister = card.PCardRegisterRead(),
                LCardDraftTranslation = card.PCardLinkRead(),
                LCardDraftTag = card.PCardLabelRead(),
                LCardDraftImage = card.PCardImageRead(),
                LCardDraftVideo = card.PCardVideoRead(),
                LCardDraftPosition = card.PCardPosition,
                LCardDraftId = card.PCardId,
            });
        }

        return drafts;
    }

    private static LCardDraft PCardDraftCreate()
    {
        return new LCardDraft(
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            [],
            [],
            [],
            [],
            string.Empty,
            [],
            [],
            [],
            0);
    }
}
