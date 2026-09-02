namespace Llyn.Core;

public sealed record LSettings(
    string LSettingsLocalization,
    LWindowState? LSettingsWindow = null);
