using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIDeportment;

public class PWing : UserControl
{
    private PWindow _pWingHost = null!;

    private LWing _lWing = null!;

    public PWing()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Library/PWing.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        PChoice.PChoiceDropperAttach(PWingOrderDropper, PWingOrderDropdown, PWingOrderDropper);
        PChoice.PChoiceDropperAttach(PWingSieveDropper, PWingSieveDropdown, PWingSieveDropper);

        PWingOrderIcon.PIconSource = PIcon.PIconResolve("sort", 24);
        PWingSieveIcon.PIconSource = PIcon.PIconResolve("filter", 24);

        PWingQuery.LostKeyboardFocus += PWingLeaveHandle;
        PWingQuery.PreviewKeyDown += PWingKeyHandle;
        PWingQuery.TextChanged += PWingQueryHandle;
        PWingIndex.LostKeyboardFocus += PWingLeaveHandle;

        PLookItem.PLookItemAttach(PWingIndex, PWingIndexApply);
        DependencyPropertyDescriptor.FromProperty(VisibilityProperty, typeof(ItemsControl))
            .AddValueChanged(PWingIndex, PWingTrayHandle);
    }

    private ToggleButton PWingOrderDropper => (ToggleButton)FindName(nameof(PWingOrderDropper));

    private PIconImage PWingOrderIcon => (PIconImage)FindName(nameof(PWingOrderIcon));

    private TextBox PWingQuery => (TextBox)FindName(nameof(PWingQuery));

    private ToggleButton PWingSieveDropper => (ToggleButton)FindName(nameof(PWingSieveDropper));

    private PIconImage PWingSieveIcon => (PIconImage)FindName(nameof(PWingSieveIcon));

    private FrameworkElement PWingSieveMark => (FrameworkElement)FindName(nameof(PWingSieveMark));

    private Popup PWingOrderDropdown => (Popup)FindName(nameof(PWingOrderDropdown));

    private StackPanel PWingOrderList => (StackPanel)FindName(nameof(PWingOrderList));

    private Popup PWingSieveDropdown => (Popup)FindName(nameof(PWingSieveDropdown));

    private StackPanel PWingSieveList => (StackPanel)FindName(nameof(PWingSieveList));

    private FrameworkElement PWingSeam => (FrameworkElement)FindName(nameof(PWingSeam));

    private FrameworkElement PWingTray => (FrameworkElement)FindName(nameof(PWingTray));

    private ItemsControl PWingIndex => (ItemsControl)FindName(nameof(PWingIndex));

    private TextBlock PWingEmpty => (TextBlock)FindName(nameof(PWingEmpty));

    private PDisplay PWingDisplay => (PDisplay)FindName(nameof(PWingDisplay));

    internal void PWingAttach(PWindow host)
    {
        _pWingHost = host;
        _lWing = host.PWindowDeportment.LWindowWingCreate();

        _lWing.LWingIndexAttach(PWingIndex, PWingEmpty, PWingQuery);
        _lWing.LWingFailed += host.PWindowFailureShow;

        PWingDisplay.PDisplayAttach(host, _lWing.LWingLectern);
    }

    internal async void PWingRestore(string tab, long? id)
    {
        _lWing.LWingVistaRestore(_pWingHost.PWindowDeportment, tab);
        _lWing.LWingObserverAttach(this, LObserver.LObserverCreate);
        PWingDisplay.PDisplayObserverAttach();
        _lWing.LWingIndexClear();
        await LEnsignImage.LEnsignLoad(_pWingHost.PWindowDeportment);

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

    private void PWingIndexApply(FrameworkElement container, object item, string? change)
    {
        LIndexItem.LIndexItemApply(container, item, change);

        if (PLook.PLookPartFind<Button>(container, "PIndexRow") is Button row)
        {
            row.Click -= PWingIndexHandle;
            row.Click += PWingIndexHandle;
        }
    }

    private void PWingTrayHandle(object? sender, EventArgs e)
    {
        PWingSeam.Visibility = PWingIndex.Visibility;
        PWingTray.Visibility = PWingIndex.Visibility;
    }
}
