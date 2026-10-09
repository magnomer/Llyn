using System.Windows;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QInflection
{
    private readonly FrameworkElement _qInflectionSettings;

    private CLedger _qInflectionLedger = null!;

    private CEnvoy _qInflectionEnvoy = null!;

    internal QInflection(FrameworkElement settings)
    {
        _qInflectionSettings = settings;
        QInflectionAnalysis.Click += QInflectionObserve;
    }

    private ToggleButton QInflectionAnalysis =>
        QContract.QContractFind<ToggleButton>(_qInflectionSettings, "PSettingsAnalysis");

    internal void QInflectionIntroduce(CLedger ledger, CEnvoy envoy)
    {
        _qInflectionLedger = ledger;
        _qInflectionEnvoy = envoy;
    }

    internal void QInflectionRefine()
    {
        QInflectionAnalysis.IsChecked = _qInflectionLedger.CLedgerAnalysisRead();
    }

    private void QInflectionObserve(object sender, RoutedEventArgs e)
    {
        _qInflectionLedger.CLedgerAnalysisSave(
            QLook.QLookCheckedRead(QInflectionAnalysis.IsChecked), _qInflectionEnvoy);
    }
}
