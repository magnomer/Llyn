using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Llyn.Application;

internal static class LLocalizationReader
{
    private const string LLocalizationReaderTerms = "terms.";

    private static readonly Regex LLocalizationReaderKey = new(
        @"^terms\.[a-z][A-Za-z0-9]*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex LLocalizationReaderReference = new(
        @"\{(?<term>[Tt]erms\.[A-Za-z][A-Za-z0-9]*)\}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex LLocalizationReaderSlot = new(
        @"\{\d+\}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    internal static IReadOnlyDictionary<string, string> LLocalizationRead(
        IReadOnlyDictionary<string, string> raw, CultureInfo culture)
    {
        Dictionary<string, string> terms = new(StringComparer.Ordinal);
        foreach ((string key, string value) in raw)
        {
            if (key.StartsWith(LLocalizationReaderTerms, StringComparison.Ordinal))
            {
                LLocalizationTermAdd(key, value, terms);
            }
        }

        if (terms.Count == 0)
        {
            throw new FormatException("The localization file must start with 'terms.*' definitions.");
        }

        Dictionary<string, string> texts = LLocalizationTextScan(raw, terms, culture);
        foreach ((string termKey, string termValue) in terms)
        {
            texts.Add(LLocalizationTermFormat(termKey), LCase.LCaseUpperChange(termValue, culture));
        }

        return texts;
    }

    private static void LLocalizationTermAdd(string key, string value, IDictionary<string, string> terms)
    {
        if (!LLocalizationReaderKey.IsMatch(key))
        {
            throw new FormatException(
                $"'{key}' is not a valid term key. Term keys must use 'terms.name'.");
        }

        if (!terms.TryAdd(key, value))
        {
            throw new FormatException($"The term '{key}' is duplicated.");
        }
    }

    private static Dictionary<string, string> LLocalizationTextScan(
        IReadOnlyDictionary<string, string> raw,
        IReadOnlyDictionary<string, string> terms,
        CultureInfo culture)
    {
        Dictionary<string, string> texts = new(StringComparer.Ordinal);
        foreach ((string key, string value) in raw)
        {
            if (key.StartsWith(LLocalizationReaderTerms, StringComparison.Ordinal))
            {
                continue;
            }

            string resolved = LLocalizationTextResolve(value, key, terms, culture);
            if (!texts.TryAdd(key, resolved))
            {
                throw new FormatException(
                    $"The localization text '{key}' is duplicated.");
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
                throw new FormatException(
                    $"The localization text '{textKey}' references the undefined term '{termKey}'.");
            }

            return uppercase
                ? LCase.LCaseUpperChange(termValue, culture)
                : LCase.LCaseLowerChange(termValue, culture);
        });

        string bare = LLocalizationReaderSlot.Replace(resolved, string.Empty);
        if (bare.Contains('{', StringComparison.Ordinal) || bare.Contains('}', StringComparison.Ordinal))
        {
            throw new FormatException(
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
