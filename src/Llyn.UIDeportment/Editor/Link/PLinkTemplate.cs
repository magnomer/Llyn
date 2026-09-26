using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public class PLinkTemplate : ResourceDictionary
{
    private readonly PEditor _pLinkHost;

    internal PLinkTemplate(PEditor host)
    {
        _pLinkHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Link/PLinkTemplate.xaml", UriKind.Relative)));
    }

    internal void PLinkDropHandle(object sender, RoutedEventArgs e)
    {
        _pLinkHost.PLinkDropHandle(sender, e);
    }

    internal void PLinkCaretHandle(object sender, KeyEventArgs e)
    {
        _pLinkHost.PLinkCaretHandle(sender, e);
    }

    internal void PLinkCloseHandle(object sender, RoutedEventArgs e)
    {
        _pLinkHost.PLinkCloseHandle(sender, e);
    }

    internal void PLinkFocusHandle(object sender, MouseButtonEventArgs e)
    {
        _pLinkHost.PLinkFocusHandle(sender, e);
    }
}
