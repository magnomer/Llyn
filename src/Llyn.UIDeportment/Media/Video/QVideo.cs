using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QVideo
{
    private readonly ObservableCollection<QVideoItem> _qVideoRow = [];

    private CVideo _cVideo = null!;

    internal void QVideoIntroduce(CVideo video)
    {
        _cVideo = video;
    }

    internal void QVideoRowIntroduce(ItemsControl row, Button fresh)
    {
        row.ItemsSource = _qVideoRow;
        QLookItem.QLookItemAttach(row, QVideoApply);
        fresh.Click += QVideoFreshObserve;
    }

    internal void QVideoRowRefine(IReadOnlyList<CVideoDraft> rows)
    {
        QLookItem.QLookItemShow(
            _qVideoRow,
            rows,
            static row => row.QVideoItemId,
            static draft => draft.CVideoDraftId,
            static draft => new QVideoItem(draft),
            (row, draft) =>
            {
                row.QVideoItemShow(draft);
                return row;
            });
    }

    private void QVideoFreshObserve(object sender, RoutedEventArgs e)
    {
        _cVideo.CVideoAdd(false, null);
    }

    internal void QVideoAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QPlacard card })
        {
            _cVideo.CVideoAdd(card.QPlacardCollocated, card.QPlacardId);
        }
    }

    private void QVideoRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QVideoItem row })
        {
            _cVideo.CVideoRemove(row.QVideoItemCollocated, row.QVideoItemHolder, row.QVideoItemId);
        }
    }

    private void QVideoOpenObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QVideoItem row })
        {
            _cVideo.CVideoFileOpen(row.QVideoItemId);
        }
    }

    internal void QVideoApply(FrameworkElement container, object item, string? name)
    {
        QVideoItem.QVideoItemRefine(container, item, name);
        if (QLook.QLookPartFind<Button>(container, "PVideoChooser") is Button open)
        {
            open.Click -= QVideoOpenObserve;
            open.Click += QVideoOpenObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PVideoEraser") is Button remove)
        {
            remove.Click -= QVideoRemoveObserve;
            remove.Click += QVideoRemoveObserve;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PVideoLocation") is TextBox location)
        {
            location.TextChanged -= QVideoLocationObserve;
            location.TextChanged += QVideoLocationObserve;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PVideoTimestamp") is TextBox span)
        {
            span.TextChanged -= QVideoSpanObserve;
            span.TextChanged += QVideoSpanObserve;
        }
    }

    private void QVideoLocationObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: QVideoItem row } box)
        {
            _cVideo.CVideoLocationSet(row.QVideoItemId, box.Text);
        }
    }

    private void QVideoSpanObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: QVideoItem row } box)
        {
            _cVideo.CVideoSpanSet(row.QVideoItemId, box.Text);
        }
    }
}
