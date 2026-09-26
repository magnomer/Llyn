using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed partial class LLanguageLoader : LLanguageVault
{
    private const string LLanguageLoaderHypothesis = "hypothesis";

    private static LHypothesis? LLanguageHypothesisRead(string language, JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderHypothesis, out JsonElement section))
        {
            return null;
        }

        if (section.ValueKind == JsonValueKind.String)
        {
            return LLanguageHypothesisLoad(language, section.GetString()!);
        }

        return section.ValueKind == JsonValueKind.Object ? LLanguageHypothesisScan(section) : null;
    }

    private static LHypothesis? LLanguageHypothesisLoad(string language, string file)
    {
        string name = file.Trim();
        if (name.Length == 0 || name != Path.GetFileName(name))
        {
            return null;
        }

        string path = Path.Combine(AppContext.BaseDirectory, LLanguageLoaderFolder, language, name);
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            using JsonDocument document = JsonDocument.Parse(stream);
            return LLanguageHypothesisScan(document.RootElement);
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static LHypothesis? LLanguageHypothesisScan(JsonElement section)
    {
        if (section.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        IReadOnlyDictionary<string, string> initials = LLanguageTableRead(section, "initial");
        IReadOnlyDictionary<string, string> finals = LLanguageTableRead(section, "final");
        if (initials.Count == 0 || finals.Count == 0)
        {
            return null;
        }

        return new LHypothesis(initials, finals, LLanguageToneScan(section), LLanguagePlaceScan(section));
    }

    private static IReadOnlyList<LHypothesisLocus> LLanguagePlaceScan(JsonElement section)
    {
        List<LHypothesisLocus> places = [];
        if (!section.TryGetProperty("place", out JsonElement rows) || rows.ValueKind != JsonValueKind.Array)
        {
            return places;
        }

        foreach (JsonElement element in rows.EnumerateArray())
        {
            LHypothesisLocus? place = LLanguagePlaceRead(element);
            if (place is not null)
            {
                places.Add(place);
            }
        }

        return places;
    }

    private static LHypothesisLocus? LLanguagePlaceRead(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LLanguageTextRead(element, "name")?.Trim() ?? string.Empty;
        if (name.Length == 0
            || !element.TryGetProperty("initial", out JsonElement rows)
            || rows.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        List<string> initials = [];
        foreach (JsonElement row in rows.EnumerateArray())
        {
            string initial = row.ValueKind == JsonValueKind.String ? row.GetString()!.Trim() : string.Empty;
            if (initial.Length > 0 && !initials.Contains(initial))
            {
                initials.Add(initial);
            }
        }

        return initials.Count > 0 ? new LHypothesisLocus(name, initials) : null;
    }

    private static IReadOnlyDictionary<string, string> LLanguageTableRead(JsonElement section, string key)
    {
        Dictionary<string, string> table = new(StringComparer.Ordinal);
        if (!section.TryGetProperty(key, out JsonElement rows) || rows.ValueKind != JsonValueKind.Object)
        {
            return table;
        }

        foreach (JsonProperty row in rows.EnumerateObject())
        {
            if (row.Value.ValueKind == JsonValueKind.String && row.Name.Trim().Length > 0)
            {
                table[row.Name.Trim()] = row.Value.GetString()!;
            }
        }

        return table;
    }

    private static IReadOnlyDictionary<string, IReadOnlyList<LHypothesisTone>> LLanguageToneScan(JsonElement section)
    {
        Dictionary<string, IReadOnlyList<LHypothesisTone>> tones = new(StringComparer.Ordinal);
        if (!section.TryGetProperty("tone", out JsonElement rows) || rows.ValueKind != JsonValueKind.Object)
        {
            return tones;
        }

        foreach (JsonProperty row in rows.EnumerateObject())
        {
            if (row.Value.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            List<LHypothesisTone> classes = new();
            foreach (JsonElement element in row.Value.EnumerateArray())
            {
                LHypothesisTone? tone = LLanguageToneRead(element);
                if (tone is not null)
                {
                    classes.Add(tone);
                }
            }

            if (classes.Count > 0)
            {
                tones[row.Name.Trim()] = classes;
            }
        }

        return tones;
    }

    private static LHypothesisTone? LLanguageToneRead(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string onset = LLanguageTextRead(element, "onset") ?? string.Empty;
        string label = LLanguageTextRead(element, "class")?.Trim() ?? string.Empty;
        IReadOnlyList<LRespellingRule> rules =
            element.TryGetProperty("rewrite", out JsonElement rows) && rows.ValueKind == JsonValueKind.Array
                ? LLanguageRuleScan(rows)
                : Array.Empty<LRespellingRule>();

        try
        {
            return new LHypothesisTone(onset, rules, label);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
