using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PLabelTemplate _pLabelTemplate;

    private void PLabelApply(FrameworkElement container, object item, string? _)
    {
        if (item is PLabelCaret caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PLabelEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.PLabelCaretHint);
                entry.Text = caret.PLabelCaretText;
                entry.PreviewKeyDown -= PLabelCaretObserve;
                entry.PreviewKeyDown += PLabelCaretObserve;
                entry.LostKeyboardFocus -= PLabelCloseObserve;
                entry.LostKeyboardFocus += PLabelCloseObserve;
            }

            return;
        }

        if (item is not PLabelChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLabelName") is TextBlock name)
        {
            name.Text = chip.PLabelChipName;
        }

        if (QLook.QLookPartFind<Button>(container, "PLabelEraser") is Button eraser)
        {
            eraser.Click -= PLabelChipObserve;
            eraser.Click += PLabelChipObserve;
            if (QLook.QLookPartFind<QIconImage>(eraser, "PLabelIcon") is QIconImage icon)
            {
                icon.QIconSource = QIcon.QIconResolve("close", 12);
            }
        }
    }

    private void PLabelFieldApply(ItemsControl list)
    {
        list.ItemTemplateSelector ??= new PLabelSelector
        {
            PLabelSelectorChip = (DataTemplate)_pLabelTemplate["Theme.Label.Chip"],
            PLabelSelectorCaret = (DataTemplate)_pLabelTemplate["Theme.Label.Entry"],
        };
        QLookItem.QLookItemAttach(list, PLabelApply);
        if (QLook.QLookPartFind<Border>(list, "PLabelFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= PLabelFocusRefine;
            frame.MouseLeftButtonDown += PLabelFocusRefine;
        }
    }

    internal void PLabelAttach(PCard card)
    {
        card.PCardLabelNotice = text => PSlateShow(card, text);
        card.PCardLabelDispatcher =
            text => _lEditor.LEditorCard.CCardTagAdd(card.PCardId, text, card.PCardLabelPosition);
    }

    private void PLabelChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PLabelChip chip } && PCardLabelFind(chip) is PCard card)
        {
            PLabelEraseObserve(card, chip);
        }
    }

    private void PLabelCaretObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PLabelCaret row } box)
        {
            return;
        }

        PCard? card = PCardLabelFind(row);
        if (card is null)
        {
            return;
        }

        if (PSlate.IsOpen && PSlateHandle(e.Key))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            PSlateHide();
            PLabelCommitObserve(card);
            e.Handled = true;
            return;
        }

        e.Handled = PCaretKeyApply(
            box,
            e.Key,
            step => PLabelEraseObserve(card, card.PCardLabelFind(step)),
            card.PCardLabelMove,
            () => PEditorCaretApply(box, row, 0));
    }

    private void PLabelCloseObserve(object sender, RoutedEventArgs e)
    {
        PSlateHide();

        if (sender is FrameworkElement { DataContext: PLabelCaret row } && PCardLabelFind(row) is PCard card)
        {
            PLabelCommitObserve(card);
        }
    }

    private void PLabelCommitObserve(PCard card)
    {
        _lEditor.LEditorCard.CCardTagAdd(card.PCardId, card.PCardLabelText, card.PCardLabelPosition);
        card.PCardLabelClear();
    }

    private void PLabelEraseObserve(PCard card, PLabelChip? chip)
    {
        if (chip is not null)
        {
            _lEditor.LEditorCard.CCardTagRemove(card.PCardId, chip.PLabelChipId);
        }
    }

    private void PLabelFocusRefine(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DependencyObject surface)
        {
            return;
        }

        TextBox? entry = PEditorCaretFind(surface);
        if (entry is null)
        {
            return;
        }

        entry.Focus();
        entry.CaretIndex = entry.Text.Length;
        e.Handled = true;
    }

    private PCard? PCardLabelFind(object row)
    {
        foreach (PCard card in _pMeaningList)
        {
            if (card.PCardLabel.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardLabel.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
