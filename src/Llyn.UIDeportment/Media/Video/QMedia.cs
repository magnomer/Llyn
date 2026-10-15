using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

internal static class QMedia
{
    internal static void QMediaRevealAttach(ItemsControl list)
    {
        list.MouseEnter -= QMediaRevealRefine;
        list.MouseEnter += QMediaRevealRefine;
        list.MouseLeave -= QMediaRevealRefine;
        list.MouseLeave += QMediaRevealRefine;
        list.IsKeyboardFocusWithinChanged -= QMediaRevealRefine;
        list.IsKeyboardFocusWithinChanged += QMediaRevealRefine;
        QMediaRevealApply(list);
    }

    private static void QMediaRevealRefine(object sender, MouseEventArgs e)
    {
        QMediaRevealApply((ItemsControl)sender);
    }

    private static void QMediaRevealRefine(object sender, DependencyPropertyChangedEventArgs e)
    {
        QMediaRevealApply((ItemsControl)sender);
    }

    private static void QMediaRevealApply(ItemsControl list)
    {
        bool shown = list.IsMouseOver || list.IsKeyboardFocusWithin;
        foreach (object item in list.Items)
        {
            if (list.ItemContainerGenerator.ContainerFromItem(item) is not FrameworkElement container
                || QLook.QLookPartFind<FrameworkElement>(container, "PMediaControl") is not FrameworkElement control)
            {
                continue;
            }

            if (shown)
            {
                control.Opacity = 1;
                control.IsHitTestVisible = true;
            }
            else
            {
                control.ClearValue(UIElement.OpacityProperty);
                control.ClearValue(UIElement.IsHitTestVisibleProperty);
            }
        }
    }
}
