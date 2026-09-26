using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace Llyn.UIDeportment;

internal static partial class PField
{
    internal const string PFieldSurfaceName = "PSurface";

    private const string PFieldPlaceholderName = "PPlaceholder";

    private static readonly Thickness PFieldPlaceholderInset = new(4, 0, 0, 0);

    private const double PFieldPlaceholderOpacity = 0.56;

    private const string PFieldTemplateKey = "Theme.Input.Field.Template";
    private const string PFieldBareKey = "Theme.Input.Field.Bare";
    private const string PFieldPlainKey = "Theme.Input.Field.Plain";

    internal static void PFieldApply(ResourceDictionary resources)
    {
        resources[PFieldTemplateKey] = PFieldTemplateBuild();
        resources[PFieldBareKey] = PFieldBareBuild();
        resources[PFieldPlainKey] = PFieldPlainBuild();
    }

    private const double PFieldPopupShade = 10;
    private const double PFieldPopupGap = 6;

    internal static CustomPopupPlacement[] PFieldPopupPlace(Size popup, Size target, Point offset)
    {
        var pBelow = new Point(-PFieldPopupShade, target.Height + PFieldPopupGap - PFieldPopupShade);
        var pAbove = new Point(-PFieldPopupShade, PFieldPopupShade - PFieldPopupGap - popup.Height);
        return
        [
            new CustomPopupPlacement(pBelow, PopupPrimaryAxis.Vertical),
            new CustomPopupPlacement(pAbove, PopupPrimaryAxis.Vertical),
        ];
    }

    internal static FrameworkElement? PFieldSurfaceFind(object? source)
    {
        if (source is not TextBox box)
        {
            return null;
        }

        box.ApplyTemplate();
        return box.Template?.FindName(PFieldSurfaceName, box) as FrameworkElement ?? box;
    }

    internal static void PFieldFocusDefer(ItemsControl host, object? item)
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

    internal static void PFieldGhostShow(TextBlock ghost, TextBox field)
    {
        ArgumentNullException.ThrowIfNull(ghost);
        ArgumentNullException.ThrowIfNull(field);

        ghost.Text = field.Text.Length > 0 ? field.Text : field.Tag as string ?? string.Empty;
    }

    internal static void PFieldTextShow(TextBox box, string text)
    {
        if (string.Equals(box.Text, text, StringComparison.Ordinal))
        {
            return;
        }

        box.Text = text;
    }

    internal static void PFieldNoteShow(TextBox box, string note)
    {
        if (string.Equals(PFieldNoteRead(box), note, StringComparison.Ordinal))
        {
            return;
        }

        box.Text = note;
    }

    internal static string PFieldNoteRead(TextBox box)
    {
        return box.Text.TrimEnd('\r', '\n');
    }

    internal static string PFieldPathRead(TextBox box)
    {
        ArgumentNullException.ThrowIfNull(box);

        return box.TemplatedParent is ComboBox choice ? choice.Name : box.Name;
    }

    internal static void PFieldPlaceholderShow(TextBlock block, bool placeholder)
    {
        block.Padding = placeholder ? PFieldPlaceholderInset : new Thickness(0);
        block.Opacity = placeholder ? PFieldPlaceholderOpacity : 1;
        block.SetResourceReference(TextBlock.ForegroundProperty, placeholder ? "Theme.Muted" : "Theme.Ink");
    }

    private static MultiBinding PFieldInsetBuild()
    {
        var pInset = new MultiBinding { Converter = new PFieldConverter() };
        pInset.Bindings.Add(
            new Binding(nameof(Control.FontFamily)) { RelativeSource = RelativeSource.TemplatedParent });
        pInset.Bindings.Add(new Binding(nameof(Control.FontSize)) { RelativeSource = RelativeSource.TemplatedParent });
        pInset.Bindings.Add(new Binding(nameof(Control.Padding)) { RelativeSource = RelativeSource.TemplatedParent });
        return pInset;
    }

    private static ControlTemplate PFieldBareBuild()
    {
        var pTemplate = new ControlTemplate(typeof(TextBox));

        var pRoot = new FrameworkElementFactory(typeof(Grid));

        var pSurface = new FrameworkElementFactory(typeof(Border), PFieldSurfaceName);
        pSurface.SetBinding(FrameworkElement.MarginProperty, PFieldInsetBuild());
        pSurface.SetValue(Border.BackgroundProperty, Brushes.Transparent);
        pSurface.SetValue(Border.BorderBrushProperty, Brushes.Transparent);
        pSurface.SetValue(Border.BorderThicknessProperty, new Thickness(1));
        pSurface.SetValue(Border.CornerRadiusProperty, new CornerRadius(7));
        pSurface.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pSurface.SetValue(UIElement.SnapsToDevicePixelsProperty, true);

        var pPlaceholder = new FrameworkElementFactory(typeof(TextBlock), PFieldPlaceholderName);
        pPlaceholder.SetValue(FrameworkElement.MarginProperty, new TemplateBindingExtension(Control.PaddingProperty));
        pPlaceholder.SetValue(TextBlock.PaddingProperty, PFieldPlaceholderInset);
        pPlaceholder.SetValue(
            FrameworkElement.VerticalAlignmentProperty,
            new TemplateBindingExtension(Control.VerticalContentAlignmentProperty));
        pPlaceholder.SetValue(TextBlock.ForegroundProperty, new DynamicResourceExtension("Theme.Muted"));
        pPlaceholder.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pPlaceholder.SetValue(UIElement.OpacityProperty, PFieldPlaceholderOpacity);
        pPlaceholder.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(FrameworkElement.TagProperty));
        pPlaceholder.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        pPlaceholder.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);

        var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");
        pContent.SetValue(FrameworkElement.MarginProperty, new Thickness(-2, 0, 0, 0));

        pRoot.AppendChild(pSurface);
        pRoot.AppendChild(pPlaceholder);
        pRoot.AppendChild(pContent);
        pTemplate.VisualTree = pRoot;

        var pEmptyTrigger = new Trigger { Property = TextBox.TextProperty, Value = string.Empty };
        pEmptyTrigger.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Visible, PFieldPlaceholderName));
        pTemplate.Triggers.Add(pEmptyTrigger);

        var pHoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
        pHoverTrigger.Setters.Add(
            new Setter(Border.BorderBrushProperty, new DynamicResourceExtension("Theme.Line"), PFieldSurfaceName));
        pTemplate.Triggers.Add(pHoverTrigger);

        var pFocusTrigger = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
        pFocusTrigger.Setters.Add(
            new Setter(Border.BorderBrushProperty, new DynamicResourceExtension("Theme.Accent"), PFieldSurfaceName));
        pFocusTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.36, PFieldPlaceholderName));
        pTemplate.Triggers.Add(pFocusTrigger);

        pTemplate.Seal();
        return pTemplate;
    }

    private static ControlTemplate PFieldPlainBuild()
    {
        var pTemplate = new ControlTemplate(typeof(TextBox));

        var pRoot = new FrameworkElementFactory(typeof(Grid));

        var pPlaceholder = new FrameworkElementFactory(typeof(TextBlock), PFieldPlaceholderName);
        pPlaceholder.SetValue(FrameworkElement.MarginProperty, new TemplateBindingExtension(Control.PaddingProperty));
        pPlaceholder.SetValue(TextBlock.PaddingProperty, PFieldPlaceholderInset);
        pPlaceholder.SetValue(
            FrameworkElement.VerticalAlignmentProperty,
            new TemplateBindingExtension(Control.VerticalContentAlignmentProperty));
        pPlaceholder.SetValue(TextBlock.ForegroundProperty, new DynamicResourceExtension("Theme.Muted"));
        pPlaceholder.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pPlaceholder.SetValue(UIElement.OpacityProperty, PFieldPlaceholderOpacity);
        pPlaceholder.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(FrameworkElement.TagProperty));
        pPlaceholder.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        pPlaceholder.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);

        var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");
        pContent.SetValue(FrameworkElement.MarginProperty, new Thickness(-2, 0, 0, 0));

        pRoot.AppendChild(pPlaceholder);
        pRoot.AppendChild(pContent);
        pTemplate.VisualTree = pRoot;

        var pEmptyTrigger = new Trigger { Property = TextBox.TextProperty, Value = string.Empty };
        pEmptyTrigger.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Visible, PFieldPlaceholderName));
        pTemplate.Triggers.Add(pEmptyTrigger);

        var pFocusTrigger = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
        pFocusTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.36, PFieldPlaceholderName));
        pTemplate.Triggers.Add(pFocusTrigger);

        pTemplate.Seal();
        return pTemplate;
    }

    private static ControlTemplate PFieldTemplateBuild()
    {
        var pTemplate = new ControlTemplate(typeof(TextBox));

        var pSurface = new FrameworkElementFactory(typeof(Border), PFieldSurfaceName);
        pSurface.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Control.BackgroundProperty));
        pSurface.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Control.BorderBrushProperty));
        pSurface.SetValue(
            Border.BorderThicknessProperty, new TemplateBindingExtension(Control.BorderThicknessProperty));
        pSurface.SetValue(Border.CornerRadiusProperty, new CornerRadius(7));

        var pGrid = new FrameworkElementFactory(typeof(Grid));

        var pPlaceholder = new FrameworkElementFactory(typeof(TextBlock), PFieldPlaceholderName);
        pPlaceholder.SetValue(FrameworkElement.MarginProperty, new TemplateBindingExtension(Control.PaddingProperty));
        pPlaceholder.SetValue(TextBlock.PaddingProperty, PFieldPlaceholderInset);
        pPlaceholder.SetValue(
            FrameworkElement.VerticalAlignmentProperty,
            new TemplateBindingExtension(Control.VerticalContentAlignmentProperty));
        pPlaceholder.SetValue(TextBlock.ForegroundProperty, new DynamicResourceExtension("Theme.Muted"));
        pPlaceholder.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pPlaceholder.SetValue(UIElement.OpacityProperty, PFieldPlaceholderOpacity);
        pPlaceholder.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(FrameworkElement.TagProperty));
        pPlaceholder.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        pPlaceholder.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);

        var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");

        pGrid.AppendChild(pPlaceholder);
        pGrid.AppendChild(pContent);
        pSurface.AppendChild(pGrid);
        pTemplate.VisualTree = pSurface;

        var pEmptyTrigger = new Trigger { Property = TextBox.TextProperty, Value = string.Empty };
        pEmptyTrigger.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Visible, PFieldPlaceholderName));
        pTemplate.Triggers.Add(pEmptyTrigger);

        var pFocusTrigger = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
        pFocusTrigger.Setters.Add(
            new Setter(Border.BorderBrushProperty, new DynamicResourceExtension("Theme.Accent"), PFieldSurfaceName));
        pFocusTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.36, PFieldPlaceholderName));
        pTemplate.Triggers.Add(pFocusTrigger);

        var pHoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
        pHoverTrigger.Setters.Add(
            new Setter(Border.BorderBrushProperty, new DynamicResourceExtension("Theme.Accent"), PFieldSurfaceName));
        pTemplate.Triggers.Add(pHoverTrigger);

        pTemplate.Seal();
        return pTemplate;
    }
}
