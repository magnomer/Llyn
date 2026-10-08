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
        _cVideo.CVideoAdd(null);
    }

    internal void QVideoAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card })
        {
            _cVideo.CVideoAdd(card.PCardId);
        }
    }

    private void QVideoRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QVideoItem row })
        {
            _cVideo.CVideoRemove(row.QVideoItemId);
        }
    }

    private void QVideoOpenObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QVideoItem row } source)
        {
            _cVideo.CVideoFileSet(row.QVideoItemId, QVideoFileConsult(Window.GetWindow(source)));
        }
    }

    private static string? QVideoFileConsult(Window owner)
    {
        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Title = "Choose a video",
            Filter = "Video files|*.mp4;*.m4v;*.mov;*.avi;*.wmv;*.mkv;*.webm|All files|*.*",
            CheckFileExists = true,
        };

        return dialog.ShowDialog(owner) == true ? dialog.FileName : null;
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
