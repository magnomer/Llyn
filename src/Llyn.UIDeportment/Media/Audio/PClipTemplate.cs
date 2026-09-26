using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PClipTemplate : ResourceDictionary
{
    private readonly PEditor _pClipHost;

    internal PClipTemplate(PEditor host)
    {
        _pClipHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Media/Audio/PClipTemplate.xaml", UriKind.Relative)));
    }

    internal void PClipPreviewHandle(object sender, RoutedEventArgs e)
    {
        _pClipHost.PClipPreviewHandle(sender, e);
    }

    internal void PClipSelectorHandle(object sender, RoutedEventArgs e)
    {
        _pClipHost.PClipSelectorHandle(sender, e);
    }
}
