using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PTagChipHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PTagChip chip })
        {
            PCardTagFind(chip)?.PCardTagRemove(chip);
        }
    }

    internal void PTagEntryHandle(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PTagEntry row } box)
        {
            return;
        }

        PCard? card = PCardTagFind(row);
        if (card is null)
        {
            return;
        }

        if (e.Key == Key.Enter)
        {
            card.PCardTagCommit();
            e.Handled = true;
            return;
        }

        if (box.SelectionLength != 0)
        {
            return;
        }

        if (e.Key == Key.Back && box.CaretIndex == 0)
        {
            card.PCardTagRemove(-1);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Delete && box.CaretIndex == box.Text.Length)
        {
            card.PCardTagRemove(1);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Left && box.Text.Length == 0 && card.PCardTagMove(-1))
        {
            PTagEntryApply(box, row, 0);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Right && box.Text.Length == 0 && card.PCardTagMove(1))
        {
            PTagEntryApply(box, row, 0);
            e.Handled = true;
        }
    }

    internal void PTagCloseHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PTagEntry row })
        {
            PCardTagFind(row)?.PCardTagCommit();
        }
    }

    internal void PTagFocusHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DependencyObject surface)
        {
            return;
        }

        TextBox? entry = PTagEntryFind(surface);
        if (entry is null)
        {
            return;
        }

        entry.Focus();
        entry.CaretIndex = entry.Text.Length;
        e.Handled = true;
    }

    private static void PTagEntryApply(TextBox box, PTagEntry row, int caret)
    {
        ItemsControl? host = ItemsControl.ItemsControlFromItemContainer(box) ?? PTagHostFind(box);
        box.Dispatcher.BeginInvoke(
            DispatcherPriority.Input,
            () =>
            {
                TextBox? entry = host is null ? box : PTagEntryFind(host) ?? box;
                if (entry.DataContext != row)
                {
                    return;
                }

                entry.Focus();
                entry.CaretIndex = caret > entry.Text.Length ? entry.Text.Length : caret;
            });
    }

    private static ItemsControl? PTagHostFind(DependencyObject start)
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

    private static TextBox? PTagEntryFind(DependencyObject root)
    {
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(root, index);
            if (child is TextBox { DataContext: PTagEntry } box)
            {
                return box;
            }

            TextBox? found = PTagEntryFind(child);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    private PCard? PCardTagFind(object row)
    {
        foreach (PCard card in _pSenseList)
        {
            if (card.PCardTag.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardTag.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
