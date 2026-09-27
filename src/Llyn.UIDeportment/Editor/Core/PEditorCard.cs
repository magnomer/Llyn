using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private void PCardShow(
        ObservableCollection<PCard> cards,
        string prefix,
        IReadOnlyList<CCardDraft> drafts,
        IReadOnlyDictionary<long, CTranslationTarget> targets,
        string language)
    {
        List<PCard> shown = new(drafts.Count);
        foreach (CCardDraft draft in drafts)
        {
            PCard? card = PCardFind(cards, draft.CCardDraftId);
            if (card is null || shown.Contains(card))
            {
                card = PCardCreate(prefix, draft);
                cards.Add(card);
            }

            PCardTextShow(card, draft);
            PCardListShow(card, draft, targets, language);
            card.PCardPosition = draft.CCardDraftPosition;
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

    private static void PCardTextShow(PCard card, CCardDraft draft)
    {
        card.PCardTitleShow(draft.CCardDraftTitle);
        card.PCardExpressionShow(draft.CCardDraftExpression);
        card.PCardDefinitionShow(draft.CCardDraftMeaning);
    }

    private PCard PCardCreate(string prefix, CCardDraft draft)
    {
        PCard card = new(
            _pEditorHost.PWindowDeportment,
            prefix,
            draft.CCardDraftPosition,
            _pEditorCitation,
            _pEditorParticle,
            _pEditorDependence,
            _pLanguageItem)
        {
            PCardId = draft.CCardDraftId,
        };

        PSentenceAttach(card);
        PLinkAttach(card);
        PContextAttach(card);
        PRegisterAttach(card);
        PLabelAttach(card);
        return card;
    }

    private void PCardListShow(
        PCard card, CCardDraft draft, IReadOnlyDictionary<long, CTranslationTarget> targets, string language)
    {
        card.PCardSentenceShow(draft.CCardDraftSentence, language);
        PSentenceMentionShow(card);
        card.PCardContextShow(draft.CCardDraftSituation);
        card.PCardRegisterShow(draft.CCardDraftRegister);
        card.PCardLinkShow(PCardTargetRead(targets, draft.CCardDraftTranslation));
        card.PCardLabelShow(draft.CCardDraftTag);
        card.PCardImageShow(draft.CCardDraftImage);
        card.PCardVideoShow(draft.CCardDraftVideo);
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

    private static IReadOnlyList<CTranslationTarget> PCardTargetRead(
        IReadOnlyDictionary<long, CTranslationTarget> targets, IReadOnlyList<long> ids)
    {
        List<CTranslationTarget> found = [];
        foreach (long id in ids)
        {
            if (targets.GetValueOrDefault(id) is CTranslationTarget target)
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

        string field = QField.QFieldPathRead(box);
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
            case PVideo row when box.Name == nameof(PVideo.PVideoLocation):
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
            case nameof(PCard.PCardPositionText):
                card.PCardPositionText = written.LStateWrittenText ?? string.Empty;
                break;
        }
    }
}
