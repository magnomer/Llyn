using System.Windows;

namespace Llyn.UIShell;

public partial class PLanguageTemplate : ResourceDictionary
{
    private readonly PEditor _pLanguageHost;

    internal PLanguageTemplate(PEditor host)
    {
        _pLanguageHost = host;
        InitializeComponent();
    }

    private void PSpeakerHandle(object sender, RoutedEventArgs e)
    {
        _pLanguageHost.PSpeakerHandle(sender, e);
    }
}
