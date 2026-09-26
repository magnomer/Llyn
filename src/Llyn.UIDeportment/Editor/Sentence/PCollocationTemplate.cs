using System;
using System.Windows;
using System.Windows.Input;

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

    internal void PCardHandle(object sender, RoutedEventArgs e)
    {
        _pCollocationHost.PCardHandle(sender, e);
    }

    internal void PCardDragHandle(object sender, MouseButtonEventArgs e)
    {
        _pCollocationHost.PCardDragHandle(sender, e);
    }

    internal void PImageAddHandle(object sender, RoutedEventArgs e)
    {
        _pCollocationHost.PImageAddHandle(sender, e);
    }

    internal void PVideoAddHandle(object sender, RoutedEventArgs e)
    {
        _pCollocationHost.PVideoAddHandle(sender, e);
    }

    internal void PCardPositionHandle(object sender, MouseButtonEventArgs e)
    {
        _pCollocationHost.PCardPositionHandle(sender, e);
    }

    internal void PCardPositionAccept(object sender, KeyEventArgs e)
    {
        _pCollocationHost.PCardPositionAccept(sender, e);
    }

    internal void PCardPositionCommit(object sender, RoutedEventArgs e)
    {
        _pCollocationHost.PCardPositionCommit(sender, e);
    }
}
