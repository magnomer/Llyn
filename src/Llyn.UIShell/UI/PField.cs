using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIShell;

// Builds the reusable text-input field control template in code. The content host must be named
// "PART_ContentHost" because WPF's TextBox looks that exact part up to mount its text editor; the
// name lives here as a string literal (never as XAML x:Name) so the framework contract is honored
// without the name entering the audited XAML naming surface. Registered by PFieldApply under the
// key the "Theme.Input.Field" style binds its Template setter to.
internal static class PField
{
    private const string PFieldTemplateKey = "Theme.Input.Field.Template";

    internal static void PFieldApply(ResourceDictionary resources)
    {
        resources[PFieldTemplateKey] = PFieldTemplateBuild();
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
        pPlaceholder.SetValue(FrameworkElement.VerticalAlignmentProperty, new TemplateBindingExtension(Control.VerticalContentAlignmentProperty));
        pPlaceholder.SetValue(TextBlock.ForegroundProperty, new DynamicResourceExtension("Theme.Muted"));
        pPlaceholder.SetValue(UIElement.IsHitTestVisibleProperty, false);
        pPlaceholder.SetValue(UIElement.OpacityProperty, 0.56);
        pPlaceholder.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(FrameworkElement.TagProperty));
        pPlaceholder.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        pPlaceholder.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);

        // No Margin binding here: WPF already offsets the editable text by TextBox.Padding
        // internally. Binding the host's Margin to Padding too would apply it twice, pushing the
        // caret right and down while the placeholder (padded once) stayed put.
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
