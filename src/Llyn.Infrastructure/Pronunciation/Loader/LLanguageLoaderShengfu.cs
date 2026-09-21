using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed partial class LLanguageLoader : LLanguageVault
{
    private const string LLanguageLoaderShengfu = "shengfu";

    private static LShengfuRule? LLanguageShengfuRead(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderShengfu, out JsonElement block) ||
            block.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string url = LLanguageTextRead(block, "url")?.Trim() ?? string.Empty;
        string pattern = LLanguageTextRead(block, "match") ?? string.Empty;
        if (url.Length == 0 || pattern.Length == 0)
        {
            return null;
        }

        string separator = LLanguageTextRead(block, "separator") ?? "·";
        return new LShengfuRule(
            url,
            pattern,
            LLanguageFormRead(block),
            LLanguageTextRead(block, "source")?.Trim() ?? string.Empty,
            LLanguageTextRead(block, "busy"),
            LLanguageNumberRead(block, "interval"),
            separator);
    }
}
