using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PDiweiItem
{
    private const string PDiweiItemKey = "Yunjing.Division";

    private const string PDiweiItemBlank = "Yunjing.DivisionNone";

    private static readonly string[] PDiweiItemOrder = ["一", "二", "三", "四"];

    private static readonly string[] PDiweiItemRoman = ["I", "II", "III", "IV"];

    private readonly List<PDiweiLine> _pDiweiItemLines = [];

    private PDiweiItem(string division, IReadOnlyList<PTally> tallies, bool switched, bool respelled)
    {
        PDiweiItemDivision = division;
        PDiweiItemLabel = PDiweiLabelFormat(division);
        PDiweiItemTallies = tallies;
        PDiweiItemSwitched = switched;
        PDiweiItemRespelled = respelled;
    }

    public string PDiweiItemDivision { get; }

    public string PDiweiItemLabel { get; }

    public IReadOnlyList<PDiweiLine> PDiweiItemLines => _pDiweiItemLines;

    public IReadOnlyList<PTally> PDiweiItemTallies { get; }

    public bool PDiweiItemSwitched { get; }

    public bool PDiweiItemRespelled { get; }

    internal static IReadOnlyList<PDiweiItem> PDiweiItemScan(
        IReadOnlyList<LFanqieRow> rows,
        LHypothesis? hypothesis,
        IReadOnlyList<LTally> tallies,
        bool switched,
        bool respelled)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(tallies);

        Dictionary<string, PDiweiItem> sections = new(StringComparer.Ordinal);
        Dictionary<(string, string, bool), PDiweiLine> lines = [];
        foreach (LFanqieRow row in rows)
        {
            string division = row.LFanqieRowDivision;
            if (!sections.TryGetValue(division, out PDiweiItem? section))
            {
                section = new PDiweiItem(
                    division,
                    PTally.PTallyScan(
                        tallies.FirstOrDefault(tally => tally.LTallyDivision == division), respelled),
                    switched,
                    respelled);
                sections[division] = section;
            }

            (string, string, bool) key =
                (division, LDiwei.LDiweiRimeNormalize(row.LFanqieRowRime), row.LFanqieRowRounded);
            if (!lines.TryGetValue(key, out PDiweiLine? line))
            {
                line = new PDiweiLine(row, hypothesis);
                lines[key] = line;
                section._pDiweiItemLines.Add(line);
            }

            line.PDiweiLineAdd(row.LFanqieRowCharacter);
        }

        List<PDiweiItem> items = [.. sections.Values];
        items.Sort((left, right) =>
            PDiweiRankRead(left.PDiweiItemDivision).CompareTo(PDiweiRankRead(right.PDiweiItemDivision)));
        foreach (PDiweiItem item in items)
        {
            item._pDiweiItemLines.Sort((left, right) =>
                string.CompareOrdinal(left.PDiweiLineRime, right.PDiweiLineRime));
        }

        return items;
    }

    private static int PDiweiRankRead(string division)
    {
        int index = Array.IndexOf(PDiweiItemOrder, division);
        return index >= 0 ? index : division.Length == 0 ? int.MaxValue : PDiweiItemOrder.Length;
    }

    private static string PDiweiLabelFormat(string division)
    {
        PLocalizationCatalog catalog = PLocalizationCatalog.PLocalizationCatalogCurrent;
        if (division.Length == 0)
        {
            return catalog[PDiweiItemBlank];
        }

        int index = Array.IndexOf(PDiweiItemOrder, division);
        string roman = index >= 0 ? PDiweiItemRoman[index] : division;
        string pattern = catalog[PDiweiItemKey];
        return pattern.Length == 0 ? division : string.Format(CultureInfo.CurrentCulture, pattern, division, roman);
    }
}
