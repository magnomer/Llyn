using System.Windows;

namespace Llyn.UIShell;

public partial class PSettings
{
    private void PSettingsEpithetHandle(object sender, RoutedEventArgs e)
    {
        if (_pSettingsReady)
        {
            _lEngine.LEngineEpithetSave(PSettingsEpithet.IsChecked == true);
            PLedgerMetaApply();
        }
    }
}
