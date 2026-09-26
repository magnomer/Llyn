using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PImageTemplate : ResourceDictionary
{
    private readonly PImageHost _pImageHost;

    internal PImageTemplate(PImageHost host)
    {
        _pImageHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Media/Image/PImageTemplate.xaml", UriKind.Relative)));
    }

    internal void PImageOpenHandle(object sender, RoutedEventArgs e)
    {
        _pImageHost.PImageOpenHandle(sender, e);
    }

    internal void PImageRemoveHandle(object sender, RoutedEventArgs e)
    {
        _pImageHost.PImageRemoveHandle(sender, e);
    }
}
