using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LSchemeLoader
{
    private const string LSchemeKey = "transcription";

    public static IReadOnlyList<LScheme> LSchemePackScan(
        JsonElement root, IReadOnlyList<LRespellingRule> spelling)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LSchemeKey, out JsonElement schemes) ||
            schemes.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LScheme>();
        }

        List<LScheme> declared = new();
        foreach (JsonElement scheme in schemes.EnumerateArray())
        {
            LScheme? read = LSchemeRowRead(scheme, spelling);
            if (read is not null && declared.All(known =>
                    !string.Equals(known.LSchemeName, read.LSchemeName, StringComparison.Ordinal)))
            {
                declared.Add(read);
            }
        }

        return declared;
    }

    private static LScheme? LSchemeRowRead(JsonElement scheme, IReadOnlyList<LRespellingRule> spelling)
    {
        if (scheme.ValueKind == JsonValueKind.String)
        {
            string bare = scheme.GetString()!.Trim();
            return bare.Length == 0 ? null : new LScheme(bare, Array.Empty<LSourceSpec>());
        }

        if (scheme.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LPack.LPackTextRead(scheme, "name")?.Trim() ?? string.Empty;
        return name.Length == 0
            ? null
            : new LScheme(
                name,
                LSourceLoader.LSourceSpecScan(scheme, LSourceLoader.LSourceSources, spelling));
    }
}
