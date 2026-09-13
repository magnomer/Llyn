namespace Llyn.Core;

public sealed record LSettings(
    string LSettingsLocalization,
    LWindowState? LSettingsWindow = null,
    double LSettingsVolume = 1,
    bool LSettingsRespelled = false,
    bool LSettingsFrequency = true,
    bool LSettingsMorphology = true);
