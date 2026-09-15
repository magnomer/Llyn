using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private static readonly TimeSpan LEngineEpithetPatience = TimeSpan.FromSeconds(1);

    public string LEngineEpithetRead(long entryId)
    {
        lock (_lEngineGate)
        {
            if (!_lEngineSettings.LSettingsEpithet)
            {
                return string.Empty;
            }

            LEntry? entry = new LEntryArchive(_lEngineDatabase).LEntryRead(entryId);
            if (entry is null)
            {
                return string.Empty;
            }

            IReadOnlyList<LReflexRule> rules = LEngineReflexRead(entry.LEntryLanguage);
            if (!LEngineEpithetCheck(rules))
            {
                return string.Empty;
            }

            return LEngineEpithetFormat(rules, new LReflexArchive(_lEngineDatabase).LReflexRead(entryId));
        }
    }

    private static bool LEngineEpithetCheck(IReadOnlyList<LReflexRule> rules)
    {
        foreach (LReflexRule rule in rules)
        {
            if (!string.IsNullOrWhiteSpace(rule.LReflexRuleEpithet))
            {
                return true;
            }
        }

        return false;
    }

    private static string LEngineEpithetFormat(IReadOnlyList<LReflexRule> rules, IReadOnlyList<LReflex> rows)
    {
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

                string piece = LEngineEpithetFormat(rule, row);
                if (piece.Length > 0)
                {
                    pieces.Add(piece);
                }
            }
        }

        return string.Join(", ", pieces);
    }

    private static string LEngineEpithetFormat(LReflexRule rule, LReflex row)
    {
        string piece = rule.LReflexRuleEpithet!
            .Replace("{" + LReflexRule.LReflexRuleText + "}", row.LReflexText, StringComparison.Ordinal)
            .Replace("{" + LReflexRule.LReflexRuleKind + "}", row.LReflexKind, StringComparison.Ordinal)
            .Replace("{" + LReflexRule.LReflexRuleNote + "}", row.LReflexNote, StringComparison.Ordinal);

        if (!string.IsNullOrEmpty(rule.LReflexRuleClip))
        {
            try
            {
                piece = Regex.Replace(
                    piece, rule.LReflexRuleClip, string.Empty, RegexOptions.CultureInvariant, LEngineEpithetPatience);
            }
            catch (Exception exception) when (exception is ArgumentException or RegexMatchTimeoutException)
            {
            }
        }

        return string.Join(' ', piece.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }
}
