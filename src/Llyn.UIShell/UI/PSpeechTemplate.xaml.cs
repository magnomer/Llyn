using System.Windows;

namespace Llyn.UIShell;

public partial class PSpeechTemplate : ResourceDictionary
{
    private readonly PEditor _pSpeechHost;

    internal PSpeechTemplate(PEditor host)
    {
        _pSpeechHost = host;
        InitializeComponent();
    }

    private void PSpeechChipHandle(object sender, RoutedEventArgs e)
    {
        _pSpeechHost.PSpeechChipHandle(sender, e);
    }
}
