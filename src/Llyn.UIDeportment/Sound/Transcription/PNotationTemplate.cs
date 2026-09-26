using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PNotationTemplate : ResourceDictionary
{
    private readonly PEditor _pNotationHost;

    internal PNotationTemplate(PEditor host)
    {
        _pNotationHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Sound/Transcription/PNotationTemplate.xaml", UriKind.Relative)));
    }

    internal void PNotationSelectorHandle(object sender, RoutedEventArgs e)
    {
        _pNotationHost.PNotationSelectorHandle(sender, e);
    }
}
