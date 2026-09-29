using System.Windows;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PLayoutLinkedObserve(object sender, RoutedEventArgs e)
    {
        PSettingsPosture.QPostureLinkedSave(QLook.QLookCheckedRead(PLayoutLinked.IsChecked));
    }
}
