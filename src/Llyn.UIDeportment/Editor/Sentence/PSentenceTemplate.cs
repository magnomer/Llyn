using System;
using System.Windows;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public class PSentenceTemplate : ResourceDictionary
{
    private readonly PEditor _pSentenceHost;

    internal PSentenceTemplate(PEditor host)
    {
        _pSentenceHost = host;
        MergedDictionaries.Add((ResourceDictionary)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Editor/Sentence/PSentenceTemplate.xaml", UriKind.Relative)));
    }

    internal void PCitationKeyHandle(object sender, KeyEventArgs e)
    {
        _pSentenceHost.PCitationKeyHandle(sender, e);
    }

    internal void PCitationLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        _pSentenceHost.PCitationLeaveHandle(sender, e);
    }

    internal void PSentenceLinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceLinkHandle(sender, e);
    }

    internal void PSentenceSenseHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceSenseHandle(sender, e);
    }

    internal void PSentenceSilenceHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceSilenceHandle(sender, e);
    }

    internal void PSentenceUnlinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceUnlinkHandle(sender, e);
    }

    internal void PSentenceLinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceLinkCheck(sender, e);
    }

    internal void PSentenceSenseCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceSenseCheck(sender, e);
    }

    internal void PSentenceUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceUnlinkCheck(sender, e);
    }
}
