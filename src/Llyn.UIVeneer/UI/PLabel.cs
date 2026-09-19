using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
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

        if (box.SelectionLength != 0)
        {
            return;
        }

        if (e.Key == Key.Back && box.CaretIndex == 0)
        {
            PLabelRemove(card, card.PCardLabelFind(-1));
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Delete && box.CaretIndex == box.Text.Length)
        {
            PLabelRemove(card, card.PCardLabelFind(1));
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Left && box.Text.Length == 0 && card.PCardLabelMove(-1))
        {
            PEditorCaretApply(box, row, 0);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Right && box.Text.Length == 0 && card.PCardLabelMove(1))
        {
            PEditorCaretApply(box, row, 0);
            e.Handled = true;
        }
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
