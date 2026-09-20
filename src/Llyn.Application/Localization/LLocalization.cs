using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public static class LLocalization
{
    public const string LLocalizationDefault = "en";

    private static readonly object LLocalizationGate = new();

    private static IReadOnlyDictionary<string, string> LLocalizationTexts =
        new Dictionary<string, string>(StringComparer.Ordinal);

    private static IReadOnlyList<string> LLocalizationListed = [LLocalizationDefault];

    public static void LLocalizationLanguageSet(IReadOnlyList<string> languages)
    {
        ArgumentNullException.ThrowIfNull(languages);

        lock (LLocalizationGate)
        {
            LLocalizationListed = languages;
        }
    }

    public static string LLocalizationNormalize(string? language)
    {
        return language is not null && LLocalizationListedCheck(language) ? language : LLocalizationDefault;
    }

    private static bool LLocalizationListedCheck(string language)
    {
        lock (LLocalizationGate)
        {
            return LLocalizationListed.Contains(language, StringComparer.Ordinal);
        }
    }

    public static bool LLocalizationDefaultCheck(string? language)
    {
        return string.Equals(LLocalizationNormalize(language), LLocalizationDefault, StringComparison.Ordinal);
    }

    public static CultureInfo LLocalizationCultureRead(string language)
    {
        ArgumentNullException.ThrowIfNull(language);

        try
        {
            return CultureInfo.GetCultureInfo(language, predefinedOnly: true);
        }
        catch (CultureNotFoundException)
        {
            throw new ArgumentOutOfRangeException(
                nameof(language), language, "The selected language is not supported.");
        }
    }

    public static IReadOnlyDictionary<string, string> LLocalizationLoad(LLocalizationVault vault, string language)
    {
        ArgumentNullException.ThrowIfNull(vault);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        LLocalizationLanguageSet(vault.LLocalizationScan());
        return LLocalizationLoad(vault.LLocalizationRead(language), language);
    }

    public static IReadOnlyDictionary<string, string> LLocalizationLoad(
        IReadOnlyDictionary<string, string> raw, string language)
    {
        ArgumentNullException.ThrowIfNull(raw);

        IReadOnlyDictionary<string, string> texts =
            LLocalizationReader.LLocalizationRead(raw, LLocalizationCultureRead(language));
        lock (LLocalizationGate)
        {
            LLocalizationTexts = texts;
        }

        return texts;
    }

    public static string LLocalizationTextRead(string key)
    {
        return LLocalizationTextFind(key) ?? key;
    }

    public static string? LLocalizationTextFind(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        lock (LLocalizationGate)
        {
            return LLocalizationTexts.TryGetValue(key, out string? text) ? text : null;
        }
    }
}
