using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PMedia
{
    public static readonly DependencyProperty PMediaProperty = DependencyProperty.RegisterAttached(
        "PMedia",
        typeof(PMedia),
        typeof(PMedia),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    private PMedia()
    {
    }

    internal static void PMediaAttach(DependencyObject root)
    {
        root.SetValue(PMediaProperty, new PMedia());
    }

    internal static PMedia? PMediaRead(DependencyObject element)
    {
        return element.GetValue(PMediaProperty) as PMedia;
    }

    internal PImage PMediaImageCreate(CImageDraft draft)
    {
        return new PImage(draft);
    }

    internal PVideo PMediaVideoCreate(CVideoDraft draft)
    {
        return new PVideo(draft);
    }

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
