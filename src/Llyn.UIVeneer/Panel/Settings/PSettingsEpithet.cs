using System.Windows;

namespace Llyn.UIVeneer;

public partial class PSettings
{
    private void PSettingsEpithetHandle(object sender, RoutedEventArgs e)
    {
        PSettingsWindow.LWindowEpithetSave(PSettingsEpithet.IsChecked == true);
    }
}
