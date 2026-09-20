using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LLocalizationLoader : LLocalizationVault
{
    private const string LLocalizationLoaderPrefix = "Llyn.Infrastructure.Localization.";
    private const string LLocalizationLoaderSuffix = ".json";
    private const string LLocalizationLoaderTerms = "terms.";
    private const string LLocalizationLoaderTexts = "texts";

    public IReadOnlyList<string> LLocalizationScan()
    {
        List<string> languages = [];
        foreach (string name in typeof(LLocalizationLoader).Assembly.GetManifestResourceNames())
        {
            if (name.StartsWith(LLocalizationLoaderPrefix, StringComparison.Ordinal)
                && name.EndsWith(LLocalizationLoaderSuffix, StringComparison.Ordinal))
            {
                languages.Add(name[LLocalizationLoaderPrefix.Length..^LLocalizationLoaderSuffix.Length]);
            }
        }

        languages.Sort(StringComparer.Ordinal);
        return languages;
    }

    public IReadOnlyDictionary<string, string> LLocalizationRead(string language)
    {
        string resourceName = LLocalizationLoaderPrefix + language + LLocalizationLoaderSuffix;
        Assembly assembly = typeof(LLocalizationLoader).Assembly;
        using Stream stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidDataException(
                $"The embedded localization file '{resourceName}' could not be found.");
        using StreamReader reader = new(stream);
        return LLocalizationLoaderParse(reader.ReadToEnd());
    }

    public static IReadOnlyDictionary<string, string> LLocalizationLoaderParse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        using JsonDocument document = JsonDocument.Parse(text);
        JsonElement root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException("The localization file must contain a JSON object.");
        }

        Dictionary<string, string> pairs = new(StringComparer.Ordinal);
        bool foundTexts = false;
        foreach (JsonProperty property in root.EnumerateObject())
        {
            if (foundTexts)
            {
                throw new InvalidDataException(
                    $"No localization entries are allowed after '{LLocalizationLoaderTexts}'.");
            }

            if (property.Name == LLocalizationLoaderTexts)
            {
                if (pairs.Count == 0)
                {
                    throw new InvalidDataException(
                        $"The localization file must start with '{LLocalizationLoaderTerms}*' definitions.");
                }

                LLocalizationLoaderScan(property.Value, pairs);
                foundTexts = true;
                continue;
            }

            if (!property.Name.StartsWith(LLocalizationLoaderTerms, StringComparison.Ordinal))
            {
                throw new InvalidDataException(
                    $"'{property.Name}' is not a valid term key. Term keys must use 'terms.name'.");
            }

            LLocalizationLoaderAdd(property, pairs, "term");
        }

        if (!foundTexts)
        {
            throw new InvalidDataException(
                $"The term definitions must be followed by a '{LLocalizationLoaderTexts}' object.");
        }

        return pairs;
    }

    private static void LLocalizationLoaderScan(JsonElement element, Dictionary<string, string> pairs)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException($"'{LLocalizationLoaderTexts}' must be a JSON object.");
        }

        foreach (JsonProperty property in element.EnumerateObject())
        {
            if (property.Name.StartsWith(LLocalizationLoaderTerms, StringComparison.Ordinal))
            {
                throw new InvalidDataException(
                    $"The localization text '{property.Name}' must not use the term prefix.");
            }

            LLocalizationLoaderAdd(property, pairs, "localization text");
        }
    }

    private static void LLocalizationLoaderAdd(JsonProperty property, Dictionary<string, string> pairs, string kind)
    {
        if (property.Value.ValueKind != JsonValueKind.String)
        {
            throw new InvalidDataException($"The {kind} '{property.Name}' must be a string.");
        }

        if (!pairs.TryAdd(property.Name, property.Value.GetString()!))
        {
            throw new InvalidDataException($"The {kind} '{property.Name}' is duplicated.");
        }
    }
}
