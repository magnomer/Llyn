using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        if (e.Key == Key.Back && box.Text.Length == 0)
        {
            card.PCardTagRemove();
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
