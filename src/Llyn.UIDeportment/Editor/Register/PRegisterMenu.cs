using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PRegisterTemplate _pRegisterTemplate;

    private void PRegisterApply(FrameworkElement container, object item, string? _)
    {
        if (item is PRegisterCaret caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PRegisterEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.PRegisterCaretHint);
                entry.Text = caret.PRegisterCaretText;
                entry.PreviewKeyDown -= PProfferKeyRefine;
                entry.PreviewKeyDown -= PProfferKeyObserve;
                entry.PreviewKeyDown -= PRegisterCommitObserve;
                entry.PreviewKeyDown -= PRegisterEraseObserve;
                entry.PreviewKeyDown -= PRegisterCaretRefine;
                entry.PreviewKeyDown += PProfferKeyRefine;
                entry.PreviewKeyDown += PProfferKeyObserve;
                entry.PreviewKeyDown += PRegisterCommitObserve;
                entry.PreviewKeyDown += PRegisterEraseObserve;
                entry.PreviewKeyDown += PRegisterCaretRefine;
                entry.LostKeyboardFocus -= PRegisterBlurRefine;
                entry.LostKeyboardFocus -= PRegisterCloseObserve;
                entry.LostKeyboardFocus += PRegisterBlurRefine;
                entry.LostKeyboardFocus += PRegisterCloseObserve;
            }

            return;
        }

        if (item is not PRegister chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PRegisterName") is TextBlock name)
        {
            name.Text = (string)new QStateConverter().Convert(
                [chip.PRegisterText, QLocalizationCatalog.QLocalizationTextRead("Display.Unknown")],
                typeof(string),
                string.Empty,
                CultureInfo.CurrentCulture);
        }

        if (QLook.QLookPartFind<Button>(container, "PRegisterEraser") is Button eraser)
        {
            eraser.Click -= PRegisterChipObserve;
            eraser.Click += PRegisterChipObserve;
            if (QLook.QLookPartFind<QIconImage>(eraser, "PRegisterIcon") is QIconImage icon)
            {
                icon.QIconSource = QIcon.QIconResolve("close", 12);
            }
        }
    }

    private void PRegisterFieldApply(ItemsControl list)
    {
        list.ItemTemplateSelector ??= new PRegisterSelector
        {
            PRegisterSelectorChip = (DataTemplate)_pRegisterTemplate["Theme.Register.Chip"],
            PRegisterSelectorCaret = (DataTemplate)_pRegisterTemplate["Theme.Register.Entry"],
        };
        QLookItem.QLookItemAttach(list, PRegisterApply);
        if (QLook.QLookPartFind<Border>(list, "PRegisterFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= PRegisterFocusRefine;
            frame.MouseLeftButtonDown += PRegisterFocusRefine;
        }
    }

    private void PRegisterTextObserve(PRegisterCaret caret, string text)
    {
        if (PCardRegisterFind(caret) is PCard card)
        {
            PProfferRegisterRefine(card, _qEditor.QEditorArea.CEditorCard.CCardRegisterAdd(
                card.PCardId, text, card.PCardRegisterPosition, false));
        }
    }

    private void PRegisterChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PRegister chip } && PCardRegisterFind(chip) is PCard card)
        {
            _qEditor.QEditorArea.CEditorCard.CCardRegisterRemove(card.PCardId, chip.PRegisterId);
        }
    }

    private void PRegisterCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not FrameworkElement { DataContext: PRegisterCaret row }
            || PCardRegisterFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = true;
        PProfferRegisterRefine(card, _qEditor.QEditorArea.CEditorCard.CCardRegisterAdd(
            card.PCardId, card.PCardRegisterText, card.PCardRegisterPosition, true));
    }

    private void PRegisterEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PRegisterCaret row } box || PCardRegisterFind(row) is not PCard card)
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
                if (card.PCardRegisterFind(step) is PRegister chip)
                {
                    _qEditor.QEditorArea.CEditorCard.CCardRegisterRemove(card.PCardId, chip.PRegisterId);
                }
            });
    }

    private void PRegisterCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PRegisterCaret row } box || PCardRegisterFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            card.PCardRegisterMove,
            () => PEditorCaretApply(box, row, 0));
    }

    private void PRegisterBlurRefine(object sender, RoutedEventArgs e)
    {
        PProfferShutRefine();
    }

    private void PRegisterCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PRegisterCaret row } && PCardRegisterFind(row) is PCard card)
        {
            PProfferRegisterRefine(card, _qEditor.QEditorArea.CEditorCard.CCardRegisterAdd(
                card.PCardId, card.PCardRegisterText, card.PCardRegisterPosition, true));
        }
    }

    private void PRegisterFocusRefine(object sender, MouseButtonEventArgs e)
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

    internal PCard? PCardRegisterFind(object row)
    {
        foreach (PCard card in _pMeaningList)
        {
            if (card.PCardRegister.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardRegister.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
