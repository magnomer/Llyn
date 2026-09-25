using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed class LTheme
{
    private static readonly IReadOnlyDictionary<string, string> LThemeSpare =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["ink"] = "#172033",
            ["muted"] = "#66758C",
            ["canvas"] = "#F4F7FB",
            ["surface"] = "#FFFFFF",
            ["line"] = "#D7E0EC",
            ["accent"] = "#2F6FED",
            ["accentSoft"] = "#E7EFFD",
            ["situation"] = "#9A5B08",
            ["situationSoft"] = "#FCF0DC",
            ["situationEdge"] = "#EFD7AE",
        };

    private readonly IReadOnlyDictionary<string, string> _lThemeColor;

    public LTheme(IReadOnlyDictionary<string, string> colors)
    {
        ArgumentNullException.ThrowIfNull(colors);

        _lThemeColor = colors;
    }

    public string LThemeFamily => "\"Segoe UI Variable\", \"Segoe UI\", system-ui, sans-serif";

    public string LThemeSerif => "Georgia, \"Segoe UI\", serif";

    public string LThemeColorRead(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _lThemeColor.TryGetValue(name, out string? found)
            ? found
            : throw new KeyNotFoundException($"The theme color '{name}' is missing.");
    }

    public string LThemeRead(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_lThemeColor.TryGetValue(name, out string? found))
        {
            return found;
        }

        return LThemeSpare.TryGetValue(name, out string? spare) ? spare : "#000000";
    }
}
