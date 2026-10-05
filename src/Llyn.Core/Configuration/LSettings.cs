namespace Llyn.Core;

public sealed record LSettings(
    string LSettingsLocalization,
    bool LSettingsRespelled = false,
    bool LSettingsFrequency = true,
    bool LSettingsMorphology = true,
    bool LSettingsEpithet = true,
    bool LSettingsTally = false,
    string LSettingsGloss = "English",
    bool LSettingsFanqieOpened = false,
    bool LSettingsScriptOpened = false,
    int LSettingsOutpost = 41184,
    string LSettingsWarrant = "")
{
    public int LSettingsOnline => (LSettingsFrequency ? 1 : 0) + (LSettingsMorphology ? 1 : 0);
}
