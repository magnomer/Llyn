using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiveryCollocation
{
    internal static void LLiveryListAppend(
        StringBuilder sheet,
        LLiveryPage page,
        LCardDraft card,
        IReadOnlyList<LCardDraft> collocations,
        Func<long, string> note,
        Func<string, string> lookup)
    {
        IReadOnlyList<LCardDraft> meanings = page.LLiveryPageDraft.LEntryDraftMeanings;
        List<LCardDraft> rows = collocations
            .Where(collocation => LCollocation.LCollocationHomeRead(meanings, collocation, true) == card.LCardDraftId)
            .ToList();
        if (rows.Count == 0)
        {
            return;
        }

        sheet.Append("<div class=\"llyn-collocations\">\n\n<span class=\"llyn-heading\">")
            .Append(LLiveryHeader.LLiveryTextFormat(string.Format(
                CultureInfo.CurrentCulture, lookup("Display.CollocationHeading"), rows.Count)))
            .Append("</span>\n\n");
        LLiveryRowAppend(sheet, page, rows, note);
        sheet.Append("</div>\n\n");
    }

    internal static void LLiveryOtherAppend(
        StringBuilder sheet,
        LLiveryPage page,
        IReadOnlyList<LCardDraft> others,
        string heading,
        Func<long, string> note)
    {
        if (others.Count == 0)
        {
            return;
        }

        sheet.Append("## ").Append(LLiveryHeader.LLiveryTextFormat(heading))
            .Append("\n\n<div class=\"llyn-collocations\">\n\n");
        LLiveryRowAppend(sheet, page, others, note);
        sheet.Append("</div>\n\n");
    }

    private static void LLiveryRowAppend(
        StringBuilder sheet, LLiveryPage page, IReadOnlyList<LCardDraft> rows, Func<long, string> note)
    {
        foreach (LCardDraft row in rows)
        {
            bool opened = !page.LLiveryPageFold.Contains(row.LCardDraftId);
            sheet.Append(LLiveryCard.LLiveryFoldFormat("llyn-collocation", opened))
                .Append("<span class=\"llyn-phrase\">")
                .Append(LLiveryHeader.LLiveryTextFormat(LLiveryPhraseFormat(row))).Append("</span>");
            string meaning = row.LCardDraftMeaning.LStateValueShow();
            if (meaning.Length > 0)
            {
                sheet.Append(" <span class=\"llyn-meaning\">").Append(LLiveryHeader.LLiveryTextFormat(meaning))
                    .Append("</span>");
            }

            sheet.Append("</summary>\n\n");
            LLiveryCard.LLiveryFieldAppend(sheet, page, row, note);
            LLiveryCard.LLiveryBodyAppend(sheet, page, row, note);
            sheet.Append("</details>\n\n");
        }
    }

    internal static void LLiveryStubAppend(
        StringBuilder sheet,
        LLiveryPage page,
        LCardDraft card,
        IReadOnlyList<LCardDraft> collocations,
        Func<string, string> lookup)
    {
        IReadOnlyList<LCardDraft> meanings = page.LLiveryPageDraft.LEntryDraftMeanings;
        foreach (LCardDraft collocation in collocations)
        {
            if (!LCollocation.LCollocationStubRead(meanings, collocation, true).Contains(card.LCardDraftId)
                || LCollocation.LCollocationHomeRead(meanings, collocation, true) is not long home
                || LMeaning.LMeaningPathRead(meanings, home) is not string number)
            {
                continue;
            }

            sheet.Append("<span class=\"llyn-stub\">")
                .Append(LLiveryHeader.LLiveryTextFormat(string.Format(
                    CultureInfo.CurrentCulture, lookup("Display.CollocationStub"), LLiveryPhraseFormat(collocation),
                    number)))
                .Append("</span>\n\n");
        }
    }

    private static string LLiveryPhraseFormat(LCardDraft collocation)
    {
        LStateValue phrase = LCollocation.LCollocationPhraseRead(collocation);
        return phrase.LStateValueUncertain ? collocation.LCardDraftTitle.LStateValueShow() : phrase.LStateValueShow();
    }
}
