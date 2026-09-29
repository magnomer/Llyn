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

    internal bool QIndexShown { get; private set; }

    internal void QIndexShownRefine(bool shown)
    {
        QIndexShown = shown;
        _qIndexView.Visibility = shown ? Visibility.Visible : Visibility.Collapsed;
    }

    internal void QIndexRefine(IReadOnlyList<CVistaRow> rows, bool empty)
    {
        ArgumentNullException.ThrowIfNull(rows);

        LSplice.LSpliceApply(
            _qIndexList,
            QIndexItem.QIndexItemBuild(rows),
            QIndexItem.QIndexItemMatch,
            QIndexItem.QIndexItemSync);
        _qIndexEmpty.Visibility = empty ? Visibility.Visible : Visibility.Collapsed;
    }

    internal void QIndexClearRefine()
    {
        _qIndexList.Clear();
    }

    internal long? QIndexChosenRead()
    {
        return _qIndexList.FirstOrDefault(row => row.QIndexItemChosen)?.QIndexItemId;
    }

    internal void QIndexScrollRefine(long? id)
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
