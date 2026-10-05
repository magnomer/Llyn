using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCourier
{
    private readonly FrameworkElement _qCourierSettings;

    private CLedger _qCourierLedger = null!;

    private CCourier _qCourierCourier = null!;

    private CEnvoy _qCourierEnvoy = null!;

    internal QCourier(FrameworkElement settings)
    {
        _qCourierSettings = settings;
        QCourierOutpost.KeyDown += QCourierEscapeRefine;
        QCourierOutpost.KeyDown += QCourierOutpostObserve;
        QCourierOutpost.LostKeyboardFocus += QCourierFocusObserve;
        QCourierWarrant.Click += QCourierAttachObserve;
        QCourierCommand.Click += QCourierSendObserve;
    }

    private TextBox QCourierOutpost => QContract.QContractFind<TextBox>(_qCourierSettings, "PDialOutpost");

    private Button QCourierWarrant => QContract.QContractFind<Button>(_qCourierSettings, "PDialWarrant");

    private Button QCourierCommand => QContract.QContractFind<Button>(_qCourierSettings, "PDialCourier");

    private TextBlock QCourierReceipt => QContract.QContractFind<TextBlock>(_qCourierSettings, "PDialReceipt");

    internal void QCourierIntroduce(CLedger ledger, CAtelier atelier, CEnvoy envoy)
    {
        _qCourierLedger = ledger;
        _qCourierCourier = CCourier.CCourierCreate(atelier);
        _qCourierEnvoy = envoy;
        _qCourierCourier.CCourierChanged +=
            QObserver.QObserverCreate<CCourierState>(_qCourierSettings, QCourierRefine);
        QCourierRefine(_qCourierCourier.CCourierRead());
    }

    internal void QCourierRefine(CCourierState state)
    {
        QCourierWarrant.IsEnabled = state.CCourierStateAllowed;
        QCourierCommand.IsEnabled = state.CCourierStateAllowed;
        QCourierReceipt.Text = state.CCourierStateLine;
    }

    internal void QCourierOutpostRefine(string port)
    {
        QCourierOutpost.Text = port;
    }

    private async void QCourierSendObserve(object sender, RoutedEventArgs e)
    {
        await _qCourierCourier.CCourierSend(_qCourierEnvoy);
    }

    private async void QCourierAttachObserve(object sender, RoutedEventArgs e)
    {
        await _qCourierCourier.CCourierAttach(_qCourierEnvoy);
    }

    private void QCourierOutpostObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        e.Handled = true;
        _qCourierLedger.CLedgerOutpostSave(QCourierOutpost.Text, _qCourierEnvoy);
    }

    private void QCourierEscapeRefine(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape)
        {
            return;
        }

        e.Handled = true;
        QCourierOutpostRefine(_qCourierLedger.CLedgerOutpostRead());
    }

    private void QCourierFocusObserve(object sender, KeyboardFocusChangedEventArgs e)
    {
        _qCourierLedger.CLedgerOutpostSave(QCourierOutpost.Text, _qCourierEnvoy);
    }
}
