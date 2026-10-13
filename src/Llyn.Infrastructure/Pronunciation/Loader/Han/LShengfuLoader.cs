using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LShengfuLoader
{
    private const string LShengfuKey = "shengfu";

    public static LShengfuRule? LShengfuPackRead(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LShengfuKey, out JsonElement block) ||
            block.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string url = LPack.LPackTextRead(block, "url")?.Trim() ?? string.Empty;
        string pattern = LPack.LPackTextRead(block, "match") ?? string.Empty;
        if (url.Length == 0 || pattern.Length == 0)
        {
            return null;
        }

        string separator = LPack.LPackTextRead(block, "separator") ?? "·";
        return new LShengfuRule(
            url,
            pattern,
            LEnvelope.LEnvelopePayloadRead(block),
            LPack.LPackTextRead(block, "source")?.Trim() ?? string.Empty,
            LPack.LPackTextRead(block, "busy"),
            LPack.LPackNumberRead(block, "interval"),
            separator);
    }
}
