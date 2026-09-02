using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PTagTemplate : ResourceDictionary
{
    private readonly PEditor _pTagHost;

    internal PTagTemplate(PEditor host)
    {
        _pTagHost = host;
        InitializeComponent();
    }

    private void PTagChipHandle(object sender, RoutedEventArgs e)
    {
        _pTagHost.PTagChipHandle(sender, e);
    }

    private void PTagEntryHandle(object sender, KeyEventArgs e)
    {
        _pTagHost.PTagEntryHandle(sender, e);
    }

    private void PTagCloseHandle(object sender, RoutedEventArgs e)
    {
        _pTagHost.PTagCloseHandle(sender, e);
    }

    private void PTagFocusHandle(object sender, MouseButtonEventArgs e)
    {
        _pTagHost.PTagFocusHandle(sender, e);
    }
}
