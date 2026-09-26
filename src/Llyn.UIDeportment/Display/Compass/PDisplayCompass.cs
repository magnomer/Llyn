using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PDisplayCompass : ResourceDictionary
{
    private readonly PDisplay _pCompassHost;

    internal PDisplayCompass(PDisplay host)
    {
        _pCompassHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Display/Compass/PDisplayCompass.xaml", UriKind.Relative)));
    }

    internal void PCompassRowHandle(object sender, RoutedEventArgs e)
    {
        _pCompassHost.PCompassRowHandle(sender, e);
    }
}
