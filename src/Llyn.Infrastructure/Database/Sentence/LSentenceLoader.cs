using System;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSentenceLoader
{
    private const string LSentenceLoaderFolder = "languages";
    private const string LSentenceLoaderFile = "vocabulary.json";
    private const string LSentenceLoaderKey = "exampleOrder";
    private const string LSentenceLoaderParticle = "particle";
    private const string LSentenceLoaderDependence = "dependence";

    public static LSentenceOrder LSentenceLoaderLoad(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return LSentenceOrder.LSentenceOrderDefault;
        }

        string path = Path.Combine(
            AppContext.BaseDirectory, LSentenceLoaderFolder, language, LSentenceLoaderFile);
        if (!LLanguageLoader.LLanguageNameValidate(language) || !File.Exists(path))
        {
            return LSentenceOrder.LSentenceOrderDefault;
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            using JsonDocument document = JsonDocument.Parse(stream);
            return LSentenceOrderRead(document.RootElement);
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return LSentenceOrder.LSentenceOrderDefault;
        }
    }

    private static LSentenceOrder LSentenceOrderRead(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LSentenceLoaderKey, out JsonElement order) ||
            order.ValueKind != JsonValueKind.Array)
        {
            return LSentenceOrder.LSentenceOrderDefault;
        }

        int particle = -1;
        int dependence = -1;
        int position = 0;
        foreach (JsonElement field in order.EnumerateArray())
        {
            if (field.ValueKind == JsonValueKind.String)
            {
                string? name = field.GetString();
                if (string.Equals(name, LSentenceLoaderParticle, StringComparison.Ordinal))
                {
                    particle = position;
                }
                else if (string.Equals(name, LSentenceLoaderDependence, StringComparison.Ordinal))
                {
                    dependence = position;
                }
            }

            position++;
        }

        if (particle < 0 || dependence < 0)
        {
            return LSentenceOrder.LSentenceOrderDefault;
        }

        return new LSentenceOrder(particle < dependence ? 0 : 1, particle < dependence ? 1 : 0);
    }
}
