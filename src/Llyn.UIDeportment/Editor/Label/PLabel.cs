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
                entry.PreviewKeyDown -= PSlateKeyRefine;
                entry.PreviewKeyDown -= PSlateKeyObserve;
                entry.PreviewKeyDown -= PLabelCommitObserve;
                entry.PreviewKeyDown -= PLabelEraseObserve;
                entry.PreviewKeyDown -= PLabelCaretRefine;
                entry.PreviewKeyDown += PSlateKeyRefine;
                entry.PreviewKeyDown += PSlateKeyObserve;
                entry.PreviewKeyDown += PLabelCommitObserve;
                entry.PreviewKeyDown += PLabelEraseObserve;
                entry.PreviewKeyDown += PLabelCaretRefine;
                entry.LostKeyboardFocus -= PLabelBlurRefine;
                entry.LostKeyboardFocus -= PLabelCloseObserve;
                entry.LostKeyboardFocus += PLabelBlurRefine;
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

    private void PLabelTextObserve(PLabelCaret caret, string text)
    {
        if (PCardLabelFind(caret) is PCard card)
        {
            PSlateRefine(card, _qEditor.QEditorArea.CEditorCard.CCardTagAdd(
                card.PCardId, text, card.PCardLabelPosition, false));
        }
    }

    private void PLabelChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PLabelChip chip } && PCardLabelFind(chip) is PCard card)
        {
            _qEditor.QEditorArea.CEditorCard.CCardTagRemove(card.PCardId, chip.PLabelChipId);
        }
    }

    private void PLabelCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not FrameworkElement { DataContext: PLabelCaret row }
            || PCardLabelFind(row) is not PCard card)
        {
            return;
        }

        _qEditor.QEditorArea.CEditorCard.CCardTagAdd(
            card.PCardId, card.PCardLabelText, card.PCardLabelPosition, true);
        e.Handled = true;
        PSlateShutRefine();
        card.PCardLabelClear();
    }

    private void PLabelEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PLabelCaret row } box || PCardLabelFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretEdgeApply(
            e.Key.ToString(),
            box.CaretIndex,
            box.Text.Length,
            box.SelectionLength,
            step =>
            {
                if (card.PCardLabelFind(step) is PLabelChip chip)
                {
                    _qEditor.QEditorArea.CEditorCard.CCardTagRemove(card.PCardId, chip.PLabelChipId);
                }
            });
    }

    private void PLabelCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PLabelCaret row } box || PCardLabelFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            card.PCardLabelMove,
            () => PEditorCaretApply(box, row, 0));
    }

    private void PLabelBlurRefine(object sender, RoutedEventArgs e)
    {
        PSlateShutRefine();
    }

    private void PLabelCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PLabelCaret row } && PCardLabelFind(row) is PCard card)
        {
            _qEditor.QEditorArea.CEditorCard.CCardTagAdd(
                card.PCardId, card.PCardLabelText, card.PCardLabelPosition, true);
            card.PCardLabelClear();
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
