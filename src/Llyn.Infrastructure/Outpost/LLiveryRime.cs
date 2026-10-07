using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiveryRime
{
    private const int LLiveryRimeColumns = 12;

    public static void LLiveryRimeAppend(
        StringBuilder sheet, LLiveryPage page, Func<string, string, string, string> link, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(link);
        ArgumentNullException.ThrowIfNull(lookup);

        string language = page.LLiveryPageDraft.LEntryDraftLanguage.Trim();
        LLiveryParadigmAppend(sheet, page.LLiveryPageParadigm);
        LLiveryFanqieAppend(sheet, page.LLiveryPageFanqie, (chip, kind, key) =>
        {
            string id = chip.Length > 0 && key.Length > 0 ? link(language, kind, key) : string.Empty;
            return id.Length > 0 ? "[" + chip + "](:/" + id + ")" : chip;
        }, lookup);
    }

    private static void LLiveryParadigmAppend(StringBuilder sheet, IReadOnlyList<LParadigmRow> rows)
    {
        if (rows.Count == 0)
        {
            return;
        }

        sheet.Append("<div class=\"llyn-paradigm\">\n\n|  |  |\n|---|---|\n");
        foreach (LParadigmRow row in rows)
        {
            if (row.LParadigmRowPart.Length > 0)
            {
                sheet.Append(LLiveryRowFormat([LLiveryChipFormat("llyn-speech", row.LParadigmRowPart), string.Empty]));
            }

            string form = row.LParadigmRowFirst.LParadigmSlotInflection?.LInflectionText ?? string.Empty;
            sheet.Append(LLiveryRowFormat(
                [LLiveryChipFormat("llyn-label", row.LParadigmRowName), LLiveryHeader.LLiveryTextFormat(form)]));
        }

        sheet.Append("\n</div>\n\n");
    }

    private static void LLiveryFanqieAppend(
        StringBuilder sheet,
        IReadOnlyList<LFanqieGroup> groups,
        Func<string, string, string, string> wrap,
        Func<string, string> lookup)
    {
        if (groups.Count == 0)
        {
            return;
        }

        string[] blank = new string[LLiveryRimeColumns];
        Array.Fill(blank, string.Empty);
        string[] rule = new string[LLiveryRimeColumns];
        Array.Fill(rule, "---");
        sheet.Append("<div class=\"llyn-card\">\n\n").Append(LLiveryRowFormat(blank)).Append(LLiveryRowFormat(rule));
        string tone = lookup("Display.FanqieTone");
        foreach (LFanqieGroup group in groups)
        {
            if (group.LFanqieGroupHeading.Length > 0)
            {
                string[] heading = (string[])blank.Clone();
                heading[0] = LLiveryChipFormat("llyn-heading", group.LFanqieGroupHeading);
                sheet.Append(LLiveryRowFormat(heading));
            }

            if (group.LFanqieGroupStems.Count > 0)
            {
                StringBuilder stems = new StringBuilder();
                foreach (string stem in group.LFanqieGroupStems)
                {
                    stems.Append(stems.Length > 0 ? " " : string.Empty)
                        .Append(wrap(LLiveryChipFormat("llyn-stem", stem), LLiveryStem.LLiveryStemKind, stem));
                }

                string[] series = (string[])blank.Clone();
                series[0] = LLiveryChipFormat("llyn-series", lookup("Display.FanqieShengfu"));
                series[1] = stems.ToString();
                sheet.Append(LLiveryRowFormat(series));
            }

            for (int place = 0; place < group.LFanqieGroupRows.Count; place++)
            {
                LFanqieRow row = group.LFanqieGroupRows[place];
                string label = row.LFanqieRowLabel.Length > 0
                    ? row.LFanqieRowLabel
                    : row.LFanqieRowClassed
                        ? string.Format(CultureInfo.CurrentCulture, tone, row.LFanqieRowClass)
                        : string.Empty;
                string rime = row.LFanqieRowCell.Length > 0
                    ? wrap(LLiveryChipFormat("llyn-rime", row.LFanqieRowCell), LDiwei.LDiweiRime, row.LFanqieRowCell)
                    : LLiveryHeader.LLiveryTextFormat(row.LFanqieRowRemainder);
                string medial = row.LFanqieRowClosed ? "llyn-medial llyn-on" : "llyn-medial";
                sheet.Append(LLiveryRowFormat(
                [
                    place == 0 ? LLiveryChipFormat("llyn-book", group.LFanqieGroupLabel) : string.Empty,
                    LLiveryHeader.LLiveryTextFormat(row.LFanqieRowSlashed),
                    wrap(LLiveryChipFormat("llyn-tone", label), LDiwei.LDiweiTone, row.LFanqieRowClass),
                    wrap(
                        LLiveryChipFormat("llyn-initial", row.LFanqieRowInitial),
                        LDiwei.LDiweiInitial,
                        row.LFanqieRowInitial),
                    rime,
                    LLiveryHeader.LLiveryTextFormat(row.LFanqieRowBracketed),
                    LLiveryHeader.LLiveryTextFormat(row.LFanqieRowKnotted),
                    LLiveryChipFormat(medial, row.LFanqieRowMedial),
                    LLiveryHeader.LLiveryTextFormat(row.LFanqieRowGraded),
                    LLiveryHeader.LLiveryTextFormat(row.LFanqieRowTone),
                    LLiveryChipFormat("llyn-spelling", row.LFanqieRowSpelling),
                    place == 0 ? LLiveryChipFormat("llyn-source", group.LFanqieGroupSource) : string.Empty,
                ]));
            }
        }

        sheet.Append("\n</div>\n\n");
    }

    internal static string LLiveryRowFormat(IReadOnlyList<string> cells)
    {
        StringBuilder row = new StringBuilder("|");
        foreach (string cell in cells)
        {
            row.Append(' ').Append(cell).Append(" |");
        }

        return row.Append('\n').ToString();
    }

    internal static string LLiveryChipFormat(string style, string text)
    {
        return string.IsNullOrEmpty(text)
            ? string.Empty
            : "<span class=\"" + style + "\">" + LLiveryHeader.LLiveryTextFormat(text) + "</span>";
    }
}
