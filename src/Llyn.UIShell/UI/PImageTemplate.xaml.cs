using System.Windows;

namespace Llyn.UIShell;

public partial class PImageTemplate : ResourceDictionary
{
    private readonly PEditor _pImageHost;

    internal PImageTemplate(PEditor host)
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
