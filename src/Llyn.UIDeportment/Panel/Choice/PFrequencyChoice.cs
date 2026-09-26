using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PFrequencyHandle(object sender, RoutedEventArgs e)
    {
        PSettingsWindow.LWindowFrequencySave(PFrequency.IsChecked == true);
    }
}
