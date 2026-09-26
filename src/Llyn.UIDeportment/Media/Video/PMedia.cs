using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PMedia
{
    public static readonly DependencyProperty PMediaProperty = DependencyProperty.RegisterAttached(
        "PMedia",
        typeof(PMedia),
        typeof(PMedia),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    private readonly LWindow _lWindow;

    private PMedia(LWindow window)
    {
        _lWindow = window;
    }

    internal static void PMediaAttach(DependencyObject root, LWindow window)
    {
        root.SetValue(PMediaProperty, new PMedia(window));
    }

    internal static PMedia? PMediaRead(DependencyObject element)
    {
        return element.GetValue(PMediaProperty) as PMedia;
    }

    internal PImage PMediaImageCreate(LImageDraft draft)
    {
        return new PImage(_lWindow, draft);
    }

    internal PVideo PMediaVideoCreate(LVideoDraft draft)
    {
        return new PVideo(_lWindow, draft);
    }

    internal static void PMediaRevealAttach(ItemsControl list)
    {
        list.MouseEnter -= PMediaRevealHandle;
        list.MouseEnter += PMediaRevealHandle;
        list.MouseLeave -= PMediaRevealHandle;
        list.MouseLeave += PMediaRevealHandle;
        list.IsKeyboardFocusWithinChanged -= PMediaRevealHandle;
        list.IsKeyboardFocusWithinChanged += PMediaRevealHandle;
        PMediaRevealApply(list);
    }

    private static void PMediaRevealHandle(object sender, MouseEventArgs e)
    {
        PMediaRevealApply((ItemsControl)sender);
    }

    private static void PMediaRevealHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        PMediaRevealApply((ItemsControl)sender);
    }

    private static void PMediaRevealApply(ItemsControl list)
    {
        bool shown = list.IsMouseOver || list.IsKeyboardFocusWithin;
        foreach (object item in list.Items)
        {
            if (list.ItemContainerGenerator.ContainerFromItem(item) is not FrameworkElement container
                || PLook.PLookPartFind<FrameworkElement>(container, "PMediaControl") is not FrameworkElement control)
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
