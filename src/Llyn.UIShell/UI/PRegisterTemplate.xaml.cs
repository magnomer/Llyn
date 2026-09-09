using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PRegisterTemplate : ResourceDictionary
{
    private readonly PEditor _pRegisterHost;

    internal PRegisterTemplate(PEditor host)
    {
        _pRegisterHost = host;
        InitializeComponent();
    }

    private void PRegisterChipHandle(object sender, RoutedEventArgs e)
    {
        _pRegisterHost.PRegisterChipHandle(sender, e);
    }

    private void PRegisterCaretHandle(object sender, KeyEventArgs e)
    {
        _pRegisterHost.PRegisterCaretHandle(sender, e);
    }

    private void PRegisterCloseHandle(object sender, RoutedEventArgs e)
    {
        _pRegisterHost.PRegisterCloseHandle(sender, e);
    }

    private void PRegisterFocusHandle(object sender, MouseButtonEventArgs e)
    {
        _pRegisterHost.PRegisterFocusHandle(sender, e);
    }
}
