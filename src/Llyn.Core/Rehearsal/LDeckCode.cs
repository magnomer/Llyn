using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Llyn.Core;

public static class LDeckCode
{
    public static string LDeckCodeResolve(string normalized, IReadOnlyCollection<string> taken)
    {
        ArgumentNullException.ThrowIfNull(normalized);
        ArgumentNullException.ThrowIfNull(taken);

        string code = normalized;
        for (int suffix = 2; !LDeckCodeCheck(code, taken); suffix++)
        {
            code = normalized + " " + suffix.ToString(CultureInfo.InvariantCulture);
        }

        return code;
    }

    public static bool LDeckCodeCheck(string code, IReadOnlyCollection<string> taken)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(taken);

        return !taken.Contains(code, StringComparer.OrdinalIgnoreCase);
    }
}
