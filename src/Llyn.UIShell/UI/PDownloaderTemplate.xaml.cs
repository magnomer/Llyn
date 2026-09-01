using System.Windows;

namespace Llyn.UIShell;

/// <summary>
/// One row of the audio download menu as a template, handing the preview and the chosen recording
/// back to the panel.
/// </summary>
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
