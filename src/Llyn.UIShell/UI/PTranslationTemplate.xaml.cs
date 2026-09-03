using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PTranslationTemplate : ResourceDictionary
{
    private readonly PEditor _pTranslationHost;

    internal PTranslationTemplate(PEditor host)
    {
        _pTranslationHost = host;
        InitializeComponent();
    }

    private void PTranslationChipHandle(object sender, RoutedEventArgs e)
    {
        _pTranslationHost.PTranslationChipHandle(sender, e);
    }

    private void PTranslationEntryHandle(object sender, KeyEventArgs e)
    {
        _pTranslationHost.PTranslationEntryHandle(sender, e);
    }

    private void PTranslationCloseHandle(object sender, RoutedEventArgs e)
    {
        _pTranslationHost.PTranslationCloseHandle(sender, e);
    }

    private void PTranslationFocusHandle(object sender, MouseButtonEventArgs e)
    {
        _pTranslationHost.PTranslationFocusHandle(sender, e);
    }

    private void PTranslationMenuHandle(object sender, MouseButtonEventArgs e)
    {
        _pTranslationHost.PTranslationMenuHandle(sender, e);
    }
}
