using System.Windows;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QFrequency
{
    private readonly FrameworkElement _qFrequencySettings;

    private CLedger _qFrequencyLedger = null!;

    private CEnvoy _qFrequencyEnvoy = null!;

    internal QFrequency(FrameworkElement settings)
    {
        _qFrequencySettings = settings;
        QFrequencySwitch.Click += QFrequencyObserve;
    }

    private ToggleButton QFrequencySwitch =>
        QContract.QContractFind<ToggleButton>(_qFrequencySettings, "PFrequency");

    internal void QFrequencyIntroduce(CLedger ledger, CEnvoy envoy)
    {
        _qFrequencyLedger = ledger;
        _qFrequencyEnvoy = envoy;
    }

    internal void QFrequencyRefine(bool chosen)
    {
        QFrequencySwitch.IsChecked = chosen;
    }

    private void QFrequencyObserve(object sender, RoutedEventArgs e)
    {
        _qFrequencyLedger.CLedgerFrequencySave(
            QLook.QLookCheckedRead(QFrequencySwitch.IsChecked), _qFrequencyEnvoy);
    }
}
