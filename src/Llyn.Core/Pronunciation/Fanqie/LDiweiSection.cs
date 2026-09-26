using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

public sealed record LDiweiSection(
    string LDiweiSectionLabel,
    IReadOnlyList<LDiweiLine> LDiweiSectionLines,
    IReadOnlyList<LTallyLine> LDiweiSectionTallies,
    bool LDiweiSectionSwitched,
    bool LDiweiSectionRespelled)
{
    private const string LDiweiSectionKey = "Yunjing.Division";

    private const string LDiweiSectionBlank = "Yunjing.DivisionNone";

    private const string LDiweiSectionPrefix = "Yunjing.Place";

    private const string LDiweiSectionUnplaced = "Yunjing.PlaceNone";

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
        Dictionary<string, List<LDiweiLine>> sections = new(StringComparer.Ordinal);
        Dictionary<(string, string, bool), List<string>> lines = [];
        foreach (LFanqieRow row in rows)
        {
            string heading = rime
                ? hypothesis?.LHypothesisLocusFind(row.LFanqieRowInitial)?.LHypothesisLocusName ?? string.Empty
                : row.LFanqieRowDivision;
            if (!sections.TryGetValue(heading, out List<LDiweiLine>? placed))
            {
                placed = [];
                sections[heading] = placed;
            }

            (string, string, bool) key = rime
                ? (heading, row.LFanqieRowInitial, false)
                : (heading, LDiwei.LDiweiRimeNormalize(row.LFanqieRowRime), row.LFanqieRowRounded);
            if (!lines.TryGetValue(key, out List<string>? characters))
            {
                characters = [];
                lines[key] = characters;
                placed.Add(LDiweiLineCreate(rime, row, hypothesis, characters));
            }

            LDiweiCharacterAdd(characters, row.LFanqieRowCharacter);
        }

        List<string> headings = [.. sections.Keys];
        headings.Sort((left, right) =>
            LDiwei.LDiweiRankRead(kind, left, hypothesis).CompareTo(LDiwei.LDiweiRankRead(kind, right, hypothesis)));
        List<LDiweiSection> built = new(headings.Count);
        foreach (string heading in headings)
        {
            List<LDiweiLine> placed = sections[heading];
            LDiweiLineSort(placed);
            built.Add(new LDiweiSection(
                rime ? LDiweiPlaceFormat(heading, localize) : LDiweiLabelFormat(heading, localize),
                placed,
                LDiweiTallyScan(tallies, heading, respelled),
                switched,
                respelled));
        }

        return built;
    }

    private static LDiweiLine LDiweiLineCreate(
        bool rime, LFanqieRow row, LHypothesis? hypothesis, List<string> characters)
    {
        string? part = rime ? hypothesis?.LHypothesisInitialFind(row) : hypothesis?.LHypothesisFinalFind(row);
        return new LDiweiLine(
            part is null ? string.Empty : '/' + part + '/',
            rime ? row.LFanqieRowInitial : LDiwei.LDiweiRimeNormalize(row.LFanqieRowRime),
            !rime && row.LFanqieRowRounded,
            rime ? hypothesis?.LHypothesisRankRead(row.LFanqieRowInitial) ?? -1 : -1,
            characters);
    }

    private static void LDiweiCharacterAdd(List<string> characters, string character)
    {
        if (character.Length > 0 && !characters.Contains(character))
        {
            characters.Add(character);
        }
    }

    private static void LDiweiLineSort(List<LDiweiLine> lines)
    {
        lines.Sort((left, right) =>
        {
            int order = LDiwei.LDiweiRankNormalize(left.LDiweiLineRank)
                .CompareTo(LDiwei.LDiweiRankNormalize(right.LDiweiLineRank));
            return order != 0 ? order : string.CompareOrdinal(left.LDiweiLineLabel, right.LDiweiLineLabel);
        });
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
