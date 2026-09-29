using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    private void PCaptionMinimizeRefine(object sender, RoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(_pWindowSurface);
    }

    private void PCaptionMaximizeRefine(object sender, RoutedEventArgs e)
    {
        LCaption.LCaptionMaximizeToggle(_pWindowSurface);
    }

    private void PCaptionExitRefine(object sender, RoutedEventArgs e)
    {
        SystemCommands.CloseWindow(_pWindowSurface);
    }

    private void PRoofRefine(object sender, MouseButtonEventArgs e)
    {
        LCaption.LCaptionDragRefine(_pWindowSurface, PRoof, e);
    }
}
