using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public class PContextTemplate : ResourceDictionary
{
    private readonly PEditor _pContextHost;

    internal PContextTemplate(PEditor host)
    {
        _pContextHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Context/PContextTemplate.xaml", UriKind.Relative)));
    }

    internal void PContextChipHandle(object sender, RoutedEventArgs e)
    {
        _pContextHost.PContextChipHandle(sender, e);
    }

    internal void PContextCaretHandle(object sender, KeyEventArgs e)
    {
        _pContextHost.PContextCaretHandle(sender, e);
    }

    internal void PContextCloseHandle(object sender, RoutedEventArgs e)
    {
        _pContextHost.PContextCloseHandle(sender, e);
    }

    internal void PContextFocusHandle(object sender, MouseButtonEventArgs e)
    {
        _pContextHost.PContextFocusHandle(sender, e);
    }
}
