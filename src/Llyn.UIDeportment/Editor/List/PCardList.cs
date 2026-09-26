using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<PCard> _pMeaningList = [];
    private readonly ObservableCollection<PCard> _pCollocationList = [];

    private readonly PMeaningTemplate _pMeaningTemplate;
    private readonly PCollocationTemplate _pCollocationTemplate;

    private ItemsControl PMeaningList => (ItemsControl)FindName(nameof(PMeaningList));

    private Button PMeaningAddition => (Button)FindName(nameof(PMeaningAddition));

    private ItemsControl PCollocationList => (ItemsControl)FindName(nameof(PCollocationList));

    private Button PCollocationAddition => (Button)FindName(nameof(PCollocationAddition));

    private void PCardListAttach()
    {
        PMeaningList.ItemsSource = _pMeaningList;
        QLookItem.QLookItemAttach(PMeaningList, PMeaningApply);
        PMeaningList.LostMouseCapture += PCardDragCancel;
        PMeaningList.MouseLeftButtonUp += PCardDragFinish;
        PMeaningList.MouseMove += PCardDragUpdate;
        PMeaningAddition.Click += PMeaningHandle;
        PCollocationList.ItemsSource = _pCollocationList;
        QLookItem.QLookItemAttach(PCollocationList, PCollocationApply);
        PCollocationList.LostMouseCapture += PCardDragCancel;
        PCollocationList.MouseLeftButtonUp += PCardDragFinish;
        PCollocationList.MouseMove += PCardDragUpdate;
        PCollocationAddition.Click += PCollocationHandle;
    }

    private void PMeaningApply(FrameworkElement container, object item, string? changed)
    {
        if (item is not PCard card)
        {
            return;
        }

        PCard.PCardRowApply(container, card, "Card.DefinitionHint", changed);
        PCardApply(container, card);
        if (QLook.QLookPartFind<Border>(container, "PCardHeader") is Border header)
        {
            header.MouseLeftButtonDown -= _pMeaningTemplate.PCardDragHandle;
            header.MouseLeftButtonDown += _pMeaningTemplate.PCardDragHandle;
        }

        if (QLook.QLookPartFind<Border>(container, "PCardPosition") is Border position)
        {
            position.MouseLeftButtonDown -= _pMeaningTemplate.PCardPositionHandle;
            position.MouseLeftButtonDown += _pMeaningTemplate.PCardPositionHandle;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardPositionText") is TextBox ordinal)
        {
            ordinal.KeyDown -= _pMeaningTemplate.PCardPositionAccept;
            ordinal.KeyDown += _pMeaningTemplate.PCardPositionAccept;
            ordinal.LostFocus -= _pMeaningTemplate.PCardPositionCommit;
            ordinal.LostFocus += _pMeaningTemplate.PCardPositionCommit;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardEraser") is Button eraser)
        {
            eraser.Click -= _pMeaningTemplate.PCardHandle;
            eraser.Click += _pMeaningTemplate.PCardHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardImageChooser") is Button image)
        {
            image.Click -= _pMeaningTemplate.PImageAddHandle;
            image.Click += _pMeaningTemplate.PImageAddHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardVideoChooser") is Button video)
        {
            video.Click -= _pMeaningTemplate.PVideoAddHandle;
            video.Click += _pMeaningTemplate.PVideoAddHandle;
        }
    }

    private void PCollocationApply(FrameworkElement container, object item, string? changed)
    {
        if (item is not PCard card)
        {
            return;
        }

        PCard.PCardRowApply(container, card, "Card.MeaningHint", changed);
        PCardApply(container, card);
        if (QLook.QLookPartFind<Border>(container, "PCardHeader") is Border header)
        {
            header.MouseLeftButtonDown -= _pCollocationTemplate.PCardDragHandle;
            header.MouseLeftButtonDown += _pCollocationTemplate.PCardDragHandle;
        }

        if (QLook.QLookPartFind<Border>(container, "PCardPosition") is Border position)
        {
            position.MouseLeftButtonDown -= _pCollocationTemplate.PCardPositionHandle;
            position.MouseLeftButtonDown += _pCollocationTemplate.PCardPositionHandle;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardPositionText") is TextBox ordinal)
        {
            ordinal.KeyDown -= _pCollocationTemplate.PCardPositionAccept;
            ordinal.KeyDown += _pCollocationTemplate.PCardPositionAccept;
            ordinal.LostFocus -= _pCollocationTemplate.PCardPositionCommit;
            ordinal.LostFocus += _pCollocationTemplate.PCardPositionCommit;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardEraser") is Button eraser)
        {
            eraser.Click -= _pCollocationTemplate.PCardHandle;
            eraser.Click += _pCollocationTemplate.PCardHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardImageChooser") is Button image)
        {
            image.Click -= _pCollocationTemplate.PImageAddHandle;
            image.Click += _pCollocationTemplate.PImageAddHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardVideoChooser") is Button video)
        {
            video.Click -= _pCollocationTemplate.PVideoAddHandle;
            video.Click += _pCollocationTemplate.PVideoAddHandle;
        }
    }

    private void PCardApply(FrameworkElement container, PCard card)
    {
        if (QLook.QLookPartFind<ItemsControl>(container, "PCardContext") is ItemsControl context)
        {
            PContextFieldApply(context);
            context.ItemsSource = card.PCardContext;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardRegister") is ItemsControl register)
        {
            PRegisterFieldApply(register);
            register.ItemsSource = card.PCardRegister;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardLink") is ItemsControl link)
        {
            PLinkFieldApply(link);
            link.ItemsSource = card.PCardLink;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardLabel") is ItemsControl label)
        {
            PLabelFieldApply(label);
            label.ItemsSource = card.PCardLabel;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardSentence") is ItemsControl sentence)
        {
            sentence.ItemsSource = card.PCardSentence;
            QLookItem.QLookItemAttach(sentence, PSentenceApply);
            PSentenceRevealAttach(sentence);
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardImage") is ItemsControl images)
        {
            images.ItemsSource = card.PCardImage;
            QLookItem.QLookItemAttach(images, PImageApply);
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PCardVideo") is ItemsControl videos)
        {
            videos.ItemsSource = card.PCardVideo;
            QLookItem.QLookItemAttach(videos, PVideoApply);
        }
    }

    private void PMeaningHandle(object sender, RoutedEventArgs e)
    {
        PEditorRequestSend(
            new LRequestCardAddition(PEditorDraft, LCardKind.LCardKindMeaning, 0, _pMeaningList.Count));
    }

    private void PCollocationHandle(object sender, RoutedEventArgs e)
    {
        PEditorRequestSend(
            new LRequestCardAddition(PEditorDraft, LCardKind.LCardKindCollocation, 0, _pCollocationList.Count));
    }

    internal void PCardHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCard card })
        {
            return;
        }

        ObservableCollection<PCard>? list = PCardListFind(card);

        if (list is null || list.Count <= 1)
        {
            return;
        }

        PEditorRequestSend(new LRequestCardRemoval(PEditorDraft, card.PCardId));
    }

    internal void PCardPositionHandle(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount < 2 || sender is not FrameworkElement { DataContext: PCard card } badge)
        {
            return;
        }

        ObservableCollection<PCard>? list = PCardListFind(card);

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
                if (PEditorCaretFind(badge) is not TextBox box || box.DataContext != card)
                {
                    return;
                }

                box.Focus();
                box.SelectAll();
            });
    }

    internal void PCardPositionAccept(object sender, KeyEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCard card })
        {
            return;
        }

        if (e.Key is Key.Enter)
        {
            e.Handled = true;
            PCardPositionApply(card);
            return;
        }

        if (e.Key is Key.Escape)
        {
            e.Handled = true;
            card.PCardPositionHide();
        }
    }

    internal void PCardPositionCommit(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card } && card.PCardPositionActive)
        {
            PCardPositionApply(card);
        }
    }

    private void PCardPositionApply(PCard card)
    {
        string written = card.PCardPositionText;
        card.PCardPositionHide();

        ObservableCollection<PCard>? list = PCardListFind(card);

        if (list is null || list.Count <= 1)
        {
            return;
        }

        int current = list.IndexOf(card);

        if (current < 0 ||
            !int.TryParse(written, NumberStyles.Integer, CultureInfo.InvariantCulture, out int wanted))
        {
            return;
        }

        int target = wanted < 1 ? 0 : wanted > list.Count ? list.Count - 1 : wanted - 1;

        if (target == current)
        {
            return;
        }

        PCardMove(card, target);
    }

    private void PCardMove(PCard card, int target)
    {
        PEditorRequestSend(new LRequestCardShift(PEditorDraft, card.PCardId, 0, target));
    }

    private ObservableCollection<PCard>? PCardListFind(PCard card)
    {
        return _pMeaningList.Contains(card) ? _pMeaningList
            : _pCollocationList.Contains(card) ? _pCollocationList
            : null;
    }
}
