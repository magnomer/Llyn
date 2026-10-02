using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LGlyphLoader
{
    private const string LGlyphKey = "glyph";

    public static LGlyph? LGlyphPackRead(JsonElement root, IReadOnlyList<LRespellingRule> spelling)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LGlyphKey, out JsonElement glyph) ||
            glyph.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LPack.LPackTextRead(glyph, "name")?.Trim() ?? string.Empty;
        string language = LPack.LPackTextRead(glyph, "language")?.Trim() ?? string.Empty;
        if (name.Length == 0 || language.Length == 0)
        {
            return null;
        }

        LFont font = LFontLoader.LFontPackRead(glyph, LFontLoader.LFontKey);
        return new LGlyph(
            name,
            language,
            LSourceLoader.LSourceSpecScan(glyph, LSourceLoader.LSourceSources, spelling),
            font.LFontFamily is null && font.LFontSize <= 0 ? null : font);
    }
}
