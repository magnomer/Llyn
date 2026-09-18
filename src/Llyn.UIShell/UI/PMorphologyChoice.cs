using System.Windows;

namespace Llyn.UIShell;

public partial class PSettings
{
    private void PMorphologyHandle(object sender, RoutedEventArgs e)
    {
        _lEngine.LEngineMorphologySave(PMorphology.IsChecked == true);
    }
}
