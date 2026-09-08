using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PContextTemplate : ResourceDictionary
{
    private readonly PEditor _pContextHost;

    internal PContextTemplate(PEditor host)
    {
        _pContextHost = host;
        InitializeComponent();
    }

    private void PContextChipHandle(object sender, RoutedEventArgs e)
    {
        _pContextHost.PContextChipHandle(sender, e);
    }

    private void PContextCaretHandle(object sender, KeyEventArgs e)
    {
        _pContextHost.PContextCaretHandle(sender, e);
    }

    private void PContextCloseHandle(object sender, RoutedEventArgs e)
    {
        _pContextHost.PContextCloseHandle(sender, e);
    }

    private void PContextFocusHandle(object sender, MouseButtonEventArgs e)
    {
        _pContextHost.PContextFocusHandle(sender, e);
    }
}
