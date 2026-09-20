using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LAnatomyRule(
    IReadOnlyList<string> LAnatomyRuleLanguages,
    LAnatomyPattern LAnatomyRuleIpa,
    LAnatomyPattern LAnatomyRuleRespelling)
{
    public bool LAnatomyRuleMatch(string language)
    {
        foreach (string name in LAnatomyRuleLanguages)
        {
            if (string.Equals(name, language, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public LAnatomy LAnatomyRuleResolve(string text, string respelling)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(respelling);

        string spelled = respelling.Trim().Length == 0 ? text : respelling;
        return LAnatomy.LAnatomyCreate(
            LAnatomyRuleIpa.LAnatomyPatternResolve(text),
            LAnatomyRuleRespelling.LAnatomyPatternResolve(spelled));
    }
}
