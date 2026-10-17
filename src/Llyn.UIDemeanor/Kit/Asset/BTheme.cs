using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Llyn.UIDemeanor;

internal sealed class BTheme
{
    internal const string BThemeDefault = "console-blue";

    private static readonly Dictionary<string, string> BThemeSpare = new(StringComparer.Ordinal)
    {
        ["text"] = "cyan on darkBlue",
        ["muted"] = "darkCyan on darkBlue",
        ["link"] = "white on darkBlue",
        ["helper"] = "darkCyan on darkBlue",
        ["situation"] = "darkYellow on darkBlue",
        ["warning"] = "red on darkBlue",
        ["favorite"] = "magenta on darkBlue",
        ["representative"] = "yellow on darkBlue",
        ["pending"] = "yellow on darkBlue",
        ["disabled"] = "darkGray on darkBlue",
        ["contour-1"] = "red on darkBlue",
        ["contour-2"] = "darkYellow on darkBlue",
        ["contour-3"] = "green on darkBlue",
        ["contour-4"] = "blue on darkBlue",
        ["contour-5"] = "magenta on darkBlue",
        ["frequency-1"] = "darkYellow on darkBlue",
        ["frequency-2"] = "green on darkBlue",
        ["frequency-3"] = "blue on darkBlue",
        ["frequency-4"] = "magenta on darkBlue",
        ["side"] = "cyan on darkBlue",
        ["frame"] = "cyan on darkBlue",
        ["title"] = "black on darkCyan",
        ["lit"] = "black on darkCyan",
        ["lit-other"] = "black on darkGray",
        ["roof"] = "black on darkCyan",
        ["menu"] = "black on darkCyan",
        ["menu-hotkey"] = "yellow on darkCyan",
        ["dialog"] = "black on gray",
        ["alert"] = "white on darkRed",
        ["field"] = "yellow on darkBlue",
        ["legend-number"] = "white on black",
        ["legend-label"] = "black on darkCyan",
        ["establishment"] = "cyan on darkBlue",
        ["shadow"] = "darkGray on black",
        ["cjk"] = "white on darkBlue",
        ["ipa"] = "white on darkBlue",
        ["script"] = "white on darkBlue",
        ["heading"] = "white on darkBlue",
        ["code"] = "green on darkBlue",
        ["emphasis"] = "white on darkBlue",
        ["headword"] = "yellow on darkBlue",
        ["chosen"] = "yellow on darkCyan",
        ["marked"] = "black on darkYellow",
        ["register"] = "darkCyan on darkBlue",
        ["phonetic"] = "white on darkBlue",
        ["warning-mark"] = "red on darkBlue",
        ["fetching"] = "darkGray on darkBlue",
        ["playing"] = "green on darkBlue",
        ["refused"] = "red on darkBlue",
    };

    private static readonly Dictionary<string, int> BThemeForeground = new(StringComparer.Ordinal)
    {
        ["black"] = 30,
        ["darkRed"] = 31,
        ["darkGreen"] = 32,
        ["darkYellow"] = 33,
        ["darkBlue"] = 34,
        ["darkMagenta"] = 35,
        ["darkCyan"] = 36,
        ["gray"] = 37,
        ["darkGray"] = 90,
        ["red"] = 91,
        ["green"] = 92,
        ["yellow"] = 93,
        ["blue"] = 94,
        ["magenta"] = 95,
        ["cyan"] = 96,
        ["white"] = 97,
    };

    private readonly Dictionary<string, string> _bThemeRoles = new(StringComparer.Ordinal);

    internal BTheme(Func<string, string> colorRead)
    {
        ArgumentNullException.ThrowIfNull(colorRead);

        string text = BThemeParse(colorRead("text"));
        foreach (string role in BThemeSpare.Keys)
        {
            string value;
            try
            {
                value = colorRead(role);
            }
            catch (KeyNotFoundException)
            {
                _bThemeRoles[role] = text;
                continue;
            }

            _bThemeRoles[role] = BThemeParse(value);
        }
    }

    internal static string BThemeSpareRead(string role)
    {
        ArgumentNullException.ThrowIfNull(role);

        return BThemeSpare.TryGetValue(role, out string? value)
            ? value
            : throw new KeyNotFoundException($"The console theme role '{role}' is missing.");
    }

    internal string BThemeRead(string role)
    {
        ArgumentNullException.ThrowIfNull(role);

        return _bThemeRoles.TryGetValue(role, out string? escape) ? escape : _bThemeRoles["text"];
    }

    private static string BThemeParse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string[] sides = value.Split(" on ", StringSplitOptions.TrimEntries);
        if (sides.Length != 2)
        {
            throw new FormatException($"The console theme value '{value}' is not 'colour on colour'.");
        }

        StringBuilder escape = new("\e[");
        StringBuilder exact = new();
        for (int side = 0; side < 2; side++)
        {
            string[] parts = sides[side].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length is < 1 or > 2 || !BThemeForeground.TryGetValue(parts[0], out int code))
            {
                throw new FormatException($"The console theme value '{value}' names an unknown colour.");
            }

            escape.Append(CultureInfo.InvariantCulture, $"{(side == 0 ? string.Empty : ";")}{code + (side * 10)}");
            if (parts.Length == 2)
            {
                if (parts[1] is not ['#', .. { Length: 6 } hex]
                    || !int.TryParse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out int rgb))
                {
                    throw new FormatException($"The console theme value '{value}' holds a colour that is not #RRGGBB.");
                }

                exact.Append(
                    CultureInfo.InvariantCulture,
                    $";{38 + (side * 10)};2;{(rgb >> 16) & 0xFF};{(rgb >> 8) & 0xFF};{rgb & 0xFF}");
            }
        }

        return escape.Append(exact).Append('m').ToString();
    }
}
