using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LParadigmView(
    LParadigmTable LParadigmViewCollapsed,
    LParadigmTable LParadigmViewExpanded)
{
    public static LParadigmView? LParadigmViewScan(
        LInflectionLayout layout,
        IReadOnlyList<LParadigmSlot> slots,
        bool pending,
        bool enabled,
        bool custom,
        LInflectionBook? book,
        string headword)
    {
        ArgumentNullException.ThrowIfNull(layout);
        ArgumentNullException.ThrowIfNull(slots);
        ArgumentNullException.ThrowIfNull(headword);

        List<LParadigmSlot> part = [.. slots.Where(
            slot => slot.LParadigmSlotParadigm.LParadigmSpeechCode == layout.LInflectionLayoutPart)];
        if (part.Count == 0)
        {
            return null;
        }

        LInflectionLayout chosen = custom ? layout.LInflectionLayoutCustom ?? layout : layout;
        return new LParadigmView(
            LParadigmViewResolve(chosen.LInflectionLayoutCollapsed, part, pending, enabled, custom, book, headword),
            LParadigmViewResolve(chosen.LInflectionLayoutExpanded, part, pending, enabled, custom, book, headword));
    }

    private static LParadigmTable LParadigmViewResolve(
        LInflectionSheet sheet,
        IReadOnlyList<LParadigmSlot> slots,
        bool pending,
        bool enabled,
        bool custom,
        LInflectionBook? book,
        string headword)
    {
        List<LParadigmLine> lines = [];
        foreach (LInflectionLine line in sheet.LInflectionSheetLines)
        {
            List<LParadigmForm> forms = [];
            foreach (IReadOnlyList<long> cell in line.LInflectionLineCells)
            {
                LParadigmSlot? slot = slots.FirstOrDefault(row => row.LParadigmSlotCodes.SequenceEqual(cell));
                if (slot is null)
                {
                    forms.Add(new LParadigmForm(string.Empty, [], LParadigmStatus.LParadigmStatusText));
                    continue;
                }

                LParadigmStatus status = slot.LParadigmSlotCheck(pending, enabled);
                if (slot.LParadigmSlotInflection is not LInflection inflection
                    || status != LParadigmStatus.LParadigmStatusText)
                {
                    forms.Add(new LParadigmForm(string.Empty, [], status));
                    continue;
                }

                string text = inflection.LInflectionText;
                if (!custom)
                {
                    forms.Add(new LParadigmForm(text, [], status));
                    continue;
                }

                IReadOnlyList<LInflectionMark> marks = inflection.LInflectionMarks ?? [];
                int split = 0;
                if (book?.LInflectionBookDivide(headword, slot.LParadigmSlotCodes, out int? root) is string prediction
                    && root is int boundary)
                {
                    int s = LInflectionDifference.LInflectionDifferenceDivide(
                        book.LInflectionBookFolds, prediction, boundary, text);
                    if (s > 0)
                    {
                        marks = LParadigmViewDivide(text, marks, s);
                        split = s < text.Length ? s : 0;
                    }
                }

                forms.Add(new LParadigmForm(text, marks, status, split));
            }

            lines.Add(new LParadigmLine(line.LInflectionLineGroup, line.LInflectionLineLabel, forms));
        }

        return new LParadigmTable(sheet.LInflectionSheetHeaders, lines);
    }

    private static IReadOnlyList<LInflectionMark> LParadigmViewDivide(
        string text, IReadOnlyList<LInflectionMark> marks, int split)
    {
        List<LInflectionMark> parts = [];
        if (marks.Any(mark => mark.LInflectionMarkOffset < split
            && mark.LInflectionMarkOffset + mark.LInflectionMarkLength > 0))
        {
            parts.Add(new LInflectionMark(0, split));
        }

        if (split < text.Length
            && marks.Any(mark => mark.LInflectionMarkOffset < text.Length
                && mark.LInflectionMarkOffset + mark.LInflectionMarkLength > split))
        {
            parts.Add(new LInflectionMark(split, text.Length - split));
        }

        return parts;
    }
}
