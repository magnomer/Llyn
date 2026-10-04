using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private readonly ObservableCollection<QImageItem> _qScenarioImage = [];

    private readonly ObservableCollection<QVideoItem> _qScenarioVideo = [];

    private readonly QImage _qImage = new();

    private readonly QVideo _qVideo = new();

    private void QScenarioImageRefine(IReadOnlyList<CImageDraft> rows)
    {
        PCard.PCardRowShow(
            _qScenarioImage,
            rows,
            static row => row.QImageItemId,
            static draft => draft.CImageDraftId,
            QScenarioImageCreate,
            (row, draft) =>
            {
                row.QImageItemShow(draft);
                return row;
            });
    }

    private void QScenarioVideoRefine(IReadOnlyList<CVideoDraft> rows)
    {
        PCard.PCardRowShow(
            _qScenarioVideo,
            rows,
            static row => row.QVideoItemId,
            static draft => draft.CVideoDraftId,
            QScenarioVideoCreate,
            (row, draft) =>
            {
                row.QVideoItemShow(draft);
                return row;
            });
    }

    private QImageItem QScenarioImageCreate(CImageDraft draft)
    {
        return new QImageItem(draft);
    }

    private QVideoItem QScenarioVideoCreate(CVideoDraft draft)
    {
        return new QVideoItem(draft);
    }

    private void QImageLocationObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: QImageItem row } box)
        {
            _qImage.QImageFieldObserve(row, box.Text);
        }
    }

    private void QImageItemRefine(FrameworkElement container, object item, string? name)
    {
        if (QLook.QLookPartFind<TextBox>(container, "PImageLocation") is TextBox location)
        {
            location.TextChanged -= QImageLocationObserve;
            _qImage.QImageApply(container, item, name);
            location.TextChanged += QImageLocationObserve;
        }
    }

    private void QVideoItemRefine(FrameworkElement container, object item, string? name)
    {
        if (QLook.QLookPartFind<TextBox>(container, "PVideoLocation") is TextBox location
            && QLook.QLookPartFind<TextBox>(container, "PVideoTimestamp") is TextBox span)
        {
            location.TextChanged -= _qVideo.QVideoLocationObserve;
            span.TextChanged -= _qVideo.QVideoSpanObserve;
            _qVideo.QVideoApply(container, item, name);
        }
    }

    private void QImageAddObserve(object sender, RoutedEventArgs e)
    {
        _cPlaywright.CPlaywrightImage.CImageAdd(0);
    }

    private void QVideoAddObserve(object sender, RoutedEventArgs e)
    {
        _cPlaywright.CPlaywrightVideo.CVideoAdd(0);
    }
}
