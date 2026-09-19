using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Llyn.Application;

internal static class LLocalizationReader
{
    private const string LLocalizationReaderTerms = "terms";

    private const string LLocalizationReaderTexts = "texts";

    private static readonly Regex LLocalizationReaderKey = new(
        @"^terms\.[a-z][A-Za-z0-9]*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex LLocalizationReaderReference = new(
        @"\{(?<term>[Tt]erms\.[A-Za-z][A-Za-z0-9]*)\}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex LLocalizationReaderSlot = new(
        @"\{\d+\}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    internal static IReadOnlyDictionary<string, string> LLocalizationRead(TextReader reader, CultureInfo culture)
    {
        using JsonDocument document = JsonDocument.Parse(reader.ReadToEnd());
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
                    $"No localization entries are allowed after '{LLocalizationReaderTexts}'.");
            }

            if (property.Name == LLocalizationReaderTexts)
            {
                if (terms.Count == 0)
                {
                    throw new InvalidDataException(
                        $"The localization file must start with '{LLocalizationReaderTerms}.*' definitions.");
                }

                textsElement = property.Value;
                foundTexts = true;
                continue;
            }

            LLocalizationTermAdd(property, terms);
        }

        if (!foundTexts)
        {
            throw new InvalidDataException(
                $"The term definitions must be followed by a '{LLocalizationReaderTexts}' object.");
        }

        Dictionary<string, string> texts = LLocalizationTextScan(textsElement, terms, culture);
        foreach ((string termKey, string termValue) in terms)
        {
            texts.Add(LLocalizationTermFormat(termKey), LCase.LCaseUpperChange(termValue, culture));
        }

        return texts;
    }

    private static void LLocalizationTermAdd(JsonProperty property, IDictionary<string, string> terms)
    {
        if (!LLocalizationReaderKey.IsMatch(property.Name))
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

    private static Dictionary<string, string> LLocalizationTextScan(
        JsonElement element,
        IReadOnlyDictionary<string, string> terms,
        CultureInfo culture)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException($"'{LLocalizationReaderTexts}' must be a JSON object.");
        }

        Dictionary<string, string> texts = new(StringComparer.Ordinal);
        foreach (JsonProperty property in element.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.String)
            {
                throw new InvalidDataException(
                    $"The localization text '{property.Name}' must be a string.");
            }

            string resolved = LLocalizationTextResolve(property.Value.GetString()!, property.Name, terms, culture);
            if (!texts.TryAdd(property.Name, resolved))
            {
                throw new InvalidDataException(
                    $"The localization text '{property.Name}' is duplicated.");
            }
        }

        return texts;
    }

    private static string LLocalizationTextResolve(
        string text,
        string textKey,
        IReadOnlyDictionary<string, string> terms,
        CultureInfo culture)
    {
        string resolved = LLocalizationReaderReference.Replace(text, match =>
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

        string bare = LLocalizationReaderSlot.Replace(resolved, string.Empty);
        if (bare.Contains('{', StringComparison.Ordinal) || bare.Contains('}', StringComparison.Ordinal))
        {
            throw new InvalidDataException(
                $"The localization text '{textKey}' contains a malformed term reference.");
        }

        return resolved;
    }

    private static string LLocalizationTermFormat(string qualifiedTermKey)
    {
        string[] segments = qualifiedTermKey.Split('.');
        for (int index = 0; index < segments.Length; index++)
        {
            segments[index] = LCase.LCaseUpperChange(segments[index], CultureInfo.InvariantCulture);
        }

        return string.Join('.', segments);
    }
}
