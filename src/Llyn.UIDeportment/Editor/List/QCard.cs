using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

    private readonly QBond _qCardBond = new();

    private readonly QLabel _qCardLabel;

    private readonly QSentence _qCardSentence;

    private readonly QCitation _qCardCitation;

    private readonly QExample _qCardExample;

    private readonly QImage _qCardImage;

    private readonly QVideo _qCardVideo;

    private readonly ObservableCollection<QLanguageItem> _qCardLanguage;

    private CCardField _cCardField = null!;

    private CCardList _cCardList = null!;

    private CCaret _cCaret = null!;

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
        ObservableCollection<QLanguageItem> languages)
    {
        _qCardDrag = drag;
        _qCardPosition = new QCardPosition();
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

    internal void QCardIntroduce(CCardField field, CCardList list, CCaret caret)
    {
        _cCardField = field;
        _cCardList = list;
        _cCaret = caret;
        _qCardBond.QBondIntroduce(field);
        _qCardPosition.QCardPositionIntroduce(list);
    }

    internal void QCardApply(FrameworkElement container, object item, string? changed)
    {
        if (item is not QPlacard card)
        {
            return;
        }

        QCardRowRefine(container, card, changed);
        QCardFieldApply(container, card);
        _qCardBond.QBondApply(container, card, changed);
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

    internal static void QCardRowRefine(FrameworkElement container, QPlacard card, string? changed)
    {
        ArgumentNullException.ThrowIfNull(card);

        if (QLook.QLookPartFind<Border>(container, "PCardPosition") is Border position)
        {
            if (card.QPlacardPositionActive)
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
            if (changed is null or nameof(QPlacard.QPlacardPosition) or nameof(QPlacard.QPlacardPositionActive))
            {
                ordinal.Text = card.QPlacardPositionText;
            }

            if (card.QPlacardPositionActive)
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
            if (changed is null or nameof(QPlacard.PTitle))
            {
                title.Text = card.PTitle.CStateWordingText;
            }

            QStateConverter.QStateHintRefine(title, QField.QFieldHintProperty, card.PTitle);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCardPeek") is TextBlock peek)
        {
            peek.Text = card.QPlacardPeek;
            peek.Visibility = QLook.QLookVisibleRead(card.QPlacardFolded && card.PTitle.CStateWordingMuted);
        }

        QCardFold.QCardFoldRefine(container, card.QPlacardFolded, card.QPlacardStored);

        if (QLook.QLookPartFind<TextBox>(container, "PCardExpression") is TextBox expression)
        {
            if (changed is null or nameof(QPlacard.QPlacardExpression))
            {
                expression.Text = card.QPlacardExpression.CStateWordingText;
            }

            QStateConverter.QStateHintRefine(expression, QField.QFieldHintProperty, card.QPlacardExpression);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardDefinition") is TextBox definition)
        {
            if (changed is null or nameof(QPlacard.QPlacardDefinition))
            {
                definition.Text = card.QPlacardDefinition.CStateWordingText;
            }

            QStateConverter.QStateHintRefine(definition, QField.QFieldHintProperty, card.QPlacardDefinition);
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

    private void QCardFieldApply(FrameworkElement container, QPlacard card)
    {
        if (QLook.QLookPartFind<ItemsControl>(container, "PCardContext") is ItemsControl context)
        {
            _qCardContext.QContextFieldApply(context, card);
            context.ItemsSource = card.QPlacardContext.QInletRow;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardRegister") is ItemsControl register)
        {
            _qCardRegister.QRegisterFieldApply(register, card);
            register.ItemsSource = card.QPlacardRegister.QInletRow;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardLink") is ItemsControl link)
        {
            _qCardLink.QLinkFieldApply(link, card);
            link.ItemsSource = card.QPlacardLink.QInletRow;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardLabel") is ItemsControl label)
        {
            _qCardLabel.QLabelFieldApply(label, card);
            label.ItemsSource = card.QPlacardLabel.QInletRow;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardSentence") is ItemsControl sentence)
        {
            sentence.ItemsSource = card.QCardSentence.QCardSentenceRow;
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
            images.ItemsSource = card.QPlacardImage;
            QLookItem.QLookItemAttach(images, _qCardImage.QImageApply);
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardVideo") is ItemsControl videos)
        {
            videos.ItemsSource = card.QPlacardVideo;
            QLookItem.QLookItemAttach(videos, _qCardVideo.QVideoApply);
        }
    }

    private void QCardRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QPlacard card })
        {
            _cCardList.CCardRemove(card.QPlacardCollocated, card.QPlacardId);
        }
    }

    internal void QCardRefine(
        ObservableCollection<QPlacard> cards, string prefix, bool collocated,
        IReadOnlyList<CStep> steps, Func<QPlacard, CStateWording> peek)
    {
        List<QPlacard> free = [.. cards];
        List<QPlacard> shown = new(steps.Count);
        foreach (CStep step in steps)
        {
            (CCardDraft draft, IReadOnlyDictionary<CCaretTrail, long?> anchor) = (step.CStepCard!, step.CStepCaret!);
            QPlacard? card = free.Find(row => row.QPlacardId == draft.CCardDraftId);
            if (card is not null)
            {
                free.Remove(card);
            }
            else
            {
                card = new QPlacard(
                    prefix,
                    collocated,
                    _qCardCitation.QCitationCatalog,
                    _qCardSentence.QSentenceParticle,
                    _qCardSentence.QSentenceDependence,
                    _qCardLanguage,
                    peek,
                    draft);
                card.QCardSentence.QCardSentenceNotice += _qCardSentence.QSentenceGlossObserve;
            }

            QCardDraftRefine(card, draft, anchor);
            card.QPlacardStep = step;
            shown.Add(card);
        }

        int kept = 0;
        List<QPlacard> gone = [];
        foreach (QPlacard card in cards)
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

        foreach (QPlacard card in gone)
        {
            cards.Remove(card);
        }

        for (int index = kept; index < shown.Count; index++)
        {
            cards.Add(shown[index]);
        }
    }

    private void QCardDraftRefine(
        QPlacard card, CCardDraft draft, IReadOnlyDictionary<CCaretTrail, long?> anchor)
    {
        card.QPlacardTitleShow(draft.CCardDraftTitle);
        card.QPlacardExpressionShow(draft.CCardDraftExpression);
        card.QPlacardDefinitionShow(draft.CCardDraftMeaning);
        card.QCardSentence.QCardSentenceShow(draft.CCardDraftSentence, _qCardSentence.QSentenceOrder);
        _qCardContext.QContextShow(card, draft, anchor[_cCaret.CCaretSituation]);
        QRegister.QRegisterShow(card, draft.CCardDraftRegister, anchor[_cCaret.CCaretRegister]);
        QLink.QLinkShow(card, draft.CCardDraftTranslation, anchor[_cCaret.CCaretTranslation]);
        QBond.QBondRefine(card, draft.CCardDraftSense);
        _qCardLabel.QLabelShow(card, draft, anchor[_cCaret.CCaretTag]);
        card.QPlacardImageShow(draft.CCardDraftImage);
        card.QPlacardVideoShow(draft.CCardDraftVideo);
        card.QPlacardPosition = draft.CCardDraftPosition;
    }

    private void QCardHingeObserve(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton { DataContext: QPlacard { QPlacardStep.CStepFold: Func<bool, bool> fold } } hinge)
        {
            QLook.QLookCheckedRefine(hinge, fold(QLook.QLookCheckedRead(hinge.IsChecked)));
        }
    }

    private void QCardTitleObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: QPlacard card } box)
        {
            _cCardField.CCardTitleSet(card.QPlacardCollocated, card.QPlacardId, box.Text);
        }
    }

    private void QCardExpressionObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: QPlacard card } box)
        {
            _cCardField.CCardExpressionSet(card.QPlacardCollocated, card.QPlacardId, box.Text);
        }
    }

    private void QCardDefinitionObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: QPlacard card } box)
        {
            _cCardField.CCardMeaningSet(card.QPlacardCollocated, card.QPlacardId, box.Text);
        }
    }
}
