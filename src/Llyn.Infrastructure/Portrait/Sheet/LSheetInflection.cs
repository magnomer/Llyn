using System;
using System.Net;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LSheetInflection
{
    internal static string? LSheetInflectionResolve(string collapsed, string expanded)
    {
        ArgumentNullException.ThrowIfNull(collapsed);
        ArgumentNullException.ThrowIfNull(expanded);

        if (collapsed.Length == 0 || expanded.Length == 0 || collapsed == expanded)
        {
            return expanded.Length > 0 ? expanded : collapsed;
        }

        return null;
    }

    internal static string LSheetInflectionFormat(LParadigmTable? table, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(lookup);

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
                rows.Append("<td>").Append(LSheetCellFormat(form, lookup)).Append("</td>");
            }

            rows.Append("</tr>\n");
        }

        return rows.ToString();
    }

    internal static string LSheetCellFormat(LParadigmForm form, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(form);
        ArgumentNullException.ThrowIfNull(lookup);

        string text = form.LParadigmFormText;
        if (form.LParadigmFormTip is string tip)
        {
            return "<span class=\"llyn-muted\" title=\"" + WebUtility.HtmlEncode(lookup(tip)) + "\">"
                + WebUtility.HtmlEncode(text) + "</span>";
        }

        StringBuilder cell = new StringBuilder();
        foreach (LParadigmPiece piece in form.LParadigmFormScan())
        {
            string run = WebUtility.HtmlEncode(piece.LParadigmPieceText);
            cell.Append(piece.LParadigmPieceMarked ? "<span class=\"llyn-marked\">" + run + "</span>" : run);
            if (piece.LParadigmPieceCut)
            {
                cell.Append("<span class=\"llyn-cut\">-</span>");
            }
        }

        return cell.ToString();
    }
}
