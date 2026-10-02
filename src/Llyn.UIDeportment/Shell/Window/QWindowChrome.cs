using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public partial class QWindow
{
    private void QCaptionMinimizeRefine(object sender, RoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(_qWindowSurface);
    }

    private void QCaptionMaximizeRefine(object sender, RoutedEventArgs e)
    {
        QCaption.QCaptionMaximizeRefine(_qWindowSurface);
    }

    private void QCaptionExitRefine(object sender, RoutedEventArgs e)
    {
        SystemCommands.CloseWindow(_qWindowSurface);
    }

    private void QRoofRefine(object sender, MouseButtonEventArgs e)
    {
        QCaption.QCaptionDragRefine(_qWindowSurface, QRoof, e);
    }
}
