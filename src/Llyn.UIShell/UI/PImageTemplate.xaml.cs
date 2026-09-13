using System.Windows;

namespace Llyn.UIShell;

public partial class PImageTemplate : ResourceDictionary
{
    private readonly PImageHost _pImageHost;

    internal PImageTemplate(PImageHost host)
    {
        _pImageHost = host;
        InitializeComponent();
    }

    private void PImageOpenHandle(object sender, RoutedEventArgs e)
    {
        _pImageHost.PImageOpenHandle(sender, e);
    }

    private void PImageRemoveHandle(object sender, RoutedEventArgs e)
    {
        _pImageHost.PImageRemoveHandle(sender, e);
    }
}
