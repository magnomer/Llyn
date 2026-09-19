using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Llyn.Application;

public static class LLocalization
{
    public const string LLocalizationDefault = "en";

    private static readonly object LLocalizationGate = new();

    private static IReadOnlyDictionary<string, string> LLocalizationTexts =
        new Dictionary<string, string>(StringComparer.Ordinal);

    public static string LLocalizationNormalize(string? language)
    {
        return language is "en" or "ko" ? language : LLocalizationDefault;
    }

    public static bool LLocalizationDefaultCheck(string? language)
    {
        return string.Equals(LLocalizationNormalize(language), LLocalizationDefault, StringComparison.Ordinal);
    }

    public static CultureInfo LLocalizationCultureRead(string language)
    {
        return language switch
        {
            "en" => CultureInfo.GetCultureInfo("en"),
            "ko" => CultureInfo.GetCultureInfo("ko"),
            _ => throw new ArgumentOutOfRangeException(
                nameof(language),
                language,
                "The selected language is not supported.")
        };
    }

    public static IReadOnlyDictionary<string, string> LLocalizationLoad(string language)
    {
        string resourceName = $"Llyn.Application.Localization.{language}.json";
        Assembly assembly = typeof(LLocalization).Assembly;
        using Stream stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidDataException(
                $"The embedded localization file '{resourceName}' could not be found.");
        return LLocalizationLoad(stream, language);
    }

    public static IReadOnlyDictionary<string, string> LLocalizationLoad(Stream stream, string language)
    {
        ArgumentNullException.ThrowIfNull(stream);

        IReadOnlyDictionary<string, string> texts =
            LLocalizationReader.LLocalizationRead(stream, LLocalizationCultureRead(language));
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
