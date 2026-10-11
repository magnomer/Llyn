namespace Llyn.Core;

public sealed record LSettings(
    string LSettingsLocalization,
    bool LSettingsRespelled = false,
    bool LSettingsFrequency = true,
    bool LSettingsMorphology = true,
    bool LSettingsEpithet = true,
    bool LSettingsTally = false,
    string LSettingsGloss = "English",
    int LSettingsOutpost = 41184,
    string LSettingsWarrant = "",
    bool LSettingsAnalysis = true,
    string LSettingsNotebook = LSettings.LSettingsNotebookFallback)
{
    public const string LSettingsNotebookFallback = "Llyn";

    private readonly string _lSettingsNotebook = LSettingsNotebookResolve(LSettingsNotebook);

    public string LSettingsNotebook
    {
        get => _lSettingsNotebook;
        init => _lSettingsNotebook = LSettingsNotebookResolve(value);
    }

    public int LSettingsOnline => (LSettingsFrequency ? 1 : 0) + (LSettingsMorphology ? 1 : 0);

    private static string LSettingsNotebookResolve(string? notebook)
    {
        string trimmed = notebook?.Trim() ?? string.Empty;
        return trimmed.Length == 0 ? LSettingsNotebookFallback : trimmed;
    }
}
