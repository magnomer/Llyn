using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    private void PCaptionMinimizeHandle(object sender, RoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(_pWindowSurface);
    }

    private void PCaptionMaximizeHandle(object sender, RoutedEventArgs e)
    {
        LCaption.LCaptionMaximizeToggle(_pWindowSurface);
    }

    private void PCaptionExitHandle(object sender, RoutedEventArgs e)
    {
        SystemCommands.CloseWindow(_pWindowSurface);
    }

    private void PRoofHandle(object sender, MouseButtonEventArgs e)
    {
        LCaption.LCaptionDragHandle(_pWindowSurface, PRoof, e);
    }
}
