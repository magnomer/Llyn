using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PLayoutLinkedHandle(object sender, RoutedEventArgs e)
    {
        PSettingsWindow.LWindowLinkedSave(PLayoutLinked.IsChecked == true);
        PLedgerMetaApply();
        _pSettingsHost.PWindowLayout.PLayoutSync();
    }
}
