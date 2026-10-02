using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

internal static class PMedia
{
    internal static void PMediaRevealAttach(ItemsControl list)
    {
        list.MouseEnter -= PMediaRevealRefine;
        list.MouseEnter += PMediaRevealRefine;
        list.MouseLeave -= PMediaRevealRefine;
        list.MouseLeave += PMediaRevealRefine;
        list.IsKeyboardFocusWithinChanged -= PMediaRevealRefine;
        list.IsKeyboardFocusWithinChanged += PMediaRevealRefine;
        PMediaRevealApply(list);
    }

    private static void PMediaRevealRefine(object sender, MouseEventArgs e)
    {
        PMediaRevealApply((ItemsControl)sender);
    }

    private static void PMediaRevealRefine(object sender, DependencyPropertyChangedEventArgs e)
    {
        PMediaRevealApply((ItemsControl)sender);
    }

    private static void PMediaRevealApply(ItemsControl list)
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
