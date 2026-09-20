using System.Windows;

namespace Llyn.UIVeneer;

public partial class PSettings
{
    private void PMorphologyHandle(object sender, RoutedEventArgs e)
    {
        PSettingsWindow.LWindowMorphologySave(PMorphology.IsChecked == true);
    }
}
