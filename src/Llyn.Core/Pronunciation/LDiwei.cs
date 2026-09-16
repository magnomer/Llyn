using System;

namespace Llyn.Core;

public sealed record LDiwei(
    long LDiweiId,
    string LDiweiLanguage,
    string LDiweiKind,
    string LDiweiKey,
    int LDiweiCount = 0)
{
    public const string LDiweiInitial = "initial";

    public const string LDiweiRime = "rime";

    public const string LDiweiTone = "tone";

    private const string LDiweiRounded = "W";

    private static readonly string[] LDiweiDivisions = ["一", "二", "三", "四"];

    private static readonly string[] LDiweiRomans = ["I", "II", "III", "IV"];

    public static string LDiweiRimeNormalize(string rime)
    {
        ArgumentNullException.ThrowIfNull(rime);

        return rime.Length > 1 && char.IsAsciiLetterUpper(rime[^1]) ? rime[..^1] : rime;
    }

    public static string LDiweiRimeFormat(string rime, string division, bool rounded)
    {
        ArgumentNullException.ThrowIfNull(division);

        string key = LDiweiRimeNormalize(rime);
        if (key.Length == 0)
        {
            return key;
        }

        if (division.Length > 0)
        {
            int index = Array.IndexOf(LDiweiDivisions, division);
            key += ' ' + (index >= 0 ? LDiweiRomans[index] : division);
        }

        return rounded ? key + ' ' + LDiweiRounded : key;
    }
}
