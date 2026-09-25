using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIDeportment;

public static class LFontFace
{
    public const double LFontBaseline = 44;

    public static void LFontPlace(params FrameworkElement[] surfaces)
    {
        ArgumentNullException.ThrowIfNull(surfaces);

        foreach (FrameworkElement surface in surfaces)
        {
            FontFamily family = TextElement.GetFontFamily(surface);
            double size = TextElement.GetFontSize(surface);
            double top = Math.Max(0, Math.Round(LFontBaseline - family.Baseline * size));
            Thickness margin = surface.Margin;
            surface.Margin = new Thickness(margin.Left, top, margin.Right, margin.Bottom);
        }
    }

    public static void LFontApply(LWindow window, string language, params DependencyObject[] surfaces)
    {
        LFontApply(window, language, LFontRole.LFontRoleHeadword, surfaces);
    }

    public static void LFontApply(LWindow window, string language, LFontRole role, params DependencyObject[] surfaces)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(surfaces);

        LFont font = LFontRoleRead(window, language, role);
        FontFamily? family = font.LFontFamily is string named ? new FontFamily(named) : null;

        foreach (DependencyObject surface in surfaces)
        {
            LFontSet(surface, TextElement.FontFamilyProperty, family);
            LFontSet(surface, TextElement.FontSizeProperty, font.LFontSized);
        }
    }

    public static void LFontGlyphApply(ResourceDictionary resources, LWindow window, string language)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(window);

        LFont glyph = LFontRoleRead(window, language, LFontRole.LFontRoleGlyph);
        LFontResourceSet(
            resources, "Theme.Glyph.Family", glyph.LFontFamily is string named ? new FontFamily(named) : null);
        LFontResourceSet(resources, "Theme.Glyph.Size", glyph.LFontSized);
    }

    public static void LFontExampleApply(ResourceDictionary resources, LWindow window, string language)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(window);

        LFont example = LFontRoleRead(window, language, LFontRole.LFontRoleExample);
        LFontResourceSet(
            resources, "Theme.Card.ExampleFamily", example.LFontFamily is string named ? new FontFamily(named) : null);
        LFontResourceSet(resources, "Theme.Card.ExampleSize", example.LFontSized);

        LFont gloss = LFontRoleRead(window, language, LFontRole.LFontRoleGloss);
        LFontResourceSet(
            resources, "Theme.Card.GlossFamily", gloss.LFontFamily is string glossed ? new FontFamily(glossed) : null);
        LFontResourceSet(resources, "Theme.Card.GlossSize", gloss.LFontSized);
        LFontResourceSet(resources, "Theme.Card.GlossStyle", LFontStyleRead(gloss.LFontStyle));
    }

    private static LFont LFontRoleRead(LWindow window, string language, LFontRole role)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return new LFont(null, 0);
        }

        try
        {
            return window.LWindowFontRead(language, role);
        }
        catch (Exception)
        {
            return new LFont(null, 0);
        }
    }

    private static FontStyle? LFontStyleRead(string? style)
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

    private static void LFontResourceSet(ResourceDictionary resources, string key, object? value)
    {
        if (value is null)
        {
            resources.Remove(key);
            return;
        }

        resources[key] = value;
    }

    private static void LFontSet(DependencyObject surface, DependencyProperty property, object? value)
    {
        if (value is null)
        {
            surface.ClearValue(property);
            return;
        }

        surface.SetValue(property, value);
    }
}
