using System;
using System.Windows;

namespace Llyn.UIDeportment;

public class PCollocationTemplate : ResourceDictionary
{
    private readonly PEditor _pCollocationHost;

    internal PCollocationTemplate(PEditor host)
    {
        _pCollocationHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Sentence/PCollocationTemplate.xaml", UriKind.Relative)));
    }

    internal void PImageAddHandle(object sender, RoutedEventArgs e)
    {
        _pCollocationHost.PImageAddHandle(sender, e);
    }

    internal void PVideoAddHandle(object sender, RoutedEventArgs e)
    {
        _pCollocationHost.PVideoAddHandle(sender, e);
    }
}
