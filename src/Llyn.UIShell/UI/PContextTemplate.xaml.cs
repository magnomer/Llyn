using System.Windows;

namespace Llyn.UIShell;

public partial class PContextTemplate : ResourceDictionary
{
    private readonly PEditor _pContextHost;

    internal PContextTemplate(PEditor host)
    {
        _pContextHost = host;
        InitializeComponent();
    }

    private void PContextAddHandle(object sender, RoutedEventArgs e)
    {
        _pContextHost.PContextAddHandle(sender, e);
    }

    private void PContextRemoveHandle(object sender, RoutedEventArgs e)
    {
        _pContextHost.PContextRemoveHandle(sender, e);
    }
}
