using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PRespellingObserve(object sender, RoutedEventArgs e)
    {
        PSettingsAtelier.CAtelierLedger.CLedgerRespellingSave(QLook.QLookCheckedRead(PRespelling.IsChecked));
    }
}
