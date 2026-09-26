using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PLabelTemplate _pLabelTemplate;

    private void PLabelApply(FrameworkElement container, object item, string? _)
    {
        if (item is PLabelCaret caret)
        {
            if (PLook.PLookPartFind<TextBox>(container, "PLabelEntry") is TextBox entry)
            {
                entry.Tag = caret.PLabelCaretHint;
                entry.Text = caret.PLabelCaretText;
                entry.PreviewKeyDown -= _pLabelTemplate.PLabelCaretHandle;
                entry.PreviewKeyDown += _pLabelTemplate.PLabelCaretHandle;
                entry.LostKeyboardFocus -= _pLabelTemplate.PLabelCloseHandle;
                entry.LostKeyboardFocus += _pLabelTemplate.PLabelCloseHandle;
            }

            return;
        }

        if (item is not PLabelChip chip)
        {
            return;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PLabelName") is TextBlock name)
        {
            name.Text = chip.PLabelChipName;
        }

        if (PLook.PLookPartFind<Button>(container, "PLabelEraser") is Button eraser)
        {
            eraser.Click -= _pLabelTemplate.PLabelChipHandle;
            eraser.Click += _pLabelTemplate.PLabelChipHandle;
            if (PLook.PLookPartFind<PIconImage>(eraser, "PLabelIcon") is PIconImage icon)
            {
                icon.PIconSource = PIcon.PIconResolve("close", 12);
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
        PLookItem.PLookItemAttach(list, PLabelApply);
        if (PLook.PLookPartFind<Border>(list, "PLabelFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= _pLabelTemplate.PLabelFocusHandle;
            frame.MouseLeftButtonDown += _pLabelTemplate.PLabelFocusHandle;
        }
    }

    internal void PLabelAttach(PCard card)
    {
        card.PCardLabelNotice = text => PSlateShow(card, text);
        card.PCardLabelDispatcher = text => PLabelSend(card, text);
    }

    internal void PLabelChipHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PLabelChip chip } && PCardLabelFind(chip) is PCard card)
        {
            PLabelRemove(card, chip);
        }
    }

    internal void PLabelCaretHandle(object sender, KeyEventArgs e)
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
            PLabelCommit(card);
            e.Handled = true;
            return;
        }

        e.Handled = PCaretKeyApply(
            box,
            e.Key,
            step => PLabelRemove(card, card.PCardLabelFind(step)),
            card.PCardLabelMove,
            () => PEditorCaretApply(box, row, 0));
    }

    internal void PLabelCloseHandle(object sender, RoutedEventArgs e)
    {
        PSlateHide();

        if (sender is FrameworkElement { DataContext: PLabelCaret row } && PCardLabelFind(row) is PCard card)
        {
            PLabelCommit(card);
        }
    }

    private void PLabelCommit(PCard card)
    {
        PLabelSend(card, card.PCardLabelText);
        card.PCardLabelClear();
    }

    private bool PLabelSend(PCard card, string text)
    {
        string written = (text ?? string.Empty).Trim();
        if (written.Length == 0 || card.PCardLabelCheck(written))
        {
            return false;
        }

        PEditorRequestSend(new LRequestTagAddition(PEditorDraft, card.PCardId, written, card.PCardLabelPosition));
        return true;
    }

    private void PLabelSend(PCard card, long id)
    {
        PEditorRequestSend(new LRequestTagPick(PEditorDraft, card.PCardId, id, card.PCardLabelPosition));
    }

    private void PLabelRemove(PCard card, PLabelChip? chip)
    {
        if (chip is not null)
        {
            PEditorRequestSend(new LRequestTagRemoval(PEditorDraft, card.PCardId, chip.PLabelChipId));
        }
    }

    internal void PLabelFocusHandle(object sender, MouseButtonEventArgs e)
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
