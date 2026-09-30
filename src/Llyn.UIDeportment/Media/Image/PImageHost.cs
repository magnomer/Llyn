using System.Windows;

namespace Llyn.UIDeportment;

internal interface PImageHost
{
    void PImageOpenObserve(object sender, RoutedEventArgs e);

    void PImageRemoveObserve(object sender, RoutedEventArgs e);
}
