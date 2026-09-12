using System.Windows;

namespace Llyn.UIShell;

public partial class PSettings
{
    private void PRespellingHandle(object sender, RoutedEventArgs e)
    {
        if (_pSettingsReady)
        {
            _lEngine.LEngineRespellingSave(PRespelling.IsChecked == true);
        }
    }
}
