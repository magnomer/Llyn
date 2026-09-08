using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PLabelChipHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PLabelChip chip })
        {
            PCardLabelFind(chip)?.PCardLabelRemove(chip);
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

        if (e.Key == Key.Enter)
        {
            card.PCardLabelCommit();
            e.Handled = true;
            return;
        }

        if (box.SelectionLength != 0)
        {
            return;
        }

        if (e.Key == Key.Back && box.CaretIndex == 0)
        {
            card.PCardLabelRemove(-1);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Delete && box.CaretIndex == box.Text.Length)
        {
            card.PCardLabelRemove(1);
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
        if (sender is FrameworkElement { DataContext: PLabelCaret row })
        {
            PCardLabelFind(row)?.PCardLabelCommit();
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
