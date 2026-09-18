using System.Windows;

namespace Llyn.UIShell;

public partial class PSettings
{
    private void PLayoutLinkedHandle(object sender, RoutedEventArgs e)
    {
        _lEngine.LEngineLinkedSave(PLayoutLinked.IsChecked == true);
    }
}
