using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCard
{
    private readonly QCardDrag _qCardDrag;

    private readonly QContext _qCardContext;

    private readonly QRegister _qCardRegister;

    private readonly QLink _qCardLink;

    private readonly QLabel _qCardLabel;

    private readonly QSentence _qCardSentence;

    private readonly QCitation _qCardCitation;

    private readonly QImage _qCardImage;

    private readonly QVideo _qCardVideo;

    private readonly ObservableCollection<PLanguageItem> _qCardLanguage;

    private readonly ObservableCollection<PCard> _qCardMeaning;

    private readonly ObservableCollection<PCard> _qCardCollocation;

    private CEditor _cEditor = null!;

    internal QCard(
        QCardDrag drag,
        QContext context,
        QRegister register,
        QLink link,
        QLabel label,
        QSentence sentence,
        QCitation citation,
        QImage image,
        QVideo video,
        ObservableCollection<PLanguageItem> languages,
        ObservableCollection<PCard> meanings,
        ObservableCollection<PCard> collocations)
    {
        _qCardDrag = drag;
        _qCardContext = context;
        _qCardRegister = register;
        _qCardLink = link;
        _qCardLabel = label;
        _qCardSentence = sentence;
        _qCardCitation = citation;
        _qCardImage = image;
        _qCardVideo = video;
        _qCardLanguage = languages;
        _qCardMeaning = meanings;
        _qCardCollocation = collocations;
    }

    internal void QCardIntroduce(CEditor editor)
    {
        _cEditor = editor;
    }

    internal void QCardApply(FrameworkElement container, object item, string? changed)
    {
        if (item is not PCard card)
        {
            return;
        }

        PCard.PCardRowApply(container, card, changed);
        QCardFieldApply(container, card);
        if (QLook.QLookPartFind<Border>(container, "PCardHeader") is Border header)
        {
            header.MouseLeftButtonDown -= _qCardDrag.QCardDragRefine;
            header.MouseLeftButtonDown += _qCardDrag.QCardDragRefine;
        }

        if (QLook.QLookPartFind<Border>(container, "PCardPosition") is Border position)
        {
            position.MouseLeftButtonDown -= QCardPositionRefine;
            position.MouseLeftButtonDown += QCardPositionRefine;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardPositionText") is TextBox ordinal)
        {
            ordinal.KeyDown -= QCardPositionRefine;
            ordinal.KeyDown -= QCardPositionObserve;
            ordinal.LostFocus -= QCardPositionObserve;
            ordinal.KeyDown += QCardPositionRefine;
            ordinal.KeyDown += QCardPositionObserve;
            ordinal.LostFocus += QCardPositionObserve;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PTitle") is TextBox title)
        {
            title.TextChanged -= QCardTitleObserve;
            title.TextChanged += QCardTitleObserve;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardExpression") is TextBox expression)
        {
            expression.TextChanged -= QCardExpressionObserve;
            expression.TextChanged += QCardExpressionObserve;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardDefinition") is TextBox definition)
        {
            definition.TextChanged -= QCardDefinitionObserve;
            definition.TextChanged += QCardDefinitionObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardEraser") is Button eraser)
        {
            eraser.Click -= QCardRemoveObserve;
            eraser.Click += QCardRemoveObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardImageChooser") is Button image)
        {
            image.Click -= _qCardImage.QImageAddObserve;
            image.Click += _qCardImage.QImageAddObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardVideoChooser") is Button video)
        {
            video.Click -= _qCardVideo.QVideoAddObserve;
            video.Click += _qCardVideo.QVideoAddObserve;
        }
    }

    private void QCardFieldApply(FrameworkElement container, PCard card)
    {
        if (QLook.QLookPartFind<ItemsControl>(container, "PCardContext") is ItemsControl context)
        {
            _qCardContext.QContextFieldApply(context, card);
            context.ItemsSource = card.PCardContext;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardRegister") is ItemsControl register)
        {
            _qCardRegister.QRegisterFieldApply(register, card);
            register.ItemsSource = card.PCardRegister;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardLink") is ItemsControl link)
        {
            _qCardLink.QLinkFieldApply(link, card);
            link.ItemsSource = card.PCardLink;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardLabel") is ItemsControl label)
        {
            _qCardLabel.QLabelFieldApply(label, card);
            label.ItemsSource = card.PCardLabel;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardSentence") is ItemsControl sentence)
        {
            sentence.ItemsSource = card.PCardSentence;
            QLookItem.QLookItemAttach(sentence, _qCardSentence.QSentenceApply);
            QSentence.QSentenceRevealAttach(sentence);
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardImage") is ItemsControl images)
        {
            images.ItemsSource = card.PCardImage;
            QLookItem.QLookItemAttach(images, _qCardImage.QImageApply);
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardVideo") is ItemsControl videos)
        {
            videos.ItemsSource = card.PCardVideo;
            QLookItem.QLookItemAttach(videos, _qCardVideo.QVideoApply);
        }
    }

    private void QCardRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card })
        {
            _cEditor.CEditorList.CCardRemove(card.PCardId);
        }
    }

    private void QCardPositionRefine(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount < 2 || sender is not FrameworkElement { DataContext: PCard card } badge)
        {
            return;
        }

        ObservableCollection<PCard>? list = QCardListFind(card);

        if (list is null || list.Count <= 1)
        {
            return;
        }

        e.Handled = true;
        card.PCardPositionActive = true;

        badge.Dispatcher.BeginInvoke(
            DispatcherPriority.Input,
            () =>
            {
                if (QField.QFieldCaretFind(badge) is not TextBox box || box.DataContext != card)
                {
                    return;
                }

                box.Focus();
                box.SelectAll();
            });
    }

    private void QCardPositionRefine(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Escape && sender is FrameworkElement { DataContext: PCard card })
        {
            e.Handled = true;
            card.PCardPositionHide();
        }
    }

    private void QCardPositionObserve(object sender, RoutedEventArgs e)
    {
        if (e is KeyEventArgs { Key: not Key.Enter }
            || sender is not TextBox { DataContext: PCard card } box
            || !card.PCardPositionActive)
        {
            return;
        }

        _cEditor.CEditorList.CCardMove(card.PCardId, box.Text);
        if (e is KeyEventArgs)
        {
            e.Handled = true;
        }

        card.PCardPositionHide();
    }

    private ObservableCollection<PCard>? QCardListFind(PCard card)
    {
        return _qCardMeaning.Contains(card) ? _qCardMeaning
            : _qCardCollocation.Contains(card) ? _qCardCollocation
            : null;
    }

    internal void QCardRefine(ObservableCollection<PCard> cards, string prefix, IReadOnlyList<CCardDraft> drafts)
    {
        List<PCard> shown = new(drafts.Count);
        foreach (CCardDraft draft in drafts)
        {
            PCard? card = QCardFind(cards, draft.CCardDraftId);
            if (card is null)
            {
                card = new PCard(
                    prefix,
                    _qCardCitation.QCitationCatalog,
                    _qCardSentence.QSentenceParticle,
                    _qCardSentence.QSentenceDependence,
                    _qCardLanguage,
                    draft)
                {
                    PCardId = draft.CCardDraftId,
                };
                card.PCardSentenceNotice += _qCardSentence.QSentenceGlossObserve;
                cards.Add(card);
            }

            QCardDraftRefine(card, draft);
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

    private void QCardDraftRefine(PCard card, CCardDraft draft)
    {
        card.PCardTitleShow(draft.CCardDraftTitle);
        card.PCardExpressionShow(draft.CCardDraftExpression);
        card.PCardDefinitionShow(draft.CCardDraftMeaning);
        card.PCardSentenceShow(draft.CCardDraftSentence, _qCardSentence.QSentenceOrder);
        card.PCardContextShow(draft.CCardDraftSituation);
        card.PCardRegisterShow(draft.CCardDraftRegister);
        card.PCardLinkShow(draft.CCardDraftTranslation);
        card.PCardLabelShow(draft.CCardDraftTag);
        card.PCardImageShow(draft.CCardDraftImage);
        card.PCardVideoShow(draft.CCardDraftVideo);
        card.PCardPosition = draft.CCardDraftPosition;
    }

    private static PCard? QCardFind(IReadOnlyList<PCard> cards, long id)
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

    private void QCardTitleObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PCard card } box)
        {
            _cEditor.CEditorField.CCardTitleSet(card.PCardId, box.Text);
        }
    }

    private void QCardExpressionObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PCard card } box)
        {
            _cEditor.CEditorField.CCardExpressionSet(card.PCardId, box.Text);
        }
    }

    private void QCardDefinitionObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PCard card } box)
        {
            _cEditor.CEditorField.CCardMeaningSet(card.PCardId, box.Text);
        }
    }
}
