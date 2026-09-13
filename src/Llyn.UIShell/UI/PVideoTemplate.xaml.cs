using System.Windows;

namespace Llyn.UIShell;

public partial class PVideoTemplate : ResourceDictionary
{
    private readonly PVideoHost _pVideoHost;

    internal PVideoTemplate(PVideoHost host)
    {
        _pVideoHost = host;
        InitializeComponent();
    }

    private void PVideoOpenHandle(object sender, RoutedEventArgs e)
    {
        _pVideoHost.PVideoOpenHandle(sender, e);
    }

    private void PVideoRemoveHandle(object sender, RoutedEventArgs e)
    {
        _pVideoHost.PVideoRemoveHandle(sender, e);
    }
}
