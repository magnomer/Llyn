using System.Windows;

namespace Llyn.UIVeneer;

public partial class PSettings
{
    private void PMorphologyHandle(object sender, RoutedEventArgs e)
    {
        _lEngine.LEngineMorphologySave(PMorphology.IsChecked == true);
    }
}
