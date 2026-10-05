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
    private const string LSettingsLoaderGloss = "gloss";
    private const string LSettingsLoaderFanqie = "fanqie";
    private const string LSettingsLoaderScript = "script";
    private const string LSettingsLoaderOutpost = "outpost";
    private const string LSettingsLoaderWarrant = "warrant";
    private const string LSettingsLoaderDefault = "en";

    private readonly string _lSettingsLoaderRoot;
    private bool _lSettingsLoaderUnread;

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
        _lSettingsLoaderUnread = false;
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

            bool fanqie =
                document.RootElement.TryGetProperty(LSettingsLoaderFanqie, out JsonElement rime) &&
                rime.ValueKind == JsonValueKind.True;

            bool script =
                document.RootElement.TryGetProperty(LSettingsLoaderScript, out JsonElement writing) &&
                writing.ValueKind == JsonValueKind.True;

            int outpost =
                document.RootElement.TryGetProperty(LSettingsLoaderOutpost, out JsonElement port) &&
                port.ValueKind == JsonValueKind.Number &&
                port.TryGetInt32(out int number) &&
                number is >= 1 and <= 65535
                    ? number
                    : 41184;

            string warrant =
                document.RootElement.TryGetProperty(LSettingsLoaderWarrant, out JsonElement token) &&
                token.ValueKind == JsonValueKind.String
                    ? token.GetString()!
                    : string.Empty;

            LSettings read = new(localization, respelled, frequency, morphology, epithet, tally)
            {
                LSettingsFanqieOpened = fanqie,
                LSettingsScriptOpened = script,
                LSettingsOutpost = outpost,
                LSettingsWarrant = warrant,
            };
            return document.RootElement.TryGetProperty(LSettingsLoaderGloss, out JsonElement speech)
                && speech.ValueKind == JsonValueKind.String
                && speech.GetString() is { Length: > 0 } gloss
                ? read with { LSettingsGloss = gloss }
                : read;
        }
        catch (JsonException)
        {
            try
            {
                File.Copy(path, Path.Combine(_lSettingsLoaderRoot, LSettingsLoaderBroken), true);
            }
            catch (IOException)
            {
                _lSettingsLoaderUnread = true;
            }
            catch (UnauthorizedAccessException)
            {
                _lSettingsLoaderUnread = true;
            }

            return new LSettings(LSettingsLoaderDefault);
        }
        catch (IOException)
        {
            _lSettingsLoaderUnread = true;
            return new LSettings(LSettingsLoaderDefault);
        }
        catch (UnauthorizedAccessException)
        {
            _lSettingsLoaderUnread = true;
            return new LSettings(LSettingsLoaderDefault);
        }
    }

    public void LSettingsSave(LSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (_lSettingsLoaderUnread)
        {
            throw new LVaultFault(new IOException("settings.json could not be read, so it is not overwritten."));
        }

        Dictionary<string, object> payload = new(StringComparer.Ordinal)
        {
            [LSettingsLoaderLocalization] = settings.LSettingsLocalization,
            [LSettingsLoaderRespelling] = settings.LSettingsRespelled,
            [LSettingsLoaderFrequency] = settings.LSettingsFrequency,
            [LSettingsLoaderMorphology] = settings.LSettingsMorphology,
            [LSettingsLoaderEpithet] = settings.LSettingsEpithet,
            [LSettingsLoaderTally] = settings.LSettingsTally,
            [LSettingsLoaderGloss] = settings.LSettingsGloss,
            [LSettingsLoaderFanqie] = settings.LSettingsFanqieOpened,
            [LSettingsLoaderScript] = settings.LSettingsScriptOpened,
            [LSettingsLoaderOutpost] = settings.LSettingsOutpost,
            [LSettingsLoaderWarrant] = settings.LSettingsWarrant
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
