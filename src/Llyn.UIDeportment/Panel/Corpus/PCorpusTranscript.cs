using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public class PCorpusTranscript : ResourceDictionary
{
    private readonly PCorpus _pTranscriptHost;

    internal PCorpusTranscript(PCorpus host)
    {
        _pTranscriptHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Corpus/PCorpusTranscript.xaml", UriKind.Relative)));
    }

    internal void PGlossAddHandle(object sender, RoutedEventArgs e)
    {
        _pTranscriptHost.PGlossAddHandle(sender, e);
    }

    internal void PGlossRemoveHandle(object sender, RoutedEventArgs e)
    {
        _pTranscriptHost.PGlossRemoveHandle(sender, e);
    }

    internal void PTranscriptGlossHandle(object sender, TextChangedEventArgs e)
    {
        _pTranscriptHost.PTranscriptGlossHandle(sender, e);
    }

    internal void PCitationPickHandle(object sender, MouseButtonEventArgs e)
    {
        _pTranscriptHost.PCitationPickHandle(sender, e);
    }

    internal void PSpeakerHandle(object sender, RoutedEventArgs e)
    {
        _pTranscriptHost.PSpeakerHandle(sender, e);
    }
}
