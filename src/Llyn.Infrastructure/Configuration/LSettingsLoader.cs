using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

/// <summary>
/// Loads and saves the user's <see cref="LSettings"/> as <c>settings.json</c> inside a workspace
/// folder. The workspace folder is supplied by the caller (resolved through
/// <see cref="LWorkspaceRoot"/>); this loader never decides where the workspace is, only how the
/// settings file within it is read and written. A missing or unreadable file yields defaults so the
/// program always starts.
/// </summary>
public static class LSettingsLoader
{
    private const string LSettingsLoaderFile = "settings.json";
    private const string LSettingsLoaderLocalization = "localization";
    private const string LSettingsLoaderDefault = "en";

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
            string localization =
                document.RootElement.ValueKind == JsonValueKind.Object &&
                document.RootElement.TryGetProperty(LSettingsLoaderLocalization, out JsonElement value) &&
                value.ValueKind == JsonValueKind.String
                    ? value.GetString()!
                    : LSettingsLoaderDefault;

            return new LSettings(localization);
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

        // Persisted keys are a data contract, so they stay lowercase and independent of member names.
        Dictionary<string, string> payload = new(StringComparer.Ordinal)
        {
            [LSettingsLoaderLocalization] = settings.LSettingsLocalization
        };

        string path = Path.Combine(root, LSettingsLoaderFile);
        File.WriteAllText(path, JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
    }
}
