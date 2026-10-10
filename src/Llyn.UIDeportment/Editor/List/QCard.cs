using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCard
{
    private readonly QCardDrag _qCardDrag;

    private readonly QCardPosition _qCardPosition;

    private readonly QContext _qCardContext;

    private readonly QRegister _qCardRegister;

    private readonly QLink _qCardLink;

    private readonly QLabel _qCardLabel;

    private readonly QSentence _qCardSentence;

    private readonly QCitation _qCardCitation;

    private readonly QExample _qCardExample;

    private readonly QImage _qCardImage;

    private readonly QVideo _qCardVideo;

    private readonly ObservableCollection<PLanguageItem> _qCardLanguage;

    private CCardField _cCardField = null!;

    private CCardList _cCardList = null!;

    internal QCard(
        QCardDrag drag,
        QContext context,
        QRegister register,
        QLink link,
        QLabel label,
        QSentence sentence,
        QCitation citation,
        QExample example,
        QImage image,
        QVideo video,
        ObservableCollection<PLanguageItem> languages,
        ObservableCollection<PCard> meanings,
        ObservableCollection<PCard> collocations)
    {
        _qCardDrag = drag;
        _qCardPosition = new QCardPosition(meanings, collocations);
        _qCardContext = context;
        _qCardRegister = register;
        _qCardLink = link;
        _qCardLabel = label;
        _qCardSentence = sentence;
        _qCardCitation = citation;
        _qCardExample = example;
        _qCardImage = image;
        _qCardVideo = video;
        _qCardLanguage = languages;
    }

    internal void QCardIntroduce(CCardField field, CCardList list)
    {
        _cCardField = field;
        _cCardList = list;
        _qCardPosition.QCardPositionIntroduce(list);
    }

    internal void QCardApply(FrameworkElement container, object item, string? changed)
    {
        if (item is not PCard card)
        {
            return;
        }

        QCardRowRefine(container, card, changed);
        QCardFieldApply(container, card);
        if (QLook.QLookPartFind<Border>(container, "PCardHeader") is Border header)
        {
            header.MouseLeftButtonDown -= _qCardDrag.QCardDragRefine;
            header.MouseLeftButtonDown += _qCardDrag.QCardDragRefine;
        }

        _qCardPosition.QCardPositionApply(container);

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

        if (QLook.QLookPartFind<ToggleButton>(container, "PCardHinge") is ToggleButton hinge)
        {
            hinge.Click -= QCardHingeObserve;
            hinge.Click += QCardHingeObserve;
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

    internal static void QCardRowRefine(FrameworkElement container, PCard card, string? changed)
    {
        ArgumentNullException.ThrowIfNull(card);

        if (QLook.QLookPartFind<Border>(container, "PCardPosition") is Border position)
        {
            if (card.PCardPositionActive)
            {
                position.SetResourceReference(Border.BorderBrushProperty, "Theme.Accent");
            }
            else
            {
                position.ClearValue(Border.BorderBrushProperty);
            }
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardPositionText") is TextBox ordinal)
        {
            if (changed is null or nameof(PCard.PCardPosition) or nameof(PCard.PCardPositionActive))
            {
                ordinal.Text = card.PCardPositionText;
            }

            if (card.PCardPositionActive)
            {
                ordinal.IsReadOnly = false;
                ordinal.IsHitTestVisible = true;
            }
            else
            {
                ordinal.ClearValue(TextBoxBase.IsReadOnlyProperty);
                ordinal.ClearValue(UIElement.IsHitTestVisibleProperty);
            }
        }

        if (QLook.QLookPartFind<TextBox>(container, "PTitle") is TextBox title)
        {
            if (changed is null or nameof(PCard.PTitle))
            {
                title.Text = card.PTitle.CStateWordingText;
            }

            QStateConverter.QStateHintRefine(title, QField.QFieldHintProperty, card.PTitle);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCardPeek") is TextBlock peek)
        {
            peek.Text = card.PCardPeek;
            peek.Visibility = QLook.QLookVisibleRead(card.PCardFolded && card.PTitle.CStateWordingMuted);
        }

        QCardFold.QCardFoldRefine(container, card.PCardFolded, card.PCardStored);

        if (QLook.QLookPartFind<TextBox>(container, "PCardExpression") is TextBox expression)
        {
            if (changed is null or nameof(PCard.PCardExpression))
            {
                expression.Text = card.PCardExpression.CStateWordingText;
            }

            QStateConverter.QStateHintRefine(expression, QField.QFieldHintProperty, card.PCardExpression);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardDefinition") is TextBox definition)
        {
            if (changed is null or nameof(PCard.PCardDefinition))
            {
                definition.Text = card.PCardDefinition.CStateWordingText;
            }

            QStateConverter.QStateHintRefine(definition, QField.QFieldHintProperty, card.PCardDefinition);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCardIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("close", 12);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCardImageIcon") is QIconImage image)
        {
            image.QIconSource = QIcon.QIconResolve("image", 24);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCardVideoIcon") is QIconImage video)
        {
            video.QIconSource = QIcon.QIconResolve("video", 24);
        }
    }

    private void QCardFieldApply(FrameworkElement container, PCard card)
    {
        if (QLook.QLookPartFind<ItemsControl>(container, "PCardContext") is ItemsControl context)
        {
            _qCardContext.QContextFieldApply(context, card);
            context.ItemsSource = card.PCardContext.PCaretRow;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardRegister") is ItemsControl register)
        {
            _qCardRegister.QRegisterFieldApply(register, card);
            register.ItemsSource = card.PCardRegister.PCaretRow;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardLink") is ItemsControl link)
        {
            _qCardLink.QLinkFieldApply(link, card);
            link.ItemsSource = card.PCardLink.PCaretRow;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardLabel") is ItemsControl label)
        {
            _qCardLabel.QLabelFieldApply(label, card);
            label.ItemsSource = card.PCardLabel.PCaretRow;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardSentence") is ItemsControl sentence)
        {
            sentence.ItemsSource = card.PCardSentence.PCardSentenceRow;
            QLookItem.QLookItemAttach(
                sentence,
                (row, item, _) =>
                {
                    _qCardExample.QExampleRefine(row, item);
                    _qCardExample.QExampleIntroduce(row, item);
                });
            QExample.QExampleRevealIntroduce(sentence);
            QExample.QExampleRevealRefine(sentence);
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
            _cCardList.CCardRemove(card.PCardId);
        }
    }

    internal void QCardRefine(
        ObservableCollection<PCard> cards,
        string prefix,
        IReadOnlyList<CCardDraft> drafts,
        Func<PCard, CStateWording> peek)
    {
        List<PCard> free = [.. cards];
        List<PCard> shown = new(drafts.Count);
        foreach (CCardDraft draft in drafts)
        {
            PCard? card = free.Find(row => row.PCardId == draft.CCardDraftId);
            if (card is not null)
            {
                free.Remove(card);
            }
            else
            {
                card = new PCard(
                    prefix,
                    _qCardCitation.QCitationCatalog,
                    _qCardSentence.QSentenceParticle,
                    _qCardSentence.QSentenceDependence,
                    _qCardLanguage,
                    peek,
                    draft);
                card.PCardSentence.PCardSentenceNotice += _qCardSentence.QSentenceGlossObserve;
            }

            QCardDraftRefine(card, draft);
            shown.Add(card);
        }

        int kept = 0;
        List<PCard> gone = [];
        foreach (PCard card in cards)
        {
            if (kept < shown.Count && ReferenceEquals(card, shown[kept]))
            {
                kept++;
            }
            else
            {
                gone.Add(card);
            }
        }

        foreach (PCard card in gone)
        {
            cards.Remove(card);
        }

        for (int index = kept; index < shown.Count; index++)
        {
            cards.Add(shown[index]);
        }
    }

    private void QCardDraftRefine(PCard card, CCardDraft draft)
    {
        card.PCardTitleShow(draft.CCardDraftTitle);
        card.PCardExpressionShow(draft.CCardDraftExpression);
        card.PCardDefinitionShow(draft.CCardDraftMeaning);
        card.PCardSentence.PCardSentenceShow(draft.CCardDraftSentence, _qCardSentence.QSentenceOrder);
        QContext.QContextShow(card, draft.CCardDraftSituation);
        QRegister.QRegisterShow(card, draft.CCardDraftRegister);
        QLink.QLinkShow(card, draft.CCardDraftTranslation);
        QLabel.QLabelShow(card, draft.CCardDraftTag);
        card.PCardImageShow(draft.CCardDraftImage);
        card.PCardVideoShow(draft.CCardDraftVideo);
        card.PCardPosition = draft.CCardDraftPosition;
        card.PCardFoldShow(draft.CCardDraftFolded);
    }

    private void QCardHingeObserve(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton { DataContext: PCard card } hinge)
        {
            QLook.QLookCheckedRefine(
                hinge, _cCardList.CCardFoldToggle(card.PCardId, QLook.QLookCheckedRead(hinge.IsChecked)));
        }
    }

    private void QCardTitleObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PCard card } box)
        {
            _cCardField.CCardTitleSet(card.PCardId, box.Text);
        }
    }

    private void QCardExpressionObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PCard card } box)
        {
            _cCardField.CCardExpressionSet(card.PCardId, box.Text);
        }
    }

    private void QCardDefinitionObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PCard card } box)
        {
            _cCardField.CCardMeaningSet(card.PCardId, box.Text);
        }
    }
}
