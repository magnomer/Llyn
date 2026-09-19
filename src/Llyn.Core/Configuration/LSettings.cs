using System.Text.Json.Serialization;

namespace Llyn.Core;

public sealed record LSettings(
    string LSettingsLocalization,
    bool LSettingsRespelled = false,
    bool LSettingsFrequency = true,
    bool LSettingsMorphology = true,
    bool LSettingsEpithet = true,
    bool LSettingsTally = false)
{
    [JsonIgnore]
    public int LSettingsOnline => (LSettingsFrequency ? 1 : 0) + (LSettingsMorphology ? 1 : 0);
}
