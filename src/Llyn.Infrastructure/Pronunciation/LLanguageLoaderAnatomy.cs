using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed partial class LLanguageLoader : LLanguageVault
{
    private const string LLanguageLoaderAnatomy = "anatomy";

    private static IReadOnlyList<LAnatomyRule> LLanguageAnatomyRead(string language, JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderAnatomy, out JsonElement section))
        {
            return Array.Empty<LAnatomyRule>();
        }

        if (section.ValueKind == JsonValueKind.String)
        {
            return LLanguageAnatomyLoad(language, section.GetString()!);
        }

        return LLanguageAnatomyScan(section);
    }

    private static IReadOnlyList<LAnatomyRule> LLanguageAnatomyLoad(string language, string file)
    {
        string name = file.Trim();
        if (name.Length == 0 || name != Path.GetFileName(name))
        {
            return Array.Empty<LAnatomyRule>();
        }

        string path = Path.Combine(AppContext.BaseDirectory, LLanguageLoaderFolder, language, name);
        if (!File.Exists(path))
        {
            return Array.Empty<LAnatomyRule>();
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            using JsonDocument document = JsonDocument.Parse(stream);
            JsonElement root = document.RootElement;
            if (root.ValueKind == JsonValueKind.Object
                && root.TryGetProperty(LLanguageLoaderAnatomy, out JsonElement rows))
            {
                root = rows;
            }

            return LLanguageAnatomyScan(root);
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return Array.Empty<LAnatomyRule>();
        }
    }

    private static IReadOnlyList<LAnatomyRule> LLanguageAnatomyScan(JsonElement rows)
    {
        if (rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LAnatomyRule>();
        }

        List<LAnatomyRule> rules = [];
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LAnatomyRule? rule = LLanguageBlockRead(row);
            if (rule is not null)
            {
                rules.Add(rule);
            }
        }

        return rules;
    }

    private static LAnatomyRule? LLanguageBlockRead(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        IReadOnlyList<string> languages = LLanguageNameScan(row);
        if (languages.Count == 0)
        {
            return null;
        }

        LAnatomyPattern? ipa = LLanguagePatternRead(row, "ipa");
        LAnatomyPattern? respelling = LLanguagePatternRead(row, "respelling") ?? ipa;
        if (ipa is null || respelling is null)
        {
            return null;
        }

        return new LAnatomyRule(languages, ipa, respelling);
    }

    private static IReadOnlyList<string> LLanguageNameScan(JsonElement row)
    {
        List<string> names = [];
        if (!row.TryGetProperty("language", out JsonElement value))
        {
            return names;
        }

        IEnumerable<JsonElement> elements = value.ValueKind switch
        {
            JsonValueKind.String => [value],
            JsonValueKind.Array => value.EnumerateArray(),
            _ => [],
        };
        foreach (JsonElement element in elements)
        {
            string trimmed = element.ValueKind == JsonValueKind.String ? element.GetString()!.Trim() : string.Empty;
            if (trimmed.Length > 0)
            {
                names.Add(trimmed);
            }
        }

        return names;
    }

    private static LAnatomyPattern? LLanguagePatternRead(JsonElement row, string key)
    {
        if (!row.TryGetProperty(key, out JsonElement block) || block.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string match = LLanguageTextRead(block, "match")?.Trim() ?? string.Empty;
        if (match.Length == 0)
        {
            return null;
        }

        IReadOnlyList<LRespellingRule> rewrites =
            block.TryGetProperty("rewrite", out JsonElement rows) && rows.ValueKind == JsonValueKind.Array
                ? LLanguageRuleScan(rows)
                : Array.Empty<LRespellingRule>();

        try
        {
            return new LAnatomyPattern(match, rewrites, LLanguageBooleanRead(block, "decompose"));
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
