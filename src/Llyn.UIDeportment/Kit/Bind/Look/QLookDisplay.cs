using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

internal static class QLookDisplay
{
    internal static readonly IReadOnlyList<QLook.QLookSetter> QLookDisplayState =
    [
        new("Display.Card.SituationLink", QLookCue.QLookCueHover, "PSurface",
            Border.BorderBrushProperty, "Theme.Situation"),
        new("Display.Card.SituationLink", QLookCue.QLookCuePress, "PSurface", UIElement.OpacityProperty, 0.72),
        new("Display.Card.RegisterLink", QLookCue.QLookCueHover, "PSurface",
            Border.BorderBrushProperty, "Theme.Helper"),
        new("Display.Card.RegisterLink", QLookCue.QLookCuePress, "PSurface", UIElement.OpacityProperty, 0.72),
        new("Display.Card.TagLink", QLookCue.QLookCueHover, "PSurface", Border.BorderBrushProperty, "Theme.Accent"),
        new("Display.Card.TagLink", QLookCue.QLookCuePress, "PSurface", UIElement.OpacityProperty, 0.72),
        new("Display.Card.TranslationLink", QLookCue.QLookCueHover, "PSurface",
            Border.BorderBrushProperty, "Theme.Accent"),
        new("Display.Card.TranslationLink", QLookCue.QLookCuePress, "PSurface", UIElement.OpacityProperty, 0.72),
        new("Display.Card.ExampleFrame", QLookCue.QLookCueEmpty, null,
            UIElement.VisibilityProperty, Visibility.Collapsed),
        new("Display.Card.ExampleCitation", QLookCue.QLookCueEmpty, null,
            UIElement.VisibilityProperty, Visibility.Collapsed),
        new("Display.Card.Picture", QLookCue.QLookCueEmpty, null, UIElement.VisibilityProperty, Visibility.Collapsed),
        new("Display.Card.Video", QLookCue.QLookCueEmpty, null, UIElement.VisibilityProperty, Visibility.Collapsed),

        new("Theme.Usage.Row", QLookCue.QLookCueBase, "PSurface", Border.PaddingProperty, Control.PaddingProperty),
        new("Theme.Usage.Row", QLookCue.QLookCueBase, "PSurface",
            Border.BackgroundProperty, Control.BackgroundProperty),
        new("Theme.Usage.Row", QLookCue.QLookCueBase, "PSurface",
            Border.BorderBrushProperty, Control.BorderBrushProperty),
        new("Theme.Usage.Row", QLookCue.QLookCueBase, "PSurface",
            Border.BorderThicknessProperty, Control.BorderThicknessProperty),
        new("Theme.Usage.Row", QLookCue.QLookCueBase, "PSurfaceContent",
            FrameworkElement.HorizontalAlignmentProperty, Control.HorizontalContentAlignmentProperty),
        new("Theme.Usage.Row", QLookCue.QLookCueHover, "PSurface", Border.BackgroundProperty, "Theme.AccentSoft"),
        new("Theme.Usage.Row", QLookCue.QLookCueHover, "PSurface", Border.BorderBrushProperty, "Theme.AccentEdge"),
        new("Theme.Usage.Row", QLookCue.QLookCuePress, "PSurface", UIElement.OpacityProperty, 0.72),
        new("Theme.Usage.Detail", QLookCue.QLookCueEmpty, null, UIElement.VisibilityProperty, Visibility.Collapsed),

        new("Theme.Compass.Choice", QLookCue.QLookCueBase, "PSurface", Border.PaddingProperty, Control.PaddingProperty),
        new("Theme.Compass.Choice", QLookCue.QLookCueBase, "PSurface",
            Border.BackgroundProperty, Control.BackgroundProperty),
        new("Theme.Compass.Choice", QLookCue.QLookCueBase, "PSurfaceContent",
            FrameworkElement.HorizontalAlignmentProperty, Control.HorizontalContentAlignmentProperty),
        new("Theme.Compass.Choice", QLookCue.QLookCueHover, "PSurface",
            Border.BackgroundProperty, "Theme.SurfaceRaised"),
        new("Theme.Compass.Choice", QLookCue.QLookCueChosen, null, Control.BackgroundProperty, "Theme.AccentSoft"),
        new("Theme.Compass.Choice", QLookCue.QLookCueChosen, null, Control.ForegroundProperty, "Theme.Accent"),
        new("Theme.Compass.Number", QLookCue.QLookCueEmpty, null, UIElement.VisibilityProperty, Visibility.Collapsed),
    ];
}
