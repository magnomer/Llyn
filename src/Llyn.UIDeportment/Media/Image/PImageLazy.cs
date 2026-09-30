using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class PImageLazy : Decorator
{
    private const double PImageLazyReach = 240;

    public static readonly DependencyProperty PImageRowProperty = DependencyProperty.Register(
        nameof(PImageLazyRow),
        typeof(PImagePending),
        typeof(PImageLazy),
        new PropertyMetadata(null, PImageRowRefine));

    private readonly List<ScrollViewer> _pImageLazyViewers = [];

    private bool _pImageLazyWatched;

    public PImageLazy()
    {
        Loaded += PImageLoadRefine;
        Unloaded += PImageDropRefine;
        IsVisibleChanged += PImageVisibleRefine;
        DataContextChanged += PImageContextRefine;
    }

    public PImagePending? PImageLazyRow
    {
        get => (PImagePending?)GetValue(PImageRowProperty);
        set => SetValue(PImageRowProperty, value);
    }

    private static void PImageRowRefine(DependencyObject holder, DependencyPropertyChangedEventArgs e)
    {
        if (holder is PImageLazy element && element.IsLoaded)
        {
            element.PImageLazyCheck();
        }
    }

    private void PImageContextRefine(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is not CImageDraft draft)
        {
            return;
        }

        PImageEmptyRefine(draft);
        PImagePendingRefine(draft);
    }

    private void PImageEmptyRefine(CImageDraft draft)
    {
        Visibility = QLook.QLookVisibleRead(!draft.CImageDraftEmpty);
    }

    private void PImagePendingRefine(CImageDraft draft)
    {
        if (PMedia.PMediaRead(this) is PMedia media)
        {
            DataContext = media.PMediaImageCreate(draft);
        }
    }

    private void PImageLoadRefine(object sender, RoutedEventArgs e)
    {
        PImageLazyCheck();
    }

    private void PImageDropRefine(object sender, RoutedEventArgs e)
    {
        PImageLazyDetach();
    }

    private void PImageVisibleRefine(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsLoaded)
        {
            PImageLazyCheck();
        }
    }

    private void PImageScrollRefine(object sender, ScrollChangedEventArgs e)
    {
        PImageLazyCheck();
    }

    private void PImageSizeRefine(object sender, SizeChangedEventArgs e)
    {
        PImageLazyCheck();
    }

    private void PImageLazyCheck()
    {
        if (PImageLazyRow is not PImagePending row)
        {
            return;
        }

        if (!PImageShownCheck())
        {
            PImageLazyAttach();
            return;
        }

        PImageLazyDetach();
        row.PImageLoad();
    }

    private bool PImageShownCheck()
    {
        if (!IsVisible)
        {
            return false;
        }

        foreach (ScrollViewer viewer in PImageViewerScan())
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
                -PImageLazyReach,
                -PImageLazyReach,
                viewer.ViewportWidth + 2 * PImageLazyReach,
                viewer.ViewportHeight + 2 * PImageLazyReach);
            if (!window.IntersectsWith(shape))
            {
                return false;
            }
        }

        return true;
    }

    private IEnumerable<ScrollViewer> PImageViewerScan()
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

    private void PImageLazyAttach()
    {
        if (_pImageLazyWatched)
        {
            return;
        }

        _pImageLazyWatched = true;
        foreach (ScrollViewer viewer in PImageViewerScan())
        {
            viewer.ScrollChanged += PImageScrollRefine;
            viewer.SizeChanged += PImageSizeRefine;
            _pImageLazyViewers.Add(viewer);
        }
    }

    private void PImageLazyDetach()
    {
        if (!_pImageLazyWatched)
        {
            return;
        }

        _pImageLazyWatched = false;
        foreach (ScrollViewer viewer in _pImageLazyViewers)
        {
            viewer.ScrollChanged -= PImageScrollRefine;
            viewer.SizeChanged -= PImageSizeRefine;
        }

        _pImageLazyViewers.Clear();
    }
}
