using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheetHeader
{
    public static void LSheetHeaderAppend(
        StringBuilder sheet, LLiveryPage page, IReadOnlyList<string> ensigns, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(ensigns);
        ArgumentNullException.ThrowIfNull(lookup);

        sheet.Append("<header class=\"header\">\n<h1 class=\"headword\"");
        if (page.LLiveryPageFont.TryGetValue(LFontRole.LFontRoleHeadword, out LFont? font)
            && (font.LFontFace is not null || font.LFontSized is not null))
        {
            List<string> rules = [];
            if (font.LFontFace is string face)
            {
                rules.Add("font-family:'" + face.Replace("\\", "\\\\", StringComparison.Ordinal)
                    .Replace("'", "\\'", StringComparison.Ordinal) + "'");
            }

            if (font.LFontSized is double size)
            {
                rules.Add("font-size:" + size.ToString("0.##", CultureInfo.InvariantCulture) + "px");
            }

            sheet.Append(" style=\"").Append(LSheet.LSheetNormalize(string.Join(';', rules))).Append('"');
        }

        sheet.Append('>').Append(LSheet.LSheetNormalize(page.LLiveryPageDraft.LEntryDraftHeadword)).Append("</h1>\n");
        if (page.LLiveryPageReading.Length > 0)
        {
            sheet.Append("<div class=\"reading\">").Append(LSheet.LSheetNormalize(page.LLiveryPageReading))
                .Append("</div>\n");
        }

        LSheetLanguageAppend(sheet, page, ensigns);

        sheet.Append("<div class=\"regard\">")
            .Append(page.LLiveryPageFavorite
                ? "<span class=\"heart on\"></span>"
                : "<span class=\"heart\"></span>");
        int grasp = LGrasp.LGraspNormalize(page.LLiveryPageGrasp);
        LSheetStarAppend(sheet, grasp);
        sheet.Append("<span class=\"rating\">")
            .Append(LSheet.LSheetNormalize(lookup(LGrasp.LGraspKeyRead(grasp))))
            .Append("</span></div>\n</header>\n");
    }

    internal static void LSheetLanguageAppend(StringBuilder sheet, LLiveryPage page, IReadOnlyList<string> ensigns)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(ensigns);

        string language = page.LLiveryPageDraft.LEntryDraftLanguage;
        if (string.IsNullOrWhiteSpace(language))
        {
            return;
        }

        string ensign = page.LLiveryPageBanner.TryGetValue(language, out string? path)
            ? LSheetEnsign.LSheetEnsignRead(ensigns, path)
            : string.Empty;
        sheet.Append("<span class=\"language\">")
            .Append(ensign.Length > 0
                ? "<span class=\"flag " + ensign + "\"></span>"
                : "<span class=\"globe\"></span>")
            .Append("<span class=\"name\">").Append(LSheet.LSheetNormalize(language)).Append("</span></span>\n");
    }

    private static void LSheetStarAppend(StringBuilder sheet, int grasp)
    {
        sheet.Append("<span class=\"grasp\">");
        foreach (int filled in LGrasp.LGraspStarScan(grasp))
        {
            sheet.Append(filled switch
            {
                2 => "<span class=\"star full\"></span>",
                1 => "<span class=\"star half\"></span>",
                _ => "<span class=\"star\"></span>",
            });
        }

        sheet.Append("</span>");
    }
}
