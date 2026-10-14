using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Llyn.Core;

public sealed record LRespelling(
    IReadOnlyList<string> LRespellingVarieties,
    IReadOnlyList<LRespellingRule> LRespellingRules)
{
    public string LRespellingResolve(string phonetic, string variety)
    {
        if (!LRespellingVarietyCheck(variety))
        {
            return phonetic;
        }

        string result = phonetic.Normalize(NormalizationForm.FormC);
        foreach (LRespellingRule rule in LRespellingRules)
        {
            result = rule.LRespellingRuleResolve(result);
        }

        return result;
    }

    public static string LRespellingScan(IReadOnlyList<LRespelling> groups, string phonetic, string variety)
    {
        string result = phonetic;
        foreach (LRespelling group in groups)
        {
            result = group.LRespellingResolve(result, variety);
        }

        return result;
    }

    public static bool LRespellingStaleCheck(
        IReadOnlyList<LRespelling> groups, string phonetic, string respelling, string variety)
    {
        string derived = LRespellingScan(groups, phonetic, variety);
        return respelling.Length > 0
            && !string.Equals(respelling, derived, StringComparison.Ordinal)
            && string.Equals(LRespellingScan(groups, respelling, variety), derived, StringComparison.Ordinal);
    }

    private bool LRespellingVarietyCheck(string variety) =>
        LRespellingVarieties.Count == 0 ||
        LRespellingVarieties.Any(name => string.Equals(name, variety, StringComparison.Ordinal));
}
