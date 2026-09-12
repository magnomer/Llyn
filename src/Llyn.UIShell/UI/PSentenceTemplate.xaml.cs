using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PSentenceTemplate : ResourceDictionary
{
    private readonly PEditor _pSentenceHost;

    internal PSentenceTemplate(PEditor host)
    {
        _pSentenceHost = host;
        InitializeComponent();
    }

    private void PSentenceAddHandle(object sender, RoutedEventArgs e)
    {
        _pSentenceHost.PSentenceAddHandle(sender, e);
    }

    private void PSentenceRemoveHandle(object sender, RoutedEventArgs e)
    {
        _pSentenceHost.PSentenceRemoveHandle(sender, e);
    }

    private void PSentenceCitationClear(object sender, RoutedEventArgs e)
    {
        _pSentenceHost.PSentenceCitationClear(sender, e);
    }

    private void PGlossAddHandle(object sender, RoutedEventArgs e)
    {
        _pSentenceHost.PGlossAddHandle(sender, e);
    }

    private void PGlossRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _pSentenceHost.PGlossRemoveHandle(sender, e);
    }

    private void PSentenceLinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceLinkHandle(sender, e);
    }

    private void PSentenceSenseHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceSenseHandle(sender, e);
    }

    private void PSentenceSilenceHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceSilenceHandle(sender, e);
    }

    private void PSentenceUnlinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceUnlinkHandle(sender, e);
    }

    private void PSentenceLinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceLinkCheck(sender, e);
    }

    private void PSentenceSenseCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceSenseCheck(sender, e);
    }

    private void PSentenceUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        _pSentenceHost.PSentenceUnlinkCheck(sender, e);
    }
}
