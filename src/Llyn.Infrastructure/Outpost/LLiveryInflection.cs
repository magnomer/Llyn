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

        string collapsed = LLiveryTableFormat(view?.LParadigmViewCollapsed, lookup);
        string expanded = LLiveryTableFormat(view?.LParadigmViewExpanded, lookup);
        if (collapsed.Length == 0 || expanded.Length == 0 || collapsed == expanded)
        {
            string rows = expanded.Length > 0 ? expanded : collapsed;
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

    private static string LLiveryTableFormat(LParadigmTable? table, Func<string, string> lookup)
    {
        if (table is null || table.LParadigmTableLines.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder rows = new StringBuilder();
        if (table.LParadigmTableHeaders.Count > 0)
        {
            rows.Append("<tr><th></th><th></th>");
            foreach (string header in table.LParadigmTableHeaders)
            {
                rows.Append("<th>").Append(header.Length > 0 ? WebUtility.HtmlEncode(lookup(header)) : string.Empty)
                    .Append("</th>");
            }

            rows.Append("</tr>\n");
        }

        for (int index = 0; index < table.LParadigmTableLines.Count; index++)
        {
            LParadigmLine line = table.LParadigmTableLines[index];
            string group = line.LParadigmLineGroup.Length > 0 ? lookup(line.LParadigmLineGroup) : string.Empty;
            string label = line.LParadigmLineLabel.Length > 0 ? lookup(line.LParadigmLineLabel) : string.Empty;
            string rule = index == 0 ? string.Empty : line.LParadigmLineGroup.Length > 0 ? "llyn-close" : "llyn-rule";
            rows.Append(rule.Length > 0 ? "<tr class=\"" + rule + "\">" : "<tr>")
                .Append("<td class=\"llyn-group\">").Append(WebUtility.HtmlEncode(group)).Append("</td>")
                .Append("<td class=\"llyn-label\">").Append(WebUtility.HtmlEncode(label)).Append("</td>");
            foreach (LParadigmForm form in line.LParadigmLineForms)
            {
                rows.Append("<td>").Append(LLiveryInflectionFormat(form, lookup)).Append("</td>");
            }

            rows.Append("</tr>\n");
        }

        return rows.ToString();
    }

    private static string LLiveryInflectionFormat(LParadigmForm form, Func<string, string> lookup)
    {
        string text = form.LParadigmFormText;
        if (form.LParadigmFormTip is string tip)
        {
            return "<span class=\"llyn-muted\" title=\"" + WebUtility.HtmlEncode(lookup(tip)) + "\">"
                + WebUtility.HtmlEncode(text) + "</span>";
        }

        bool[] marked = new bool[text.Length];
        foreach (LInflectionMark mark in form.LParadigmFormMarks)
        {
            int offset = Math.Clamp(mark.LInflectionMarkOffset, 0, text.Length);
            int end = Math.Clamp(mark.LInflectionMarkOffset + mark.LInflectionMarkLength, offset, text.Length);
            Array.Fill(marked, true, offset, end - offset);
        }

        int split = form.LParadigmFormSplit;
        StringBuilder cell = new StringBuilder();
        int start = 0;
        for (int index = 1; index <= text.Length; index++)
        {
            if (index < text.Length && index != split && marked[index] == marked[start])
            {
                continue;
            }

            string run = WebUtility.HtmlEncode(text[start..index]);
            cell.Append(marked[start] ? "<span class=\"llyn-marked\">" + run + "</span>" : run);
            if (index == split && split < text.Length)
            {
                cell.Append("<span class=\"llyn-cut\">-</span>");
            }

            start = index;
        }

        return cell.ToString();
    }
}
