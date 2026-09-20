using System.Windows;

namespace Llyn.UIVeneer;

public partial class PSettings
{
    private void PRespellingHandle(object sender, RoutedEventArgs e)
    {
        PSettingsWindow.LWindowRespellingSave(PRespelling.IsChecked == true);
    }
}
