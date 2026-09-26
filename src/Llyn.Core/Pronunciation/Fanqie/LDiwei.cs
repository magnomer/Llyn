using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LDiwei(
    long LDiweiId,
    string LDiweiLanguage,
    string LDiweiKind,
    string LDiweiKey,
    int LDiweiCount = 0,
    bool LDiweiChosen = false)
{
    public bool LDiweiFinal => string.Equals(LDiweiKind, LDiweiRime, StringComparison.Ordinal);

    public bool LDiweiMatch(long id)
    {
        return LDiweiId == id;
    }

    public static LDiwei? LDiweiFind(IReadOnlyList<LDiwei> rows, long id)
    {
        ArgumentNullException.ThrowIfNull(rows);

        foreach (LDiwei row in rows)
        {
            if (row.LDiweiMatch(id))
            {
                return row;
            }
        }

        return null;
    }

    public const string LDiweiInitial = "initial";

    public const string LDiweiRime = "rime";

    public const string LDiweiTone = "tone";

    private const string LDiweiRounded = "W";

    private static readonly string[] LDiweiDivisions = ["一", "二", "三", "四"];

    private static readonly string[] LDiweiRomans = ["I", "II", "III", "IV"];

    public static string LDiweiChongniuRead(string rime)
    {
        ArgumentNullException.ThrowIfNull(rime);

        return rime.Length > 1 && char.IsAsciiLetterUpper(rime[^1]) ? rime[^1..] : string.Empty;
    }

    public static string LDiweiRimeNormalize(string rime)
    {
        ArgumentNullException.ThrowIfNull(rime);

        return rime[..(rime.Length - LDiweiChongniuRead(rime).Length)];
    }

    public static string LDiweiRimeFormat(string rime, string division, bool rounded)
    {
        ArgumentNullException.ThrowIfNull(division);

        string key = LDiweiRimeNormalize(rime);
        if (key.Length == 0)
        {
            return key;
        }

        if (rounded)
        {
            key += LDiweiRounded;
        }

        return division.Length == 0 ? key : key + ' ' + LDiweiDivisionFormat(division);
    }

    public static string LDiweiDivisionFormat(string division)
    {
        ArgumentNullException.ThrowIfNull(division);

        int index = Array.IndexOf(LDiweiDivisions, division);
        return index >= 0 ? LDiweiRomans[index] : division;
    }

    public static int LDiweiRankRead(string kind, string heading, LHypothesis? hypothesis)
    {
        ArgumentNullException.ThrowIfNull(heading);

        if (heading.Length == 0)
        {
            return int.MaxValue;
        }

        if (kind != LDiweiRime)
        {
            int index = Array.IndexOf(LDiweiDivisions, heading);
            return index >= 0 ? index : LDiweiDivisions.Length;
        }

        IReadOnlyList<LHypothesisLocus> places = hypothesis?.LHypothesisPlaces ?? [];
        for (int index = 0; index < places.Count; index++)
        {
            if (string.Equals(places[index].LHypothesisLocusName, heading, StringComparison.Ordinal))
            {
                return index;
            }
        }

        return places.Count;
    }

    public static int LDiweiRankNormalize(int rank) => rank < 0 ? int.MaxValue : rank;
}
