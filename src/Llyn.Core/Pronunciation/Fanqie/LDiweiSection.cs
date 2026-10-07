using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Llyn.Core;

public sealed record LDiweiSection(
    string LDiweiSectionLabel,
    IReadOnlyList<LDiweiLine> LDiweiSectionLines,
    IReadOnlyList<LTallyRow> LDiweiSectionTallies,
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
        IEnumerable<LFanqieRow> ordered = rows
            .OrderBy(row => row.LFanqieRowCharacter, LGlyphOrder.LGlyphOrderComparer)
            .ThenBy(row => row.LFanqieRowId);
        foreach (LFanqieRow row in ordered)
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

        IReadOnlyList<string> headings = sections.Keys
            .OrderBy(heading => LDiwei.LDiweiRankRead(kind, heading, hypothesis))
            .ThenBy(static heading => heading, LGlyphOrder.LGlyphOrderComparer)
            .ThenBy(static heading => heading, StringComparer.Ordinal)
            .ToList();
        List<LDiweiSection> built = new(headings.Count);
        foreach (string heading in headings)
        {
            built.Add(new LDiweiSection(
                rime ? LDiweiPlaceFormat(heading, localize) : LDiweiLabelFormat(heading, localize),
                LDiweiLineSort(sections[heading]),
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

    private static IReadOnlyList<LDiweiLine> LDiweiLineSort(List<LDiweiLine> lines)
    {
        return lines
            .OrderBy(static line => LDiwei.LDiweiRankNormalize(line.LDiweiLineRank))
            .ThenBy(static line => line.LDiweiLineLabel, LGlyphOrder.LGlyphOrderComparer)
            .ThenBy(static line => line.LDiweiLineRounded)
            .ThenBy(
                static line => line.LDiweiLineCharacters.FirstOrDefault() ?? string.Empty,
                LGlyphOrder.LGlyphOrderComparer)
            .ToList();
    }

    private static IReadOnlyList<LTallyRow> LDiweiTallyScan(
        IReadOnlyList<LTally> tallies, string heading, bool respelled)
    {
        List<LTallyRow> kept = [];
        foreach (LTally tally in tallies)
        {
            if (!string.Equals(tally.LTallyHeading, heading, StringComparison.Ordinal))
            {
                continue;
            }

            foreach (LTallyLine line in tally.LTallyLines)
            {
                IReadOnlyList<LTallyMark> marks = line.LTallyLineRead(respelled);
                if (marks.Count > 0)
                {
                    kept.Add(new LTallyRow(line.LTallyLineLanguage, line.LTallyLineKind, marks));
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
