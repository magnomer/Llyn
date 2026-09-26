using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public class PLabelTemplate : ResourceDictionary
{
    private readonly PEditor _pLabelHost;

    internal PLabelTemplate(PEditor host)
    {
        _pLabelHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Label/PLabelTemplate.xaml", UriKind.Relative)));
    }

    internal void PLabelChipHandle(object sender, RoutedEventArgs e)
    {
        _pLabelHost.PLabelChipHandle(sender, e);
    }

    internal void PLabelCaretHandle(object sender, KeyEventArgs e)
    {
        _pLabelHost.PLabelCaretHandle(sender, e);
    }

    internal void PLabelCloseHandle(object sender, RoutedEventArgs e)
    {
        _pLabelHost.PLabelCloseHandle(sender, e);
    }

    internal void PLabelFocusHandle(object sender, MouseButtonEventArgs e)
    {
        _pLabelHost.PLabelFocusHandle(sender, e);
    }
}
