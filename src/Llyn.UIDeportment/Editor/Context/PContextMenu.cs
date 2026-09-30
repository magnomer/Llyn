using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PContextTemplate _pContextTemplate;

    private void PContextApply(FrameworkElement container, object item, string? _)
    {
        if (item is PContextCaret caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PContextEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.PContextCaretHint);
                entry.Text = caret.PContextCaretText;
                entry.PreviewKeyDown -= PProfferKeyRefine;
                entry.PreviewKeyDown -= PProfferKeyObserve;
                entry.PreviewKeyDown -= PContextCommitObserve;
                entry.PreviewKeyDown -= PContextEraseObserve;
                entry.PreviewKeyDown -= PContextCaretRefine;
                entry.PreviewKeyDown += PProfferKeyRefine;
                entry.PreviewKeyDown += PProfferKeyObserve;
                entry.PreviewKeyDown += PContextCommitObserve;
                entry.PreviewKeyDown += PContextEraseObserve;
                entry.PreviewKeyDown += PContextCaretRefine;
                entry.LostKeyboardFocus -= PContextBlurRefine;
                entry.LostKeyboardFocus -= PContextCloseObserve;
                entry.LostKeyboardFocus += PContextBlurRefine;
                entry.LostKeyboardFocus += PContextCloseObserve;
            }

            return;
        }

        if (item is not PContext chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PContextName") is TextBlock name)
        {
            QStateConverter.QStateTextRefine(name, TextBlock.TextProperty, chip.PContextText);
        }

        if (QLook.QLookPartFind<Button>(container, "PContextEraser") is Button eraser)
        {
            eraser.Click -= PContextChipObserve;
            eraser.Click += PContextChipObserve;
            if (QLook.QLookPartFind<QIconImage>(eraser, "PContextIcon") is QIconImage icon)
            {
                icon.QIconSource = QIcon.QIconResolve("close", 12);
            }
        }
    }

    private void PContextFieldApply(ItemsControl list)
    {
        list.ItemTemplateSelector ??= new PContextSelector
        {
            PContextSelectorChip = (DataTemplate)_pContextTemplate["Theme.Context.Chip"],
            PContextSelectorCaret = (DataTemplate)_pContextTemplate["Theme.Context.Entry"],
        };
        QLookItem.QLookItemAttach(list, PContextApply);
        if (QLook.QLookPartFind<Border>(list, "PContextFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= PContextFocusRefine;
            frame.MouseLeftButtonDown += PContextFocusRefine;
        }
    }

    private void PContextTextObserve(PContextCaret caret, string text)
    {
        if (PCardContextFind(caret) is PCard card)
        {
            PProfferSituationRefine(card, _qEditor.QEditorArea.CEditorCard.CCardSituationAdd(
                card.PCardId, text, card.PCardContextPosition, false));
        }
    }

    private void PContextChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PContext chip } && PCardContextFind(chip) is PCard card)
        {
            _qEditor.QEditorArea.CEditorCard.CCardSituationRemove(card.PCardId, chip.PContextId);
        }
    }

    private void PContextCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not TextBox { DataContext: PContextCaret row }
            || PCardContextFind(row) is not PCard card)
        {
            return;
        }

        _qEditor.QEditorArea.CEditorCard.CCardSituationAdd(
            card.PCardId, card.PCardContextText, card.PCardContextPosition, true);
        e.Handled = true;
        PProfferShutRefine();
        card.PCardContextClear();
    }

    private void PContextEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PContextCaret row } box || PCardContextFind(row) is not PCard card)
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
                if (card.PCardContextFind(step) is PContext chip)
                {
                    _qEditor.QEditorArea.CEditorCard.CCardSituationRemove(card.PCardId, chip.PContextId);
                }
            });
    }

    private void PContextCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PContextCaret row } box || PCardContextFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            card.PCardContextMove,
            () => PEditorCaretApply(box, row, 0));
    }

    private void PContextBlurRefine(object sender, RoutedEventArgs e)
    {
        PProfferShutRefine();
    }

    private void PContextCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PContextCaret row } && PCardContextFind(row) is PCard card)
        {
            _qEditor.QEditorArea.CEditorCard.CCardSituationAdd(
                card.PCardId, card.PCardContextText, card.PCardContextPosition, true);
            card.PCardContextClear();
        }
    }

    private void PContextFocusRefine(object sender, MouseButtonEventArgs e)
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

    private PCard? PCardContextFind(object row)
    {
        foreach (PCard card in _pMeaningList)
        {
            if (card.PCardContext.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardContext.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
