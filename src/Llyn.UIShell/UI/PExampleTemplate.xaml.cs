using System.Windows;

namespace Llyn.UIShell;

public partial class PExampleTemplate : ResourceDictionary
{
    private readonly PEditor _pExampleHost;

    internal PExampleTemplate(PEditor host)
    {
        _pExampleHost = host;
        InitializeComponent();
    }

    private void PExampleAddHandle(object sender, RoutedEventArgs e)
    {
        _pExampleHost.PExampleAddHandle(sender, e);
    }

    private void PExampleRemoveHandle(object sender, RoutedEventArgs e)
    {
        _pExampleHost.PExampleRemoveHandle(sender, e);
    }

    private void PExampleReferenceClear(object sender, RoutedEventArgs e)
    {
        _pExampleHost.PExampleReferenceClear(sender, e);
    }

    private void PExampleReferenceCreate(object sender, RoutedEventArgs e)
    {
        _pExampleHost.PExampleReferenceCreate(sender, e);
    }
}
