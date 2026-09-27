namespace Llyn.Conduct;

public sealed record CSettings(
    string CSettingsLocalization,
    bool CSettingsRespelled,
    bool CSettingsFrequency,
    bool CSettingsMorphology,
    bool CSettingsEpithet,
    int CSettingsOnline);
