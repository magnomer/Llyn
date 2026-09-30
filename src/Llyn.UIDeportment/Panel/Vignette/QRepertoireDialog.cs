using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private readonly ObservableCollection<PImage> _qScenarioImage = [];

    private readonly ObservableCollection<PVideo> _qScenarioVideo = [];

    private void QScenarioImageRefine(IReadOnlyList<CImageDraft> rows)
    {
        PCard.PCardRowShow(
            _qScenarioImage,
            rows,
            static row => row.PImageId,
            static draft => draft.CImageDraftId,
            QScenarioImageCreate,
            (row, draft) =>
            {
                row.PImageShow(draft);
                return row;
            });
    }

    private void QScenarioVideoRefine(IReadOnlyList<CVideoDraft> rows)
    {
        PCard.PCardRowShow(
            _qScenarioVideo,
            rows,
            static row => row.PVideoId,
            static draft => draft.CVideoDraftId,
            QScenarioVideoCreate,
            (row, draft) =>
            {
                row.PVideoShow(draft);
                return row;
            });
    }

    private PImage QScenarioImageCreate(CImageDraft draft)
    {
        return new PImage(draft);
    }

    private PVideo QScenarioVideoCreate(CVideoDraft draft)
    {
        return new PVideo(draft);
    }

    private void QImageLocationObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PImage row } box)
        {
            _cRepertoire.CRepertoireImage.CImageLocationSet(row.PImageId, box.Text);
        }
    }

    private void QVideoLocationObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PVideo row } box)
        {
            _cRepertoire.CRepertoireVideo.CVideoLocationSet(row.PVideoId, box.Text);
        }
    }

    private void QVideoSpanObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PVideo row } box)
        {
            _cRepertoire.CRepertoireVideo.CVideoSpanSet(row.PVideoId, box.Text);
        }
    }

    private void QImageItemRefine(FrameworkElement container, object item, string? name)
    {
        if (QLook.QLookPartFind<TextBox>(container, "PImageLocation") is TextBox location)
        {
            location.TextChanged -= QImageLocationObserve;
            PImage.PImageItemRefine(container, item, name);
            location.TextChanged += QImageLocationObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PImageChooser") is Button open)
        {
            open.Click -= QImageOpenObserve;
            open.Click += QImageOpenObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PImageEraser") is Button remove)
        {
            remove.Click -= QImageRemoveObserve;
            remove.Click += QImageRemoveObserve;
        }
    }

    private void QVideoItemRefine(FrameworkElement container, object item, string? name)
    {
        if (QLook.QLookPartFind<TextBox>(container, "PVideoLocation") is TextBox location
            && QLook.QLookPartFind<TextBox>(container, "PVideoTimestamp") is TextBox span)
        {
            location.TextChanged -= QVideoLocationObserve;
            span.TextChanged -= QVideoSpanObserve;
            PVideo.PVideoItemRefine(container, item, name);
            location.TextChanged += QVideoLocationObserve;
            span.TextChanged += QVideoSpanObserve;
        }

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
    }

    private void QImageAddObserve(object sender, RoutedEventArgs e)
    {
        _cRepertoire.CRepertoireImage.CImageAdd(0);
    }

    private void QVideoAddObserve(object sender, RoutedEventArgs e)
    {
        _cRepertoire.CRepertoireVideo.CVideoAdd(0);
    }

    private void QImageRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row })
        {
            _cRepertoire.CRepertoireImage.CImageRemove(row.PImageId);
        }
    }

    private void QVideoRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row })
        {
            _cRepertoire.CRepertoireVideo.CVideoRemove(row.PVideoId);
        }
    }

    private void QImageOpenObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row } source)
        {
            _cRepertoire.CRepertoireImage.CImageFileSet(row.PImageId, PImage.PImageOpen(Window.GetWindow(source)));
        }
    }

    private void QVideoOpenObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row } source)
        {
            _cRepertoire.CRepertoireVideo.CVideoFileSet(row.PVideoId, PVideo.PVideoOpen(Window.GetWindow(source)));
        }
    }
}
