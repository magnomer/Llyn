using System.Windows;
using System.Windows.Input;

namespace Llyn.UIVeneer;

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

    private void PCardPositionHandle(object sender, MouseButtonEventArgs e)
    {
        _pMeaningHost.PCardPositionHandle(sender, e);
    }

    private void PCardPositionAccept(object sender, KeyEventArgs e)
    {
        _pMeaningHost.PCardPositionAccept(sender, e);
    }

    private void PCardPositionCommit(object sender, RoutedEventArgs e)
    {
        _pMeaningHost.PCardPositionCommit(sender, e);
    }
}
