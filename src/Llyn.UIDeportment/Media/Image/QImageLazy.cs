using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Llyn.UIDeportment;

public sealed class QImageLazy : Decorator
{
    private const double QImageLazyReach = 240;

    public static readonly DependencyProperty QImageRowProperty = DependencyProperty.Register(
        nameof(QImageLazyRow),
        typeof(QImagePending),
        typeof(QImageLazy),
        new PropertyMetadata(null, QImageRowRefine));

    private readonly List<ScrollViewer> _pImageLazyViewers = [];

    private bool _pImageLazyWatched;

    public QImageLazy()
    {
        Loaded += QImageLoadRefine;
        Unloaded += QImageDropRefine;
        IsVisibleChanged += QImageVisibleRefine;
        DataContextChanged += QImageContextRefine;
    }

    public QImagePending? QImageLazyRow
    {
        get => (QImagePending?)GetValue(QImageRowProperty);
        set => SetValue(QImageRowProperty, value);
    }

    private static void QImageRowRefine(DependencyObject holder, DependencyPropertyChangedEventArgs e)
    {
        if (holder is QImageLazy element && element.IsLoaded)
        {
            element.QImageLazyCheck();
        }
    }

    private void QImageContextRefine(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is not QLeafImage image)
        {
            return;
        }

        QImageEmptyRefine(image);
        QImagePendingRefine(image);
    }

    private void QImageEmptyRefine(QLeafImage image)
    {
        Visibility = QLook.QLookVisibleRead(!image.QLeafImageEmpty);
    }

    private void QImagePendingRefine(QLeafImage image)
    {
        DataContext = image.QLeafImageRow;
    }

    private void QImageLoadRefine(object sender, RoutedEventArgs e)
    {
        QImageLazyCheck();
    }

    private void QImageDropRefine(object sender, RoutedEventArgs e)
    {
        QImageLazyDetach();
    }

    private void QImageVisibleRefine(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsLoaded)
        {
            QImageLazyCheck();
        }
    }

    private void QImageScrollRefine(object sender, ScrollChangedEventArgs e)
    {
        QImageLazyCheck();
    }

    private void QImageSizeRefine(object sender, SizeChangedEventArgs e)
    {
        QImageLazyCheck();
    }

    private void QImageLazyCheck()
    {
        if (QImageLazyRow is not QImagePending row)
        {
            return;
        }

        if (!QImageShownCheck())
        {
            QImageLazyAttach();
            return;
        }

        QImageLazyDetach();
        row.QImagePendingLoad();
    }

    private bool QImageShownCheck()
    {
        if (!IsVisible)
        {
            return false;
        }

        foreach (ScrollViewer viewer in QImageViewerScan())
        {
            if (viewer.ViewportHeight <= 0 && viewer.ViewportWidth <= 0)
            {
                continue;
            }

            Point origin;
            try
            {
                origin = TransformToAncestor(viewer).Transform(new Point(0, 0));
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            Rect shape = new(origin, new Size(Math.Max(ActualWidth, 1), Math.Max(ActualHeight, 1)));
            Rect window = new(
                -QImageLazyReach,
                -QImageLazyReach,
                viewer.ViewportWidth + 2 * QImageLazyReach,
                viewer.ViewportHeight + 2 * QImageLazyReach);
            if (!window.IntersectsWith(shape))
            {
                return false;
            }
        }

        return true;
    }

    private IEnumerable<ScrollViewer> QImageViewerScan()
    {
        DependencyObject? node = VisualTreeHelper.GetParent(this);
        while (node is not null)
        {
            if (node is ScrollViewer viewer)
            {
                yield return viewer;
            }

            node = VisualTreeHelper.GetParent(node);
        }
    }

    private void QImageLazyAttach()
    {
        if (_pImageLazyWatched)
        {
            return;
        }

        _pImageLazyWatched = true;
        foreach (ScrollViewer viewer in QImageViewerScan())
        {
            viewer.ScrollChanged += QImageScrollRefine;
            viewer.SizeChanged += QImageSizeRefine;
            _pImageLazyViewers.Add(viewer);
        }
    }

    private void QImageLazyDetach()
    {
        if (!_pImageLazyWatched)
        {
            return;
        }

        _pImageLazyWatched = false;
        foreach (ScrollViewer viewer in _pImageLazyViewers)
        {
            viewer.ScrollChanged -= QImageScrollRefine;
            viewer.SizeChanged -= QImageSizeRefine;
        }

        _pImageLazyViewers.Clear();
    }
}
