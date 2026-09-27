using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PLayoutLinkedHandle(object sender, RoutedEventArgs e)
    {
        PSettingsWindow.LWindowPosture.QPostureLinkedSave(PLayoutLinked.IsChecked == true);
        PLedgerMetaApply();
        _pSettingsHost.PWindowLayout.PLayoutSync();
    }
}
