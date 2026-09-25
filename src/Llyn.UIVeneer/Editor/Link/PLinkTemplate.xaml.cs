using System.Windows;
using System.Windows.Input;

namespace Llyn.UIVeneer;

public partial class PLinkTemplate : ResourceDictionary
{
    private readonly PEditor _pLinkHost;

    internal PLinkTemplate(PEditor host)
    {
        _pLinkHost = host;
        InitializeComponent();
    }

    private void PLinkDropHandle(object sender, RoutedEventArgs e)
    {
        _pLinkHost.PLinkDropHandle(sender, e);
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
}
