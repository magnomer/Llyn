using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QSCoinage
{
    private readonly Window _qsCoinageSurface;

    private QSCoinage(Window owner, string message)
    {
        _qsCoinageSurface = QContract.QContractSheetFind<Window>("PSCoinage");
        _qsCoinageSurface.Owner = owner;
        QSCoinageWording.TextChanged += QSCoinageWordingHandle;
        QSCoinageMint.Click += QSCoinageMintHandle;
        QSCoinageMessage.SetResourceReference(TextBlock.TextProperty, message);
    }

    private TextBlock QSCoinageMessage => QContract.QContractFind<TextBlock>(_qsCoinageSurface, "PSCoinageMessage");

    private TextBox QSCoinageWording => QContract.QContractFind<TextBox>(_qsCoinageSurface, "PSCoinageWording");

    private Button QSCoinageMint => QContract.QContractFind<Button>(_qsCoinageSurface, "PSCoinageMint");

    internal static string? QSCoinageShow(Window owner, string message)
    {
        QSCoinage dialog = new(owner, message);
        dialog._qsCoinageSurface.Loaded += (_, _) => dialog.QSCoinageWording.Focus();
        return dialog._qsCoinageSurface.ShowDialog() == true ? dialog.QSCoinageWording.Text.Trim() : null;
    }

    private void QSCoinageWordingHandle(object sender, TextChangedEventArgs e)
    {
        QSCoinageMint.IsEnabled = CSCoinage.CSCoinageWordingCheck(QSCoinageWording.Text);
    }

    private void QSCoinageMintHandle(object sender, RoutedEventArgs e)
    {
        _qsCoinageSurface.DialogResult = CSCoinage.CSCoinageWordingCheck(QSCoinageWording.Text) ? true : null;
    }
}
