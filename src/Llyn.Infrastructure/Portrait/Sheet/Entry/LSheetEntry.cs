using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheetEntry
{
    public static string LSheetEntryFormat(
        LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label, LTheme theme, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(livery);
        ArgumentNullException.ThrowIfNull(portrait);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentNullException.ThrowIfNull(lookup);

        IReadOnlyList<string> ensigns = LSheetEnsign.LSheetEnsignScan(livery);
        StringBuilder sheet = new StringBuilder();

        sheet.Append("<!doctype html>\n<html>\n<head>\n<meta charset=\"utf-8\">\n")
            .Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\n")
            .Append("<title>")
            .Append(LSheet.LSheetNormalize(portrait.LPortraitPageTitle))
            .Append("</title>\n<style>")
            .Append(LSheetCascade.LSheetCascadeRead(theme))
            .Append(LSheetEnsign.LSheetEnsignFormat(ensigns))
            .Append("</style>\n</head>\n<body>\n<main class=\"entry\">\n");

        LSheetHeader.LSheetHeaderAppend(sheet, livery, ensigns, lookup);
        LSheetSound.LSheetSoundAppend(sheet, livery, ensigns, theme, lookup);
        LSheetSpeechAppend(sheet, livery, lookup);
        LSheetFrequencyAppend(sheet, livery, lookup);
        LSheetParadigm.LSheetParadigmAppend(sheet, livery, lookup);

        LEntryDraft draft = livery.LLiveryPageDraft;
        LSheetBox.LSheetFanqieAppend(sheet, livery, portrait, label);
        LSheetDeck.LSheetDeckAppend(sheet, livery, portrait, label.LPortraitLabelMeanings, draft.LEntryDraftMeanings);
        if (LSheetSection.LSheetSectionFind(portrait, label.LPortraitLabelOther) is LPortraitSection other)
        {
            LSheetSection.LSheetSectionAppend(sheet, other);
        }

        LSheetLineage.LSheetIncomingAppend(sheet, portrait, label);
        LSheetLineage.LSheetEtymologyAppend(sheet, portrait, label);
        LSheetLineage.LSheetAnnotationAppend(sheet, portrait, label);
        LSheetBox.LSheetScriptAppend(sheet, livery, portrait, label);

        LSheetStampAppend(sheet, livery, lookup);

        sheet.Append("</main>\n</body>\n</html>\n");
        return sheet.ToString();
    }

    private static void LSheetSpeechAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)
    {
        LEntryDraft draft = page.LLiveryPageDraft;
        if (!draft.LEntryDraftMarked)
        {
            return;
        }

        string unit = LUnitKey.LUnitKeyRead(draft.LEntryDraftUnit);
        string freedom = LFreedomKey.LFreedomKeyRead(draft.LEntryDraftFreedom);
        sheet.Append("<div class=\"speech\">\n");
        if (unit.Length > 0 || freedom.Length > 0)
        {
            sheet.Append("<div class=\"scope\">");
            if (unit.Length > 0)
            {
                sheet.Append("<span class=\"unit\">").Append(LSheet.LSheetNormalize(lookup(unit))).Append("</span>");
            }

            if (freedom.Length > 0)
            {
                sheet.Append("<span class=\"freedom\">").Append(LSheet.LSheetNormalize(lookup(freedom)))
                    .Append("</span>");
            }

            sheet.Append("</div>\n");
        }

        if (draft.LEntryDraftNames.Count > 0)
        {
            sheet.Append("<div class=\"chips\">");
            foreach (string name in draft.LEntryDraftNames)
            {
                sheet.Append("<span class=\"chip\">").Append(LSheet.LSheetNormalize(name)).Append("</span>");
            }

            sheet.Append("</div>\n");
        }

        sheet.Append("</div>\n");
    }

    private static void LSheetFrequencyAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)
    {
        if (LFrequencyGauge.LFrequencyGaugeResolve(page.LLiveryPageFrequency, lookup("Frequency.Once"))
            is not LFrequencyGauge gauge)
        {
            return;
        }

        sheet.Append("<div class=\"frequency\"><span class=\"pill frequency-")
            .Append(gauge.LFrequencyGaugeRank.ToLowerInvariant())
            .Append("\" title=\"").Append(LSheet.LSheetNormalize(gauge.LFrequencyGaugeSource)).Append("\">")
            .Append("<span class=\"tier\">")
            .Append(LSheet.LSheetNormalize(lookup("Frequency." + gauge.LFrequencyGaugeRank))).Append("</span>");
        if (gauge.LFrequencyGaugeRanked)
        {
            sheet.Append("<span class=\"band\">");
            for (int pip = 0; pip < gauge.LFrequencyGaugeBand; pip++)
            {
                sheet.Append("<span class=\"pip on\">✦</span>");
            }

            for (int pip = 0; pip < gauge.LFrequencyGaugeSpare; pip++)
            {
                sheet.Append("<span class=\"pip\">✦</span>");
            }

            sheet.Append("</span>");
        }

        sheet.Append("</span></div>\n");
    }

    private static void LSheetStampAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)
    {
        sheet.Append("<footer class=\"stamp\">\n<hr>\n<div class=\"grid\">")
            .Append("<span class=\"label\">")
            .Append(LSheet.LSheetNormalize(lookup("Display.Created"))).Append("</span>")
            .Append("<span class=\"time\">").Append(LSheet.LSheetNormalize(page.LLiveryPageCreated)).Append("</span>")
            .Append("<span class=\"label\">")
            .Append(LSheet.LSheetNormalize(lookup("Display.Updated"))).Append("</span>")
            .Append("<span class=\"time\">").Append(LSheet.LSheetNormalize(page.LLiveryPageUpdated)).Append("</span>")
            .Append("</div>\n</footer>\n");
    }
}
