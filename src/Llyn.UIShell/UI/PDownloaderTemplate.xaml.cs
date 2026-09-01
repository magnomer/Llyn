using System.Windows;

namespace Llyn.UIShell;

/// <summary>
/// One row of the audio download menu as a template, handing the preview and the chosen recording
/// back to the panel.
/// </summary>
public partial class PDownloaderTemplate : ResourceDictionary
{
    private readonly PInput _pDownloaderHost;

    internal PDownloaderTemplate(PInput host)
    {
        _pDownloaderHost = host;
        InitializeComponent();
    }

    private void PDownloaderPlayHandle(object sender, RoutedEventArgs e)
    {
        _pDownloaderHost.PDownloaderPlayHandle(sender, e);
    }

    private void PDownloaderMenuHandle(object sender, RoutedEventArgs e)
    {
        _pDownloaderHost.PDownloaderMenuHandle(sender, e);
    }
}
