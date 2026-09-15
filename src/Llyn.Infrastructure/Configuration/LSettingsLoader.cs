using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSettingsLoader
{
    private const string LSettingsLoaderFile = "settings.json";
    private const string LSettingsLoaderPending = "settings.json.tmp";
    private const string LSettingsLoaderBroken = "settings.broken.json";
    private const string LSettingsLoaderLocalization = "localization";
    private const string LSettingsLoaderWindow = "window";
    private const string LSettingsLoaderVolume = "volume";
    private const string LSettingsLoaderRespelling = "respelling";
    private const string LSettingsLoaderFrequency = "frequency";
    private const string LSettingsLoaderMorphology = "morphology";
    private const string LSettingsLoaderLayout = "layout";
    private const string LSettingsLoaderLinked = "linked";
    private const string LSettingsLoaderMode = "mode";
    private const string LSettingsLoaderSplit = "split";
    private const string LSettingsLoaderEditor = "Editor";
    private const string LSettingsLoaderDisplay = "Display";
    private const string LSettingsLoaderDefault = "en";
    private const double LSettingsLoaderLoudest = 1;

    public static bool LSettingsLoaderExist(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        return File.Exists(Path.Combine(root, LSettingsLoaderFile));
    }

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

            bool frequency =
                !document.RootElement.TryGetProperty(LSettingsLoaderFrequency, out JsonElement fetch) ||
                fetch.ValueKind != JsonValueKind.False;

            bool morphology =
                !document.RootElement.TryGetProperty(LSettingsLoaderMorphology, out JsonElement inflect) ||
                inflect.ValueKind != JsonValueKind.False;

            IReadOnlyList<LLayout>? layout =
                document.RootElement.TryGetProperty(LSettingsLoaderLayout, out JsonElement panels)
                    ? LLayoutLoader.LLayoutLoaderRead(panels)
                    : null;

            bool linked =
                !document.RootElement.TryGetProperty(LSettingsLoaderLinked, out JsonElement share) ||
                share.ValueKind != JsonValueKind.False;

            string? mode =
                document.RootElement.TryGetProperty(LSettingsLoaderMode, out JsonElement tab) &&
                tab.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(tab.GetString())
                    ? tab.GetString()
                    : null;

            bool split =
                document.RootElement.TryGetProperty(LSettingsLoaderSplit, out JsonElement side) &&
                side.ValueKind == JsonValueKind.String &&
                string.Equals(side.GetString(), LSettingsLoaderEditor, StringComparison.Ordinal);

            return new LSettings(
                localization, window, volume, respelled, frequency, morphology, layout, linked, mode, split);
        }
        catch (JsonException)
        {
            try
            {
                File.Copy(path, Path.Combine(root, LSettingsLoaderBroken), true);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }

            return new LSettings(LSettingsLoaderDefault);
        }
        catch (IOException)
        {
            return new LSettings(LSettingsLoaderDefault);
        }
        catch (UnauthorizedAccessException)
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
            [LSettingsLoaderRespelling] = settings.LSettingsRespelled,
            [LSettingsLoaderFrequency] = settings.LSettingsFrequency,
            [LSettingsLoaderMorphology] = settings.LSettingsMorphology,
            [LSettingsLoaderLinked] = settings.LSettingsLinked,
            [LSettingsLoaderSplit] = settings.LSettingsSplit ? LSettingsLoaderEditor : LSettingsLoaderDisplay
        };

        if (!string.IsNullOrWhiteSpace(settings.LSettingsMode))
        {
            payload[LSettingsLoaderMode] = settings.LSettingsMode;
        }

        if (settings.LSettingsLayout is { Count: > 0 } layout)
        {
            payload[LSettingsLoaderLayout] = LLayoutLoader.LLayoutLoaderCreate(layout);
        }

        if (settings.LSettingsWindow is LWindowState window)
        {
            payload[LSettingsLoaderWindow] = LWindowLoader.LWindowLoaderCreate(window);
        }

        string pending = Path.Combine(root, LSettingsLoaderPending);
        string path = Path.Combine(root, LSettingsLoaderFile);
        File.WriteAllText(
            pending, JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
        LWorkspaceRoot.LWorkspacePendingCommit(pending, path);
    }
}
