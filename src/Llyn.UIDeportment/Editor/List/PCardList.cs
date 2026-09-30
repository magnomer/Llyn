using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<PCard> _pMeaningList = [];
    private readonly ObservableCollection<PCard> _pCollocationList = [];

    private ItemsControl PMeaningList => (ItemsControl)FindName(nameof(PMeaningList));

    private Button PMeaningAddition => (Button)FindName(nameof(PMeaningAddition));

    private ItemsControl PCollocationList => (ItemsControl)FindName(nameof(PCollocationList));

    private Button PCollocationAddition => (Button)FindName(nameof(PCollocationAddition));

    private void PCardListAttach()
    {
        PMeaningList.ItemsSource = _pMeaningList;
        QLookItem.QLookItemAttach(PMeaningList, PMeaningApply);
        PMeaningList.LostMouseCapture += PCardDragReset;
        PMeaningList.MouseLeftButtonUp += PCardDragReset;
        PMeaningList.MouseMove += PCardDragUpdate;
        PMeaningAddition.Click += PMeaningAddObserve;
        PCollocationList.ItemsSource = _pCollocationList;
        QLookItem.QLookItemAttach(PCollocationList, PCollocationApply);
        PCollocationList.LostMouseCapture += PCardDragReset;
        PCollocationList.MouseLeftButtonUp += PCardDragReset;
        PCollocationList.MouseMove += PCardDragUpdate;
        PCollocationAddition.Click += PCollocationAddObserve;
    }

    private void PMeaningApply(FrameworkElement container, object item, string? changed)
    {
        if (item is not PCard card)
        {
            return;
        }

        PCard.PCardRowApply(container, card, changed);
        PCardApply(container, card);
        if (QLook.QLookPartFind<Border>(container, "PCardHeader") is Border header)
        {
            header.MouseLeftButtonDown -= PCardDragRefine;
            header.MouseLeftButtonDown += PCardDragRefine;
        }

        if (QLook.QLookPartFind<Border>(container, "PCardPosition") is Border position)
        {
            position.MouseLeftButtonDown -= PCardPositionRefine;
            position.MouseLeftButtonDown += PCardPositionRefine;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardPositionText") is TextBox ordinal)
        {
            ordinal.KeyDown -= PCardPositionRefine;
            ordinal.KeyDown -= PCardPositionObserve;
            ordinal.LostFocus -= PCardPositionObserve;
            ordinal.KeyDown += PCardPositionRefine;
            ordinal.KeyDown += PCardPositionObserve;
            ordinal.LostFocus += PCardPositionObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardEraser") is Button eraser)
        {
            eraser.Click -= PCardRemoveObserve;
            eraser.Click += PCardRemoveObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardImageChooser") is Button image)
        {
            image.Click -= PImageAddObserve;
            image.Click += PImageAddObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardVideoChooser") is Button video)
        {
            video.Click -= PVideoAddObserve;
            video.Click += PVideoAddObserve;
        }
    }

    private void PCollocationApply(FrameworkElement container, object item, string? changed)
    {
        if (item is not PCard card)
        {
            return;
        }

        PCard.PCardRowApply(container, card, changed);
        PCardApply(container, card);
        if (QLook.QLookPartFind<Border>(container, "PCardHeader") is Border header)
        {
            header.MouseLeftButtonDown -= PCardDragRefine;
            header.MouseLeftButtonDown += PCardDragRefine;
        }

        if (QLook.QLookPartFind<Border>(container, "PCardPosition") is Border position)
        {
            position.MouseLeftButtonDown -= PCardPositionRefine;
            position.MouseLeftButtonDown += PCardPositionRefine;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PCardPositionText") is TextBox ordinal)
        {
            ordinal.KeyDown -= PCardPositionRefine;
            ordinal.KeyDown -= PCardPositionObserve;
            ordinal.LostFocus -= PCardPositionObserve;
            ordinal.KeyDown += PCardPositionRefine;
            ordinal.KeyDown += PCardPositionObserve;
            ordinal.LostFocus += PCardPositionObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardEraser") is Button eraser)
        {
            eraser.Click -= PCardRemoveObserve;
            eraser.Click += PCardRemoveObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardImageChooser") is Button image)
        {
            image.Click -= PImageAddObserve;
            image.Click += PImageAddObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PCardVideoChooser") is Button video)
        {
            video.Click -= PVideoAddObserve;
            video.Click += PVideoAddObserve;
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

    private void PMeaningAddObserve(object sender, RoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorList.CCardMeaningAdd();
    }

    private void PCollocationAddObserve(object sender, RoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorList.CCardCollocationAdd();
    }

    private void PCardRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card })
        {
            _qEditor.QEditorArea.CEditorList.CCardRemove(card.PCardId);
        }
    }

    private void PCardPositionRefine(object sender, MouseButtonEventArgs e)
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

    private void PCardPositionRefine(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Escape && sender is FrameworkElement { DataContext: PCard card })
        {
            e.Handled = true;
            card.PCardPositionHide();
        }
    }

    private void PCardPositionObserve(object sender, RoutedEventArgs e)
    {
        if (e is KeyEventArgs { Key: not Key.Enter }
            || sender is not FrameworkElement { DataContext: PCard card }
            || !card.PCardPositionActive)
        {
            return;
        }

        _qEditor.QEditorArea.CEditorList.CCardMove(card.PCardId, card.PCardPositionText);
        if (e is KeyEventArgs)
        {
            e.Handled = true;
        }

        card.PCardPositionHide();
    }

    private ObservableCollection<PCard>? PCardListFind(PCard card)
    {
        return _pMeaningList.Contains(card) ? _pMeaningList
            : _pCollocationList.Contains(card) ? _pCollocationList
            : null;
    }
}
