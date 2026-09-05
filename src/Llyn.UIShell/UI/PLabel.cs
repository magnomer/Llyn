using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

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
            PLabelCaretApply(box, row, 0);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Right && box.Text.Length == 0 && card.PCardLabelMove(1))
        {
            PLabelCaretApply(box, row, 0);
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

        TextBox? entry = PLabelCaretFind(surface);
        if (entry is null)
        {
            return;
        }

        entry.Focus();
        entry.CaretIndex = entry.Text.Length;
        e.Handled = true;
    }

    private static void PLabelCaretApply(TextBox box, PLabelCaret row, int caret)
    {
        ItemsControl? host = ItemsControl.ItemsControlFromItemContainer(box) ?? PLabelHostFind(box);
        box.Dispatcher.BeginInvoke(
            DispatcherPriority.Input,
            () =>
            {
                TextBox? entry = host is null ? box : PLabelCaretFind(host) ?? box;
                if (entry.DataContext != row)
                {
                    return;
                }

                entry.Focus();
                entry.CaretIndex = caret > entry.Text.Length ? entry.Text.Length : caret;
            });
    }

    private static ItemsControl? PLabelHostFind(DependencyObject start)
    {
        DependencyObject? step = start;
        while (step is not null)
        {
            if (step is ItemsControl host)
            {
                return host;
            }

            step = VisualTreeHelper.GetParent(step);
        }

        return null;
    }

    private static TextBox? PLabelCaretFind(DependencyObject root)
    {
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(root, index);
            if (child is TextBox { DataContext: PLabelCaret } box)
            {
                return box;
            }

            TextBox? found = PLabelCaretFind(child);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
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
