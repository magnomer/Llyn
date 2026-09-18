using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PSettings : UserControl
{
    private PWindow _pSettingsHost = null!;

    private LEngine _lEngine = null!;

    public PSettings()
    {
        InitializeComponent();
    }

    internal void PSettingsAttach(PWindow host, LEngine engine)
    {
        _pSettingsHost = host;
        _lEngine = engine;
        _lEngine.LEngineObserverAttach(new PObserver(this, PSettingsBulletinHandle));

        PSettingsSync();
        PLedgerBuild();
        PDialShow("Workspace");
    }

    internal void PSettingsSync()
    {
        LSettings settings = _lEngine.LEngineSettingsRead();
        PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
        PLocalization.SelectedValue = PLocalizationLoader.PLocalizationLoaderNormalize(settings.LSettingsLocalization);
        PRespelling.IsChecked = settings.LSettingsRespelled;
        PSettingsEpithet.IsChecked = settings.LSettingsEpithet;
        PFrequency.IsChecked = settings.LSettingsFrequency;
        PMorphology.IsChecked = settings.LSettingsMorphology;
        PLayoutLinked.IsChecked = settings.LSettingsLinked;
    }

    private void PSettingsBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject != LSubject.LSubjectSettings)
        {
            return;
        }

        LSettings settings = _lEngine.LEngineSettingsRead();
        PLocalizationApply(PLocalizationLoader.PLocalizationLoaderNormalize(settings.LSettingsLocalization));
        PLedgerMetaApply();

        if (settings.LSettingsLinked)
        {
            _pSettingsHost.PWindowLayout.PLayoutSync();
        }
    }
}
