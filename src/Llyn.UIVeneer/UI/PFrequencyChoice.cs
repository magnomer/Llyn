using System.Windows;

namespace Llyn.UIVeneer;

public partial class PSettings
{
    private void PFrequencyHandle(object sender, RoutedEventArgs e)
    {
        _lEngine.LEngineFrequencySave(PFrequency.IsChecked == true);
    }
}
