using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LHypothesis(
    IReadOnlyDictionary<string, string> LHypothesisInitials,
    IReadOnlyDictionary<string, string> LHypothesisFinals,
    IReadOnlyDictionary<string, IReadOnlyList<LHypothesisTone>> LHypothesisTones,
    IReadOnlyList<LHypothesisLocus>? LHypothesisPlaces = null)
{
    private const string LHypothesisRounded = "合";

    public IReadOnlyList<LHypothesisLocus> LHypothesisPlaces { get; init; } = LHypothesisPlaces ?? [];

    public LHypothesisSound? LHypothesisResolve(LFanqieRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        if (row.LFanqieRowInitial.Length == 0
            || !LHypothesisInitials.TryGetValue(row.LFanqieRowInitial, out string? initial))
        {
            return null;
        }

        string? final = LHypothesisFinalFind(row);
        if (final is null)
        {
            return null;
        }

        string syllable = initial + final;
        if (LHypothesisTones.TryGetValue(row.LFanqieRowTone, out IReadOnlyList<LHypothesisTone>? tones))
        {
            foreach (LHypothesisTone tone in tones)
            {
                if (tone.LHypothesisToneMatch(initial))
                {
                    return new LHypothesisSound(tone.LHypothesisToneResolve(syllable), tone.LHypothesisToneClass);
                }
            }
        }

        return new LHypothesisSound(syllable, string.Empty);
    }

    public string? LHypothesisInitialFind(LFanqieRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        return row.LFanqieRowInitial.Length > 0
            && LHypothesisInitials.TryGetValue(row.LFanqieRowInitial, out string? initial)
            ? initial
            : null;
    }

    public LHypothesisLocus? LHypothesisLocusFind(string initial)
    {
        ArgumentNullException.ThrowIfNull(initial);

        foreach (LHypothesisLocus place in LHypothesisPlaces)
        {
            if (place.LHypothesisLocusInitials.Contains(initial))
            {
                return place;
            }
        }

        return null;
    }

    public int LHypothesisRankRead(string initial)
    {
        ArgumentNullException.ThrowIfNull(initial);

        int rank = 0;
        foreach (LHypothesisLocus place in LHypothesisPlaces)
        {
            foreach (string held in place.LHypothesisLocusInitials)
            {
                if (string.Equals(held, initial, StringComparison.Ordinal))
                {
                    return rank;
                }

                rank++;
            }
        }

        return -1;
    }

    public string? LHypothesisFinalFind(LFanqieRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        if (row.LFanqieRowRime.Length == 0)
        {
            return null;
        }

        foreach (string rime in LHypothesisRimeScan(row.LFanqieRowRime))
        {
            if (row.LFanqieRowRounded
                && LHypothesisFinals.TryGetValue(
                    LHypothesisKeyFormat(rime, row.LFanqieRowDivision, true), out string? rounded))
            {
                return rounded;
            }

            if (LHypothesisFinals.TryGetValue(
                LHypothesisKeyFormat(rime, row.LFanqieRowDivision, false), out string? plain))
            {
                return plain;
            }
        }

        return null;
    }

    private static IEnumerable<string> LHypothesisRimeScan(string rime)
    {
        yield return rime;
        if (rime.Length > 1 && char.IsAsciiLetterUpper(rime[^1]))
        {
            yield return rime[..^1];
        }
    }

    private static string LHypothesisKeyFormat(string rime, string division, bool rounded)
    {
        string key = division.Length == 0 ? rime : rime + ' ' + division;
        return rounded ? key + ' ' + LHypothesisRounded : key;
    }
}
