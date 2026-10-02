using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LDescent(
    IReadOnlyList<string> LDescentLanguages,
    IReadOnlyDictionary<string, IReadOnlyList<string>> LDescentClasses)
{
    private const char LDescentJoiner = '-';

    public bool LDescentMatch(string language)
    {
        foreach (string name in LDescentLanguages)
        {
            if (string.Equals(name, language, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public IReadOnlyList<string> LDescentResolve(string tone)
    {
        string contour = (tone ?? string.Empty).Trim();
        int joiner = contour.IndexOf(LDescentJoiner);
        if (joiner >= 0)
        {
            contour = contour[..joiner].Trim();
        }

        return contour.Length > 0 && LDescentClasses.TryGetValue(contour, out IReadOnlyList<string>? classes)
            ? classes
            : [];
    }

    public static IReadOnlyList<string> LDescentScan(
        IReadOnlyList<LDescent> rules,
        string language,
        string tone)
    {
        ArgumentNullException.ThrowIfNull(rules);

        string name = (language ?? string.Empty).Trim();
        foreach (LDescent rule in rules)
        {
            if (rule.LDescentMatch(name))
            {
                return rule.LDescentResolve(tone);
            }
        }

        return [];
    }
}
