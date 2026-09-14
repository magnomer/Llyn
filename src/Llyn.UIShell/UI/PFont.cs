using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

internal static class PFont
{
    internal const double PFontBaseline = 44;

    internal static void PFontPlace(params FrameworkElement[] surfaces)
    {
        ArgumentNullException.ThrowIfNull(surfaces);

        foreach (FrameworkElement surface in surfaces)
        {
            FontFamily family = TextElement.GetFontFamily(surface);
            double size = TextElement.GetFontSize(surface);
            double top = Math.Max(0, Math.Round(PFontBaseline - family.Baseline * size));
            Thickness margin = surface.Margin;
            surface.Margin = new Thickness(margin.Left, top, margin.Right, margin.Bottom);
        }
    }

    internal static void PFontApply(LEngine engine, string language, params DependencyObject[] surfaces)
    {
        PFontApply(engine, language, LFontRole.LFontRoleHeadword, surfaces);
    }

    internal static void PFontApply(LEngine engine, string language, LFontRole role, params DependencyObject[] surfaces)
    {
        ArgumentNullException.ThrowIfNull(engine);
        ArgumentNullException.ThrowIfNull(surfaces);

        LFont font = PFontRoleRead(engine, language, role);
        FontFamily? family = font.LFontFamily is string named ? new FontFamily(named) : null;

        foreach (DependencyObject surface in surfaces)
        {
            PFontSet(surface, TextElement.FontFamilyProperty, family);
            PFontSet(surface, TextElement.FontSizeProperty, font.LFontSize > 0 ? font.LFontSize : null);
        }
    }

    internal static void PFontExampleApply(ResourceDictionary resources, LEngine engine, string language)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(engine);

        LFont example = PFontRoleRead(engine, language, LFontRole.LFontRoleExample);
        PFontResourceSet(resources, "Theme.Card.ExampleFamily", example.LFontFamily is string named ? new FontFamily(named) : null);
        PFontResourceSet(resources, "Theme.Card.ExampleSize", example.LFontSize > 0 ? example.LFontSize : null);

        LFont gloss = PFontRoleRead(engine, language, LFontRole.LFontRoleGloss);
        PFontResourceSet(resources, "Theme.Card.GlossFamily", gloss.LFontFamily is string glossed ? new FontFamily(glossed) : null);
        PFontResourceSet(resources, "Theme.Card.GlossSize", gloss.LFontSize > 0 ? gloss.LFontSize : null);
        PFontResourceSet(resources, "Theme.Card.GlossStyle", PFontStyleRead(gloss.LFontStyle));
    }

    private static LFont PFontRoleRead(LEngine engine, string language, LFontRole role)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return new LFont(null, 0);
        }

        try
        {
            return engine.LEngineFontRead(language, role);
        }
        catch (Exception)
        {
            return new LFont(null, 0);
        }
    }

    private static FontStyle? PFontStyleRead(string? style)
    {
        if (string.Equals(style, "italic", StringComparison.OrdinalIgnoreCase))
        {
            return FontStyles.Italic;
        }

        if (string.Equals(style, "oblique", StringComparison.OrdinalIgnoreCase))
        {
            return FontStyles.Oblique;
        }

        return null;
    }

    private static void PFontResourceSet(ResourceDictionary resources, string key, object? value)
    {
        if (value is null)
        {
            resources.Remove(key);
            return;
        }

        resources[key] = value;
    }

    private static void PFontSet(DependencyObject surface, DependencyProperty property, object? value)
    {
        if (value is null)
        {
            surface.ClearValue(property);
            return;
        }

        surface.SetValue(property, value);
    }
}
