using System;
using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LHypothesisLoader
{
    private const string LHypothesisKey = "hypothesis";

    public static LHypothesis? LHypothesisPackRead(string language, JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LHypothesisKey, out JsonElement section))
        {
            return null;
        }

        if (section.ValueKind == JsonValueKind.String)
        {
            return LPackFile.LPackFileLoad(language, section.GetString()!) is JsonElement file
                ? LHypothesisSectionScan(file)
                : null;
        }

        return section.ValueKind == JsonValueKind.Object ? LHypothesisSectionScan(section) : null;
    }

    private static LHypothesis? LHypothesisSectionScan(JsonElement section)
    {
        if (section.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        IReadOnlyDictionary<string, string> initials = LHypothesisTableRead(section, "initial");
        IReadOnlyDictionary<string, string> finals = LHypothesisTableRead(section, "final");
        if (initials.Count == 0 || finals.Count == 0)
        {
            return null;
        }

        return new LHypothesis(initials, finals, LHypothesisToneScan(section), LHypothesisLocusScan(section));
    }

    private static IReadOnlyList<LHypothesisLocus> LHypothesisLocusScan(JsonElement section)
    {
        List<LHypothesisLocus> places = [];
        if (!section.TryGetProperty("place", out JsonElement rows) || rows.ValueKind != JsonValueKind.Array)
        {
            return places;
        }

        foreach (JsonElement element in rows.EnumerateArray())
        {
            LHypothesisLocus? place = LHypothesisLocusRead(element);
            if (place is not null)
            {
                places.Add(place);
            }
        }

        return places;
    }

    private static LHypothesisLocus? LHypothesisLocusRead(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LPack.LPackTextRead(element, "name")?.Trim() ?? string.Empty;
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

    private static IReadOnlyDictionary<string, string> LHypothesisTableRead(JsonElement section, string key)
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

    private static IReadOnlyDictionary<string, IReadOnlyList<LHypothesisTone>> LHypothesisToneScan(JsonElement section)
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
                LHypothesisTone? tone = LHypothesisToneRead(element);
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

    private static LHypothesisTone? LHypothesisToneRead(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string onset = LPack.LPackTextRead(element, "onset") ?? string.Empty;
        string label = LPack.LPackTextRead(element, "class")?.Trim() ?? string.Empty;
        IReadOnlyList<LRespellingRule> rules =
            element.TryGetProperty("rewrite", out JsonElement rows) && rows.ValueKind == JsonValueKind.Array
                ? LRespellingLoader.LRespellingRuleScan(rows)
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
