using System;
using System.Collections.Generic;
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

    public static void LFontRefine(
        CAtelier atelier, string language, CFontRole role, params DependencyObject[] surfaces)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(surfaces);

        CFont font = atelier.CAtelierCatalog.CCatalogFontRead(language, role);
        foreach (DependencyObject surface in surfaces)
        {
            LFontSurfaceRefine(surface, font);
        }
    }

    public static void LFontRefine(
        CAtelier atelier,
        string language,
        IReadOnlyList<CFontRole> roles,
        IReadOnlyList<DependencyObject> surfaces)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(surfaces);

        IReadOnlyList<CFont> fonts = atelier.CAtelierCatalog.CCatalogFontRead(language, roles);
        for (int index = 0; index < surfaces.Count; index++)
        {
            LFontSurfaceRefine(surfaces[index], fonts[index]);
        }
    }

    private static void LFontSurfaceRefine(DependencyObject surface, CFont font)
    {
        LFontSet(
            surface,
            TextElement.FontFamilyProperty,
            font.CFontFamily is string named ? new FontFamily(named) : null);
        LFontSet(surface, TextElement.FontSizeProperty, font.CFontSize);
    }

    public static void LFontGlyphRefine(ResourceDictionary resources, CAtelier atelier, string language)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(atelier);

        CFont glyph = atelier.CAtelierCatalog.CCatalogFontRead(language, CFontRole.CFontRoleGlyph);
        LFontResourceSet(
            resources, "Theme.Glyph.Family", glyph.CFontFamily is string named ? new FontFamily(named) : null);
        LFontResourceSet(resources, "Theme.Glyph.Size", glyph.CFontSize);
    }

    public static void LFontExampleRefine(ResourceDictionary resources, CAtelier atelier, string language)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(atelier);

        IReadOnlyList<CFont> fonts = atelier.CAtelierCatalog.CCatalogFontRead(
            language, [CFontRole.CFontRoleExample, CFontRole.CFontRoleGloss]);
        CFont example = fonts[0];
        LFontResourceSet(
            resources, "Theme.Card.ExampleFamily", example.CFontFamily is string named ? new FontFamily(named) : null);
        LFontResourceSet(resources, "Theme.Card.ExampleSize", example.CFontSize);

        CFont gloss = fonts[1];
        LFontResourceSet(
            resources, "Theme.Card.GlossFamily", gloss.CFontFamily is string glossed ? new FontFamily(glossed) : null);
        LFontResourceSet(resources, "Theme.Card.GlossSize", gloss.CFontSize);
        LFontResourceSet(resources, "Theme.Card.GlossStyle", LFontStyleRead(gloss.CFontStyle));
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
