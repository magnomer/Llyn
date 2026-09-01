using System.Windows;

namespace Llyn.UIShell;

/// <summary>
/// The collocation card as a template — the same card shape a sense uses, over the expression a
/// collocation adds. Like every template dictionary here, it only hands its events back to the panel.
/// </summary>
public partial class PCollocationTemplate : ResourceDictionary
{
    private readonly PInput _pCollocationHost;

    internal PCollocationTemplate(PInput host)
    {
        _pCollocationHost = host;
        InitializeComponent();
    }

    private void PCardHandle(object sender, RoutedEventArgs e)
    {
        _pCollocationHost.PCardHandle(sender, e);
    }
}
