using System.Windows;

namespace Llyn.UIVeneer;

public partial class PSLeave : Window
{
    private PSLeaveAnswer _psLeaveAnswer = PSLeaveAnswer.PSLeaveAnswerStay;

    internal PSLeave(Window owner)
    {
        InitializeComponent();
        Owner = owner;
    }

    internal static PSLeaveAnswer PSLeaveShow(Window owner)
    {
        PSLeave dialog = new(owner);
        dialog.ShowDialog();
        return dialog._psLeaveAnswer;
    }

    private void PSLeaveStoreHandle(object sender, RoutedEventArgs e)
    {
        PSLeaveClose(PSLeaveAnswer.PSLeaveAnswerStore);
    }

    private void PSLeaveDiscardHandle(object sender, RoutedEventArgs e)
    {
        PSLeaveClose(PSLeaveAnswer.PSLeaveAnswerDiscard);
    }

    private void PSLeaveStayHandle(object sender, RoutedEventArgs e)
    {
        PSLeaveClose(PSLeaveAnswer.PSLeaveAnswerStay);
    }

    private void PSLeaveClose(PSLeaveAnswer answer)
    {
        _psLeaveAnswer = answer;
        DialogResult = true;
    }
}
