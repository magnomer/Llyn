using System;
using System.Net;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiveryInflection
{
    public static void LLiveryInflectionAppend(StringBuilder sheet, LParadigmView? view, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(lookup);

        string collapsed = LSheetInflection.LSheetInflectionFormat(view?.LParadigmViewCollapsed, lookup);
        string expanded = LSheetInflection.LSheetInflectionFormat(view?.LParadigmViewExpanded, lookup);
        if (LSheetInflection.LSheetInflectionResolve(collapsed, expanded) is string rows)
        {
            if (rows.Length > 0)
            {
                sheet.Append("<div class=\"llyn-inflection-box\">\n<table class=\"llyn-inflection\">\n").Append(rows)
                    .Append("</table>\n</div>\n\n");
            }

            return;
        }

        sheet.Append("<div class=\"llyn-inflection-box\">\n<details class=\"llyn-inflection-full\">\n<summary>")
            .Append("<span class=\"llyn-inflection-switch-short\">")
            .Append(WebUtility.HtmlEncode(lookup("Paradigm.Short")))
            .Append("</span><span class=\"llyn-inflection-switch-long\">")
            .Append(WebUtility.HtmlEncode(lookup("Paradigm.Full")))
            .Append("</span></summary>\n")
            .Append("<table class=\"llyn-inflection\">\n").Append(expanded).Append("</table>\n</details>\n")
            .Append("<table class=\"llyn-inflection llyn-inflection-short\">\n").Append(collapsed).Append("</table>\n")
            .Append("</div>\n\n");
    }
}
