using System.Windows;

namespace Llyn.UIShell;

public partial class PDownloaderTemplate : ResourceDictionary
{
    private readonly PEditor _pDownloaderHost;

    internal PDownloaderTemplate(PEditor host)
    {
        _pDownloaderHost = host;
        InitializeComponent();
    }

    private void PDownloaderPreviewHandle(object sender, RoutedEventArgs e)
    {
        _pDownloaderHost.PDownloaderPreviewHandle(sender, e);
    }

    private void PDownloaderMenuHandle(object sender, RoutedEventArgs e)
    {
        _pDownloaderHost.PDownloaderMenuHandle(sender, e);
    }
}
