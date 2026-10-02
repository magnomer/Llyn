using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal static class QAccentControl
{
    private static readonly DependencyProperty QAccentRowProperty = DependencyProperty.RegisterAttached(
        "QAccentRow",
        typeof(DependencyObject),
        typeof(QAccentControl));

    internal static void QAccentControlAttach(ItemsControl host)
    {
        ArgumentNullException.ThrowIfNull(host);
        host.MouseMove += QAccentHoverRefine;
        host.GotKeyboardFocus += QAccentFocusRefine;
    }

    private static void QAccentHoverRefine(object sender, MouseEventArgs e)
    {
        if (sender is ItemsControl host && e.OriginalSource is DependencyObject origin)
        {
            QAccentControlRefine(host, origin);
        }
    }

    private static void QAccentFocusRefine(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is ItemsControl host && e.NewFocus is DependencyObject origin)
        {
            QAccentControlRefine(host, origin);
        }
    }

    private static void QAccentControlRefine(ItemsControl host, DependencyObject origin)
    {
        if (host.ContainerFromElement(origin) is not DependencyObject row
            || ReferenceEquals(host.GetValue(QAccentRowProperty), row))
        {
            return;
        }

        host.SetValue(QAccentRowProperty, row);
        ContentControl? slot = QAccentSlotFind(row);
        if (slot is null || slot.ContentTemplate is not null || slot.Tag is not string key
            || slot.TryFindResource(key) is not DataTemplate template)
        {
            return;
        }

        slot.ContentTemplate = template;
        slot.SetBinding(ContentControl.ContentProperty, new Binding());
    }

    private static ContentControl? QAccentSlotFind(DependencyObject node)
    {
        if (node is ContentControl { Tag: string } slot)
        {
            return slot;
        }

        int count = VisualTreeHelper.GetChildrenCount(node);
        for (int index = 0; index < count; index++)
        {
            ContentControl? found = QAccentSlotFind(VisualTreeHelper.GetChild(node, index));
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }
}
