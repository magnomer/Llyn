using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    internal static void PEditorCaretApply(TextBox box, object row, int caret)
    {
        ItemsControl? host = ItemsControl.ItemsControlFromItemContainer(box) ?? PEditorHostFind(box);
        box.Dispatcher.BeginInvoke(
            DispatcherPriority.Input,
            () =>
            {
                TextBox? entry = host is null ? box : PEditorCaretFind(host) ?? box;
                if (entry.DataContext != row)
                {
                    return;
                }

                entry.Focus();
                entry.CaretIndex = caret > entry.Text.Length ? entry.Text.Length : caret;
            });
    }

    internal static ItemsControl? PEditorHostFind(DependencyObject start)
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

    internal static TextBox? PEditorCaretFind(DependencyObject root)
    {
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(root, index);
            if (child is TextBox box)
            {
                return box;
            }

            TextBox? found = PEditorCaretFind(child);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }
}
