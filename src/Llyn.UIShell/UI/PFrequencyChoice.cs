using System.Windows;

namespace Llyn.UIShell;

public partial class PSettings
{
    private void PFrequencyHandle(object sender, RoutedEventArgs e)
    {
        if (_pSettingsReady)
        {
            _lEngine.LEngineFrequencySave(PFrequency.IsChecked == true);
            PLedgerMetaApply();
        }
    }
}
