using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PMorphologyHandle(object sender, RoutedEventArgs e)
    {
        PSettingsWindow.LWindowMorphologySave(PMorphology.IsChecked == true);
    }
}
