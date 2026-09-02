using System.Windows;

namespace Llyn.UIShell;

public partial class PSituationTemplate : ResourceDictionary
{
    private readonly PEditor _pSituationHost;

    internal PSituationTemplate(PEditor host)
    {
        _pSituationHost = host;
        InitializeComponent();
    }

    private void PSituationAddHandle(object sender, RoutedEventArgs e)
    {
        _pSituationHost.PSituationAddHandle(sender, e);
    }

    private void PSituationRemoveHandle(object sender, RoutedEventArgs e)
    {
        _pSituationHost.PSituationRemoveHandle(sender, e);
    }

    private void PSituationReferenceClear(object sender, RoutedEventArgs e)
    {
        _pSituationHost.PSituationReferenceClear(sender, e);
    }

    private void PSituationReferenceCreate(object sender, RoutedEventArgs e)
    {
        _pSituationHost.PSituationReferenceCreate(sender, e);
    }
}
