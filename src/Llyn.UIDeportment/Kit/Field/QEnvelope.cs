using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal static class QEnvelope
{
    private const string QEnvelopePlaceholderName = "PPlaceholder";

    private const string QEnvelopeFrameKey = "Theme.Input.Field.Template";
    private const string QEnvelopeBareKey = "Theme.Input.Field.Bare";
    private const string QEnvelopePlainKey = "Theme.Input.Field.Plain";

    internal static void QEnvelopeIntroduce(ResourceDictionary resources)
    {
        resources[QEnvelopeFrameKey] = QEnvelopeFrameIntroduce();
        resources[QEnvelopeBareKey] = QEnvelopeBareIntroduce();
        resources[QEnvelopePlainKey] = QEnvelopePlainIntroduce();
    }

    private static MultiBinding QEnvelopeInsetIntroduce()
    {
        var pInset = new MultiBinding { Converter = new QFieldConverter() };
        pInset.Bindings.Add(
            new Binding(nameof(Control.FontFamily)) { RelativeSource = RelativeSource.TemplatedParent });
        pInset.Bindings.Add(new Binding(nameof(Control.FontSize)) { RelativeSource = RelativeSource.TemplatedParent });
        pInset.Bindings.Add(new Binding(nameof(Control.Padding)) { RelativeSource = RelativeSource.TemplatedParent });
        return pInset;
    }

    private static ControlTemplate QEnvelopeBareIntroduce()
    {
        var pTemplate = new ControlTemplate(typeof(TextBox));

        var pRoot = new FrameworkElementFactory(typeof(Grid));

        var pSurface = new FrameworkElementFactory(typeof(Border), QField.QFieldSurfaceName);
        pSurface.SetBinding(FrameworkElement.MarginProperty, QEnvelopeInsetIntroduce());
        pSurface.SetValue(Border.BackgroundProperty, Brushes.Transparent);
        pSurface.SetValue(Border.BorderBrushProperty, Brushes.Transparent);
        pSurface.SetValue(Border.BorderThicknessProperty, new Thickness(1));
        pSurface.SetValue(Border.CornerRadiusProperty, new CornerRadius(7));
        pSurface.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pSurface.SetValue(UIElement.SnapsToDevicePixelsProperty, true);

        var pPlaceholder = new FrameworkElementFactory(typeof(TextBlock), QEnvelopePlaceholderName);
        pPlaceholder.SetValue(FrameworkElement.MarginProperty, new TemplateBindingExtension(Control.PaddingProperty));
        pPlaceholder.SetValue(TextBlock.PaddingProperty, QField.QFieldPlaceholderInset);
        pPlaceholder.SetValue(
            FrameworkElement.VerticalAlignmentProperty,
            new TemplateBindingExtension(Control.VerticalContentAlignmentProperty));
        pPlaceholder.SetValue(TextBlock.ForegroundProperty, new DynamicResourceExtension("Theme.Muted"));
        pPlaceholder.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pPlaceholder.SetValue(UIElement.OpacityProperty, QField.QFieldPlaceholderOpacity);
        pPlaceholder.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(QField.QFieldHintProperty));
        pPlaceholder.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        pPlaceholder.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);

        var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");
        pContent.SetValue(FrameworkElement.MarginProperty, new Thickness(-2, 0, 0, 0));

        pRoot.AppendChild(pSurface);
        pRoot.AppendChild(pPlaceholder);
        pRoot.AppendChild(pContent);
        pTemplate.VisualTree = pRoot;

        var pEmptyTrigger = new Trigger { Property = TextBox.TextProperty, Value = string.Empty };
        pEmptyTrigger.Setters.Add(
            new Setter(UIElement.VisibilityProperty, Visibility.Visible, QEnvelopePlaceholderName));
        pTemplate.Triggers.Add(pEmptyTrigger);

        var pHoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
        pHoverTrigger.Setters.Add(
            new Setter(
                Border.BorderBrushProperty,
                new DynamicResourceExtension("Theme.Line"),
                QField.QFieldSurfaceName));
        pTemplate.Triggers.Add(pHoverTrigger);

        var pFocusTrigger = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
        pFocusTrigger.Setters.Add(
            new Setter(
                Border.BorderBrushProperty,
                new DynamicResourceExtension("Theme.Accent"),
                QField.QFieldSurfaceName));
        pFocusTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.36, QEnvelopePlaceholderName));
        pTemplate.Triggers.Add(pFocusTrigger);

        pTemplate.Seal();
        return pTemplate;
    }

    private static ControlTemplate QEnvelopePlainIntroduce()
    {
        var pTemplate = new ControlTemplate(typeof(TextBox));

        var pRoot = new FrameworkElementFactory(typeof(Grid));

        var pPlaceholder = new FrameworkElementFactory(typeof(TextBlock), QEnvelopePlaceholderName);
        pPlaceholder.SetValue(FrameworkElement.MarginProperty, new TemplateBindingExtension(Control.PaddingProperty));
        pPlaceholder.SetValue(TextBlock.PaddingProperty, QField.QFieldPlaceholderInset);
        pPlaceholder.SetValue(
            FrameworkElement.VerticalAlignmentProperty,
            new TemplateBindingExtension(Control.VerticalContentAlignmentProperty));
        pPlaceholder.SetValue(TextBlock.ForegroundProperty, new DynamicResourceExtension("Theme.Muted"));
        pPlaceholder.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pPlaceholder.SetValue(UIElement.OpacityProperty, QField.QFieldPlaceholderOpacity);
        pPlaceholder.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(QField.QFieldHintProperty));
        pPlaceholder.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        pPlaceholder.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);

        var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");
        pContent.SetValue(FrameworkElement.MarginProperty, new Thickness(-2, 0, 0, 0));

        pRoot.AppendChild(pPlaceholder);
        pRoot.AppendChild(pContent);
        pTemplate.VisualTree = pRoot;

        var pEmptyTrigger = new Trigger { Property = TextBox.TextProperty, Value = string.Empty };
        pEmptyTrigger.Setters.Add(
            new Setter(UIElement.VisibilityProperty, Visibility.Visible, QEnvelopePlaceholderName));
        pTemplate.Triggers.Add(pEmptyTrigger);

        var pFocusTrigger = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
        pFocusTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.36, QEnvelopePlaceholderName));
        pTemplate.Triggers.Add(pFocusTrigger);

        pTemplate.Seal();
        return pTemplate;
    }

    private static ControlTemplate QEnvelopeFrameIntroduce()
    {
        var pTemplate = new ControlTemplate(typeof(TextBox));

        var pSurface = new FrameworkElementFactory(typeof(Border), QField.QFieldSurfaceName);
        pSurface.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Control.BackgroundProperty));
        pSurface.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Control.BorderBrushProperty));
        pSurface.SetValue(
            Border.BorderThicknessProperty, new TemplateBindingExtension(Control.BorderThicknessProperty));
        pSurface.SetValue(Border.CornerRadiusProperty, new CornerRadius(7));

        var pGrid = new FrameworkElementFactory(typeof(Grid));

        var pPlaceholder = new FrameworkElementFactory(typeof(TextBlock), QEnvelopePlaceholderName);
        pPlaceholder.SetValue(FrameworkElement.MarginProperty, new TemplateBindingExtension(Control.PaddingProperty));
        pPlaceholder.SetValue(TextBlock.PaddingProperty, QField.QFieldPlaceholderInset);
        pPlaceholder.SetValue(
            FrameworkElement.VerticalAlignmentProperty,
            new TemplateBindingExtension(Control.VerticalContentAlignmentProperty));
        pPlaceholder.SetValue(TextBlock.ForegroundProperty, new DynamicResourceExtension("Theme.Muted"));
        pPlaceholder.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pPlaceholder.SetValue(UIElement.OpacityProperty, QField.QFieldPlaceholderOpacity);
        pPlaceholder.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(QField.QFieldHintProperty));
        pPlaceholder.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        pPlaceholder.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);

        var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");

        pGrid.AppendChild(pPlaceholder);
        pGrid.AppendChild(pContent);
        pSurface.AppendChild(pGrid);
        pTemplate.VisualTree = pSurface;

        var pEmptyTrigger = new Trigger { Property = TextBox.TextProperty, Value = string.Empty };
        pEmptyTrigger.Setters.Add(
            new Setter(UIElement.VisibilityProperty, Visibility.Visible, QEnvelopePlaceholderName));
        pTemplate.Triggers.Add(pEmptyTrigger);

        var pFocusTrigger = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
        pFocusTrigger.Setters.Add(
            new Setter(
                Border.BorderBrushProperty,
                new DynamicResourceExtension("Theme.Accent"),
                QField.QFieldSurfaceName));
        pFocusTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.36, QEnvelopePlaceholderName));
        pTemplate.Triggers.Add(pFocusTrigger);

        var pHoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
        pHoverTrigger.Setters.Add(
            new Setter(
                Border.BorderBrushProperty,
                new DynamicResourceExtension("Theme.Accent"),
                QField.QFieldSurfaceName));
        pTemplate.Triggers.Add(pHoverTrigger);

        pTemplate.Seal();
        return pTemplate;
    }
}
