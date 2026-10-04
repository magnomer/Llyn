using System.Windows;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QRespelling
{
    private readonly FrameworkElement _qRespellingSettings;

    private CLedger _qRespellingLedger = null!;

    private CEnvoy _qRespellingEnvoy = null!;

    internal QRespelling(FrameworkElement settings)
    {
        _qRespellingSettings = settings;
        QRespellingSwitch.Click += QRespellingObserve;
    }

    private ToggleButton QRespellingSwitch =>
        QContract.QContractFind<ToggleButton>(_qRespellingSettings, "PRespelling");

    internal void QRespellingIntroduce(CLedger ledger, CEnvoy envoy)
    {
        _qRespellingLedger = ledger;
        _qRespellingEnvoy = envoy;
    }

    internal void QRespellingRefine(bool chosen)
    {
        QRespellingSwitch.IsChecked = chosen;
    }

    private void QRespellingObserve(object sender, RoutedEventArgs e)
    {
        _qRespellingLedger.CLedgerRespellingSave(
            QLook.QLookCheckedRead(QRespellingSwitch.IsChecked), _qRespellingEnvoy);
    }
}
