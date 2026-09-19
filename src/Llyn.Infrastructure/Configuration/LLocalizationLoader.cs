using System.IO;
using System.Reflection;

namespace Llyn.Infrastructure;

public static class LLocalizationLoader
{
    public static TextReader LLocalizationLoaderOpen(string language)
    {
        string resourceName = $"Llyn.Infrastructure.Localization.{language}.json";
        Assembly assembly = typeof(LLocalizationLoader).Assembly;
        Stream stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidDataException(
                $"The embedded localization file '{resourceName}' could not be found.");
        return new StreamReader(stream);
    }
}
