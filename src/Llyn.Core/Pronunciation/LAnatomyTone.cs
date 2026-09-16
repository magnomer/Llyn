using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LAnatomyTone(
    IReadOnlyList<string> LAnatomyToneLanguages,
    IReadOnlyDictionary<string, IReadOnlyList<string>> LAnatomyToneClasses)
{
    private const char LAnatomyToneJoiner = '-';

    public bool LAnatomyToneMatch(string language)
    {
        foreach (string name in LAnatomyToneLanguages)
        {
            if (string.Equals(name, language, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public IReadOnlyList<string> LAnatomyToneResolve(string tone)
    {
        string contour = (tone ?? string.Empty).Trim();
        int joiner = contour.IndexOf(LAnatomyToneJoiner);
        if (joiner >= 0)
        {
            contour = contour[..joiner].Trim();
        }

        return contour.Length > 0 && LAnatomyToneClasses.TryGetValue(contour, out IReadOnlyList<string>? classes)
            ? classes
            : [];
    }

    public static IReadOnlyList<string> LAnatomyToneScan(IReadOnlyList<LAnatomyTone> rules, string language, string tone)
    {
        ArgumentNullException.ThrowIfNull(rules);

        string name = (language ?? string.Empty).Trim();
        foreach (LAnatomyTone rule in rules)
        {
            if (rule.LAnatomyToneMatch(name))
            {
                return rule.LAnatomyToneResolve(tone);
            }
        }

        return [];
    }
}
