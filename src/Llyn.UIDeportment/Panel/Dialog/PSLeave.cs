using System;
using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public class PSLeave
{
    private readonly Window _psLeaveSurface;
    private PSLeaveAnswer _psLeaveAnswer = PSLeaveAnswer.PSLeaveAnswerStay;

    internal PSLeave(Window owner)
    {
        _psLeaveSurface = (Window)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Dialog/PSLeave.xaml", UriKind.Relative));
        _psLeaveSurface.Owner = owner;
        PSLeaveStore.Click += PSLeaveStoreHandle;
        PSLeaveDiscard.Click += PSLeaveDiscardHandle;
        PSLeaveStay.Click += PSLeaveStayHandle;
    }

    private Button PSLeaveStore => (Button)_psLeaveSurface.FindName(nameof(PSLeaveStore));

    private Button PSLeaveDiscard => (Button)_psLeaveSurface.FindName(nameof(PSLeaveDiscard));

    private Button PSLeaveStay => (Button)_psLeaveSurface.FindName(nameof(PSLeaveStay));

    internal static PSLeaveAnswer PSLeaveShow(Window owner)
    {
        PSLeave dialog = new(owner);
        dialog._psLeaveSurface.ShowDialog();
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
        _psLeaveSurface.DialogResult = true;
    }
}
