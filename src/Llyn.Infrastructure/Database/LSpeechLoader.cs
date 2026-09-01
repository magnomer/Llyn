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
            return new LSpeechPack([], []);
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            using JsonDocument document = JsonDocument.Parse(stream);
            return LSpeechPackRead(language, document.RootElement);
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return new LSpeechPack([], []);
        }
    }

    private static LSpeechPack LSpeechPackRead(string language, JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object)
        {
            return new LSpeechPack([], []);
        }

        List<LSpeechValue> values = [];
        if (root.TryGetProperty("parts", out JsonElement parts) && parts.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement part in parts.EnumerateArray())
            {
                string? id = LSpeechTextRead(part, "id");
                if (id is null)
                {
                    continue;
                }

                values.Add(new LSpeechValue(
                    language, id, LSpeechTextRead(part, "name") ?? id, values.Count));
            }
        }

        List<LMorphology> morphology = [];
        Dictionary<string, int> order = new(StringComparer.Ordinal);
        if (root.TryGetProperty("morphology", out JsonElement rows) && rows.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement row in rows.EnumerateArray())
            {
                string? speech = LSpeechTextRead(row, "part");
                string? feature = LSpeechTextRead(row, "feature");
                string? value = LSpeechTextRead(row, "value");
                if (speech is null || feature is null || value is null)
                {
                    continue;
                }

                string group = speech + '\u001f' + feature;
                order.TryGetValue(group, out int position);
                order[group] = position + 1;

                morphology.Add(new LMorphology(
                    language,
                    speech,
                    feature,
                    LSpeechTextRead(row, "featureName") ?? feature,
                    value,
                    LSpeechTextRead(row, "valueName") ?? value,
                    position));
            }
        }

        return new LSpeechPack(values, morphology);
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
