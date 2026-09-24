using System.Windows;
using System.Windows.Input;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PWindow
{
    private void PCaptionMinimizeHandle(object sender, RoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(this);
    }

    private void PCaptionMaximizeHandle(object sender, RoutedEventArgs e)
    {
        LCaption.LCaptionMaximizeToggle(this);
    }

    private void PCaptionExitHandle(object sender, RoutedEventArgs e)
    {
        SystemCommands.CloseWindow(this);
    }

    private void PRoofHandle(object sender, MouseButtonEventArgs e)
    {
        LCaption.LCaptionDragHandle(this, PRoof, e);
    }
}
