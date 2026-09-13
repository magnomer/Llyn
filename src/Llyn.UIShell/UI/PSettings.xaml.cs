using System.Windows.Controls;
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

        PWorkspacePath.Text = engine.LEngineWorkspaceRead();
        PLocalization.SelectedValue = engine.LEngineSettingsRead().LSettingsLocalization;
        PRespelling.IsChecked = engine.LEngineSettingsRead().LSettingsRespelled;
        PFrequency.IsChecked = engine.LEngineSettingsRead().LSettingsFrequency;
        PMorphology.IsChecked = engine.LEngineSettingsRead().LSettingsMorphology;
        PLayoutLinked.IsChecked = engine.LEngineSettingsRead().LSettingsLinked;
        _pSettingsReady = true;
    }
}
