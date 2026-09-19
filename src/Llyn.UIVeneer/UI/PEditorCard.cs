using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private void PCardShow(
        ObservableCollection<PCard> cards,
        string prefix,
        IReadOnlyList<LCardDraft> drafts,
        IReadOnlyDictionary<long, LTranslationTarget> targets,
        string language)
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
            PCardListShow(card, draft, targets, language);
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

    private static void PCardTextShow(PCard card, LCardDraft draft)
    {
        card.PCardTitleShow(draft.LCardDraftTitle);
        card.PCardExpressionShow(draft.LCardDraftExpression);
        card.PCardDefinitionShow(draft.LCardDraftMeaning);
    }

    private PCard PCardCreate(string prefix, LCardDraft draft)
    {
        PCard card = new(
            _lEngine,
            prefix,
            draft.LCardDraftPosition,
            _pEditorCitation,
            _pEditorParticle,
            _pEditorDependence,
            _pLanguageItem)
        {
            PCardId = draft.LCardDraftId,
        };

        PSentenceAttach(card);
        PLinkAttach(card);
        PContextAttach(card);
        PRegisterAttach(card);
        PLabelAttach(card);
        return card;
    }

    private void PCardListShow(
        PCard card, LCardDraft draft, IReadOnlyDictionary<long, LTranslationTarget> targets, string language)
    {
        card.PCardSentenceShow(draft.LCardDraftSentence, language);
        PSentenceMentionShow(card);
        card.PCardContextShow(draft.LCardDraftSituation);
        card.PCardRegisterShow(draft.LCardDraftRegister);
        card.PCardLinkShow(PCardTargetRead(targets, draft.LCardDraftTranslation));
        card.PCardLabelShow(draft.LCardDraftTag);
        card.PCardImageShow(draft.LCardDraftImage);
        card.PCardVideoShow(draft.LCardDraftVideo);
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
            if (targets.GetValueOrDefault(id) is LTranslationTarget target)
            {
                found.Add(target);
            }
        }

        return found;
    }

    private void PEditorFieldHandle(TextBox box)
    {
        UIElement owner = box.TemplatedParent as ComboBox ?? (UIElement)box;
        if (!owner.IsKeyboardFocusWithin)
        {
            return;
        }

        string field = PField.PFieldPathRead(box);
        LStateWritten written = new(box.Text);
        switch (box.DataContext)
        {
            case PCard card:
                PCardChangeHandle(card, field, written);
                break;
            case PSentence row when PCardSentenceFind(row) is PCard card:
                PSentenceChangeHandle(card, row, field, box);
                break;
            case PGloss gloss:
                PGlossChangeHandle(gloss, written);
                break;
            case PImage row:
                PEditorRequestDefer(new LRequestImageLocation(PEditorDraft, row.PImageId, written));
                break;
            case PVideo row when field == nameof(PVideo.PVideoLocation):
                PEditorRequestDefer(new LRequestVideoLocation(PEditorDraft, row.PVideoId, written));
                break;
            case PVideo row:
                PEditorRequestDefer(new LRequestVideoSpan(PEditorDraft, row.PVideoId, written));
                break;
        }
    }

    private void PCardChangeHandle(PCard card, string field, LStateWritten written)
    {
        switch (field)
        {
            case nameof(PCard.PTitle):
                PEditorRequestDefer(new LRequestCardTitle(PEditorDraft, card.PCardId, written));
                break;
            case nameof(PCard.PCardExpression):
                PEditorRequestDefer(new LRequestCardExpression(PEditorDraft, card.PCardId, written));
                break;
            case nameof(PCard.PCardDefinition):
                PEditorRequestDefer(new LRequestCardMeaning(PEditorDraft, card.PCardId, written));
                break;
        }
    }
}
