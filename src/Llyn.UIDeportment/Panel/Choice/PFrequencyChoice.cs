using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PFrequencyObserve(object sender, RoutedEventArgs e)
    {
        PSettingsAtelier.CAtelierLedger.CLedgerFrequencySave(QLook.QLookCheckedRead(PFrequency.IsChecked));
    }
}
