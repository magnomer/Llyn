using System.Windows;

namespace Llyn.UIVeneer;

internal interface PVideoHost
{
    void PVideoOpenHandle(object sender, RoutedEventArgs e);

    void PVideoRemoveHandle(object sender, RoutedEventArgs e);
}
