using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Llyn.Core;

namespace Llyn.Application;

public static class LReflexClerkEpithet
{
    private static readonly TimeSpan LReflexEpithetPatience = TimeSpan.FromSeconds(1);

    public static bool LReflexEpithetCheck(IReadOnlyList<LReflexRule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        foreach (LReflexRule rule in rules)
        {
            if (!string.IsNullOrWhiteSpace(rule.LReflexRuleEpithet))
            {
                return true;
            }
        }

        return false;
    }

    public static string LReflexEpithetFormat(IReadOnlyList<LReflexRule> rules, IReadOnlyList<LReflex> rows)
    {
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(rows);

        List<string> pieces = [];
        foreach (LReflexRule rule in rules)
        {
            if (string.IsNullOrWhiteSpace(rule.LReflexRuleEpithet))
            {
                continue;
            }

            foreach (LReflex row in rows)
            {
                if (!string.Equals(row.LReflexLanguage, rule.LReflexRuleLanguage, StringComparison.Ordinal))
                {
                    continue;
                }

                string piece = LReflexEpithetFormat(rule, row);
                if (piece.Length > 0)
                {
                    pieces.Add(piece);
                }
            }
        }

        return string.Join(", ", pieces);
    }

    private static string LReflexEpithetFormat(LReflexRule rule, LReflex row)
    {
        string piece = rule.LReflexRuleEpithet!
            .Replace("{" + LReflexRule.LReflexRuleText + "}", row.LReflexText, StringComparison.Ordinal)
            .Replace("{" + LReflexRule.LReflexRuleKind + "}", row.LReflexKind, StringComparison.Ordinal)
            .Replace(
                "{" + LReflexRule.LReflexRuleRomanization + "}", row.LReflexRomanization, StringComparison.Ordinal)
            .Replace("{" + LReflexRule.LReflexRuleMeaning + "}", row.LReflexMeaning, StringComparison.Ordinal)
            .Replace("{" + LReflexRule.LReflexRuleNote + "}", row.LReflexNote, StringComparison.Ordinal);

        if (!string.IsNullOrEmpty(rule.LReflexRuleClip))
        {
            try
            {
                piece = Regex.Replace(
                    piece, rule.LReflexRuleClip, string.Empty, RegexOptions.CultureInvariant, LReflexEpithetPatience);
            }
            catch (Exception exception) when (exception is ArgumentException or RegexMatchTimeoutException)
            {
            }
        }

        return string.Join(' ', piece.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }
}
