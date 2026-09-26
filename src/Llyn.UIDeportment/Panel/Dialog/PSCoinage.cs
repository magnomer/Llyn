using System;
using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public class PSCoinage
{
    private readonly Window _psCoinageSurface;
    private string? _psCoinageAnswer;

    private PSCoinage(Window owner, string message)
    {
        _psCoinageSurface = (Window)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Dialog/PSCoinage.xaml", UriKind.Relative));
        _psCoinageSurface.Owner = owner;
        PSCoinageWording.TextChanged += PSCoinageWordingHandle;
        PSCoinageMint.Click += PSCoinageMintHandle;
        PSCoinageMessage.SetResourceReference(TextBlock.TextProperty, message);
    }

    private TextBlock PSCoinageMessage => (TextBlock)_psCoinageSurface.FindName(nameof(PSCoinageMessage));

    private TextBox PSCoinageWording => (TextBox)_psCoinageSurface.FindName(nameof(PSCoinageWording));

    private Button PSCoinageMint => (Button)_psCoinageSurface.FindName(nameof(PSCoinageMint));

    internal static string? PSCoinageShow(Window owner, string message)
    {
        PSCoinage dialog = new(owner, message);
        dialog._psCoinageSurface.Loaded += (_, _) => dialog.PSCoinageWording.Focus();
        dialog._psCoinageSurface.ShowDialog();
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
        _psCoinageSurface.DialogResult = true;
    }
}
