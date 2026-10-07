using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

internal sealed class QSLeave
{
    private readonly Window _qsLeaveSurface;
    private QSLeaveAnswer _qsLeaveAnswer = QSLeaveAnswer.QSLeaveAnswerStay;

    private QSLeave(Window owner)
    {
        _qsLeaveSurface = QContract.QContractSheetFind<Window>("PSLeave");
        _qsLeaveSurface.Owner = owner;
        QLook.QLookStyleAttach(_qsLeaveSurface);
        QSLeaveStore.Click += QSLeaveStoreObserve;
        QSLeaveDiscard.Click += QSLeaveDiscardObserve;
        QSLeaveStay.Click += QSLeaveStayObserve;
    }

    private Button QSLeaveStore => QContract.QContractFind<Button>(_qsLeaveSurface, "PSLeaveStore");

    private Button QSLeaveDiscard => QContract.QContractFind<Button>(_qsLeaveSurface, "PSLeaveDiscard");

    private Button QSLeaveStay => QContract.QContractFind<Button>(_qsLeaveSurface, "PSLeaveStay");

    internal static QSLeaveAnswer QSLeaveShow(Window owner)
    {
        QSLeave dialog = new(owner);
        dialog._qsLeaveSurface.ShowDialog();
        return dialog._qsLeaveAnswer;
    }

    private void QSLeaveStoreObserve(object sender, RoutedEventArgs e)
    {
        QSLeaveClose(QSLeaveAnswer.QSLeaveAnswerStore);
    }

    private void QSLeaveDiscardObserve(object sender, RoutedEventArgs e)
    {
        QSLeaveClose(QSLeaveAnswer.QSLeaveAnswerDiscard);
    }

    private void QSLeaveStayObserve(object sender, RoutedEventArgs e)
    {
        QSLeaveClose(QSLeaveAnswer.QSLeaveAnswerStay);
    }

    private void QSLeaveClose(QSLeaveAnswer answer)
    {
        _qsLeaveAnswer = answer;
        _qsLeaveSurface.DialogResult = true;
    }
}
