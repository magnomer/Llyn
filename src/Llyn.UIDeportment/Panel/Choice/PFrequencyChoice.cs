using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PFrequencyHandle(object sender, RoutedEventArgs e)
    {
        PSettingsWindow.LWindowWorkspace.QWorkspaceFrequencySave(PFrequency.IsChecked == true);
    }
}
