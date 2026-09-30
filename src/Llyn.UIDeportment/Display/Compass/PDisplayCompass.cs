using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PDisplayCompass : ResourceDictionary
{
    private QCompass _pCompassDriver = null!;

    internal PDisplayCompass()
    {
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Display/Compass/PDisplayCompass.xaml", UriKind.Relative)));
    }

    internal void PCompassIntroduce(QCompass driver)
    {
        _pCompassDriver = driver;
    }

    internal void PCompassRowRefine(object sender, RoutedEventArgs e)
    {
        _pCompassDriver.QCompassRowRefine(sender);
    }
}
