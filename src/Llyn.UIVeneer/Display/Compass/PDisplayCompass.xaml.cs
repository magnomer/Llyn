using System.Windows;

namespace Llyn.UIVeneer;

public partial class PDisplayCompass : ResourceDictionary
{
    private readonly PDisplay _pCompassHost;

    internal PDisplayCompass(PDisplay host)
    {
        _pCompassHost = host;
        InitializeComponent();
    }

    private void PCompassRowHandle(object sender, RoutedEventArgs e)
    {
        _pCompassHost.PCompassRowHandle(sender, e);
    }
}
