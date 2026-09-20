using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LSettingsLoader : LSettingsVault
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

    private readonly string _lSettingsLoaderRoot;

    public LSettingsLoader(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        _lSettingsLoaderRoot = root;
    }

    public bool LSettingsExist()
    {
        return File.Exists(Path.Combine(_lSettingsLoaderRoot, LSettingsLoaderFile));
    }

    public LSettings LSettingsRead()
    {
        string path = Path.Combine(_lSettingsLoaderRoot, LSettingsLoaderFile);
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
                File.Copy(path, Path.Combine(_lSettingsLoaderRoot, LSettingsLoaderBroken), true);
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

    public void LSettingsSave(LSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        Dictionary<string, object> payload = new(StringComparer.Ordinal)
        {
            [LSettingsLoaderLocalization] = settings.LSettingsLocalization,
            [LSettingsLoaderRespelling] = settings.LSettingsRespelled,
            [LSettingsLoaderFrequency] = settings.LSettingsFrequency,
            [LSettingsLoaderMorphology] = settings.LSettingsMorphology,
            [LSettingsLoaderEpithet] = settings.LSettingsEpithet,
            [LSettingsLoaderTally] = settings.LSettingsTally
        };

        string pending = Path.Combine(_lSettingsLoaderRoot, LSettingsLoaderPending);
        string path = Path.Combine(_lSettingsLoaderRoot, LSettingsLoaderFile);
        try
        {
            Directory.CreateDirectory(_lSettingsLoaderRoot);
            File.WriteAllText(
                pending, JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
            LWorkspaceRoot.LWorkspacePendingCommit(pending, path);
        }
        catch (IOException exception)
        {
            throw new LVaultFault(exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new LVaultFault(exception);
        }
    }
}
