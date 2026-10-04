using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal static class QQuill
{
    internal static readonly DependencyProperty QQuillCellProperty = DependencyProperty.RegisterAttached(
        "QQuillCell",
        typeof(QQuillCell),
        typeof(QQuill));

    internal sealed record QQuillCell(string QQuillCellStyle, string QQuillCellPath, string? QQuillCellHint);

    internal static void QQuillIntroduce(UIElement host)
    {
        ArgumentNullException.ThrowIfNull(host);
        host.PreviewMouseLeftButtonDown += QQuillPressRefine;
    }

    private static void QQuillPressRefine(object sender, MouseButtonEventArgs e)
    {
        if (sender is not UIElement host || e.OriginalSource is not DependencyObject origin)
        {
            return;
        }

        Grid? cell = QQuillCellFind(origin, host);
        if (cell?.GetValue(QQuillCellProperty) is not QQuillCell order)
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

        if (block is null || cell.TryFindResource(order.QQuillCellStyle) is not Style style)
        {
            return;
        }

        if (cell.DataContext is not object row)
        {
            return;
        }

        TextBox box = QQuillOpenIntroduce(style, row, order.QQuillCellPath, order.QQuillCellHint);
        box.ToolTip = block.ToolTip;
        block.Visibility = Visibility.Hidden;
        cell.Children.Add(box);
        cell.UpdateLayout();
        box.CaretIndex = box.GetCharacterIndexFromPoint(e.GetPosition(box), true) is int index && index >= 0
            ? index + (QQuillTrailDraw(box, e.GetPosition(box), index) ? 1 : 0)
            : box.Text.Length;
        e.Handled = true;
        if (!box.Focus())
        {
            QQuillCloseIntroduce(box, cell);
            QQuillBlockRefine(cell);
        }
    }

    private static bool QQuillTrailDraw(TextBox box, Point point, int index)
    {
        Rect glyph = box.GetRectFromCharacterIndex(index);
        return point.X > glyph.X + glyph.Width / 2;
    }

    private static TextBox QQuillOpenIntroduce(Style style, object row, string path, string? hint)
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
            box.SetResourceReference(QField.QFieldHintProperty, hint);
        }

        box.LostKeyboardFocus += QQuillBlurRefine;
        return box;
    }

    private static void QQuillBlurRefine(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is TextBox box && box.Parent is Grid cell)
        {
            QQuillCloseIntroduce(box, cell);
            QQuillBlockRefine(cell);
        }
    }

    private static void QQuillCloseIntroduce(TextBox box, Grid cell)
    {
        box.LostKeyboardFocus -= QQuillBlurRefine;
        cell.Children.Remove(box);
        BindingOperations.ClearBinding(box, TextBox.TextProperty);
    }

    private static void QQuillBlockRefine(Grid cell)
    {
        foreach (UIElement child in cell.Children)
        {
            if (child is TextBlock block)
            {
                block.ClearValue(UIElement.VisibilityProperty);
            }
        }
    }

    private static Grid? QQuillCellFind(DependencyObject origin, UIElement host)
    {
        DependencyObject? node = origin;
        while (node is not null && !ReferenceEquals(node, host))
        {
            if (node is Grid cell && cell.GetValue(QQuillCellProperty) is QQuillCell)
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
