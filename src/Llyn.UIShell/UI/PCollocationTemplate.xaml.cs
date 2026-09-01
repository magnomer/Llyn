using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

/// <summary>
/// The collocation card as a template — the same card shape a sense uses, over the expression a
/// collocation adds. Like every template dictionary here, it only hands its events back to the panel.
/// </summary>
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
}
