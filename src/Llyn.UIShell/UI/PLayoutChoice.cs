using System.Windows;

namespace Llyn.UIShell;

public partial class PSettings
{
    private void PLayoutLinkedHandle(object sender, RoutedEventArgs e)
    {
        if (!_pSettingsReady)
        {
            return;
        }

        bool linked = PLayoutLinked.IsChecked == true;
        _lEngine.LEngineLinkedSave(linked);
        PLedgerMetaApply();

        if (linked)
        {
            _pSettingsHost.PWindowLayout.PLayoutSync();
        }
    }
}
