using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PMarkerTemplate : ResourceDictionary
{
    private readonly PEditor _pSpeechHost;

    internal PMarkerTemplate(PEditor host)
    {
        _pSpeechHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Marker/PMarkerTemplate.xaml", UriKind.Relative)));
    }

    internal void PMarkerChipHandle(object sender, RoutedEventArgs e)
    {
        _pSpeechHost.PMarkerChipHandle(sender, e);
    }
}
