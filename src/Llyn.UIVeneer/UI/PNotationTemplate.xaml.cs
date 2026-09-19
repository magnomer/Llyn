using System.Windows;

namespace Llyn.UIVeneer;

public partial class PNotationTemplate : ResourceDictionary
{
    private readonly PEditor _pNotationHost;

    internal PNotationTemplate(PEditor host)
    {
        _pNotationHost = host;
        InitializeComponent();
    }

    private void PNotationSelectorHandle(object sender, RoutedEventArgs e)
    {
        _pNotationHost.PNotationSelectorHandle(sender, e);
    }
}
