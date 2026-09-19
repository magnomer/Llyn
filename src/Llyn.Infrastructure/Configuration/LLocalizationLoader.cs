using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LLocalizationLoader : LLocalizationVault
{
    private const string LLocalizationLoaderPrefix = "Llyn.Infrastructure.Localization.";
    private const string LLocalizationLoaderSuffix = ".json";

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

    public TextReader LLocalizationOpen(string language)
    {
        string resourceName = LLocalizationLoaderPrefix + language + LLocalizationLoaderSuffix;
        Assembly assembly = typeof(LLocalizationLoader).Assembly;
        Stream stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidDataException(
                $"The embedded localization file '{resourceName}' could not be found.");
        return new StreamReader(stream);
    }
}
