using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LFontLoader
{
    public const string LFontKey = "font";
    public const string LFontExample = "example";
    public const string LFontGloss = "gloss";

    public static LFont LFontBlank => new(null, 0);

    public static LFont LFontPackRead(JsonElement root, string key)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(key, out JsonElement font) ||
            font.ValueKind != JsonValueKind.Object)
        {
            return LFontBlank;
        }

        string? family = LPack.LPackTextRead(font, "family");
        double size = LPack.LPackMeasureRead(font, "size");
        string? style = LPack.LPackTextRead(font, "style");

        return new LFont(
            string.IsNullOrWhiteSpace(family) ? null : family,
            size,
            string.IsNullOrWhiteSpace(style) ? null : style.Trim());
    }
}
