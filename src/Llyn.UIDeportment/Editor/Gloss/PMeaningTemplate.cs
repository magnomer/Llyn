using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public class PMeaningTemplate : ResourceDictionary
{
    private readonly PEditor _pMeaningHost;

    internal PMeaningTemplate(PEditor host)
    {
        _pMeaningHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Gloss/PMeaningTemplate.xaml", UriKind.Relative)));
    }

    internal void PCardHandle(object sender, RoutedEventArgs e)
    {
        _pMeaningHost.PCardHandle(sender, e);
    }

    internal void PCardDragHandle(object sender, MouseButtonEventArgs e)
    {
        _pMeaningHost.PCardDragHandle(sender, e);
    }

    internal void PImageAddHandle(object sender, RoutedEventArgs e)
    {
        _pMeaningHost.PImageAddHandle(sender, e);
    }

    internal void PVideoAddHandle(object sender, RoutedEventArgs e)
    {
        _pMeaningHost.PVideoAddHandle(sender, e);
    }

    internal void PCardPositionHandle(object sender, MouseButtonEventArgs e)
    {
        _pMeaningHost.PCardPositionHandle(sender, e);
    }

    internal void PCardPositionAccept(object sender, KeyEventArgs e)
    {
        _pMeaningHost.PCardPositionAccept(sender, e);
    }

    internal void PCardPositionCommit(object sender, RoutedEventArgs e)
    {
        _pMeaningHost.PCardPositionCommit(sender, e);
    }
}
