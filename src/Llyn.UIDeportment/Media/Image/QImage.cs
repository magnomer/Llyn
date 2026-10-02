using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QImage
{
    private CImage _cImage = null!;

    internal void QImageIntroduce(CImage image)
    {
        _cImage = image;
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
