using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Conduct;

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

    public static void LFontApply(CAtelier atelier, string language, params DependencyObject[] surfaces)
    {
        LFontApply(atelier, language, CFontRole.CFontRoleHeadword, surfaces);
    }

    public static void LFontApply(CAtelier atelier, string language, CFontRole role, params DependencyObject[] surfaces)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(surfaces);

        CFont font = LFontRoleRead(atelier, language, role);
        FontFamily? family = font.CFontFamily is string named ? new FontFamily(named) : null;

        foreach (DependencyObject surface in surfaces)
        {
            LFontSet(surface, TextElement.FontFamilyProperty, family);
            LFontSet(surface, TextElement.FontSizeProperty, font.CFontSize);
        }
    }

    public static void LFontGlyphApply(ResourceDictionary resources, CAtelier atelier, string language)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(atelier);

        CFont glyph = LFontRoleRead(atelier, language, CFontRole.CFontRoleGlyph);
        LFontResourceSet(
            resources, "Theme.Glyph.Family", glyph.CFontFamily is string named ? new FontFamily(named) : null);
        LFontResourceSet(resources, "Theme.Glyph.Size", glyph.CFontSize);
    }

    public static void LFontExampleApply(ResourceDictionary resources, CAtelier atelier, string language)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(atelier);

        CFont example = LFontRoleRead(atelier, language, CFontRole.CFontRoleExample);
        LFontResourceSet(
            resources, "Theme.Card.ExampleFamily", example.CFontFamily is string named ? new FontFamily(named) : null);
        LFontResourceSet(resources, "Theme.Card.ExampleSize", example.CFontSize);

        CFont gloss = LFontRoleRead(atelier, language, CFontRole.CFontRoleGloss);
        LFontResourceSet(
            resources, "Theme.Card.GlossFamily", gloss.CFontFamily is string glossed ? new FontFamily(glossed) : null);
        LFontResourceSet(resources, "Theme.Card.GlossSize", gloss.CFontSize);
        LFontResourceSet(resources, "Theme.Card.GlossStyle", LFontStyleRead(gloss.CFontStyle));
    }

    private static CFont LFontRoleRead(CAtelier atelier, string language, CFontRole role)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return new CFont(null, null, null);
        }

        try
        {
            return atelier.CAtelierCatalog.CCatalogFontRead(language, role);
        }
        catch (Exception)
        {
            return new CFont(null, null, null);
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
