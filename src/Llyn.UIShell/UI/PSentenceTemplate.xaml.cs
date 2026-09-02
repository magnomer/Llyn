using System.Windows;

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

    private void PSentenceReferenceClear(object sender, RoutedEventArgs e)
    {
        _pSentenceHost.PSentenceReferenceClear(sender, e);
    }

    private void PSentenceReferenceCreate(object sender, RoutedEventArgs e)
    {
        _pSentenceHost.PSentenceReferenceCreate(sender, e);
    }
}
