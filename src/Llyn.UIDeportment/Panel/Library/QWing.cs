using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QWing
{
    private readonly UserControl _qWingSurface;

    private readonly QIndex _qIndex;

    private PWindow _qWingHost = null!;

    private CWing _cWing = null!;

    private LLectern _qWingLectern = null!;

    internal QWing(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qWingSurface = surface;
        _qIndex = new QIndex(QWingIndex, QWingEmpty);

        QWingQuery.SetResourceReference(QField.QFieldHintProperty, "Duplex.Search");
        QChoice.QChoiceDropperAttach(QWingOrderDropper, QWingOrderDropdown, QWingOrderDropper);
        QChoice.QChoiceDropperAttach(QWingSieveDropper, QWingSieveDropdown, QWingSieveDropper);

        QWingOrderIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QWingSieveIcon.QIconSource = QIcon.QIconResolve("filter", 24);

        QWingQuery.LostKeyboardFocus += QWingLeaveHandle;
        QWingQuery.PreviewKeyDown += QWingKeyHandle;
        QWingQuery.TextChanged += QWingQueryHandle;
        QWingIndex.LostKeyboardFocus += QWingLeaveHandle;

        QLookItem.QLookItemAttach(QWingIndex, QWingIndexApply);
        DependencyPropertyDescriptor.FromProperty(UIElement.VisibilityProperty, typeof(ItemsControl))
            .AddValueChanged(QWingIndex, QWingTrayHandle);
    }

    private ToggleButton QWingOrderDropper =>
        QContract.QContractFind<ToggleButton>(_qWingSurface, "PWingOrderDropper");

    private QIconImage QWingOrderIcon => QContract.QContractFind<QIconImage>(_qWingSurface, "PWingOrderIcon");

    private TextBox QWingQuery => QContract.QContractFind<TextBox>(_qWingSurface, "PWingQuery");

    private ToggleButton QWingSieveDropper =>
        QContract.QContractFind<ToggleButton>(_qWingSurface, "PWingSieveDropper");

    private QIconImage QWingSieveIcon => QContract.QContractFind<QIconImage>(_qWingSurface, "PWingSieveIcon");

    private FrameworkElement QWingSieveMark =>
        QContract.QContractFind<FrameworkElement>(_qWingSurface, "PWingSieveMark");

    private Popup QWingOrderDropdown => QContract.QContractFind<Popup>(_qWingSurface, "PWingOrderDropdown");

    private StackPanel QWingOrderList => QContract.QContractFind<StackPanel>(_qWingSurface, "PWingOrderList");

    private Popup QWingSieveDropdown => QContract.QContractFind<Popup>(_qWingSurface, "PWingSieveDropdown");

    private StackPanel QWingSieveList => QContract.QContractFind<StackPanel>(_qWingSurface, "PWingSieveList");

    private FrameworkElement QWingSeam => QContract.QContractFind<FrameworkElement>(_qWingSurface, "PWingSeam");

    private FrameworkElement QWingTray => QContract.QContractFind<FrameworkElement>(_qWingSurface, "PWingTray");

    private ItemsControl QWingIndex => QContract.QContractFind<ItemsControl>(_qWingSurface, "PWingIndex");

    private TextBlock QWingEmpty => QContract.QContractFind<TextBlock>(_qWingSurface, "PWingEmpty");

    private PDisplay QWingDisplay => QContract.QContractFind<PDisplay>(_qWingSurface, "PWingDisplay");

    internal void QWingAttach(PWindow host)
    {
        _qWingHost = host;
        _cWing = CWing.CWingCreate(host.PWindowAtelier, host.PWindowEnvoy);
        _qWingLectern = new LLectern(_cWing.CWingDisplay);

        _cWing.CWingLoaded += QWingLoadedRefine;
        _cWing.CWingChanged += LObserver.LObserverCreate<CBulletin>(_qWingSurface, QWingIndexShow);

        QWingDisplay.PDisplayAttach(host, _qWingLectern);
    }

    internal async void QWingRestore(string tab, long? id)
    {
        _cWing.CWingVistaRestore(tab);
        QWingDisplay.PDisplayObserverAttach();
        _qIndex.QIndexClear();
        await LEnsignImage.LEnsignLoad(_qWingHost.PWindowAtelier);

        QChoice.QChoiceOrderBuild(QWingOrderList, "Order", QWingOrderHandle, QIndex.QIndexOrder);
        QChoice.QChoiceOrderApply(QWingOrderDropdown, _cWing.CWingOrder);
        QWingSieveShow();
        QChoice.QChoiceFilterBuild(
            QWingSieveList, _cWing.CWingLanguageRead(), _cWing.CWingFilter, QWingSieveHandle);

        QWingQuery.Text = string.Empty;
        _qWingLectern.LLecternClear();
        _cWing.CWingEntryRestore(id);
    }

    internal void QWingClose()
    {
        QWingDisplay.PDisplayClose();
    }

    private void QWingIndexShow()
    {
        _qIndex.QIndexShow(_cWing.CWingRowsRead(), _cWing.CWingQueried);
    }

    private void QWingLoadedRefine()
    {
        QWingIndexShow();
        _qWingLectern.LLecternLoadedShow();
    }

    private void QWingSieveShow()
    {
        QWingSieveMark.Visibility = _cWing.CWingFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QWingOrderHandle(object sender, RoutedEventArgs e)
    {
        QWingOrderDropper.IsChecked = false;
        _cWing.CWingOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    private void QWingSieveHandle(object sender, RoutedEventArgs e)
    {
        _cWing.CWingFilterSet(QChoice.QChoiceFilterRead(sender));
        QWingSieveShow();
    }

    private void QWingQueryHandle(object sender, TextChangedEventArgs e)
    {
        _cWing.CWingQuerySet(QWingQuery.Text ?? string.Empty);
        _qIndex.QIndexShownSet(_cWing.CWingQueried);
    }

    private void QWingKeyHandle(object sender, KeyEventArgs e)
    {
        if (!_qIndex.QIndexShown)
        {
            return;
        }

        if (e.Key == Key.Escape)
        {
            _qIndex.QIndexShownSet(false);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            QWingRowOpen(_qIndex.QIndexChosenRead());
            e.Handled = true;
            return;
        }

        if (e.Key is not (Key.Down or Key.Up))
        {
            return;
        }

        e.Handled = QWingRowMove(_qIndex.QIndexNeighbourFind(e.Key == Key.Down));
    }

    private bool QWingRowMove(long? target)
    {
        if (target is not long id)
        {
            return false;
        }

        _cWing.CWingEntrySelect(id);
        QWingIndexShow();
        _qIndex.QIndexEntryScroll(id);
        return true;
    }

    private void QWingLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (e.NewFocus is Visual target
            && (ReferenceEquals(target, QWingQuery) || _qIndex.QIndexHoldCheck(target)))
        {
            return;
        }

        _qIndex.QIndexShownSet(false);
    }

    private void QWingIndexHandle(object sender, RoutedEventArgs e)
    {
        QWingRowOpen(((sender as FrameworkElement)?.DataContext as QIndexItem)?.QIndexItemId);
    }

    private void QWingRowOpen(long? id)
    {
        if (id is not long chosen)
        {
            return;
        }

        _qIndex.QIndexShownSet(false);
        _cWing.CWingEntryOpen(chosen);
    }

    private void QWingIndexApply(FrameworkElement container, object item, string? change)
    {
        QIndexItem.QIndexItemApply(container, item, change);

        if (QLook.QLookPartFind<Button>(container, "PIndexRow") is Button row)
        {
            row.Click -= QWingIndexHandle;
            row.Click += QWingIndexHandle;
        }
    }

    private void QWingTrayHandle(object? sender, EventArgs e)
    {
        QWingSeam.Visibility = QWingIndex.Visibility;
        QWingTray.Visibility = QWingIndex.Visibility;
    }
}
