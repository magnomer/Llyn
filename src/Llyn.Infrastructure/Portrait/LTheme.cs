using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Llyn.Infrastructure;

public sealed class LTheme
{
    private const string LThemeResource = "Llyn.Infrastructure.Themes.default.json";

    private static readonly IReadOnlyDictionary<string, string> LThemeSpare =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["ink"] = "#172033",
            ["muted"] = "#66758C",
            ["canvas"] = "#F4F7FB",
            ["surface"] = "#FFFFFF",
            ["line"] = "#D7E0EC",
            ["accent"] = "#2F6FED",
            ["accentSoft"] = "#E7EFFD",
            ["situation"] = "#9A5B08",
            ["situationSoft"] = "#FCF0DC",
            ["situationEdge"] = "#EFD7AE",
        };

    private readonly IReadOnlyDictionary<string, string> _lThemeColor;

    private LTheme(IReadOnlyDictionary<string, string> colors)
    {
        _lThemeColor = colors;
    }

    public string LThemeFamily => "\"Segoe UI Variable\", \"Segoe UI\", system-ui, sans-serif";

    public string LThemeSerif => "Georgia, \"Segoe UI\", serif";

    public IReadOnlyDictionary<string, string> LThemeColor => _lThemeColor;

    public static LTheme LThemeLoad()
    {
        Dictionary<string, string> colors = new Dictionary<string, string>(StringComparer.Ordinal);

        try
        {
            Assembly assembly = typeof(LTheme).Assembly;
            using Stream? stream = assembly.GetManifestResourceStream(LThemeResource);

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

    public string LThemeColorRead(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _lThemeColor.TryGetValue(name, out string? found)
            ? found
            : throw new InvalidDataException($"The theme color '{name}' is missing.");
    }

    public string LThemeRead(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_lThemeColor.TryGetValue(name, out string? found))
        {
            return found;
        }

        return LThemeSpare.TryGetValue(name, out string? spare) ? spare : "#000000";
    }
}
