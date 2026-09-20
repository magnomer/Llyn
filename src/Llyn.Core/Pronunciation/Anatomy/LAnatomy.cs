using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LAnatomy(
    string LAnatomyOnsetIpa = "",
    string LAnatomyVowelIpa = "",
    string LAnatomyCodaIpa = "",
    string LAnatomyToneIpa = "",
    string LAnatomyOnsetRespelling = "",
    string LAnatomyVowelRespelling = "",
    string LAnatomyCodaRespelling = "",
    string LAnatomyToneRespelling = "")
{
    public static readonly LAnatomy LAnatomyEmpty = new();

    public string LAnatomyOnsetIpa { get; init; } = LAnatomyOnsetIpa ?? string.Empty;

    public string LAnatomyVowelIpa { get; init; } = LAnatomyVowelIpa ?? string.Empty;

    public string LAnatomyCodaIpa { get; init; } = LAnatomyCodaIpa ?? string.Empty;

    public string LAnatomyToneIpa { get; init; } = LAnatomyToneIpa ?? string.Empty;

    public string LAnatomyOnsetRespelling { get; init; } = LAnatomyOnsetRespelling ?? string.Empty;

    public string LAnatomyVowelRespelling { get; init; } = LAnatomyVowelRespelling ?? string.Empty;

    public string LAnatomyCodaRespelling { get; init; } = LAnatomyCodaRespelling ?? string.Empty;

    public string LAnatomyToneRespelling { get; init; } = LAnatomyToneRespelling ?? string.Empty;

    public bool LAnatomyBlank =>
        LAnatomyOnsetIpa.Length == 0
        && LAnatomyVowelIpa.Length == 0
        && LAnatomyCodaIpa.Length == 0
        && LAnatomyToneIpa.Length == 0
        && LAnatomyOnsetRespelling.Length == 0
        && LAnatomyVowelRespelling.Length == 0
        && LAnatomyCodaRespelling.Length == 0
        && LAnatomyToneRespelling.Length == 0;

    public string LAnatomyIpaRead(string kind) =>
        LAnatomyPartRead(kind, LAnatomyOnsetIpa, LAnatomyVowelIpa, LAnatomyCodaIpa);

    public string LAnatomyRespellingRead(string kind) =>
        LAnatomyPartRead(kind, LAnatomyOnsetRespelling, LAnatomyVowelRespelling, LAnatomyCodaRespelling);

    private static string LAnatomyPartRead(string kind, string onset, string vowel, string coda)
    {
        return kind switch
        {
            LDiwei.LDiweiInitial => onset,
            LDiwei.LDiweiRime => vowel + coda,
            _ => string.Empty,
        };
    }

    public static LAnatomy LAnatomyCreate(LAnatomyPiece ipa, LAnatomyPiece respelling)
    {
        ArgumentNullException.ThrowIfNull(ipa);
        ArgumentNullException.ThrowIfNull(respelling);

        return new LAnatomy(
            ipa.LAnatomyPieceOnset,
            ipa.LAnatomyPieceVowel,
            ipa.LAnatomyPieceCoda,
            ipa.LAnatomyPieceTone,
            respelling.LAnatomyPieceOnset,
            respelling.LAnatomyPieceVowel,
            respelling.LAnatomyPieceCoda,
            respelling.LAnatomyPieceTone);
    }

    public static LAnatomy LAnatomyScan(
        IReadOnlyList<LAnatomyRule> rules, string language, string text, string respelling)
    {
        ArgumentNullException.ThrowIfNull(rules);

        string name = (language ?? string.Empty).Trim();
        foreach (LAnatomyRule rule in rules)
        {
            if (rule.LAnatomyRuleMatch(name))
            {
                return rule.LAnatomyRuleResolve(text ?? string.Empty, respelling ?? string.Empty);
            }
        }

        return LAnatomyEmpty;
    }
}
