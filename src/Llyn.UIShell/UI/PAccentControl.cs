using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIShell;

internal static class PAccentControl
{
    private static readonly DependencyProperty PAccentRowProperty = DependencyProperty.RegisterAttached(
        "PAccentRow",
        typeof(DependencyObject),
        typeof(PAccentControl));

    internal static void PAccentControlAttach(ItemsControl host)
    {
        ArgumentNullException.ThrowIfNull(host);
        host.MouseMove += PAccentHoverHandle;
        host.GotKeyboardFocus += PAccentFocusHandle;
    }

    private static void PAccentHoverHandle(object sender, MouseEventArgs e)
    {
        if (sender is ItemsControl host && e.OriginalSource is DependencyObject origin)
        {
            PAccentControlShow(host, origin);
        }
    }

    private static void PAccentFocusHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is ItemsControl host && e.NewFocus is DependencyObject origin)
        {
            PAccentControlShow(host, origin);
        }
    }

    private static void PAccentControlShow(ItemsControl host, DependencyObject origin)
    {
        if (host.ContainerFromElement(origin) is not DependencyObject row
            || ReferenceEquals(host.GetValue(PAccentRowProperty), row))
        {
            return;
        }

        host.SetValue(PAccentRowProperty, row);
        ContentControl? slot = PAccentSlotFind(row);
        if (slot is null || slot.ContentTemplate is not null || slot.Tag is not string key
            || slot.TryFindResource(key) is not DataTemplate template)
        {
            return;
        }

        slot.ContentTemplate = template;
        slot.SetBinding(ContentControl.ContentProperty, new Binding());
    }

    private static ContentControl? PAccentSlotFind(DependencyObject node)
    {
        if (node is ContentControl { Tag: string } slot)
        {
            return slot;
        }

        int count = VisualTreeHelper.GetChildrenCount(node);
        for (int index = 0; index < count; index++)
        {
            ContentControl? found = PAccentSlotFind(VisualTreeHelper.GetChild(node, index));
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }
}
