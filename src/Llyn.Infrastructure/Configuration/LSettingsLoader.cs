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
    private const string LSettingsLoaderRespelling = "respelling";
    private const string LSettingsLoaderFrequency = "frequency";
    private const string LSettingsLoaderMorphology = "morphology";
    private const string LSettingsLoaderEpithet = "epithet";
    private const string LSettingsLoaderTally = "tally";
    private const string LSettingsLoaderDefault = "en";

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

            bool respelled =
                document.RootElement.TryGetProperty(LSettingsLoaderRespelling, out JsonElement flag) &&
                flag.ValueKind == JsonValueKind.True;

            bool frequency =
                !document.RootElement.TryGetProperty(LSettingsLoaderFrequency, out JsonElement fetch) ||
                fetch.ValueKind != JsonValueKind.False;

            bool morphology =
                !document.RootElement.TryGetProperty(LSettingsLoaderMorphology, out JsonElement inflect) ||
                inflect.ValueKind != JsonValueKind.False;

            bool epithet =
                !document.RootElement.TryGetProperty(LSettingsLoaderEpithet, out JsonElement byname) ||
                byname.ValueKind != JsonValueKind.False;

            bool tally =
                document.RootElement.TryGetProperty(LSettingsLoaderTally, out JsonElement set) &&
                set.ValueKind == JsonValueKind.True;

            return new LSettings(localization, respelled, frequency, morphology, epithet, tally);
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
            [LSettingsLoaderRespelling] = settings.LSettingsRespelled,
            [LSettingsLoaderFrequency] = settings.LSettingsFrequency,
            [LSettingsLoaderMorphology] = settings.LSettingsMorphology,
            [LSettingsLoaderEpithet] = settings.LSettingsEpithet,
            [LSettingsLoaderTally] = settings.LSettingsTally
        };

        string pending = Path.Combine(root, LSettingsLoaderPending);
        string path = Path.Combine(root, LSettingsLoaderFile);
        File.WriteAllText(
            pending, JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
        LWorkspaceRoot.LWorkspacePendingCommit(pending, path);
    }
}
