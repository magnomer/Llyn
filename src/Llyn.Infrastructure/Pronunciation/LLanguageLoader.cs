using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static partial class LLanguageLoader
{
    private const string LLanguageLoaderFolder = "languages";
    private const string LLanguageLoaderFile = "source.json";
    private const string LLanguageLoaderPrimary = "English";
    private const string LLanguageLoaderLookup = "pronunciation";
    private const string LLanguageLoaderHarvest = "audio";
    private const string LLanguageLoaderFont = "font";
    private const string LLanguageLoaderExample = "example";
    private const string LLanguageLoaderGloss = "gloss";
    private const string LLanguageLoaderScheme = "transcription";
    private const string LLanguageLoaderSources = "sources";
    private const string LLanguageLoaderSeparator = "separator";
    private const string LLanguageLoaderVarieties = "varieties";
    private const string LLanguageLoaderReadings = "readings";
    private const string LLanguageLoaderCleanup = "cleanup";
    private const string LLanguageLoaderRespelling = "respelling";
    private const string LLanguageLoaderFollow = "follow";
    private const string LLanguageLoaderFrequency = "frequency";
    private const string LLanguageLoaderMorphology = "morphology";
    private const string LLanguageLoaderBands = "bands";
    private const string LLanguageLoaderTonal = "tonal";
    private const string LLanguageLoaderGlyph = "glyph";
    private const string LLanguageLoaderEmblem = ".svg";

    public static IReadOnlyList<string> LLanguageLoaderScan()
    {
        string root = Path.Combine(AppContext.BaseDirectory, LLanguageLoaderFolder);
        if (!Directory.Exists(root))
        {
            return Array.Empty<string>();
        }

        List<string> names = new();
        foreach (string directory in Directory.EnumerateDirectories(root))
        {
            if (File.Exists(Path.Combine(directory, LLanguageLoaderFile)))
            {
                names.Add(Path.GetFileName(directory));
            }
        }

        LLanguageSort(names);
        return names;
    }

    private static void LLanguageSort(List<string> names)
    {
        names.Sort(static (left, right) =>
        {
            bool leftPrimary = string.Equals(left, LLanguageLoaderPrimary, StringComparison.OrdinalIgnoreCase);
            bool rightPrimary = string.Equals(right, LLanguageLoaderPrimary, StringComparison.OrdinalIgnoreCase);
            if (leftPrimary != rightPrimary)
            {
                return leftPrimary ? -1 : 1;
            }

            return string.CompareOrdinal(left, right);
        });
    }

    public static bool LLanguageNameValidate(string? language)
    {
        if (string.IsNullOrWhiteSpace(language)
            || language == "."
            || language == ".."
            || Path.IsPathRooted(language)
            || language.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            return false;
        }

        return !language.Contains('/') && !language.Contains('\\');
    }

    public static LLanguage LLanguageLoaderLoad(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string folder = Path.Combine(AppContext.BaseDirectory, LLanguageLoaderFolder, language);
        string path = Path.Combine(folder, LLanguageLoaderFile);
        if (!LLanguageNameValidate(language) || !File.Exists(path))
        {
            return LLanguageBlankCreate(language);
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            using JsonDocument document = JsonDocument.Parse(stream);
            return LLanguageRead(language, document.RootElement);
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return LLanguageBlankCreate(language);
        }
    }

    private static LLanguage LLanguageBlankCreate(string language)
    {
        return new LLanguage(
            language,
            null,
            LLanguageFontBlank,
            LLanguageFontBlank,
            Array.Empty<LSourceSpec>(),
            Array.Empty<LSourceSpec>());
    }

    private static LLanguage LLanguageRead(string language, JsonElement root)
    {
        string? flag = LLanguageFlagRead(language, root);
        IReadOnlyList<LVariety> varieties = LLanguageVarietyScan(root);
        bool scoped = varieties.Count > 0;

        return new LLanguage(
            language,
            flag,
            LLanguageFontRead(root, LLanguageLoaderFont),
            LLanguageFontRead(root, LLanguageLoaderExample),
            LLanguageSourceScan(root, LLanguageLoaderLookup),
            LLanguageSourceScan(root, LLanguageLoaderHarvest),
            LLanguageSchemeScan(root),
            LLanguageSeparatorRead(root),
            varieties,
            LLanguageFlaggedCheck(root),
            LLanguageFontRead(root, LLanguageLoaderGloss),
            LLanguageRespellingScan(root, LLanguageLoaderCleanup, scoped),
            LLanguageRespellingScan(root, LLanguageLoaderRespelling, scoped),
            LLanguageSourceScan(root, LLanguageLoaderFrequency),
            LLanguageBandScan(root),
            LLanguageSourceScan(root, LLanguageLoaderMorphology),
            root.ValueKind == JsonValueKind.Object && LLanguageBooleanRead(root, LLanguageLoaderTonal),
            LLanguageGlyphRead(root),
            LLanguageScriptScan(root));
    }

    private static string? LLanguageFlagRead(string language, JsonElement root)
    {
        string? flag = root.ValueKind == JsonValueKind.Object ? LLanguageTextRead(root, "flag")?.Trim() : null;
        if (string.IsNullOrEmpty(flag))
        {
            return null;
        }

        return flag.EndsWith(LLanguageLoaderEmblem, StringComparison.OrdinalIgnoreCase)
            ? Path.Combine(AppContext.BaseDirectory, LLanguageLoaderFolder, language, flag)
            : flag;
    }

    private static IReadOnlyList<LBand> LLanguageBandScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderBands, out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LBand>();
        }

        List<LBand> bands = new();
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LBand? band = LLanguageBandRead(row);
            if (band is not null)
            {
                bands.Add(band);
            }
        }

        return bands;
    }

    private static LBand? LLanguageBandRead(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LLanguageTextRead(row, "name")?.Trim() ?? string.Empty;
        if (name.Length == 0)
        {
            return null;
        }

        if (row.TryGetProperty("upTo", out JsonElement limit) &&
            limit.ValueKind == JsonValueKind.Number &&
            limit.TryGetDouble(out double upTo))
        {
            return new LBand(name, upTo, null);
        }

        string? pattern = LLanguageTextRead(row, "match");
        if (string.IsNullOrEmpty(pattern))
        {
            return null;
        }

        try
        {
            _ = new Regex(pattern);
        }
        catch (ArgumentException)
        {
            return null;
        }

        return new LBand(name, null, pattern);
    }

    private static IReadOnlyList<LRespelling> LLanguageRespellingScan(JsonElement root, string key, bool scoped)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(key, out JsonElement groups) ||
            groups.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LRespelling>();
        }

        List<LRespelling> respellings = new();
        foreach (JsonElement group in groups.EnumerateArray())
        {
            LRespelling? respelling = LLanguageRespellingRead(group);
            if (respelling is not null && (!scoped || respelling.LRespellingVarieties.Count > 0))
            {
                respellings.Add(respelling);
            }
        }

        return respellings;
    }

    private static LRespelling? LLanguageRespellingRead(JsonElement group)
    {
        if (group.ValueKind != JsonValueKind.Object ||
            !group.TryGetProperty("rules", out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        string name = LLanguageTextRead(group, "name")?.Trim() ?? string.Empty;
        List<string> varieties = new();
        if (group.TryGetProperty(LLanguageLoaderVarieties, out JsonElement names) && names.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement variety in names.EnumerateArray())
            {
                string tag = variety.ValueKind == JsonValueKind.String ? variety.GetString()!.Trim() : string.Empty;
                if (tag.Length > 0)
                {
                    varieties.Add(tag);
                }
            }
        }

        try
        {
            List<LRespellingRule> rules = new();
            foreach (JsonElement row in rows.EnumerateArray())
            {
                if (row.ValueKind == JsonValueKind.Array && row.GetArrayLength() == 2 &&
                    row[0].ValueKind == JsonValueKind.String && row[1].ValueKind == JsonValueKind.String)
                {
                    rules.Add(new LRespellingRule(row[0].GetString()!, row[1].GetString()!));
                }
            }

            return rules.Count == 0 ? null : new LRespelling(name, varieties, rules);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private static bool LLanguageSeparatorRead(JsonElement root)
    {
        string? separator = root.ValueKind == JsonValueKind.Object
            ? LLanguageTextRead(root, LLanguageLoaderSeparator)
            : null;
        return !string.Equals(separator?.Trim(), "none", StringComparison.OrdinalIgnoreCase);
    }

    private static IReadOnlyList<LScheme> LLanguageSchemeScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderScheme, out JsonElement schemes) ||
            schemes.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LScheme>();
        }

        List<LScheme> declared = new();
        foreach (JsonElement scheme in schemes.EnumerateArray())
        {
            LScheme? read = LLanguageSchemeRead(scheme);
            if (read is not null && declared.All(known => !string.Equals(known.LSchemeName, read.LSchemeName, StringComparison.Ordinal)))
            {
                declared.Add(read);
            }
        }

        return declared;
    }

    private static LScheme? LLanguageSchemeRead(JsonElement scheme)
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

        string name = LLanguageTextRead(scheme, "name")?.Trim() ?? string.Empty;
        return name.Length == 0 ? null : new LScheme(name, LLanguageSourceScan(scheme, LLanguageLoaderSources));
    }

    private static LFont LLanguageFontBlank => new(null, 0);

    private static LFont LLanguageFontRead(JsonElement root, string key)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(key, out JsonElement font) ||
            font.ValueKind != JsonValueKind.Object)
        {
            return LLanguageFontBlank;
        }

        string? family = LLanguageTextRead(font, "family");
        double size = LLanguageMeasureRead(font, "size");
        string? style = LLanguageTextRead(font, "style");

        return new LFont(
            string.IsNullOrWhiteSpace(family) ? null : family,
            size,
            string.IsNullOrWhiteSpace(style) ? null : style.Trim());
    }

    private static IReadOnlyList<LVariety> LLanguageVarietyScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderVarieties, out JsonElement block) ||
            block.ValueKind != JsonValueKind.Object ||
            !block.TryGetProperty("list", out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LVariety>();
        }

        List<LVariety> varieties = new();
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LVariety? variety = LLanguageVarietyRead(row);
            if (variety is not null && varieties.All(known => !string.Equals(known.LVarietyName, variety.LVarietyName, StringComparison.Ordinal)))
            {
                varieties.Add(variety);
            }
        }

        return varieties;
    }

    private static LVariety? LLanguageVarietyRead(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LLanguageTextRead(row, "name")?.Trim() ?? string.Empty;
        if (name.Length == 0)
        {
            return null;
        }

        string? flag = LLanguageTextRead(row, "flag")?.Trim();
        return new LVariety(name, string.IsNullOrEmpty(flag) ? null : flag);
    }

    private static bool LLanguageFlaggedCheck(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderVarieties, out JsonElement block) ||
            block.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        string? shown = LLanguageTextRead(block, "shown");
        return string.Equals(shown?.Trim(), "flag", StringComparison.OrdinalIgnoreCase);
    }

    private static string? LLanguageTextRead(JsonElement element, string key)
    {
        return element.TryGetProperty(key, out JsonElement value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private static int LLanguageNumberRead(JsonElement element, string key)
    {
        return element.TryGetProperty(key, out JsonElement value)
            && value.ValueKind == JsonValueKind.Number
            && value.TryGetInt32(out int number)
            ? number
            : 0;
    }

    private static double LLanguageMeasureRead(JsonElement element, string key)
    {
        return element.TryGetProperty(key, out JsonElement value)
            && value.ValueKind == JsonValueKind.Number
            && value.TryGetDouble(out double measure)
            && measure > 0
            ? measure
            : 0;
    }

    private static bool LLanguageBooleanRead(JsonElement element, string key)
    {
        return element.TryGetProperty(key, out JsonElement value) && value.ValueKind == JsonValueKind.True;
    }
}
