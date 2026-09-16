using System.Collections.Generic;

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
    bool LSettingsTally = false);
