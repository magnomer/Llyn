using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PWing : UserControl
{
    private PWindow _pWingHost = null!;

    private LWing _lWing = null!;

    public PWing()
    {
        InitializeComponent();
    }

    internal void PWingAttach(PWindow host)
    {
        _pWingHost = host;
        _lWing = host.PWindowDeportment.LWindowWingCreate();

        _lWing.LWingIndexAttach(PWingIndex, PWingEmpty, PWingQuery, PEnsign.PEnsignFind);
        _lWing.LWingFailed += host.PWindowFailureShow;
        _lWing.LWingDraftChanged += PWingDisplay.PDisplayShow;
        _lWing.LWingCleared += PWingDisplay.PDisplayClear;

        PWingDisplay.PDisplayAttach(host, _lWing.LWingLectern);
    }

    internal async void PWingRestore(string tab, long? id)
    {
        _lWing.LWingVistaRestore(_pWingHost.PWindowDeportment, tab);
        _lWing.LWingObserverAttach(this, PObserver.PObserverCreate);
        PWingDisplay.PDisplayObserverAttach();
        _lWing.LWingIndexClear();
        await PEnsign.PEnsignLoad(_pWingHost.PWindowDeportment);

        PChoice.PChoiceOrderBuild(PWingOrderList, "Order", PWingOrderHandle, LIndex.LIndexOrder);
        PChoice.PChoiceOrderApply(PWingOrderDropdown, _lWing.LWingOrder);
        _lWing.LWingSieveShow(PWingSieveMark);
        PChoice.PChoiceFilterBuild(
            PWingSieveList, _lWing.LWingLanguageRead(), _lWing.LWingFilter, PWingSieveHandle);

        PWingQuery.Text = string.Empty;
        _lWing.LWingEntryShow(id);
    }

    internal void PWingClose()
    {
        PWingDisplay.PDisplayClose();
    }

    private void PWingOrderHandle(object sender, RoutedEventArgs e)
    {
        _lWing.LWingOrderHandle(sender, PWingOrderDropper);
    }

    private void PWingSieveHandle(object sender, RoutedEventArgs e)
    {
        _lWing.LWingSieveHandle(PWingSieveList, PWingSieveMark);
    }

    private void PWingQueryHandle(object sender, TextChangedEventArgs e)
    {
        _lWing.LWingQueryHandle();
    }

    private void PWingKeyHandle(object sender, KeyEventArgs e)
    {
        _lWing.LWingKeyHandle(e);
    }

    private void PWingLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        _lWing.LWingLeaveHandle(e);
    }

    private void PWingIndexHandle(object sender, RoutedEventArgs e)
    {
        _lWing.LWingIndexHandle(sender);
    }
}
