using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public static class QFontFace
{
    public const double QFontBaseline = 44;

    public static void QFontBaselineRefine(params FrameworkElement[] surfaces)
    {
        ArgumentNullException.ThrowIfNull(surfaces);

        foreach (FrameworkElement surface in surfaces)
        {
            FontFamily family = TextElement.GetFontFamily(surface);
            double size = TextElement.GetFontSize(surface);
            double top = Math.Max(0, Math.Round(QFontBaseline - family.Baseline * size));
            Thickness margin = surface.Margin;
            surface.Margin = new Thickness(margin.Left, top, margin.Right, margin.Bottom);
        }
    }

    public static void QFontRefine(CFont font, params DependencyObject[] surfaces)
    {
        ArgumentNullException.ThrowIfNull(font);
        ArgumentNullException.ThrowIfNull(surfaces);

        foreach (DependencyObject surface in surfaces)
        {
            QFontSurfaceRefine(surface, font);
        }
    }

    private static void QFontSurfaceRefine(DependencyObject surface, CFont font)
    {
        QFontPropertyRefine(
            surface,
            TextElement.FontFamilyProperty,
            font.CFontFamily is string named ? new FontFamily(named) : null);
        QFontPropertyRefine(surface, TextElement.FontSizeProperty, font.CFontSize);
    }

    public static void QFontGlyphRefine(ResourceDictionary resources, CFont glyph)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(glyph);

        QFontResourceRefine(
            resources, "Theme.Glyph.Family", glyph.CFontFamily is string named ? new FontFamily(named) : null);
        QFontResourceRefine(resources, "Theme.Glyph.Size", glyph.CFontSize);
    }

    public static void QFontExampleRefine(ResourceDictionary resources, CFont example)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(example);

        QFontResourceRefine(
            resources, "Theme.Card.ExampleFamily", example.CFontFamily is string named ? new FontFamily(named) : null);
        QFontResourceRefine(resources, "Theme.Card.ExampleSize", example.CFontSize);
    }

    public static void QFontGlossRefine(ResourceDictionary resources, CFont gloss)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(gloss);

        QFontResourceRefine(
            resources, "Theme.Card.GlossFamily", gloss.CFontFamily is string glossed ? new FontFamily(glossed) : null);
        QFontResourceRefine(resources, "Theme.Card.GlossSize", gloss.CFontSize);
        QFontResourceRefine(resources, "Theme.Card.GlossStyle", QFontStyleRead(gloss.CFontStyle));
    }

    private static FontStyle? QFontStyleRead(string? style)
    {
        return style switch
        {
            "italic" => FontStyles.Italic,
            "oblique" => FontStyles.Oblique,
            _ => null,
        };
    }

    private static void QFontResourceRefine(ResourceDictionary resources, string key, object? value)
    {
        if (value is null)
        {
            resources.Remove(key);
            return;
        }

        resources[key] = value;
    }

    private static void QFontPropertyRefine(DependencyObject surface, DependencyProperty property, object? value)
    {
        if (value is null)
        {
            surface.ClearValue(property);
            return;
        }

        surface.SetValue(property, value);
    }
}
