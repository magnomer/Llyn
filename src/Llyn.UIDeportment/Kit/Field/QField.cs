using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace Llyn.UIDeportment;

internal static class QField
{
    internal const string QFieldSurfaceName = "PSurface";

    internal static readonly DependencyProperty QFieldHintProperty = DependencyProperty.RegisterAttached(
        "QFieldHint",
        typeof(string),
        typeof(QField),
        new FrameworkPropertyMetadata(string.Empty));

    internal static readonly Thickness QFieldPlaceholderInset = new(4, 0, 0, 0);

    internal const double QFieldPlaceholderOpacity = 0.56;

    private const double QFieldPopupShade = 10;
    private const double QFieldPopupGap = 6;

    internal static CustomPopupPlacement[] QFieldPopupPlace(Size popup, Size target, Point offset)
    {
        var pBelow = new Point(-QFieldPopupShade, target.Height + QFieldPopupGap - QFieldPopupShade);
        var pAbove = new Point(-QFieldPopupShade, QFieldPopupShade - QFieldPopupGap - popup.Height);
        return
        [
            new CustomPopupPlacement(pBelow, PopupPrimaryAxis.Vertical),
            new CustomPopupPlacement(pAbove, PopupPrimaryAxis.Vertical),
        ];
    }

    internal static FrameworkElement? QFieldSurfaceFind(object? source)
    {
        if (source is not TextBox box)
        {
            return null;
        }

        box.ApplyTemplate();
        return box.Template?.FindName(QFieldSurfaceName, box) as FrameworkElement ?? box;
    }

    internal static void QFieldFocusDefer(ItemsControl host, object? item)
    {
        if (item is null)
        {
            return;
        }

        host.Dispatcher.BeginInvoke(
            DispatcherPriority.Loaded,
            () =>
            {
                if (host.ItemContainerGenerator.ContainerFromItem(item) is DependencyObject container
                    && QFieldCaretFind(container) is TextBox box)
                {
                    box.Focus();
                    box.CaretIndex = box.Text.Length;
                }
            });
    }

    internal static void QFieldGhostAttach(TextBlock ghost, TextBox field)
    {
        ArgumentNullException.ThrowIfNull(ghost);
        ArgumentNullException.ThrowIfNull(field);

        MultiBinding echo = new() { Converter = new QFieldGhost() };
        echo.Bindings.Add(new Binding(nameof(TextBox.Text)) { Source = field });
        echo.Bindings.Add(new Binding { Source = field, Path = new PropertyPath(QFieldHintProperty) });
        ghost.SetBinding(TextBlock.TextProperty, echo);
    }

    internal static void QFieldTextShow(TextBox box, string text)
    {
        box.Text = text;
    }

    internal static void QFieldPlaceholderShow(TextBlock block, bool placeholder)
    {
        block.Padding = placeholder ? QFieldPlaceholderInset : new Thickness(0);
        block.Opacity = placeholder ? QFieldPlaceholderOpacity : 1;
        block.SetResourceReference(TextBlock.ForegroundProperty, placeholder ? "Theme.Muted" : "Theme.Ink");
    }

    internal static void QFieldCaretApply(TextBox box, object row, int caret)
    {
        ItemsControl? host = ItemsControl.ItemsControlFromItemContainer(box) ?? QFieldHostFind(box);
        box.Dispatcher.BeginInvoke(
            DispatcherPriority.Input,
            () =>
            {
                TextBox? entry = host is null ? box : QFieldCaretFind(host) ?? box;
                if (entry.DataContext != row)
                {
                    return;
                }

                entry.Focus();
                entry.CaretIndex = caret;
            });
    }

    internal static ItemsControl? QFieldHostFind(DependencyObject start)
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

    internal static TextBox? QFieldCaretFind(DependencyObject root)
    {
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(root, index);
            if (child is TextBox box)
            {
                return box;
            }

            TextBox? found = QFieldCaretFind(child);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }
}
