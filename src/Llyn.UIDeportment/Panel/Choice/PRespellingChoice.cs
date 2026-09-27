using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PRespellingHandle(object sender, RoutedEventArgs e)
    {
        PSettingsAtelier.CAtelierLedger.CLedgerRespellingSave(PRespelling.IsChecked == true);
    }
}
