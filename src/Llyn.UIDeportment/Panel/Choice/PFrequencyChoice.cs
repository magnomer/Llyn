using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PFrequencyHandle(object sender, RoutedEventArgs e)
    {
        PSettingsAtelier.CAtelierLedger.CLedgerFrequencySave(PFrequency.IsChecked == true);
    }
}
