using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private void PCardRefine(ObservableCollection<PCard> cards, string prefix, IReadOnlyList<CCardDraft> drafts)
    {
        List<PCard> shown = new(drafts.Count);
        foreach (CCardDraft draft in drafts)
        {
            PCard? card = PCardFind(cards, draft.CCardDraftId);
            if (card is null)
            {
                card = new PCard(prefix, _pEditorCitation, _pEditorParticle, _pEditorDependence, _pLanguageItem)
                {
                    PCardId = draft.CCardDraftId,
                };
                card.PCardSentenceNotice += PSentenceGlossObserve;
                cards.Add(card);
            }

            PCardDraftRefine(card, draft);
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

    private void PCardDraftRefine(PCard card, CCardDraft draft)
    {
        card.PCardTitleShow(draft.CCardDraftTitle);
        card.PCardExpressionShow(draft.CCardDraftExpression);
        card.PCardDefinitionShow(draft.CCardDraftMeaning);
        card.PCardSentenceShow(draft.CCardDraftSentence, _pSentenceOrder);
        PSentenceMentionShow(card);
        card.PCardContextShow(draft.CCardDraftSituation);
        card.PCardRegisterShow(draft.CCardDraftRegister);
        card.PCardLinkShow(draft.CCardDraftTranslation);
        card.PCardLabelShow(draft.CCardDraftTag);
        card.PCardImageShow(draft.CCardDraftImage, _pEditorHost.PWindowAtelier);
        card.PCardVideoShow(draft.CCardDraftVideo, _pEditorHost.PWindowAtelier);
        card.PCardPosition = draft.CCardDraftPosition;
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

    private void PEditorFieldObserve(TextBox box)
    {
        UIElement owner = box.TemplatedParent as ComboBox ?? (UIElement)box;
        if (!owner.IsKeyboardFocusWithin)
        {
            return;
        }

        string field = QField.QFieldPathRead(box);
        switch (box.DataContext)
        {
            case PCard card when field == nameof(PCard.PCardPositionText):
                PCardPositionRefine(card, box.Text);
                break;
            case PCard card:
                PCardFieldObserve(card, field, box.Text);
                break;
            case PSentence row when PCardSentenceFind(row) is PCard card:
                PSentenceFieldObserve(card, row, field, box);
                break;
            case PGloss gloss:
                PGlossTextObserve(gloss, box.Text);
                break;
            case PImage row:
                _qEditor.QEditorArea.CEditorImage.CImageLocationSet(row.PImageId, box.Text);
                break;
            case PVideo row when box.Name == nameof(PVideo.PVideoLocation):
                _qEditor.QEditorArea.CEditorVideo.CVideoLocationSet(row.PVideoId, box.Text);
                break;
            case PVideo row:
                _qEditor.QEditorArea.CEditorVideo.CVideoSpanSet(row.PVideoId, box.Text);
                break;
        }
    }

    private void PCardFieldObserve(PCard card, string field, string text)
    {
        switch (field)
        {
            case nameof(PCard.PTitle):
                _qEditor.QEditorArea.CEditorField.CCardTitleSet(card.PCardId, text);
                break;
            case nameof(PCard.PCardExpression):
                _qEditor.QEditorArea.CEditorField.CCardExpressionSet(card.PCardId, text);
                break;
            case nameof(PCard.PCardDefinition):
                _qEditor.QEditorArea.CEditorField.CCardMeaningSet(card.PCardId, text);
                break;
        }
    }

    private static void PCardPositionRefine(PCard card, string text)
    {
        card.PCardPositionText = text;
    }
}
