using System.Windows;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QMorphology
{
    private readonly FrameworkElement _qMorphologySettings;

    private CLedger _qMorphologyLedger = null!;

    private CEnvoy _qMorphologyEnvoy = null!;

    internal QMorphology(FrameworkElement settings)
    {
        _qMorphologySettings = settings;
        QMorphologySwitch.Click += QMorphologyObserve;
    }

    private ToggleButton QMorphologySwitch =>
        QContract.QContractFind<ToggleButton>(_qMorphologySettings, "PMorphology");

    internal void QMorphologyIntroduce(CLedger ledger, CEnvoy envoy)
    {
        _qMorphologyLedger = ledger;
        _qMorphologyEnvoy = envoy;
    }

    internal void QMorphologyRefine(bool chosen)
    {
        QMorphologySwitch.IsChecked = chosen;
    }

    private void QMorphologyObserve(object sender, RoutedEventArgs e)
    {
        _qMorphologyLedger.CLedgerMorphologySave(
            QLook.QLookCheckedRead(QMorphologySwitch.IsChecked), _qMorphologyEnvoy);
    }
}
