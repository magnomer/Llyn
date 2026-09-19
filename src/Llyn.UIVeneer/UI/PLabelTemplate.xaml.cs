using System.Windows;
using System.Windows.Input;

namespace Llyn.UIVeneer;

public partial class PLabelTemplate : ResourceDictionary
{
    private readonly PEditor _pLabelHost;

    internal PLabelTemplate(PEditor host)
    {
        _pLabelHost = host;
        InitializeComponent();
    }

    private void PLabelChipHandle(object sender, RoutedEventArgs e)
    {
        _pLabelHost.PLabelChipHandle(sender, e);
    }

    private void PLabelCaretHandle(object sender, KeyEventArgs e)
    {
        _pLabelHost.PLabelCaretHandle(sender, e);
    }

    private void PLabelCloseHandle(object sender, RoutedEventArgs e)
    {
        _pLabelHost.PLabelCloseHandle(sender, e);
    }

    private void PLabelFocusHandle(object sender, MouseButtonEventArgs e)
    {
        _pLabelHost.PLabelFocusHandle(sender, e);
    }
}
