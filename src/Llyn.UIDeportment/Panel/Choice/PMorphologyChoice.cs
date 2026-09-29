using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PMorphologyObserve(object sender, RoutedEventArgs e)
    {
        PSettingsAtelier.CAtelierLedger.CLedgerMorphologySave(QLook.QLookCheckedRead(PMorphology.IsChecked));
    }
}
