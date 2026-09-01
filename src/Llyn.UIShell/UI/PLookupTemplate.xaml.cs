using System.Windows;

namespace Llyn.UIShell;

public partial class PLookupTemplate : ResourceDictionary
{
    private readonly PEditor _pLookupHost;

    internal PLookupTemplate(PEditor host)
    {
        _pLookupHost = host;
        InitializeComponent();
    }

    private void PLookupMenuHandle(object sender, RoutedEventArgs e)
    {
        _pLookupHost.PLookupMenuHandle(sender, e);
    }
}
