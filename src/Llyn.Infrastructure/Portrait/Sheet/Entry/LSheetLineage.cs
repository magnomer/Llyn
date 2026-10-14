using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LSheetLineage
{
    internal static void LSheetIncomingAppend(StringBuilder sheet, LPortraitPage portrait, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(label);

        if (LSheetSection.LSheetSectionFind(portrait, label.LPortraitLabelIncoming) is LPortraitSection incoming)
        {
            LSheetSection.LSheetSectionAppend(sheet, incoming);
        }
    }

    internal static void LSheetEtymologyAppend(StringBuilder sheet, LPortraitPage portrait, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(label);

        LPortraitSection? etymology = LSheetSection.LSheetSectionFind(portrait, label.LPortraitLabelEtymology);
        if (etymology is null)
        {
            return;
        }

        if (etymology.LPortraitSectionLineage is not LLineage root)
        {
            LSheetSection.LSheetSectionAppend(sheet, etymology);
            return;
        }

        Queue<string> relations = new(
            etymology.LPortraitSectionLine.Skip(1).Select(static line => line.LPortraitLineLabel));
        sheet.Append("<section class=\"band\">\n<h2>")
            .Append(LSheet.LSheetNormalize(etymology.LPortraitSectionHeading))
            .Append("</h2>\n<div class=\"lineage\"><div class=\"tree\">\n");
        LSheetLineageAppend(sheet, root.LLineageSources, relations);
        sheet.Append("<i class=\"stem\"></i><div class=\"root\">")
            .Append(LSheet.LSheetNormalize(root.LLineageText))
            .Append("</div>\n</div></div>\n</section>\n");
    }

    internal static void LSheetAnnotationAppend(StringBuilder sheet, LPortraitPage portrait, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(label);

        if (LSheetSection.LSheetSectionFind(portrait, label.LPortraitLabelNote) is LPortraitSection note)
        {
            LSheetSection.LSheetSectionAppend(sheet, note);
        }
    }

    private static void LSheetLineageAppend(
        StringBuilder sheet, IReadOnlyList<LLineage> sources, Queue<string> relations)
    {
        sheet.Append("<div class=\"stock\">\n");
        foreach (LLineage box in sources)
        {
            string relation = relations.TryDequeue(out string? name) ? name : string.Empty;
            sheet.Append("<div class=\"etymon\">");
            if (box.LLineageSources.Count > 0)
            {
                LSheetLineageAppend(sheet, box.LLineageSources, relations);
                sheet.Append("<i class=\"stem\"></i>");
            }

            LSheetPieceAppend(sheet, "tongue", box.LLineageLanguage);
            sheet.Append("<span class=\"word")
                .Append(box.LLineageEntryId is null ? string.Empty : " linked")
                .Append(box.LLineageBorrowed ? " borrowed" : string.Empty)
                .Append("\">")
                .Append(LSheet.LSheetNormalize(box.LLineageText))
                .Append(box.LLineageEntryId is null ? string.Empty : "<b>↗</b>")
                .Append("</span>");
            LSheetPieceAppend(sheet, "gloss", box.LLineageGloss);
            sheet.Append("<i class=\"stem\"></i>");
            LSheetPieceAppend(sheet, "relation", relation);
            sheet.Append("<i class=\"stem\"></i></div>\n");
        }

        sheet.Append("</div>\n");
    }

    private static void LSheetPieceAppend(StringBuilder sheet, string style, string text)
    {
        if (text.Length > 0)
        {
            sheet.Append("<span class=\"")
                .Append(style)
                .Append("\">")
                .Append(LSheet.LSheetNormalize(text))
                .Append("</span>");
        }
    }
}
