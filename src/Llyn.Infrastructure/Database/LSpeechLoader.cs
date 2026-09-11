using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSpeechLoader
{
    private const string LSpeechLoaderFolder = "languages";
    private const string LSpeechLoaderFile = "vocabulary.json";

    public static LSpeechPack LSpeechLoaderLoad(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string path = Path.Combine(
            AppContext.BaseDirectory, LSpeechLoaderFolder, language, LSpeechLoaderFile);
        if (!File.Exists(path))
        {
            return new LSpeechPack([], [], []);
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            using JsonDocument document = JsonDocument.Parse(stream);
            return LSpeechPackRead(language, document.RootElement);
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return new LSpeechPack([], [], []);
        }
    }

    private static LSpeechPack LSpeechPackRead(string language, JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object)
        {
            return new LSpeechPack([], [], []);
        }

        List<LSpeechValue> values = [];
        foreach (JsonElement part in LSpeechRowRead(root, "parts"))
        {
            long? id = LSpeechNumberRead(part, "id");
            string? name = LSpeechTextRead(part, "name");
            if (id is null || name is null)
            {
                continue;
            }

            values.Add(new LSpeechValue(0, language, id.Value, name, values.Count));
        }

        List<LFeature> features = [];
        Dictionary<long, int> featureOrder = [];
        foreach (JsonElement row in LSpeechRowRead(root, "features"))
        {
            long? id = LSpeechNumberRead(row, "id");
            long? part = LSpeechNumberRead(row, "part");
            string? name = LSpeechTextRead(row, "name");
            if (id is null || part is null || name is null)
            {
                continue;
            }

            featureOrder.TryGetValue(part.Value, out int position);
            featureOrder[part.Value] = position + 1;
            features.Add(new LFeature(0, part.Value, id.Value, name, position));
        }

        List<LMorphology> morphology = [];
        Dictionary<long, int> valueOrder = [];
        foreach (JsonElement row in LSpeechRowRead(root, "values"))
        {
            long? id = LSpeechNumberRead(row, "id");
            long? feature = LSpeechNumberRead(row, "feature");
            string? name = LSpeechTextRead(row, "name");
            if (id is null || feature is null || name is null)
            {
                continue;
            }

            valueOrder.TryGetValue(feature.Value, out int position);
            valueOrder[feature.Value] = position + 1;
            morphology.Add(new LMorphology(0, feature.Value, id.Value, name, position));
        }

        return new LSpeechPack(values, features, morphology);
    }

    private static IEnumerable<JsonElement> LSpeechRowRead(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out JsonElement rows) || rows.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return rows.EnumerateArray();
    }

    private static long? LSpeechNumberRead(JsonElement element, string name)
    {
        if (element.ValueKind != JsonValueKind.Object ||
            !element.TryGetProperty(name, out JsonElement found) ||
            found.ValueKind != JsonValueKind.Number ||
            !found.TryGetInt64(out long number) ||
            number <= 0)
        {
            return null;
        }

        return number;
    }

    private static string? LSpeechTextRead(JsonElement element, string name)
    {
        if (element.ValueKind != JsonValueKind.Object ||
            !element.TryGetProperty(name, out JsonElement found) ||
            found.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        string? text = found.GetString();
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }
}
