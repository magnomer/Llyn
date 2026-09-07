using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Llyn.UIShell;

internal static class PField
{
    private const string PFieldTemplateKey = "Theme.Input.Field.Template";
    private const string PFieldBareKey = "Theme.Input.Field.Bare";

    internal static void PFieldApply(ResourceDictionary resources)
    {
        resources[PFieldTemplateKey] = PFieldTemplateBuild();
        resources[PFieldBareKey] = PFieldBareBuild();
    }

    private static ControlTemplate PFieldBareBuild()
    {
        var pTemplate = new ControlTemplate(typeof(TextBox));

        var pRoot = new FrameworkElementFactory(typeof(Grid));

        var pSurface = new FrameworkElementFactory(typeof(Border), "PSurface");
        pSurface.SetValue(FrameworkElement.MarginProperty, new Thickness(-9, -5, -9, -5));
        pSurface.SetValue(Border.BackgroundProperty, Brushes.Transparent);
        pSurface.SetValue(Border.BorderBrushProperty, Brushes.Transparent);
        pSurface.SetValue(Border.BorderThicknessProperty, new Thickness(1));
        pSurface.SetValue(Border.CornerRadiusProperty, new CornerRadius(7));
        pSurface.SetValue(UIElement.IsHitTestVisibleProperty, false);

        var pPlaceholder = new FrameworkElementFactory(typeof(TextBlock), "PPlaceholder");
        pPlaceholder.SetValue(FrameworkElement.VerticalAlignmentProperty, new TemplateBindingExtension(Control.VerticalContentAlignmentProperty));
        pPlaceholder.SetValue(TextBlock.ForegroundProperty, new DynamicResourceExtension("Theme.Muted"));
        pPlaceholder.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pPlaceholder.SetValue(UIElement.OpacityProperty, 0.56);
        pPlaceholder.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(FrameworkElement.TagProperty));
        pPlaceholder.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        pPlaceholder.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);

        var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");

        pRoot.AppendChild(pSurface);
        pRoot.AppendChild(pPlaceholder);
        pRoot.AppendChild(pContent);
        pTemplate.VisualTree = pRoot;

        var pEmptyTrigger = new Trigger { Property = TextBox.TextProperty, Value = string.Empty };
        pEmptyTrigger.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Visible, "PPlaceholder"));
        pTemplate.Triggers.Add(pEmptyTrigger);

        var pHoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
        pHoverTrigger.Setters.Add(new Setter(Border.BorderBrushProperty, new DynamicResourceExtension("Theme.Line"), "PSurface"));
        pTemplate.Triggers.Add(pHoverTrigger);

        var pFocusTrigger = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
        pFocusTrigger.Setters.Add(new Setter(Border.BorderBrushProperty, new DynamicResourceExtension("Theme.Accent"), "PSurface"));
        pFocusTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.36, "PPlaceholder"));
        pTemplate.Triggers.Add(pFocusTrigger);

        pTemplate.Seal();
        return pTemplate;
    }

    private static ControlTemplate PFieldTemplateBuild()
    {
        var pTemplate = new ControlTemplate(typeof(TextBox));

        var pSurface = new FrameworkElementFactory(typeof(Border), "PSurface");
        pSurface.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Control.BackgroundProperty));
        pSurface.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Control.BorderBrushProperty));
        pSurface.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Control.BorderThicknessProperty));
        pSurface.SetValue(Border.CornerRadiusProperty, new CornerRadius(7));

        var pGrid = new FrameworkElementFactory(typeof(Grid));

        var pPlaceholder = new FrameworkElementFactory(typeof(TextBlock), "PPlaceholder");
        pPlaceholder.SetValue(FrameworkElement.MarginProperty, new TemplateBindingExtension(Control.PaddingProperty));
        pPlaceholder.SetValue(TextBlock.PaddingProperty, new Thickness(4, 0, 0, 0));
        pPlaceholder.SetValue(FrameworkElement.VerticalAlignmentProperty, new TemplateBindingExtension(Control.VerticalContentAlignmentProperty));
        pPlaceholder.SetValue(TextBlock.ForegroundProperty, new DynamicResourceExtension("Theme.Muted"));
        pPlaceholder.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pPlaceholder.SetValue(UIElement.OpacityProperty, 0.56);
        pPlaceholder.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(FrameworkElement.TagProperty));
        pPlaceholder.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        pPlaceholder.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);

        var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");

        pGrid.AppendChild(pPlaceholder);
        pGrid.AppendChild(pContent);
        pSurface.AppendChild(pGrid);
        pTemplate.VisualTree = pSurface;

        var pEmptyTrigger = new Trigger { Property = TextBox.TextProperty, Value = string.Empty };
        pEmptyTrigger.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Visible, "PPlaceholder"));
        pTemplate.Triggers.Add(pEmptyTrigger);

        var pFocusTrigger = new Trigger { Property = UIElement.IsKeyboardFocusedProperty, Value = true };
        pFocusTrigger.Setters.Add(new Setter(Border.BorderBrushProperty, new DynamicResourceExtension("Theme.Accent"), "PSurface"));
        pFocusTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.36, "PPlaceholder"));
        pTemplate.Triggers.Add(pFocusTrigger);

        var pHoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
        pHoverTrigger.Setters.Add(new Setter(Border.BorderBrushProperty, new DynamicResourceExtension("Theme.Accent"), "PSurface"));
        pTemplate.Triggers.Add(pHoverTrigger);

        pTemplate.Seal();
        return pTemplate;
    }
}
