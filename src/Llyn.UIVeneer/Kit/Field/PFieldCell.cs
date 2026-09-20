using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIVeneer;

internal static partial class PField
{
    private const char PFieldCellSeparator = ' ';

    internal static void PFieldCellAttach(UIElement host)
    {
        ArgumentNullException.ThrowIfNull(host);
        host.PreviewMouseLeftButtonDown += PFieldPressHandle;
    }

    private static void PFieldPressHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not UIElement host || e.OriginalSource is not DependencyObject origin)
        {
            return;
        }

        Grid? cell = PFieldCellFind(origin, host);
        if (cell is null || cell.Tag is not string order)
        {
            return;
        }

        foreach (UIElement child in cell.Children)
        {
            if (child is TextBox)
            {
                return;
            }
        }

        TextBlock? block = null;
        foreach (UIElement child in cell.Children)
        {
            if (child is TextBlock found)
            {
                block = found;
                break;
            }
        }

        string[] parts = order.Split(PFieldCellSeparator, StringSplitOptions.RemoveEmptyEntries);
        if (block is null || parts.Length < 2 || cell.TryFindResource(parts[0]) is not Style style)
        {
            return;
        }

        if (cell.DataContext is not object row)
        {
            return;
        }

        TextBox box = PFieldCellBuild(style, row, parts[1], parts.Length > 2 ? parts[2] : null);
        block.Visibility = Visibility.Hidden;
        cell.Children.Add(box);
        cell.UpdateLayout();
        box.CaretIndex = box.GetCharacterIndexFromPoint(e.GetPosition(box), true) is int index && index >= 0
            ? index + (PFieldTrailCheck(box, e.GetPosition(box), index) ? 1 : 0)
            : box.Text.Length;
        e.Handled = true;
        if (!box.Focus())
        {
            PFieldCellDetach(box, cell);
        }
    }

    private static bool PFieldTrailCheck(TextBox box, Point point, int index)
    {
        Rect glyph = box.GetRectFromCharacterIndex(index);
        return point.X > glyph.X + glyph.Width / 2;
    }

    private static TextBox PFieldCellBuild(Style style, object row, string path, string? hint)
    {
        TextBox box = new() { Style = style };
        box.SetBinding(TextBox.TextProperty, new Binding(path)
        {
            Source = row,
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });
        if (hint is not null)
        {
            box.SetResourceReference(FrameworkElement.TagProperty, hint);
        }

        box.LostKeyboardFocus += PFieldBlurHandle;
        return box;
    }

    private static void PFieldBlurHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is TextBox box && box.Parent is Grid cell)
        {
            PFieldCellDetach(box, cell);
        }
    }

    private static void PFieldCellDetach(TextBox box, Grid cell)
    {
        box.LostKeyboardFocus -= PFieldBlurHandle;
        cell.Children.Remove(box);
        BindingOperations.ClearBinding(box, TextBox.TextProperty);
        foreach (UIElement child in cell.Children)
        {
            if (child is TextBlock block)
            {
                block.ClearValue(UIElement.VisibilityProperty);
            }
        }
    }

    private static Grid? PFieldCellFind(DependencyObject origin, UIElement host)
    {
        DependencyObject? node = origin;
        while (node is not null && !ReferenceEquals(node, host))
        {
            if (node is Grid { Tag: string } cell)
            {
                return cell;
            }

            node = node is Visual or System.Windows.Media.Media3D.Visual3D
                ? VisualTreeHelper.GetParent(node)
                : LogicalTreeHelper.GetParent(node);
        }

        return null;
    }
}
