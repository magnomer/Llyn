using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Llyn.Core;

public sealed record LSettings(
    string LSettingsLocalization,
    LWindowState? LSettingsWindow = null,
    double LSettingsVolume = 1,
    bool LSettingsRespelled = false,
    bool LSettingsFrequency = true,
    bool LSettingsMorphology = true,
    IReadOnlyList<LLayout>? LSettingsLayout = null,
    bool LSettingsLinked = true,
    string? LSettingsMode = null,
    bool LSettingsSplit = false,
    bool LSettingsEpithet = true,
    bool LSettingsTally = false)
{
    [JsonIgnore]
    public int LSettingsOnline => (LSettingsFrequency ? 1 : 0) + (LSettingsMorphology ? 1 : 0);

    public bool LSettingsVolumeMatch(double volume)
    {
        return LSettingsVolume == volume;
    }

    public bool LSettingsModeMatch(string? mode)
    {
        return string.Equals(LSettingsMode, mode, StringComparison.Ordinal);
    }
}
