using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiveryEtymology
{
    public static void LLiveryEtymologyAppend(
        StringBuilder sheet, LLiveryPage page, Func<long, string> note, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(lookup);

        if (page.LLiveryPageLineage.Count == 0)
        {
            return;
        }

        Dictionary<long, string> relations = page.LLiveryPageRelation.ToDictionary(
            static relation => relation.LAncestorRelationId,
            relation => relation.LAncestorRelationKey.Length > 0
                ? lookup(relation.LAncestorRelationKey)
                : relation.LAncestorRelationName);
        sheet.Append("## ").Append(LLiveryHeader.LLiveryTextFormat(lookup("Display.Etymology"))).Append("\n\n");
        sheet.Append("<div class=\"lineage\"><div class=\"tree\">\n");
        LLiveryStockAppend(sheet, page.LLiveryPageLineage, relations, note);
        sheet.Append("<i class=\"stem\"></i><div class=\"root\">")
            .Append(LLiveryHeader.LLiveryHtmlFormat(page.LLiveryPageDraft.LEntryDraftHeadword))
            .Append("</div>\n</div></div>\n\n");
    }

    private static void LLiveryStockAppend(
        StringBuilder sheet,
        IReadOnlyList<LLineage> sources,
        Dictionary<long, string> relations,
        Func<long, string> note)
    {
        sheet.Append("<div class=\"stock\">\n");
        foreach (LLineage box in sources)
        {
            string relation = box.LLineageRelationId is long id && relations.TryGetValue(id, out string? name)
                ? name
                : string.Empty;
            sheet.Append("<div class=\"etymon\">");
            if (box.LLineageSources.Count > 0)
            {
                LLiveryStockAppend(sheet, box.LLineageSources, relations, note);
                sheet.Append("<i class=\"stem\"></i>");
            }

            LLiveryPieceAppend(sheet, "tongue", box.LLineageLanguage);
            sheet.Append("<span class=\"word")
                .Append(box.LLineageEntryId is null ? string.Empty : " linked")
                .Append(box.LLineageBorrowed ? " borrowed" : string.Empty)
                .Append("\">")
                .Append(box.LLineageEntryId is long entry
                    ? LLiveryLink.LLiveryEtymonFormat(box.LLineageText, entry, note)
                    : LLiveryHeader.LLiveryHtmlFormat(box.LLineageText))
                .Append(box.LLineageEntryId is null ? string.Empty : "<b>↗</b>")
                .Append("</span>");
            LLiveryPieceAppend(sheet, "gloss", box.LLineageGloss);
            sheet.Append("<i class=\"stem\"></i>");
            LLiveryPieceAppend(sheet, "relation", relation);
            sheet.Append("<i class=\"stem\"></i></div>\n");
        }

        sheet.Append("</div>\n");
    }

    private static void LLiveryPieceAppend(StringBuilder sheet, string style, string text)
    {
        if (text.Length > 0)
        {
            sheet.Append("<span class=\"")
                .Append(style)
                .Append("\">")
                .Append(LLiveryHeader.LLiveryHtmlFormat(text))
                .Append("</span>");
        }
    }
}
