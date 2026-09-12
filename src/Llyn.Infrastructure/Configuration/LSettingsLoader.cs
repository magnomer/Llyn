using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSettingsLoader
{
    private const string LSettingsLoaderFile = "settings.json";
    private const string LSettingsLoaderLocalization = "localization";
    private const string LSettingsLoaderWindow = "window";
    private const string LSettingsLoaderVolume = "volume";
    private const string LSettingsLoaderRespelling = "respelling";
    private const string LSettingsLoaderDefault = "en";
    private const double LSettingsLoaderLoudest = 1;

    public static LSettings LSettingsLoaderLoad(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        string path = Path.Combine(root, LSettingsLoaderFile);
        if (!File.Exists(path))
        {
            return new LSettings(LSettingsLoaderDefault);
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return new LSettings(LSettingsLoaderDefault);
            }

            string localization =
                document.RootElement.TryGetProperty(LSettingsLoaderLocalization, out JsonElement value) &&
                value.ValueKind == JsonValueKind.String
                    ? value.GetString()!
                    : LSettingsLoaderDefault;

            LWindowState? window = document.RootElement.TryGetProperty(LSettingsLoaderWindow, out JsonElement block)
                ? LWindowLoader.LWindowLoaderRead(block)
                : null;

            double volume =
                document.RootElement.TryGetProperty(LSettingsLoaderVolume, out JsonElement level) &&
                level.ValueKind == JsonValueKind.Number
                    ? Math.Clamp(level.GetDouble(), 0, LSettingsLoaderLoudest)
                    : LSettingsLoaderLoudest;

            bool respelled =
                document.RootElement.TryGetProperty(LSettingsLoaderRespelling, out JsonElement flag) &&
                flag.ValueKind == JsonValueKind.True;

            return new LSettings(localization, window, volume, respelled);
        }
        catch (JsonException)
        {
            return new LSettings(LSettingsLoaderDefault);
        }
        catch (IOException)
        {
            return new LSettings(LSettingsLoaderDefault);
        }
    }

    public static void LSettingsLoaderSave(string root, LSettings settings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentNullException.ThrowIfNull(settings);

        Directory.CreateDirectory(root);

        Dictionary<string, object> payload = new(StringComparer.Ordinal)
        {
            [LSettingsLoaderLocalization] = settings.LSettingsLocalization,
            [LSettingsLoaderVolume] = Math.Clamp(settings.LSettingsVolume, 0, LSettingsLoaderLoudest),
            [LSettingsLoaderRespelling] = settings.LSettingsRespelled
        };

        if (settings.LSettingsWindow is LWindowState window)
        {
            payload[LSettingsLoaderWindow] = LWindowLoader.LWindowLoaderCreate(window);
        }

        string path = Path.Combine(root, LSettingsLoaderFile);
        File.WriteAllText(path, JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
    }
}
