using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiveryYunjing
{
    public static void LLiveryYunjingAppend(
        StringBuilder sheet, LLiveryDiwei diwei, Func<long, string> note, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(diwei);
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(lookup);

        LDiweiPage page = diwei.LLiveryDiweiPage;
        string key = string.Equals(diwei.LLiveryDiweiKind, LDiwei.LDiweiTone, StringComparison.Ordinal)
            ? string.Format(CultureInfo.CurrentCulture, lookup("Display.FanqieTone"), page.LDiweiPageKey)
            : page.LDiweiPageKey;
        sheet.Append("# ").Append(LLiveryHeader.LLiveryTextFormat(key)).Append("\n\n");
        if (page.LDiweiPageEmpty)
        {
            sheet.Append(LLiveryRime.LLiveryChipFormat("llyn-vacant", lookup("Yunjing.DiweiEmpty"))).Append("\n\n");
        }

        foreach (LDiweiSection section in page.LDiweiPageSections)
        {
            LLiverySectionAppend(sheet, section, diwei.LLiveryDiweiEntry, note);
        }

        LLiveryXiesheng.LLiveryEntryAppend(sheet, diwei.LLiveryDiweiEntry, note, string.Empty);
    }

    private static void LLiverySectionAppend(
        StringBuilder sheet, LDiweiSection section, IReadOnlyList<LEntry> entries, Func<long, string> note)
    {
        sheet.Append("<div class=\"llyn-diwei\">\n\n");
        if (section.LDiweiSectionLabel.Length > 0)
        {
            sheet.Append(LLiveryRime.LLiveryChipFormat("llyn-heading", section.LDiweiSectionLabel)).Append("\n\n");
        }

        sheet.Append(LLiveryRime.LLiveryRowFormat([string.Empty, string.Empty, string.Empty, string.Empty]))
            .Append(LLiveryRime.LLiveryRowFormat(["---", "---", "---", "---"]));
        foreach (LDiweiLine line in section.LDiweiSectionLines)
        {
            sheet.Append(LLiveryLineFormat(line, entries, note));
        }

        foreach (LTallyRow tally in section.LDiweiSectionTallies)
        {
            sheet.Append(LLiveryTallyFormat(tally));
        }

        sheet.Append("\n</div>\n\n");
    }

    private static string LLiveryLineFormat(LDiweiLine line, IReadOnlyList<LEntry> entries, Func<long, string> note)
    {
        StringBuilder characters = new StringBuilder();
        foreach (string character in line.LDiweiLineCharacters)
        {
            characters.Append(characters.Length > 0 ? " " : string.Empty)
                .Append(LLiveryXiesheng.LLiveryCharacterFormat(character, entries, note));
        }

        return LLiveryRime.LLiveryRowFormat(
        [
            LLiveryHeader.LLiveryTextFormat(line.LDiweiLineReading),
            LLiveryHeader.LLiveryTextFormat(line.LDiweiLineLabel),
            line.LDiweiLineRounded ? "<span class=\"llyn-rounded\">合</span>" : string.Empty,
            characters.ToString(),
        ]);
    }

    private static string LLiveryTallyFormat(LTallyRow tally)
    {
        StringBuilder marks = new StringBuilder();
        foreach (LTallyMark mark in tally.LTallyRowMarks)
        {
            marks.Append(marks.Length > 0 ? " " : string.Empty)
                .Append("<span class=\"llyn-mark\">").Append(LLiveryHeader.LLiveryTextFormat(mark.LTallyMarkText))
                .Append(" <span class=\"llyn-count\">")
                .Append(mark.LTallyMarkCount.ToString(CultureInfo.InvariantCulture)).Append("</span></span>");
        }

        return LLiveryRime.LLiveryRowFormat(
        [
            LLiveryRime.LLiveryChipFormat("llyn-language", tally.LTallyRowLanguage),
            LLiveryRime.LLiveryChipFormat("llyn-label", tally.LTallyRowKind),
            string.Empty,
            marks.ToString(),
        ]);
    }
}
