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

    public static string LDiweiRimeNormalize(string rime)
    {
        ArgumentNullException.ThrowIfNull(rime);

        return rime.Length > 1 && char.IsAsciiLetterUpper(rime[^1]) ? rime[..^1] : rime;
    }
}
