using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PCorpusTranscript : ResourceDictionary
{
    private readonly PCorpus _pTranscriptHost;

    internal PCorpusTranscript(PCorpus host)
    {
        _pTranscriptHost = host;
        InitializeComponent();
    }

    private void PGlossAddHandle(object sender, RoutedEventArgs e)
    {
        _pTranscriptHost.PGlossAddHandle(sender, e);
    }

    private void PGlossRemoveHandle(object sender, RoutedEventArgs e)
    {
        _pTranscriptHost.PGlossRemoveHandle(sender, e);
    }

    private void PCitationPickHandle(object sender, MouseButtonEventArgs e)
    {
        _pTranscriptHost.PCitationPickHandle(sender, e);
    }

    private void PSpeakerHandle(object sender, RoutedEventArgs e)
    {
        _pTranscriptHost.PSpeakerHandle(sender, e);
    }
}
