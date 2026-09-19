using System.Windows;

namespace Llyn.UIVeneer;

public partial class PSettings
{
    private void PRespellingHandle(object sender, RoutedEventArgs e)
    {
        _lEngine.LEngineRespellingSave(PRespelling.IsChecked == true);
    }
}
