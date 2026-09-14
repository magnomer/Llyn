using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIShell;

public partial class PSCoinage : Window
{
    private string? _psCoinageAnswer;

    private PSCoinage(Window owner, string message)
    {
        InitializeComponent();
        Owner = owner;
        PSCoinageMessage.SetResourceReference(TextBlock.TextProperty, message);
    }

    internal static string? PSCoinageShow(Window owner, string message)
    {
        PSCoinage dialog = new(owner, message);
        dialog.Loaded += (_, _) => dialog.PSCoinageWording.Focus();
        dialog.ShowDialog();
        return dialog._psCoinageAnswer;
    }

    private void PSCoinageWordingHandle(object sender, TextChangedEventArgs e)
    {
        PSCoinageMint.IsEnabled = !string.IsNullOrWhiteSpace(PSCoinageWording.Text);
    }

    private void PSCoinageMintHandle(object sender, RoutedEventArgs e)
    {
        string written = PSCoinageWording.Text.Trim();
        if (written.Length == 0)
        {
            return;
        }

        _psCoinageAnswer = written;
        DialogResult = true;
    }
}
