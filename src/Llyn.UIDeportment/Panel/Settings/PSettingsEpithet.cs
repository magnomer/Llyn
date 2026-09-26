using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PSettingsEpithetHandle(object sender, RoutedEventArgs e)
    {
        PSettingsWindow.LWindowEpithetSave(PSettingsEpithet.IsChecked == true);
    }
}
