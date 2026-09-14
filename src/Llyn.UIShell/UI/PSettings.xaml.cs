using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PSettings : UserControl
{
    private PWindow _pSettingsHost = null!;

    private LEngine _lEngine = null!;

    private bool _pSettingsReady;

    public PSettings()
    {
        InitializeComponent();
    }

    internal void PSettingsAttach(PWindow host, LEngine engine)
    {
        _pSettingsHost = host;
        _lEngine = engine;

        PSettingsSync();
        PLedgerBuild();
        PDialShow("Workspace");
    }

    internal void PSettingsSync()
    {
        _pSettingsReady = false;

        LSettings settings = _lEngine.LEngineSettingsRead();
        PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
        PLocalization.SelectedValue = PLocalizationLoader.PLocalizationLoaderNormalize(settings.LSettingsLocalization);
        PRespelling.IsChecked = settings.LSettingsRespelled;
        PFrequency.IsChecked = settings.LSettingsFrequency;
        PMorphology.IsChecked = settings.LSettingsMorphology;
        PLayoutLinked.IsChecked = settings.LSettingsLinked;
        PDialFooterApply();

        _pSettingsReady = true;
    }
}
