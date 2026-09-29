using System.Globalization;
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
                entry.PreviewKeyDown -= PContextCaretObserve;
                entry.PreviewKeyDown += PContextCaretObserve;
                entry.LostKeyboardFocus -= PContextCloseObserve;
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
            name.Text = (string)new QStateConverter().Convert(
                [chip.PContextText, QLocalizationCatalog.QLocalizationTextRead("Display.Unknown")],
                typeof(string),
                string.Empty,
                CultureInfo.CurrentCulture);
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

    internal void PContextAttach(PCard card)
    {
        card.PCardContextNotice = text => PCandidateShow(card, text);
    }

    private void PContextTextObserve(PContextCaret caret, string text)
    {
        if (PCardContextFind(caret) is PCard card)
        {
            card.PCardContextRefine(_lEditor.LEditorStudio.CEditorCard.CCardSituationAdd(
                card.PCardId, text, card.PCardContextPosition, false));
        }
    }

    private void PContextChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PContext chip } && PCardContextFind(chip) is PCard card)
        {
            PContextEraseObserve(card, chip);
        }
    }

    private void PContextCaretObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PContextCaret row } box)
        {
            return;
        }

        PCard? card = PCardContextFind(row);
        if (card is null)
        {
            return;
        }

        if (PCandidate.IsOpen && PCandidateHandle(e.Key))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            PContextCommitObserve(card);
            e.Handled = true;
            return;
        }

        e.Handled = PCaretKeyApply(
            box,
            e.Key,
            step => PContextEraseObserve(card, card.PCardContextFind(step)),
            card.PCardContextMove,
            () => PEditorCaretApply(box, row, 0));
    }

    private void PContextCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PContextCaret row } && PCardContextFind(row) is PCard card)
        {
            PContextCommitObserve(card);
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

    private void PContextCommitObserve(PCard card)
    {
        PCandidateHide();
        _lEditor.LEditorStudio.CEditorCard.CCardSituationAdd(
            card.PCardId, card.PCardContextText, card.PCardContextPosition, true);
        card.PCardContextClear();
    }

    private void PContextEraseObserve(PCard card, PContext? chip)
    {
        if (chip is not null)
        {
            _lEditor.LEditorStudio.CEditorCard.CCardSituationRemove(card.PCardId, chip.PContextId);
        }
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
