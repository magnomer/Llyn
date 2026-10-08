using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCourier
{
    private readonly FrameworkElement _qCourierSurface;

    private CCourier _qCourierCourier = null!;

    private CEnvoy _qCourierEnvoy = null!;

    internal QCourier(FrameworkElement surface)
    {
        _qCourierSurface = surface;
        QCourierWarrant.Click += QCourierAttachObserve;
        QCourierCommand.Click += QCourierSendObserve;
    }

    private Button QCourierWarrant => QContract.QContractFind<Button>(_qCourierSurface, "PCourierWarrant");

    private TextBlock QCourierBadge => QContract.QContractFind<TextBlock>(_qCourierSurface, "PCourierBadge");

    private Button QCourierCommand => QContract.QContractFind<Button>(_qCourierSurface, "PCourierCommand");

    private TextBlock QCourierSeparator => QContract.QContractFind<TextBlock>(_qCourierSurface, "PCourierSeparator");

    private TextBlock QCourierReceipt => QContract.QContractFind<TextBlock>(_qCourierSurface, "PCourierReceipt");

    internal void QCourierIntroduce(CAtelier atelier, CEnvoy envoy)
    {
        _qCourierCourier = CCourier.CCourierCreate(atelier);
        _qCourierEnvoy = envoy;
        _qCourierCourier.CCourierChanged +=
            QObserver.QObserverCreate<CCourierState>(_qCourierSurface, QCourierRefine);
        QCourierRefine(_qCourierCourier.CCourierRead());
    }

    private void QCourierRefine(CCourierState state)
    {
        QCourierWarrant.Visibility = state.CCourierStateAttached ? Visibility.Collapsed : Visibility.Visible;
        QCourierBadge.Visibility = state.CCourierStateAttached ? Visibility.Visible : Visibility.Collapsed;
        QCourierWarrant.IsEnabled = state.CCourierStateAllowed;
        QCourierCommand.IsEnabled = state.CCourierStateAllowed;
        QCourierSeparator.Visibility = state.CCourierStateLine.Length == 0 ? Visibility.Collapsed : Visibility.Visible;
        QCourierReceipt.Text = state.CCourierStateLine;
    }

    private async void QCourierSendObserve(object sender, RoutedEventArgs e)
    {
        await _qCourierCourier.CCourierSend(_qCourierEnvoy);
    }

    private async void QCourierAttachObserve(object sender, RoutedEventArgs e)
    {
        await _qCourierCourier.CCourierAttach(_qCourierEnvoy);
    }
}
