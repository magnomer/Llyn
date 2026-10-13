using System;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LSheetParadigm
{
    internal static void LSheetParadigmAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(lookup);

        string collapsed = LSheetInflection.LSheetInflectionFormat(
            page.LLiveryPageInflection?.LParadigmViewCollapsed, lookup);
        string expanded = LSheetInflection.LSheetInflectionFormat(
            page.LLiveryPageInflection?.LParadigmViewExpanded, lookup);
        if (page.LLiveryPageParadigm.Count == 0 && collapsed.Length == 0 && expanded.Length == 0)
        {
            return;
        }

        sheet.Append("<div class=\"paradigm\">\n");
        if (page.LLiveryPageParadigm.Count > 0)
        {
            sheet.Append("<table class=\"paradigm-rows\">\n");
            for (int index = 0; index < page.LLiveryPageParadigm.Count; index++)
            {
                LParadigmRow row = page.LLiveryPageParadigm[index];
                LParadigmShown shown = page.LLiveryPageShown[index];
                sheet.Append("<tr><td class=\"part\">").Append(LSheet.LSheetNormalize(row.LParadigmRowPart))
                    .Append("</td><td class=\"name\">").Append(LSheet.LSheetNormalize(row.LParadigmRowName))
                    .Append("</td><td class=\"text\"");
                if (shown.LParadigmShownTip is string tip)
                {
                    sheet.Append(" title=\"").Append(LSheet.LSheetNormalize(lookup(tip))).Append('"');
                }

                sheet.Append('>').Append(LSheet.LSheetNormalize(shown.LParadigmShownText)).Append("</td></tr>\n");
            }

            sheet.Append("</table>\n");
        }

        if (LSheetInflection.LSheetInflectionResolve(collapsed, expanded) is string rows)
        {
            if (rows.Length > 0)
            {
                sheet.Append("<table class=\"inflection\">\n").Append(rows).Append("</table>\n");
            }
        }
        else
        {
            LSheetSwitchAppend(sheet, lookup);
            sheet.Append("<table class=\"inflection inflection-short\">\n").Append(collapsed).Append("</table>\n")
                .Append("<table class=\"inflection inflection-full\">\n").Append(expanded).Append("</table>\n");
        }

        sheet.Append("</div>\n");
    }

    private static void LSheetSwitchAppend(StringBuilder sheet, Func<string, string> lookup)
    {
        sheet.Append("<input type=\"radio\" name=\"paradigm-fold\" id=\"paradigm-short\" checked>")
            .Append("<input type=\"radio\" name=\"paradigm-fold\" id=\"paradigm-full\">")
            .Append("<div class=\"switch\" title=\"").Append(LSheet.LSheetNormalize(lookup("Paradigm.Fold")))
            .Append("\"><label for=\"paradigm-short\">").Append(LSheet.LSheetNormalize(lookup("Paradigm.Short")))
            .Append("</label><label for=\"paradigm-full\">").Append(LSheet.LSheetNormalize(lookup("Paradigm.Full")))
            .Append("</label></div>\n");
    }
}
