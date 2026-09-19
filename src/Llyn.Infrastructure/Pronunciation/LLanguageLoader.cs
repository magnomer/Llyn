using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed partial class LLanguageLoader : LLanguageVault
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
    private const string LLanguageLoaderSpelling = "spelling";
    private const string LLanguageLoaderFollow = "follow";
    private const string LLanguageLoaderFrequency = "frequency";
    private const string LLanguageLoaderMorphology = "morphology";
    private const string LLanguageLoaderTonal = "tonal";
    private const string LLanguageLoaderSilent = "silent";
    private const string LLanguageLoaderPhonemic = "phonemic";
    private const string LLanguageLoaderListed = "listed";
    private const string LLanguageLoaderGlyph = "glyph";
    private const string LLanguageLoaderEmblem = ".svg";
    private const string LLanguageLoaderFlags = "flags";
    private const string LLanguageFlagHost = "https://cdn.jsdelivr.net/gh/lipis/flag-icons/flags/4x3/";

    private readonly string _lLanguageLoaderRoot;

    private readonly HttpClient _lLanguageLoaderClient;

    public LLanguageLoader(string root, HttpClient client)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentNullException.ThrowIfNull(client);
        _lLanguageLoaderRoot = root;
        _lLanguageLoaderClient = client;
    }

    public IReadOnlyList<string> LLanguageScan()
    {
        return LLanguageLoaderScan();
    }

    public LLanguage LLanguageRead(string language)
    {
        return LLanguageLoaderLoad(language);
    }

    bool LLanguageVault.LLanguageNameValidate(string? language)
    {
        return LLanguageNameValidate(language);
    }

    public async Task<string?> LLanguageFlagRead(string code, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        string key = LLanguageFlagNormalize(code);
        string directory = Path.Combine(_lLanguageLoaderRoot, LLanguageLoaderFlags);
        Directory.CreateDirectory(directory);

        string path = Path.Combine(directory, key + LLanguageLoaderEmblem);
        if (File.Exists(path))
        {
            return path;
        }

        try
        {
            byte[] svg = await _lLanguageLoaderClient
                .GetByteArrayAsync(LLanguageFlagHost + key + LLanguageLoaderEmblem, cancellation)
                .ConfigureAwait(false);
            await LWorkspaceRoot.LWorkspaceFileSave(path, svg, cancellation).ConfigureAwait(false);
            return path;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    private static string LLanguageFlagNormalize(string code)
    {
        foreach (char invalid in Path.GetInvalidFileNameChars())
        {
            code = code.Replace(invalid, '_');
        }

        return code.Trim().ToLowerInvariant();
    }

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
            string file = Path.Combine(directory, LLanguageLoaderFile);
            if (File.Exists(file) && LLanguageListedCheck(file))
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

    private static bool LLanguageListedCheck(string file)
    {
        try
        {
            using FileStream stream = File.OpenRead(file);
            using JsonDocument document = JsonDocument.Parse(stream);
            JsonElement root = document.RootElement;
            return root.ValueKind != JsonValueKind.Object || LLanguageListedRead(root);
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return true;
        }
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
        string? flag = LLanguageEmblemRead(language, root);
        IReadOnlyList<LVariety> varieties = LLanguageVarietyScan(root);
        bool scoped = varieties.Count > 0;
        IReadOnlyList<LRespellingRule> spelling = LLanguageSpellingScan(root);

        return new LLanguage(
            language,
            flag,
            LLanguageFontRead(root, LLanguageLoaderFont),
            LLanguageFontRead(root, LLanguageLoaderExample),
            LLanguageSourceScan(root, LLanguageLoaderLookup, spelling),
            LLanguageSourceScan(root, LLanguageLoaderHarvest, spelling),
            LLanguageSchemeScan(root, spelling),
            LLanguageSeparatorRead(root),
            varieties,
            LLanguageFlaggedCheck(root),
            LLanguageFontRead(root, LLanguageLoaderGloss),
            LLanguageRespellingScan(root, LLanguageLoaderCleanup, scoped),
            LLanguageRespellingScan(root, LLanguageLoaderRespelling, scoped),
            LLanguageSourceScan(root, LLanguageLoaderFrequency, spelling),
            LLanguageSourceScan(root, LLanguageLoaderMorphology, spelling),
            root.ValueKind == JsonValueKind.Object && LLanguageBooleanRead(root, LLanguageLoaderTonal),
            LLanguageGlyphRead(root, spelling),
            LLanguageScriptScan(root),
            LLanguageFanqieScan(root),
            LLanguageHypothesisRead(language, root),
            root.ValueKind == JsonValueKind.Object && LLanguageBooleanRead(root, LLanguageLoaderSilent),
            LLanguageReflexScan(root),
            root.ValueKind == JsonValueKind.Object && LLanguageBooleanRead(root, LLanguageLoaderPhonemic),
            root.ValueKind != JsonValueKind.Object || LLanguageListedRead(root),
            LLanguageAnatomyRead(language, root),
            LLanguageClassRead(language, root));
    }

    private static bool LLanguageListedRead(JsonElement root)
    {
        return !root.TryGetProperty(LLanguageLoaderListed, out JsonElement listed)
            || listed.ValueKind != JsonValueKind.False;
    }

    private static string? LLanguageEmblemRead(string language, JsonElement root)
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
            if (variety is not null && varieties.All(known =>
                    !string.Equals(known.LVarietyName, variety.LVarietyName, StringComparison.Ordinal)))
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
