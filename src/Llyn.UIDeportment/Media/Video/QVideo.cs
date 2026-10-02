using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QVideo
{
    private CVideo _cVideo = null!;

    internal void QVideoIntroduce(CVideo video)
    {
        _cVideo = video;
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
            _cVideo.CVideoFileSet(row.QVideoItemId, QVideoItem.QVideoItemOpen(Window.GetWindow(source)));
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

    internal void QVideoLocationObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: QVideoItem row } box)
        {
            _cVideo.CVideoLocationSet(row.QVideoItemId, box.Text);
        }
    }

    internal void QVideoSpanObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: QVideoItem row } box)
        {
            _cVideo.CVideoSpanSet(row.QVideoItemId, box.Text);
        }
    }
}
