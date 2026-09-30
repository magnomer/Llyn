using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
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

    private const string QFieldPlaceholderName = "PPlaceholder";

    private static readonly Thickness QFieldPlaceholderInset = new(4, 0, 0, 0);

    private const double QFieldPlaceholderOpacity = 0.56;

    private const string QFieldTemplateKey = "Theme.Input.Field.Template";
    private const string QFieldBareKey = "Theme.Input.Field.Bare";
    private const string QFieldPlainKey = "Theme.Input.Field.Plain";

    internal static void QFieldApply(ResourceDictionary resources)
    {
        resources[QFieldTemplateKey] = QFieldTemplateBuild();
        resources[QFieldBareKey] = QFieldBareBuild();
        resources[QFieldPlainKey] = QFieldPlainBuild();
    }

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
                    && PEditor.PEditorCaretFind(container) is TextBox box)
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

    internal static string QFieldPathRead(TextBox box)
    {
        ArgumentNullException.ThrowIfNull(box);

        return box.TemplatedParent is ComboBox choice ? choice.Name : box.Name;
    }

    internal static void QFieldPlaceholderShow(TextBlock block, bool placeholder)
    {
        block.Padding = placeholder ? QFieldPlaceholderInset : new Thickness(0);
        block.Opacity = placeholder ? QFieldPlaceholderOpacity : 1;
        block.SetResourceReference(TextBlock.ForegroundProperty, placeholder ? "Theme.Muted" : "Theme.Ink");
    }

    private static MultiBinding QFieldInsetBuild()
    {
        var pInset = new MultiBinding { Converter = new QFieldConverter() };
        pInset.Bindings.Add(
            new Binding(nameof(Control.FontFamily)) { RelativeSource = RelativeSource.TemplatedParent });
        pInset.Bindings.Add(new Binding(nameof(Control.FontSize)) { RelativeSource = RelativeSource.TemplatedParent });
        pInset.Bindings.Add(new Binding(nameof(Control.Padding)) { RelativeSource = RelativeSource.TemplatedParent });
        return pInset;
    }

    private static ControlTemplate QFieldBareBuild()
    {
        var pTemplate = new ControlTemplate(typeof(TextBox));

        var pRoot = new FrameworkElementFactory(typeof(Grid));

        var pSurface = new FrameworkElementFactory(typeof(Border), QFieldSurfaceName);
        pSurface.SetBinding(FrameworkElement.MarginProperty, QFieldInsetBuild());
        pSurface.SetValue(Border.BackgroundProperty, Brushes.Transparent);
        pSurface.SetValue(Border.BorderBrushProperty, Brushes.Transparent);
        pSurface.SetValue(Border.BorderThicknessProperty, new Thickness(1));
        pSurface.SetValue(Border.CornerRadiusProperty, new CornerRadius(7));
        pSurface.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pSurface.SetValue(UIElement.SnapsToDevicePixelsProperty, true);

        var pPlaceholder = new FrameworkElementFactory(typeof(TextBlock), QFieldPlaceholderName);
        pPlaceholder.SetValue(FrameworkElement.MarginProperty, new TemplateBindingExtension(Control.PaddingProperty));
        pPlaceholder.SetValue(TextBlock.PaddingProperty, QFieldPlaceholderInset);
        pPlaceholder.SetValue(
            FrameworkElement.VerticalAlignmentProperty,
            new TemplateBindingExtension(Control.VerticalContentAlignmentProperty));
        pPlaceholder.SetValue(TextBlock.ForegroundProperty, new DynamicResourceExtension("Theme.Muted"));
        pPlaceholder.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pPlaceholder.SetValue(UIElement.OpacityProperty, QFieldPlaceholderOpacity);
        pPlaceholder.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(QFieldHintProperty));
        pPlaceholder.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        pPlaceholder.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);

        var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");
        pContent.SetValue(FrameworkElement.MarginProperty, new Thickness(-2, 0, 0, 0));

        pRoot.AppendChild(pSurface);
        pRoot.AppendChild(pPlaceholder);
        pRoot.AppendChild(pContent);
        pTemplate.VisualTree = pRoot;

        var pEmptyTrigger = new Trigger { Property = TextBox.TextProperty, Value = string.Empty };
        pEmptyTrigger.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Visible, QFieldPlaceholderName));
        pTemplate.Triggers.Add(pEmptyTrigger);

        var pHoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
        pHoverTrigger.Setters.Add(
            new Setter(Border.BorderBrushProperty, new DynamicResourceExtension("Theme.Line"), QFieldSurfaceName));
        pTemplate.Triggers.Add(pHoverTrigger);

        var pFocusTrigger = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
        pFocusTrigger.Setters.Add(
            new Setter(Border.BorderBrushProperty, new DynamicResourceExtension("Theme.Accent"), QFieldSurfaceName));
        pFocusTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.36, QFieldPlaceholderName));
        pTemplate.Triggers.Add(pFocusTrigger);

        pTemplate.Seal();
        return pTemplate;
    }

    private static ControlTemplate QFieldPlainBuild()
    {
        var pTemplate = new ControlTemplate(typeof(TextBox));

        var pRoot = new FrameworkElementFactory(typeof(Grid));

        var pPlaceholder = new FrameworkElementFactory(typeof(TextBlock), QFieldPlaceholderName);
        pPlaceholder.SetValue(FrameworkElement.MarginProperty, new TemplateBindingExtension(Control.PaddingProperty));
        pPlaceholder.SetValue(TextBlock.PaddingProperty, QFieldPlaceholderInset);
        pPlaceholder.SetValue(
            FrameworkElement.VerticalAlignmentProperty,
            new TemplateBindingExtension(Control.VerticalContentAlignmentProperty));
        pPlaceholder.SetValue(TextBlock.ForegroundProperty, new DynamicResourceExtension("Theme.Muted"));
        pPlaceholder.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pPlaceholder.SetValue(UIElement.OpacityProperty, QFieldPlaceholderOpacity);
        pPlaceholder.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(QFieldHintProperty));
        pPlaceholder.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        pPlaceholder.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);

        var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");
        pContent.SetValue(FrameworkElement.MarginProperty, new Thickness(-2, 0, 0, 0));

        pRoot.AppendChild(pPlaceholder);
        pRoot.AppendChild(pContent);
        pTemplate.VisualTree = pRoot;

        var pEmptyTrigger = new Trigger { Property = TextBox.TextProperty, Value = string.Empty };
        pEmptyTrigger.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Visible, QFieldPlaceholderName));
        pTemplate.Triggers.Add(pEmptyTrigger);

        var pFocusTrigger = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
        pFocusTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.36, QFieldPlaceholderName));
        pTemplate.Triggers.Add(pFocusTrigger);

        pTemplate.Seal();
        return pTemplate;
    }

    private static ControlTemplate QFieldTemplateBuild()
    {
        var pTemplate = new ControlTemplate(typeof(TextBox));

        var pSurface = new FrameworkElementFactory(typeof(Border), QFieldSurfaceName);
        pSurface.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Control.BackgroundProperty));
        pSurface.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Control.BorderBrushProperty));
        pSurface.SetValue(
            Border.BorderThicknessProperty, new TemplateBindingExtension(Control.BorderThicknessProperty));
        pSurface.SetValue(Border.CornerRadiusProperty, new CornerRadius(7));

        var pGrid = new FrameworkElementFactory(typeof(Grid));

        var pPlaceholder = new FrameworkElementFactory(typeof(TextBlock), QFieldPlaceholderName);
        pPlaceholder.SetValue(FrameworkElement.MarginProperty, new TemplateBindingExtension(Control.PaddingProperty));
        pPlaceholder.SetValue(TextBlock.PaddingProperty, QFieldPlaceholderInset);
        pPlaceholder.SetValue(
            FrameworkElement.VerticalAlignmentProperty,
            new TemplateBindingExtension(Control.VerticalContentAlignmentProperty));
        pPlaceholder.SetValue(TextBlock.ForegroundProperty, new DynamicResourceExtension("Theme.Muted"));
        pPlaceholder.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pPlaceholder.SetValue(UIElement.OpacityProperty, QFieldPlaceholderOpacity);
        pPlaceholder.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(QFieldHintProperty));
        pPlaceholder.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        pPlaceholder.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);

        var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");

        pGrid.AppendChild(pPlaceholder);
        pGrid.AppendChild(pContent);
        pSurface.AppendChild(pGrid);
        pTemplate.VisualTree = pSurface;

        var pEmptyTrigger = new Trigger { Property = TextBox.TextProperty, Value = string.Empty };
        pEmptyTrigger.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Visible, QFieldPlaceholderName));
        pTemplate.Triggers.Add(pEmptyTrigger);

        var pFocusTrigger = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
        pFocusTrigger.Setters.Add(
            new Setter(Border.BorderBrushProperty, new DynamicResourceExtension("Theme.Accent"), QFieldSurfaceName));
        pFocusTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.36, QFieldPlaceholderName));
        pTemplate.Triggers.Add(pFocusTrigger);

        var pHoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
        pHoverTrigger.Setters.Add(
            new Setter(Border.BorderBrushProperty, new DynamicResourceExtension("Theme.Accent"), QFieldSurfaceName));
        pTemplate.Triggers.Add(pHoverTrigger);

        pTemplate.Seal();
        return pTemplate;
    }

    internal static readonly DependencyProperty QFieldCellProperty = DependencyProperty.RegisterAttached(
        "QFieldCell",
        typeof(QFieldCell),
        typeof(QField));

    internal sealed record QFieldCell(string QFieldCellStyle, string QFieldCellPath, string? QFieldCellHint);

    internal static void QFieldCellAttach(UIElement host)
    {
        ArgumentNullException.ThrowIfNull(host);
        host.PreviewMouseLeftButtonDown += QFieldPressRefine;
    }

    private static void QFieldPressRefine(object sender, MouseButtonEventArgs e)
    {
        if (sender is not UIElement host || e.OriginalSource is not DependencyObject origin)
        {
            return;
        }

        Grid? cell = QFieldCellFind(origin, host);
        if (cell?.GetValue(QFieldCellProperty) is not QFieldCell order)
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

        if (block is null || cell.TryFindResource(order.QFieldCellStyle) is not Style style)
        {
            return;
        }

        if (cell.DataContext is not object row)
        {
            return;
        }

        TextBox box = QFieldCellBuild(style, row, order.QFieldCellPath, order.QFieldCellHint);
        box.ToolTip = block.ToolTip;
        block.Visibility = Visibility.Hidden;
        cell.Children.Add(box);
        cell.UpdateLayout();
        box.CaretIndex = box.GetCharacterIndexFromPoint(e.GetPosition(box), true) is int index && index >= 0
            ? index + (QFieldTrailCheck(box, e.GetPosition(box), index) ? 1 : 0)
            : box.Text.Length;
        e.Handled = true;
        if (!box.Focus())
        {
            QFieldCellDetach(box, cell);
        }
    }

    private static bool QFieldTrailCheck(TextBox box, Point point, int index)
    {
        Rect glyph = box.GetRectFromCharacterIndex(index);
        return point.X > glyph.X + glyph.Width / 2;
    }

    private static TextBox QFieldCellBuild(Style style, object row, string path, string? hint)
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
            box.SetResourceReference(QFieldHintProperty, hint);
        }

        box.LostKeyboardFocus += QFieldBlurRefine;
        return box;
    }

    private static void QFieldBlurRefine(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is TextBox box && box.Parent is Grid cell)
        {
            QFieldCellDetach(box, cell);
        }
    }

    private static void QFieldCellDetach(TextBox box, Grid cell)
    {
        box.LostKeyboardFocus -= QFieldBlurRefine;
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

    private static Grid? QFieldCellFind(DependencyObject origin, UIElement host)
    {
        DependencyObject? node = origin;
        while (node is not null && !ReferenceEquals(node, host))
        {
            if (node is Grid cell && cell.GetValue(QFieldCellProperty) is QFieldCell)
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
