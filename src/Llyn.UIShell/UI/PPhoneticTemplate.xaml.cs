using System.Windows;

namespace Llyn.UIShell;

public partial class PPhoneticTemplate : ResourceDictionary
{
    private readonly PEditor _pPhoneticHost;

    internal PPhoneticTemplate(PEditor host)
    {
        _pPhoneticHost = host;
        InitializeComponent();
    }

    private void PPhoneticSelectorHandle(object sender, RoutedEventArgs e)
    {
        _pPhoneticHost.PPhoneticSelectorHandle(sender, e);
    }
}
