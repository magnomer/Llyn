using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public class PRegisterTemplate : ResourceDictionary
{
    private readonly PEditor _pRegisterHost;

    internal PRegisterTemplate(PEditor host)
    {
        _pRegisterHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Register/PRegisterTemplate.xaml", UriKind.Relative)));
    }

    internal void PRegisterChipHandle(object sender, RoutedEventArgs e)
    {
        _pRegisterHost.PRegisterChipHandle(sender, e);
    }

    internal void PRegisterCaretHandle(object sender, KeyEventArgs e)
    {
        _pRegisterHost.PRegisterCaretHandle(sender, e);
    }

    internal void PRegisterCloseHandle(object sender, RoutedEventArgs e)
    {
        _pRegisterHost.PRegisterCloseHandle(sender, e);
    }

    internal void PRegisterFocusHandle(object sender, MouseButtonEventArgs e)
    {
        _pRegisterHost.PRegisterFocusHandle(sender, e);
    }
}
