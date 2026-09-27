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

    private readonly PImageTemplate _qImageTemplate;

    private readonly PVideoTemplate _qVideoTemplate;

    private void QScenarioImageShow(IReadOnlyList<CImageDraft> rows)
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

    private void QScenarioVideoShow(IReadOnlyList<CVideoDraft> rows)
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
        return new PImage(_qRepertoireHost.PWindowAtelier, draft);
    }

    private PVideo QScenarioVideoCreate(CVideoDraft draft)
    {
        return new PVideo(_qRepertoireHost.PWindowAtelier, draft);
    }

    private void QImageLocationHandle(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PImage row } box)
        {
            QScenarioDesk.CDeskEasel?.LEaselImageSet(row.PImageId, box.Text, true);
        }
    }

    private void QVideoLocationHandle(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PVideo row } box)
        {
            QScenarioDesk.CDeskEasel?.LEaselVideoSet(row.PVideoId, box.Text, true);
        }
    }

    private void QVideoSpanHandle(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PVideo row } box)
        {
            QScenarioDesk.CDeskEasel?.LEaselSpanSet(row.PVideoId, box.Text);
        }
    }

    private void QImageApply(FrameworkElement container, object item, string? name)
    {
        if (QLook.QLookPartFind<TextBox>(container, "PImageLocation") is TextBox location)
        {
            location.TextChanged -= QImageLocationHandle;
            PImage.PImageRowApply(container, item, name);
            location.TextChanged += QImageLocationHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PImageChooser") is Button open)
        {
            open.Click -= _qImageTemplate.PImageOpenHandle;
            open.Click += _qImageTemplate.PImageOpenHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PImageEraser") is Button remove)
        {
            remove.Click -= _qImageTemplate.PImageRemoveHandle;
            remove.Click += _qImageTemplate.PImageRemoveHandle;
        }
    }

    private void QVideoApply(FrameworkElement container, object item, string? name)
    {
        if (QLook.QLookPartFind<TextBox>(container, "PVideoLocation") is TextBox location
            && QLook.QLookPartFind<TextBox>(container, "PVideoTimestamp") is TextBox span)
        {
            location.TextChanged -= QVideoLocationHandle;
            span.TextChanged -= QVideoSpanHandle;
            PVideo.PVideoRowApply(container, item, name);
            location.TextChanged += QVideoLocationHandle;
            span.TextChanged += QVideoSpanHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PVideoChooser") is Button open)
        {
            open.Click -= _qVideoTemplate.PVideoOpenHandle;
            open.Click += _qVideoTemplate.PVideoOpenHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PVideoEraser") is Button remove)
        {
            remove.Click -= _qVideoTemplate.PVideoRemoveHandle;
            remove.Click += _qVideoTemplate.PVideoRemoveHandle;
        }
    }

    private void QImageAddHandle(object sender, RoutedEventArgs e)
    {
        QScenarioDesk.CDeskEasel?.LEaselImageAdd(0, _qScenarioImage.Count);
    }

    private void QVideoAddHandle(object sender, RoutedEventArgs e)
    {
        QScenarioDesk.CDeskEasel?.LEaselVideoAdd(0, _qScenarioVideo.Count);
    }

    public void PImageRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row })
        {
            QScenarioDesk.CDeskEasel?.LEaselImageRemove(0, row.PImageId);
        }
    }

    public void PVideoRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row })
        {
            QScenarioDesk.CDeskEasel?.LEaselVideoRemove(0, row.PVideoId);
        }
    }

    public void PImageOpenHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row } source)
        {
            QScenarioImageOpen(row.PImageId, Window.GetWindow(source));
        }
    }

    private void QScenarioImageOpen(long image, Window owner)
    {
        if (PImage.PImageOpen(owner) is not string chosen)
        {
            return;
        }

        QScenarioDesk.CDeskEasel?.LEaselImageSet(image, chosen, false);
    }

    public void PVideoOpenHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row } source)
        {
            QScenarioVideoOpen(row.PVideoId, Window.GetWindow(source));
        }
    }

    private void QScenarioVideoOpen(long video, Window owner)
    {
        if (PVideo.PVideoOpen(owner) is not string chosen)
        {
            return;
        }

        QScenarioDesk.CDeskEasel?.LEaselVideoSet(video, chosen, false);
    }
}
