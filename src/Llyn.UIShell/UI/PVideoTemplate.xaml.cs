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

    private void PVideoLoadHandle(object sender, RoutedEventArgs e)
    {
        _pVideoHost.PVideoLoadHandle(sender, e);
    }

    private void PVideoReadyHandle(object sender, RoutedEventArgs e)
    {
        _pVideoHost.PVideoReadyHandle(sender, e);
    }

    private void PVideoFinishHandle(object sender, RoutedEventArgs e)
    {
        _pVideoHost.PVideoFinishHandle(sender, e);
    }

    private void PVideoDropHandle(object sender, RoutedEventArgs e)
    {
        _pVideoHost.PVideoDropHandle(sender, e);
    }

    private void PVideoPlayHandle(object sender, RoutedEventArgs e)
    {
        _pVideoHost.PVideoPlayHandle(sender, e);
    }
}
