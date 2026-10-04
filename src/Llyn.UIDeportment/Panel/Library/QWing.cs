using System;
using System.Collections.Generic;
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

    private readonly QDisplay _qWingDisplay;

    private readonly QIndex _qIndex;

    private QWindow _qWingHost = null!;

    private CWing _cWing = null!;

    private QLectern _qWingLectern = null!;

    internal QWing(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qWingSurface = surface;
        _qWingDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PWingDisplay"));
        _qIndex = new QIndex(QWingIndex, QWingEmpty);

        QWingQuery.SetResourceReference(QField.QFieldHintProperty, "Duplex.Search");
        QChoice.QChoiceDropperAttach(QWingOrderDropper, QWingOrderDropdown, QWingOrderDropper);
        QChoice.QChoiceDropperAttach(QWingSieveDropper, QWingSieveDropdown, QWingSieveDropper);

        QWingOrderIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QWingSieveIcon.QIconSource = QIcon.QIconResolve("filter", 24);

        QWingQuery.LostKeyboardFocus += QWingLeaveRefine;
        QWingQuery.PreviewKeyDown += QWingKeyRefine;
        QWingQuery.PreviewKeyDown += QWingKeyObserve;
        QWingQuery.TextChanged += QWingQueryObserve;
        QWingIndex.LostKeyboardFocus += QWingLeaveRefine;

        QLookItem.QLookItemAttach(QWingIndex, QWingRowRefine);
        DependencyPropertyDescriptor.FromProperty(UIElement.VisibilityProperty, typeof(ItemsControl))
            .AddValueChanged(QWingIndex, QWingTrayRefine);
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

    internal void QWingIntroduce(QWindow host, bool left)
    {
        _qWingHost = host;
        _cWing = CWing.CWingCreate(host.QWindowAtelier, host.QWindowEnvoy, left);
        _qWingLectern = new QLectern(_cWing.CWingDisplay);

        _cWing.CWingLoaded += QWingIndexRefine;
        _cWing.CWingRowsChanged += QWingIndexRefine;
        _cWing.CWingChanged += QObserver.QObserverCreate<CBulletin>(_qWingSurface, QWingIndexRefine);
        host.QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += QWingRefine;

        _qWingDisplay.QDisplayIntroduce(host, _qWingLectern);
    }

    private async void QWingRefine()
    {
        _qIndex.QIndexClearRefine();
        IReadOnlyList<string> languages =
            await _qWingHost.QWindowAtelier.CAtelierCatalog.CCatalogEnsignLoad(
                _qWingHost.QWindowEnvoy, QEnsignImage.QEnsignDraw);

        QChoice.QChoiceOrderBuild(QWingOrderList, "Order", QWingOrderObserve, CLibrary.CLibraryOrderRead());
        QChoice.QChoiceOrderApply(QWingOrderDropdown, _cWing.CWingOrder);
        QWingSieveRefine();
        QWingSieveBuild(languages);

        QWingQuery.Text = string.Empty;
    }

    private void QWingSieveBuild(IReadOnlyList<string> languages)
    {
        QChoice.QChoiceFilterBuild(QWingSieveList, languages, _cWing.CWingFilter, QWingSieveObserve);
    }

    private void QWingIndexRefine()
    {
        _qIndex.QIndexRefine(_cWing.CWingRowsRead(), _cWing.CWingEmpty);
    }

    private void QWingSieveRefine()
    {
        QWingSieveMark.Visibility = _cWing.CWingFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QWingOrderObserve(object sender, RoutedEventArgs e)
    {
        _cWing.CWingOrderSet(QChoice.QChoiceOrderRead(sender));
        QWingOrderRefine();
    }

    private void QWingOrderRefine()
    {
        QWingOrderDropper.IsChecked = false;
    }

    private void QWingSieveObserve(object sender, RoutedEventArgs e)
    {
        _cWing.CWingFilterSet(QChoice.QChoiceFilterRead(sender));
        QWingSieveRefine();
    }

    private void QWingQueryObserve(object sender, TextChangedEventArgs e)
    {
        _cWing.CWingQuerySet(QWingQuery.Text ?? string.Empty);
        _qIndex.QIndexShownRefine(_cWing.CWingQueried);
    }

    private void QWingKeyRefine(object sender, KeyEventArgs e)
    {
        if (!_qIndex.QIndexShown || e.Key != Key.Escape)
        {
            return;
        }

        _qIndex.QIndexShownRefine(false);
        e.Handled = true;
    }

    private void QWingKeyObserve(object sender, KeyEventArgs e)
    {
        if (!_qIndex.QIndexShown)
        {
            return;
        }

        if (e.Key == Key.Enter)
        {
            bool opened = _cWing.CWingEntryOpen(_qIndex.QIndexChosenRead());
            e.Handled = opened;
            _qIndex.QIndexShownRefine(!opened);
            return;
        }

        if (e.Key is not (Key.Down or Key.Up))
        {
            return;
        }

        long? moved = _cWing.CWingRowMove(e.Key == Key.Down);
        e.Handled = moved is not null;
        _qIndex.QIndexScrollRefine(moved);
    }

    private void QWingLeaveRefine(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (e.NewFocus is Visual target
            && (ReferenceEquals(target, QWingQuery) || _qIndex.QIndexHoldCheck(target)))
        {
            return;
        }

        _qIndex.QIndexShownRefine(false);
    }

    private void QWingIndexObserve(object sender, RoutedEventArgs e)
    {
        _qIndex.QIndexShownRefine(
            !_cWing.CWingEntryOpen(((sender as FrameworkElement)?.DataContext as QIndexItem)?.QIndexItemId));
    }

    private void QWingRowRefine(FrameworkElement container, object item, string? change)
    {
        QIndexItem.QIndexItemRefine(container, item, change);

        if (QLook.QLookPartFind<Button>(container, "PIndexRow") is Button row)
        {
            row.Click -= QWingIndexObserve;
            row.Click += QWingIndexObserve;
        }
    }

    private void QWingTrayRefine(object? sender, EventArgs e)
    {
        QWingSeam.Visibility = QWingIndex.Visibility;
        QWingTray.Visibility = QWingIndex.Visibility;
    }
}
