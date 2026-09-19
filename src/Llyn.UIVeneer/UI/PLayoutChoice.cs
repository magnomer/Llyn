using System.Windows;

namespace Llyn.UIVeneer;

public partial class PSettings
{
    private void PLayoutLinkedHandle(object sender, RoutedEventArgs e)
    {
        _pSettingsHost.PWindowPosture.LPostureLinkedSave(PLayoutLinked.IsChecked == true);
        PLedgerMetaApply();
        _pSettingsHost.PWindowLayout.PLayoutSync();
    }
}
