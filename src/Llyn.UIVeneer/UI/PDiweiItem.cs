using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PDiweiItem
{
    private const string PDiweiItemKey = "Yunjing.Division";

    private const string PDiweiItemBlank = "Yunjing.DivisionNone";

    private const string PDiweiItemPrefix = "Yunjing.Place";

    private const string PDiweiItemUnplaced = "Yunjing.PlaceNone";

    private readonly List<PDiweiLine> _pDiweiItemLines = [];

    private PDiweiItem(bool rime, string heading, IReadOnlyList<PTally> tallies, bool switched, bool respelled)
    {
        PDiweiItemHeading = heading;
        PDiweiItemLabel = rime ? PDiweiPlaceFormat(heading) : PDiweiLabelFormat(heading);
        PDiweiItemTallies = tallies;
        PDiweiItemSwitched = switched;
        PDiweiItemRespelled = respelled;
    }

    public string PDiweiItemHeading { get; }

    public string PDiweiItemLabel { get; }

    public IReadOnlyList<PDiweiLine> PDiweiItemLines => _pDiweiItemLines;

    public IReadOnlyList<PTally> PDiweiItemTallies { get; }

    public bool PDiweiItemSwitched { get; }

    public bool PDiweiItemRespelled { get; }

    internal static IReadOnlyList<PDiweiItem> PDiweiItemScan(
        string kind,
        bool rime,
        IReadOnlyList<LFanqieRow> rows,
        LHypothesis? hypothesis,
        IReadOnlyList<LTally> tallies,
        bool switched,
        bool respelled)
    {
        ArgumentNullException.ThrowIfNull(kind);
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(tallies);

        Dictionary<string, PDiweiItem> sections = new(StringComparer.Ordinal);
        Dictionary<(string, string, bool), PDiweiLine> lines = [];
        foreach (LFanqieRow row in rows)
        {
            string heading = rime
                ? hypothesis?.LHypothesisPlaceFind(row.LFanqieRowInitial)?.LHypothesisPlaceName ?? string.Empty
                : row.LFanqieRowDivision;
            if (!sections.TryGetValue(heading, out PDiweiItem? section))
            {
                section = new PDiweiItem(
                    rime,
                    heading,
                    PTally.PTallyScan(
                        tallies.FirstOrDefault(tally => tally.LTallyHeading == heading), respelled),
                    switched,
                    respelled);
                sections[heading] = section;
            }

            (string, string, bool) key = rime
                ? (heading, row.LFanqieRowInitial, false)
                : (heading, LDiwei.LDiweiRimeNormalize(row.LFanqieRowRime), row.LFanqieRowRounded);
            if (!lines.TryGetValue(key, out PDiweiLine? line))
            {
                line = new PDiweiLine(rime, row, hypothesis);
                lines[key] = line;
                section._pDiweiItemLines.Add(line);
            }

            line.PDiweiLineAdd(row.LFanqieRowCharacter);
        }

        List<PDiweiItem> items = [.. sections.Values];
        items.Sort((left, right) =>
            LDiwei.LDiweiRankRead(kind, left.PDiweiItemHeading, hypothesis)
                .CompareTo(LDiwei.LDiweiRankRead(kind, right.PDiweiItemHeading, hypothesis)));
        foreach (PDiweiItem item in items)
        {
            PDiweiLine.PDiweiLineSort(item._pDiweiItemLines);
        }

        return items;
    }

    private static string PDiweiLabelFormat(string division)
    {
        PLocalizationCatalog catalog = PLocalizationCatalog.PLocalizationCatalogCurrent;
        if (division.Length == 0)
        {
            return catalog[PDiweiItemBlank];
        }

        string roman = LDiwei.LDiweiDivisionFormat(division);
        string pattern = catalog[PDiweiItemKey];
        return pattern.Length == 0 ? division : string.Format(CultureInfo.CurrentCulture, pattern, division, roman);
    }

    private static string PDiweiPlaceFormat(string place)
    {
        PLocalizationCatalog catalog = PLocalizationCatalog.PLocalizationCatalogCurrent;
        if (place.Length == 0)
        {
            return catalog[PDiweiItemUnplaced];
        }

        string label = catalog[PDiweiItemPrefix + char.ToUpperInvariant(place[0]) + place[1..]];
        return label.Length == 0 ? place : label;
    }
}
