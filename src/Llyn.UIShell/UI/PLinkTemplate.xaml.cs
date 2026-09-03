using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PLinkTemplate : ResourceDictionary
{
    private readonly PEditor _pLinkHost;

    internal PLinkTemplate(PEditor host)
    {
        _pLinkHost = host;
        InitializeComponent();
    }

    private void PLinkChipHandle(object sender, RoutedEventArgs e)
    {
        _pLinkHost.PLinkChipHandle(sender, e);
    }

    private void PLinkCaretHandle(object sender, KeyEventArgs e)
    {
        _pLinkHost.PLinkCaretHandle(sender, e);
    }

    private void PLinkCloseHandle(object sender, RoutedEventArgs e)
    {
        _pLinkHost.PLinkCloseHandle(sender, e);
    }

    private void PLinkFocusHandle(object sender, MouseButtonEventArgs e)
    {
        _pLinkHost.PLinkFocusHandle(sender, e);
    }

    private void PLinkMenuHandle(object sender, MouseButtonEventArgs e)
    {
        _pLinkHost.PLinkMenuHandle(sender, e);
    }
}
