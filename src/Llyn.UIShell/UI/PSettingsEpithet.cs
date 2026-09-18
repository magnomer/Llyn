using System.Windows;

namespace Llyn.UIShell;

public partial class PSettings
{
    private void PSettingsEpithetHandle(object sender, RoutedEventArgs e)
    {
        _lEngine.LEngineEpithetSave(PSettingsEpithet.IsChecked == true);
    }
}
