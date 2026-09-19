using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

public sealed class LDiweiSection
{
    private const string LDiweiSectionKey = "Yunjing.Division";

    private const string LDiweiSectionBlank = "Yunjing.DivisionNone";

    private const string LDiweiSectionPrefix = "Yunjing.Place";

    private const string LDiweiSectionUnplaced = "Yunjing.PlaceNone";

    private readonly List<LDiweiLine> _lDiweiSectionLines = [];

    private LDiweiSection(
        string heading, string label, IReadOnlyList<LTallyLine> tallies, bool switched, bool respelled)
    {
        LDiweiSectionHeading = heading;
        LDiweiSectionLabel = label;
        LDiweiSectionTallies = tallies;
        LDiweiSectionSwitched = switched;
        LDiweiSectionRespelled = respelled;
    }

    public string LDiweiSectionHeading { get; }

    public string LDiweiSectionLabel { get; }

    public IReadOnlyList<LDiweiLine> LDiweiSectionLines => _lDiweiSectionLines;

    public IReadOnlyList<LTallyLine> LDiweiSectionTallies { get; }

    public bool LDiweiSectionSwitched { get; }

    public bool LDiweiSectionRespelled { get; }

    public static IReadOnlyList<LDiweiSection> LDiweiSectionScan(
        string kind,
        IReadOnlyList<LFanqieRow> rows,
        LHypothesis? hypothesis,
        IReadOnlyList<LTally> tallies,
        bool switched,
        bool respelled,
        Func<string, string?> localize)
    {
        ArgumentNullException.ThrowIfNull(kind);
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(tallies);
        ArgumentNullException.ThrowIfNull(localize);

        bool rime = string.Equals(kind, LDiwei.LDiweiRime, StringComparison.Ordinal);
        Dictionary<string, LDiweiSection> sections = new(StringComparer.Ordinal);
        Dictionary<(string, string, bool), LDiweiLine> lines = [];
        foreach (LFanqieRow row in rows)
        {
            string heading = rime
                ? hypothesis?.LHypothesisPlaceFind(row.LFanqieRowInitial)?.LHypothesisPlaceName ?? string.Empty
                : row.LFanqieRowDivision;
            if (!sections.TryGetValue(heading, out LDiweiSection? section))
            {
                section = new LDiweiSection(
                    heading,
                    rime ? LDiweiPlaceFormat(heading, localize) : LDiweiLabelFormat(heading, localize),
                    LDiweiTallyScan(tallies, heading, respelled),
                    switched,
                    respelled);
                sections[heading] = section;
            }

            (string, string, bool) key = rime
                ? (heading, row.LFanqieRowInitial, false)
                : (heading, LDiwei.LDiweiRimeNormalize(row.LFanqieRowRime), row.LFanqieRowRounded);
            if (!lines.TryGetValue(key, out LDiweiLine? line))
            {
                line = new LDiweiLine(rime, row, hypothesis);
                lines[key] = line;
                section._lDiweiSectionLines.Add(line);
            }

            line.LDiweiLineAdd(row.LFanqieRowCharacter);
        }

        List<LDiweiSection> built = [.. sections.Values];
        built.Sort((left, right) =>
            LDiwei.LDiweiRankRead(kind, left.LDiweiSectionHeading, hypothesis)
                .CompareTo(LDiwei.LDiweiRankRead(kind, right.LDiweiSectionHeading, hypothesis)));
        foreach (LDiweiSection section in built)
        {
            LDiweiLine.LDiweiLineSort(section._lDiweiSectionLines);
        }

        return built;
    }

    private static IReadOnlyList<LTallyLine> LDiweiTallyScan(
        IReadOnlyList<LTally> tallies, string heading, bool respelled)
    {
        List<LTallyLine> kept = [];
        foreach (LTally tally in tallies)
        {
            if (!string.Equals(tally.LTallyHeading, heading, StringComparison.Ordinal))
            {
                continue;
            }

            foreach (LTallyLine line in tally.LTallyLines)
            {
                if (line.LTallyLineRead(respelled).Count > 0)
                {
                    kept.Add(line);
                }
            }
        }

        return kept;
    }

    private static string LDiweiLabelFormat(string division, Func<string, string?> localize)
    {
        if (division.Length == 0)
        {
            return localize(LDiweiSectionBlank) ?? string.Empty;
        }

        string? pattern = localize(LDiweiSectionKey);
        if (string.IsNullOrEmpty(pattern))
        {
            return division;
        }

        return string.Format(CultureInfo.CurrentCulture, pattern, division, LDiwei.LDiweiDivisionFormat(division));
    }

    private static string LDiweiPlaceFormat(string place, Func<string, string?> localize)
    {
        if (place.Length == 0)
        {
            return localize(LDiweiSectionUnplaced) ?? string.Empty;
        }

        string? label = localize(LDiweiSectionPrefix + char.ToUpperInvariant(place[0]) + place[1..]);
        return string.IsNullOrEmpty(label) ? place : label;
    }
}
