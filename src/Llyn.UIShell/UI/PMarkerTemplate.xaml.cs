using System.Windows;

namespace Llyn.UIShell;

public partial class PMarkerTemplate : ResourceDictionary
{
    private readonly PEditor _pSpeechHost;

    internal PMarkerTemplate(PEditor host)
    {
        _pSpeechHost = host;
        InitializeComponent();
    }

    private void PMarkerChipHandle(object sender, RoutedEventArgs e)
    {
        _pSpeechHost.PMarkerChipHandle(sender, e);
    }
}
