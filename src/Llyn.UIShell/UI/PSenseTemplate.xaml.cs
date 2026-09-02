using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PSenseTemplate : ResourceDictionary
{
    private readonly PEditor _pSenseHost;

    internal PSenseTemplate(PEditor host)
    {
        _pSenseHost = host;
        InitializeComponent();
    }

    private void PCardHandle(object sender, RoutedEventArgs e)
    {
        _pSenseHost.PCardHandle(sender, e);
    }

    private void PCardDragHandle(object sender, MouseButtonEventArgs e)
    {
        _pSenseHost.PCardDragHandle(sender, e);
    }

    private void PImageAddHandle(object sender, RoutedEventArgs e)
    {
        _pSenseHost.PImageAddHandle(sender, e);
    }

    private void PVideoAddHandle(object sender, RoutedEventArgs e)
    {
        _pSenseHost.PVideoAddHandle(sender, e);
    }
}
