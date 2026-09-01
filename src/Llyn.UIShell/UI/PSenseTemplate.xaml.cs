using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

/// <summary>
/// The sense card as a template. A template lives in a dictionary rather than in the panel it fills,
/// so the panel keeps its own layout; the events a card raises belong to the panel all the same, and
/// this class exists only to hand them back to it.
/// </summary>
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
}
