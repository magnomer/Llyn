using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QIndex
{
    private readonly ObservableCollection<QIndexItem> _qIndexList = [];

    private readonly ItemsControl _qIndexView;

    private readonly FrameworkElement _qIndexEmpty;

    internal QIndex(ItemsControl view, FrameworkElement empty)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(empty);

        _qIndexView = view;
        _qIndexEmpty = empty;
        view.ItemsSource = _qIndexList;
    }

    internal static IReadOnlyList<CCatalogOrder> QIndexOrder { get; } =
    [
        CCatalogOrder.CCatalogOrderHeadword,
        CCatalogOrder.CCatalogOrderReverse,
        CCatalogOrder.CCatalogOrderRecent,
        CCatalogOrder.CCatalogOrderEarliest,
    ];

    internal bool QIndexShown { get; private set; }

    internal void QIndexShownSet(bool shown)
    {
        QIndexShown = shown;
        _qIndexView.Visibility = shown ? Visibility.Visible : Visibility.Collapsed;
    }

    internal void QIndexShow(IReadOnlyList<CVistaRow> rows, bool asked)
    {
        ArgumentNullException.ThrowIfNull(rows);

        LSplice.LSpliceApply(
            _qIndexList,
            QIndexItem.QIndexItemBuild(rows),
            QIndexItem.QIndexItemMatch,
            QIndexItem.QIndexItemSync);
        _qIndexEmpty.Visibility = asked && _qIndexList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    internal void QIndexClear()
    {
        _qIndexList.Clear();
    }

    internal long? QIndexChosenRead()
    {
        return _qIndexList.FirstOrDefault(row => row.QIndexItemChosen)?.QIndexItemId;
    }

    internal long? QIndexNeighbourFind(bool down)
    {
        return QIndexNeighbourFind(_qIndexList, down);
    }

    internal static long? QIndexNeighbourFind(IReadOnlyList<QIndexItem> items, bool down)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (items.Count == 0)
        {
            return null;
        }

        int place = -1;
        for (int index = 0; index < items.Count; index++)
        {
            if (items[index].QIndexItemChosen)
            {
                place = index;
                break;
            }
        }

        place = down ? Math.Min(place + 1, items.Count - 1) : Math.Max(place - 1, 0);
        return items[place].QIndexItemId;
    }

    internal void QIndexEntryScroll(long id)
    {
        if (_qIndexList.FirstOrDefault(row => row.QIndexItemId == id) is not QIndexItem target)
        {
            return;
        }

        if (_qIndexView.ItemContainerGenerator.ContainerFromItem(target) is FrameworkElement container)
        {
            container.BringIntoView();
        }
    }

    internal bool QIndexHoldCheck(Visual target)
    {
        return _qIndexView.IsAncestorOf(target);
    }
}
