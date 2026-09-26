using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PLanguageTemplate : ResourceDictionary
{
    private readonly PEditor _pLanguageHost;

    internal PLanguageTemplate(PEditor host)
    {
        _pLanguageHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Gloss/PLanguageTemplate.xaml", UriKind.Relative)));
    }

    internal void PSpeakerHandle(object sender, RoutedEventArgs e)
    {
        _pLanguageHost.PSpeakerHandle(sender, e);
    }
}
