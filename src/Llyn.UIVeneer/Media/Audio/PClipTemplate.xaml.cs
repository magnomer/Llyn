using System.Windows;

namespace Llyn.UIVeneer;

public partial class PClipTemplate : ResourceDictionary
{
    private readonly PEditor _pClipHost;

    internal PClipTemplate(PEditor host)
    {
        _pClipHost = host;
        InitializeComponent();
    }

    private void PClipPreviewHandle(object sender, RoutedEventArgs e)
    {
        _pClipHost.PClipPreviewHandle(sender, e);
    }

    private void PClipSelectorHandle(object sender, RoutedEventArgs e)
    {
        _pClipHost.PClipSelectorHandle(sender, e);
    }
}
