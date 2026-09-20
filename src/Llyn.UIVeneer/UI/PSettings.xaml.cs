using System.Windows.Controls;
using Llyn.Application;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PSettings : UserControl
{
    private PWindow _pSettingsHost = null!;

    private LWindow PSettingsWindow => _pSettingsHost.PWindowDeportment;

    public PSettings()
    {
        InitializeComponent();
    }

    internal void PSettingsAttach(PWindow host)
    {
        _pSettingsHost = host;
        PSettingsWindow.LWindowObserverAttach(new PObserver(this, PSettingsBulletinHandle));

        PSettingsSync();
        PLedgerBuild();
        PDialShow("Workspace");
    }

    internal void PSettingsSync()
    {
        LSettings settings = PSettingsWindow.LWindowSettingsRead();
        PWorkspacePath.Text = PSettingsWindow.LWindowWorkspaceRead();
        PLocalization.SelectedValue =
            PSettingsWindow.LWindowLocalizationRead();
        PRespelling.IsChecked = settings.LSettingsRespelled;
        PSettingsEpithet.IsChecked = settings.LSettingsEpithet;
        PFrequency.IsChecked = settings.LSettingsFrequency;
        PMorphology.IsChecked = settings.LSettingsMorphology;
        PLayoutLinked.IsChecked = _pSettingsHost.PWindowPosture.LPostureRead().LPostureStateLinked;
    }

    private void PSettingsBulletinHandle(LBulletin bulletin)
    {
        if (!bulletin.LBulletinMatch(LSubject.LSubjectSettings))
        {
            return;
        }

        PLocalizationApply(PSettingsWindow.LWindowLocalizationRead());
        PLedgerMetaApply();
    }
}
