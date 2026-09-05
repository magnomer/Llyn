using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;

namespace Llyn.UIShell;

internal static class PLocalizationLoader
{
    internal const string PLocalizationLoaderLanguage = "en";

    private const string PLocalizationLoaderTerms = "terms";
    private const string PLocalizationLoaderTexts = "texts";

    private static readonly Regex PLocalizationLoaderKey = new(
        @"^terms\.[a-z][A-Za-z0-9]*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex PLocalizationLoaderReference = new(
        @"\{(?<term>[Tt]erms\.[A-Za-z][A-Za-z0-9]*)\}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    internal static void PLocalizationLoaderApply(ResourceDictionary resources, string language)
    {
        CultureInfo culture = language switch
        {
            "en" => CultureInfo.GetCultureInfo("en"),
            "ko" => CultureInfo.GetCultureInfo("ko"),
            _ => throw new ArgumentOutOfRangeException(
                nameof(language),
                language,
                "The selected language is not supported.")
        };
        string resourceName = $"Llyn.UIShell.Localization.{language}.json";
        Assembly assembly = typeof(PLocalizationLoader).Assembly;

        using Stream stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidDataException(
                $"The embedded localization file '{resourceName}' could not be found.");
        using JsonDocument document = JsonDocument.Parse(stream);

        JsonElement root = document.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException("The localization file must contain a JSON object.");
        }

        Dictionary<string, string> terms = new(StringComparer.Ordinal);
        JsonElement textsElement = default;
        bool foundTexts = false;

        foreach (JsonProperty property in root.EnumerateObject())
        {
            if (foundTexts)
            {
                throw new InvalidDataException(
                    $"No localization entries are allowed after '{PLocalizationLoaderTexts}'.");
            }

            if (property.Name == PLocalizationLoaderTexts)
            {
                if (terms.Count == 0)
                {
                    throw new InvalidDataException(
                        $"The localization file must start with '{PLocalizationLoaderTerms}.*' definitions.");
                }

                textsElement = property.Value;
                foundTexts = true;
                continue;
            }

            PLocalizationLoaderAdd(property, terms);
        }

        if (!foundTexts)
        {
            throw new InvalidDataException(
                $"The term definitions must be followed by a '{PLocalizationLoaderTexts}' object.");
        }

        Dictionary<string, string> localizedResources = PLocalizationLoaderRead(textsElement, terms, culture);

        foreach ((string termKey, string termValue) in terms)
        {
            localizedResources.Add(
                PLocalizationLoaderFormat(termKey),
                LCase.LCaseUpperChange(termValue, culture));
        }

        foreach ((string key, string value) in localizedResources)
        {
            resources[key] = value;
        }

        PLocalizationCatalog.PLocalizationCatalogUpdate();
    }

    private static void PLocalizationLoaderAdd(JsonProperty property, IDictionary<string, string> terms)
    {
        if (!PLocalizationLoaderKey.IsMatch(property.Name))
        {
            throw new InvalidDataException(
                $"'{property.Name}' is not a valid term key. Term keys must use 'terms.name'.");
        }

        if (property.Value.ValueKind != JsonValueKind.String)
        {
            throw new InvalidDataException($"The term '{property.Name}' must be a string.");
        }

        if (!terms.TryAdd(property.Name, property.Value.GetString()!))
        {
            throw new InvalidDataException($"The term '{property.Name}' is duplicated.");
        }
    }

    private static Dictionary<string, string> PLocalizationLoaderRead(
        JsonElement element,
        IReadOnlyDictionary<string, string> terms,
        CultureInfo culture)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException($"'{PLocalizationLoaderTexts}' must be a JSON object.");
        }

        Dictionary<string, string> texts = new(StringComparer.Ordinal);

        foreach (JsonProperty property in element.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.String)
            {
                throw new InvalidDataException(
                    $"The localization text '{property.Name}' must be a string.");
            }

            string resolved = PLocalizationLoaderResolve(property.Value.GetString()!, property.Name, terms, culture);

            if (!texts.TryAdd(property.Name, resolved))
            {
                throw new InvalidDataException(
                    $"The localization text '{property.Name}' is duplicated.");
            }
        }

        return texts;
    }

    private static string PLocalizationLoaderResolve(
        string text,
        string textKey,
        IReadOnlyDictionary<string, string> terms,
        CultureInfo culture)
    {
        string resolved = PLocalizationLoaderReference.Replace(text, match =>
        {
            string requestedKey = match.Groups["term"].Value;
            bool uppercase = char.IsUpper(requestedKey[0]);
            string termKey = LCase.LCaseLowerChange(requestedKey, CultureInfo.InvariantCulture);

            if (!terms.TryGetValue(termKey, out string? termValue))
            {
                throw new InvalidDataException(
                    $"The localization text '{textKey}' references the undefined term '{termKey}'.");
            }

            return uppercase
                ? LCase.LCaseUpperChange(termValue, culture)
                : LCase.LCaseLowerChange(termValue, culture);
        });

        if (resolved.Contains('{', StringComparison.Ordinal) ||
            resolved.Contains('}', StringComparison.Ordinal))
        {
            throw new InvalidDataException(
                $"The localization text '{textKey}' contains a malformed term reference.");
        }

        return resolved;
    }

    private static string PLocalizationLoaderFormat(string qualifiedTermKey)
    {
        string[] segments = qualifiedTermKey.Split('.');

        for (int index = 0; index < segments.Length; index++)
        {
            segments[index] = LCase.LCaseUpperChange(segments[index], CultureInfo.InvariantCulture);
        }

        return string.Join('.', segments);
    }
}
