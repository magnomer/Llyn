using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PMorphologyHandle(object sender, RoutedEventArgs e)
    {
        PSettingsAtelier.CAtelierLedger.CLedgerMorphologySave(PMorphology.IsChecked == true);
    }
}
