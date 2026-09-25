using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class LIndex
{
    private readonly ObservableCollection<LIndexItem> _lIndexList = [];

    private readonly ItemsControl _lIndexView;

    private readonly FrameworkElement _lIndexEmpty;

    private readonly Func<string, ImageSource?> _lIndexFlag;

    public LIndex(ItemsControl view, FrameworkElement empty, Func<string, ImageSource?> flagSeam)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(empty);
        ArgumentNullException.ThrowIfNull(flagSeam);

        _lIndexView = view;
        _lIndexEmpty = empty;
        _lIndexFlag = flagSeam;
        view.ItemsSource = _lIndexList;
    }

    public static IReadOnlyList<LCatalogOrder> LIndexOrder { get; } =
    [
        LCatalogOrder.LCatalogOrderHeadword,
        LCatalogOrder.LCatalogOrderReverse,
        LCatalogOrder.LCatalogOrderRecent,
        LCatalogOrder.LCatalogOrderEarliest,
    ];

    public bool LIndexShown { get; private set; }

    public void LIndexShownSet(bool shown)
    {
        LIndexShown = shown;
        _lIndexView.Visibility = shown ? Visibility.Visible : Visibility.Collapsed;
    }

    public void LIndexShow(IReadOnlyList<LVistaRow> rows, bool asked)
    {
        ArgumentNullException.ThrowIfNull(rows);

        LSplice.LSpliceApply(
            _lIndexList,
            LIndexItem.LIndexItemBuild(rows, _lIndexFlag),
            LIndexItem.LIndexItemMatch,
            LIndexItem.LIndexItemSync);
        _lIndexEmpty.Visibility = asked && _lIndexList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public void LIndexClear()
    {
        _lIndexList.Clear();
    }

    public long? LIndexChosenRead()
    {
        return _lIndexList.FirstOrDefault(row => row.LIndexItemChosen)?.LIndexItemId;
    }

    public long? LIndexNeighbourFind(bool down)
    {
        return LIndexNeighbourFind(_lIndexList, down);
    }

    internal static long? LIndexNeighbourFind(IReadOnlyList<LIndexItem> items, bool down)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (items.Count == 0)
        {
            return null;
        }

        int place = -1;
        for (int index = 0; index < items.Count; index++)
        {
            if (items[index].LIndexItemChosen)
            {
                place = index;
                break;
            }
        }

        place = down ? Math.Min(place + 1, items.Count - 1) : Math.Max(place - 1, 0);
        return items[place].LIndexItemId;
    }

    public void LIndexEntryScroll(long id)
    {
        if (_lIndexList.FirstOrDefault(row => row.LIndexItemId == id) is not LIndexItem target)
        {
            return;
        }

        if (_lIndexView.ItemContainerGenerator.ContainerFromItem(target) is FrameworkElement container)
        {
            container.BringIntoView();
        }
    }

    public bool LIndexHoldCheck(Visual target)
    {
        return _lIndexView.IsAncestorOf(target);
    }

    public static long? LIndexEntryRead(object sender)
    {
        return (sender as FrameworkElement)?.DataContext is LIndexItem item ? item.LIndexItemId : null;
    }
}
