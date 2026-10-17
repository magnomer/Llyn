using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LThemeLoader
{
    private const string LThemeLoaderResource = "Llyn.Infrastructure.Themes.{0}.json";

    public static LTheme LThemeLoaderLoad()
    {
        try
        {
            return LThemeLoaderLoad("default");
        }
        catch (JsonException)
        {
            return new LTheme(new Dictionary<string, string>(StringComparer.Ordinal));
        }
        catch (IOException)
        {
            return new LTheme(new Dictionary<string, string>(StringComparer.Ordinal));
        }
    }

    public static LTheme LThemeLoaderLoad(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Dictionary<string, string> colors = new Dictionary<string, string>(StringComparer.Ordinal);
        Assembly assembly = typeof(LThemeLoader).Assembly;
        using Stream stream = assembly.GetManifestResourceStream(
                string.Format(CultureInfo.InvariantCulture, LThemeLoaderResource, name))
            ?? throw new FileNotFoundException($"The theme '{name}' is missing.");
        using JsonDocument document = JsonDocument.Parse(stream);

        if (document.RootElement.TryGetProperty("colors", out JsonElement read)
            && read.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty color in read.EnumerateObject())
            {
                if (color.Value.ValueKind == JsonValueKind.String)
                {
                    colors[color.Name] = color.Value.GetString()!;
                }
            }
        }

        return new LTheme(colors);
    }
}
