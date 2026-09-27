using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PLayoutLinkedHandle(object sender, RoutedEventArgs e)
    {
        PSettingsPosture.QPostureLinkedSave(PLayoutLinked.IsChecked == true);
        PLedgerMetaApply();
        _pSettingsHost.PWindowLayout.PLayoutSync();
    }
}
