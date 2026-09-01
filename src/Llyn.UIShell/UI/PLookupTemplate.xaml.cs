using System.Windows;

namespace Llyn.UIShell;

/// <summary>
/// One row of the pronunciation lookup menu as a template, handing the chosen candidate back to the
/// panel.
/// </summary>
public partial class PLookupTemplate : ResourceDictionary
{
    private readonly PInput _pLookupHost;

    internal PLookupTemplate(PInput host)
    {
        _pLookupHost = host;
        InitializeComponent();
    }

    private void PLookupMenuHandle(object sender, RoutedEventArgs e)
    {
        _pLookupHost.PLookupMenuHandle(sender, e);
    }
}
