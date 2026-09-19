using System.Windows;

namespace Llyn.UIVeneer;

public partial class PSettings
{
    private void PLayoutLinkedHandle(object sender, RoutedEventArgs e)
    {
        _lEngine.LEngineLinkedSave(PLayoutLinked.IsChecked == true);
    }
}
