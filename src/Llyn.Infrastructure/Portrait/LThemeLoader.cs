using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LThemeLoader
{
    private const string LThemeLoaderResource = "Llyn.Infrastructure.Themes.default.json";

    public static LTheme LThemeLoaderLoad()
    {
        Dictionary<string, string> colors = new Dictionary<string, string>(StringComparer.Ordinal);

        try
        {
            Assembly assembly = typeof(LThemeLoader).Assembly;
            using Stream? stream = assembly.GetManifestResourceStream(LThemeLoaderResource);

            if (stream is not null)
            {
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
            }
        }
        catch (JsonException)
        {
            colors.Clear();
        }
        catch (IOException)
        {
            colors.Clear();
        }

        return new LTheme(colors);
    }
}
