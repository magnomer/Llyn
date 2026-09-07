using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

internal static class PFont
{
    internal static void PFontApply(LEngine engine, string language, params DependencyObject[] surfaces)
    {
        ArgumentNullException.ThrowIfNull(engine);
        ArgumentNullException.ThrowIfNull(surfaces);

        LFont font = PFontRead(engine, language);
        FontFamily? family = font.LFontFamily is string named ? new FontFamily(named) : null;

        foreach (DependencyObject surface in surfaces)
        {
            PFontSet(surface, TextElement.FontFamilyProperty, family);
            PFontSet(surface, TextElement.FontSizeProperty, font.LFontSize > 0 ? font.LFontSize : null);
        }
    }

    internal static LFont PFontExampleRead(LEngine engine, string language)
    {
        ArgumentNullException.ThrowIfNull(engine);

        if (string.IsNullOrWhiteSpace(language))
        {
            return new LFont(null, 0);
        }

        try
        {
            return engine.LEngineFontRead(language, LFontRole.LFontRoleExample);
        }
        catch (Exception)
        {
            return new LFont(null, 0);
        }
    }

    private static LFont PFontRead(LEngine engine, string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return new LFont(null, 0);
        }

        try
        {
            return engine.LEngineFontRead(language);
        }
        catch (Exception)
        {
            return new LFont(null, 0);
        }
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
