using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PMeaningTemplate : ResourceDictionary
{
    private readonly PEditor _pMeaningHost;

    internal PMeaningTemplate(PEditor host)
    {
        _pMeaningHost = host;
        InitializeComponent();
    }

    private void PCardHandle(object sender, RoutedEventArgs e)
    {
        _pMeaningHost.PCardHandle(sender, e);
    }

    private void PCardDragHandle(object sender, MouseButtonEventArgs e)
    {
        _pMeaningHost.PCardDragHandle(sender, e);
    }

    private void PImageAddHandle(object sender, RoutedEventArgs e)
    {
        _pMeaningHost.PImageAddHandle(sender, e);
    }

    private void PVideoAddHandle(object sender, RoutedEventArgs e)
    {
        _pMeaningHost.PVideoAddHandle(sender, e);
    }
}
