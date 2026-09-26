using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

internal static class PLookDisplay
{
    internal static readonly IReadOnlyList<PLook.PLookSetter> PLookDisplayState =
    [
        new("Display.Card.SituationLink", "Hover", "PSurface", Border.BorderBrushProperty, "Theme.Situation"),
        new("Display.Card.SituationLink", "Press", "PSurface", UIElement.OpacityProperty, 0.72),
        new("Display.Card.RegisterLink", "Hover", "PSurface", Border.BorderBrushProperty, "Theme.Helper"),
        new("Display.Card.RegisterLink", "Press", "PSurface", UIElement.OpacityProperty, 0.72),
        new("Display.Card.TagLink", "Hover", "PSurface", Border.BorderBrushProperty, "Theme.Accent"),
        new("Display.Card.TagLink", "Press", "PSurface", UIElement.OpacityProperty, 0.72),
        new("Display.Card.TranslationLink", "Hover", "PSurface", Border.BorderBrushProperty, "Theme.Accent"),
        new("Display.Card.TranslationLink", "Press", "PSurface", UIElement.OpacityProperty, 0.72),
        new("Display.Card.ExampleFrame", "Empty", null, UIElement.VisibilityProperty, Visibility.Collapsed),
        new("Display.Card.ExampleCitation", "Empty", null, UIElement.VisibilityProperty, Visibility.Collapsed),
        new("Display.Card.Picture", "Empty", null, UIElement.VisibilityProperty, Visibility.Collapsed),
        new("Display.Card.Video", "Empty", null, UIElement.VisibilityProperty, Visibility.Collapsed),

        new("Theme.Usage.Row", "Base", "PSurface", Border.PaddingProperty, Control.PaddingProperty),
        new("Theme.Usage.Row", "Base", "PSurface", Border.BackgroundProperty, Control.BackgroundProperty),
        new("Theme.Usage.Row", "Base", "PSurface", Border.BorderBrushProperty, Control.BorderBrushProperty),
        new("Theme.Usage.Row", "Base", "PSurface", Border.BorderThicknessProperty, Control.BorderThicknessProperty),
        new("Theme.Usage.Row", "Base", "PSurfaceContent",
            FrameworkElement.HorizontalAlignmentProperty, Control.HorizontalContentAlignmentProperty),
        new("Theme.Usage.Row", "Hover", "PSurface", Border.BackgroundProperty, "Theme.AccentSoft"),
        new("Theme.Usage.Row", "Hover", "PSurface", Border.BorderBrushProperty, "Theme.AccentEdge"),
        new("Theme.Usage.Row", "Press", "PSurface", UIElement.OpacityProperty, 0.72),
        new("Theme.Usage.Detail", "Empty", null, UIElement.VisibilityProperty, Visibility.Collapsed),

        new("Theme.Compass.Choice", "Base", "PSurface", Border.PaddingProperty, Control.PaddingProperty),
        new("Theme.Compass.Choice", "Base", "PSurface", Border.BackgroundProperty, Control.BackgroundProperty),
        new("Theme.Compass.Choice", "Base", "PSurfaceContent",
            FrameworkElement.HorizontalAlignmentProperty, Control.HorizontalContentAlignmentProperty),
        new("Theme.Compass.Choice", "Hover", "PSurface", Border.BackgroundProperty, "Theme.SurfaceRaised"),
        new("Theme.Compass.Choice", "Chosen", null, Control.BackgroundProperty, "Theme.AccentSoft"),
        new("Theme.Compass.Choice", "Chosen", null, Control.ForegroundProperty, "Theme.Accent"),
        new("Theme.Compass.Number", "Empty", null, UIElement.VisibilityProperty, Visibility.Collapsed),
    ];
}
