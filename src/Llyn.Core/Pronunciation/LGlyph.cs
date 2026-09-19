using System;
using System.Collections.Generic;
using System.Text;

namespace Llyn.Core;

public sealed record LGlyph(
    string LGlyphName,
    string LGlyphLanguage,
    IReadOnlyList<LSourceSpec> LGlyphSources,
    LFont? LGlyphFont = null)
{
    public static IReadOnlyList<string> LGlyphScan(string text)
    {
        List<string> characters = [];
        HashSet<string> seen = [];
        foreach (Rune rune in (text ?? string.Empty).EnumerateRunes())
        {
            if (!LGlyphHanCheck(rune.Value))
            {
                continue;
            }

            string character = rune.ToString();
            if (seen.Add(character))
            {
                characters.Add(character);
            }
        }

        return characters;
    }

    public static bool LGlyphSingleCheck(string text)
    {
        return LGlyphScan(text).Count == 1;
    }

    public bool LGlyphSourced => LGlyphSources.Count > 0;

    public IReadOnlyList<LSourceSpec>? LGlyphSourceRead(string scheme)
    {
        return string.Equals(LGlyphName, scheme, StringComparison.Ordinal) ? LGlyphSources : null;
    }

    private static bool LGlyphHanCheck(int value)
    {
        return value is (>= 0x4E00 and <= 0x9FFF)
            or (>= 0x3400 and <= 0x4DBF)
            or (>= 0xF900 and <= 0xFAFF)
            or (>= 0x20000 and <= 0x3134F)
            or (>= 0x2F800 and <= 0x2FA1F);
    }
}
