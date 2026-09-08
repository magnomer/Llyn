using System.Windows;

namespace Llyn.UIShell;

public partial class PVideoTemplate : ResourceDictionary
{
    private readonly PEditor _pVideoHost;

    internal PVideoTemplate(PEditor host)
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
