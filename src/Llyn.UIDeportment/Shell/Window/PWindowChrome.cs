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
        QCaption.QCaptionMaximizeRefine(_pWindowSurface);
    }

    private void PCaptionExitRefine(object sender, RoutedEventArgs e)
    {
        SystemCommands.CloseWindow(_pWindowSurface);
    }

    private void PRoofRefine(object sender, MouseButtonEventArgs e)
    {
        QCaption.QCaptionDragRefine(_pWindowSurface, PRoof, e);
    }
}
