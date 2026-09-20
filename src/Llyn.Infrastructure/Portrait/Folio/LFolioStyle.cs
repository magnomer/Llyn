using System;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LFolioStyle
{
    public static string LFolioStyleRead(LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        string ink = LFolioStyleNormalize(theme.LThemeRead("ink"));
        string muted = LFolioStyleNormalize(theme.LThemeRead("muted"));
        string accent = LFolioStyleNormalize(theme.LThemeRead("accent"));
        string situation = LFolioStyleNormalize(theme.LThemeRead("situation"));

        StringBuilder styles = new StringBuilder();

        styles.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>")
            .Append("<w:styles xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">")
            .Append("<w:docDefaults><w:rPrDefault><w:rPr>")
            .Append("<w:rFonts w:ascii=\"Segoe UI\" w:hAnsi=\"Segoe UI\" w:cs=\"Segoe UI\"/>")
            .Append("<w:color w:val=\"").Append(ink).Append("\"/>")
            .Append("<w:sz w:val=\"21\"/><w:szCs w:val=\"21\"/>")
            .Append("</w:rPr></w:rPrDefault>")
            .Append("<w:pPrDefault><w:pPr><w:spacing w:after=\"0\" w:line=\"264\" ")
            .Append("w:lineRule=\"auto\"/></w:pPr></w:pPrDefault></w:docDefaults>");

        LFolioStyleAppend(styles, "Headword", "Headword", 60, true, ink, null, 200);
        LFolioStyleAppend(styles, "Sound", "Sound", 28, false, ink, null, 120);
        LFolioStyleAppend(styles, "Band", "Band", 28, true, ink, null, 180);
        LFolioStyleAppend(styles, "CardTitle", "Card Title", 24, true, ink, null, 0);
        LFolioStyleAppend(styles, "CardKind", "Card Kind", 24, true, muted, null, 0);
        LFolioStyleAppend(styles, "Phrase", "Phrase", 24, true, ink, null, 100);
        LFolioStyleAppend(styles, "Sense", "Sense", 24, false, ink, null, 100);
        LFolioStyleAppend(styles, "Scene", "Scene", 20, false, situation, null, 80);
        LFolioStyleAppend(styles, "Tone", "Tone", 20, false, muted, null, 80);
        LFolioStyleAppend(styles, "Bridge", "Bridge", 20, true, accent, null, 80);
        LFolioStyleAppend(styles, "Quote", "Quote", 22, false, ink, "Georgia", 100);
        LFolioStyleAppend(styles, "Label", "Label", 18, false, muted, null, 80);
        LFolioStyleAppend(styles, "Plate", "Plate", 21, false, ink, null, 100);
        LFolioStyleAppend(styles, "Row", "Row", 22, false, ink, null, 60);
        LFolioStyleAppend(styles, "RowDetail", "Row Detail", 18, false, muted, null, 120);
        LFolioStyleAppend(styles, "Note", "Note", 21, false, ink, null, 100);
        LFolioStyleAppend(styles, "NoteHeading", "Note Heading", 24, true, ink, null, 60);
        LFolioStyleAppend(styles, "NoteQuote", "Note Quote", 21, false, muted, "Georgia", 100);
        LFolioStyleAppend(styles, "NoteCode", "Note Code", 19, false, ink, "Consolas", 100);

        styles.Append("</w:styles>");
        return styles.ToString();
    }

    public static string LFolioStyleNormalize(string color)
    {
        string trimmed = (color ?? string.Empty).TrimStart('#');
        return trimmed.Length >= 6 ? trimmed[..6].ToUpperInvariant() : "000000";
    }

    private static void LFolioStyleAppend(
        StringBuilder styles,
        string id,
        string name,
        int size,
        bool bold,
        string color,
        string? family,
        int after)
    {
        styles.Append("<w:style w:type=\"paragraph\" w:styleId=\"").Append(id).Append("\">")
            .Append("<w:name w:val=\"").Append(name).Append("\"/>")
            .Append("<w:qFormat/><w:pPr><w:spacing w:after=\"")
            .Append(after.ToString(CultureInfo.InvariantCulture))
            .Append("\"/></w:pPr><w:rPr>");

        if (family is not null)
        {
            styles.Append("<w:rFonts w:ascii=\"").Append(family)
                .Append("\" w:hAnsi=\"").Append(family).Append("\"/>");
        }

        if (bold)
        {
            styles.Append("<w:b/>");
        }

        styles.Append("<w:color w:val=\"").Append(color).Append("\"/>")
            .Append("<w:sz w:val=\"").Append(size.ToString(CultureInfo.InvariantCulture))
            .Append("\"/><w:szCs w:val=\"").Append(size.ToString(CultureInfo.InvariantCulture))
            .Append("\"/></w:rPr></w:style>");
    }
}
