using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PCollocationTemplate : ResourceDictionary
{
    private readonly PEditor _pCollocationHost;

    internal PCollocationTemplate(PEditor host)
    {
        _pCollocationHost = host;
        InitializeComponent();
    }

    private void PCardHandle(object sender, RoutedEventArgs e)
    {
        _pCollocationHost.PCardHandle(sender, e);
    }

    private void PCardDragHandle(object sender, MouseButtonEventArgs e)
    {
        _pCollocationHost.PCardDragHandle(sender, e);
    }

    private void PImageAddHandle(object sender, RoutedEventArgs e)
    {
        _pCollocationHost.PImageAddHandle(sender, e);
    }

    private void PVideoAddHandle(object sender, RoutedEventArgs e)
    {
        _pCollocationHost.PVideoAddHandle(sender, e);
    }
}
