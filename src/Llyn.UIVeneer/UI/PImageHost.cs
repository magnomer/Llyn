using System.Windows;

namespace Llyn.UIVeneer;

internal interface PImageHost
{
    void PImageOpenHandle(object sender, RoutedEventArgs e);

    void PImageRemoveHandle(object sender, RoutedEventArgs e);
}
