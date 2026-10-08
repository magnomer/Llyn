using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QImage
{
    private readonly ObservableCollection<QImageItem> _qImageRow = [];

    private CImage _cImage = null!;

    internal void QImageIntroduce(CImage image)
    {
        _cImage = image;
    }

    internal void QImageRowIntroduce(ItemsControl row, Button fresh)
    {
        row.ItemsSource = _qImageRow;
        QLookItem.QLookItemAttach(row, QImageRowApply);
        fresh.Click += QImageFreshObserve;
    }

    internal void QImageRowRefine(IReadOnlyList<CImageDraft> rows)
    {
        QLookItem.QLookItemShow(
            _qImageRow,
            rows,
            static row => row.QImageItemId,
            static draft => draft.CImageDraftId,
            static draft => new QImageItem(draft),
            (row, draft) =>
            {
                row.QImageItemShow(draft);
                return row;
            });
    }

    private void QImageRowApply(FrameworkElement container, object item, string? name)
    {
        if (QLook.QLookPartFind<TextBox>(container, "PImageLocation") is TextBox location)
        {
            location.TextChanged -= QImageLocationObserve;
            QImageApply(container, item, name);
            location.TextChanged += QImageLocationObserve;
        }
    }

    private void QImageLocationObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: QImageItem row } box)
        {
            QImageFieldObserve(row, box.Text);
        }
    }

    private void QImageFreshObserve(object sender, RoutedEventArgs e)
    {
        _cImage.CImageAdd(null);
    }

    internal void QImageAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card })
        {
            _cImage.CImageAdd(card.PCardId);
        }
    }

    private void QImageRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QImageItem row })
        {
            _cImage.CImageRemove(row.QImageItemId);
        }
    }

    private void QImageOpenObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QImageItem row } source)
        {
            _cImage.CImageFileSet(row.QImageItemId, QImageItem.QImageItemOpen(Window.GetWindow(source)));
        }
    }

    internal void QImageApply(FrameworkElement container, object item, string? name)
    {
        QImageItem.QImageItemRefine(container, item, name);
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

    internal void QImageFieldObserve(QImageItem row, string text)
    {
        _cImage.CImageLocationSet(row.QImageItemId, text);
    }
}
