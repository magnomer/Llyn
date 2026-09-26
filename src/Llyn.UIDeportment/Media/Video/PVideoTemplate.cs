using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PVideoTemplate : ResourceDictionary
{
    private readonly PVideoHost _pVideoHost;

    internal PVideoTemplate(PVideoHost host)
    {
        _pVideoHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Media/Video/PVideoTemplate.xaml", UriKind.Relative)));
    }

    internal void PVideoOpenHandle(object sender, RoutedEventArgs e)
    {
        _pVideoHost.PVideoOpenHandle(sender, e);
    }

    internal void PVideoRemoveHandle(object sender, RoutedEventArgs e)
    {
        _pVideoHost.PVideoRemoveHandle(sender, e);
    }
}
