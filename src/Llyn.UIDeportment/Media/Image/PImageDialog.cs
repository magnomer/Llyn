using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PImageTemplate _pImageTemplate;

    internal void PImageAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card })
        {
            _qEditor.QEditorArea.CEditorImage.CImageAdd(card.PCardId);
        }
    }

    public void PImageRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row })
        {
            _qEditor.QEditorArea.CEditorImage.CImageRemove(row.PImageId);
        }
    }

    public void PImageOpenObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row } source)
        {
            _qEditor.QEditorArea.CEditorImage.CImageFileSet(row.PImageId, PImage.PImageOpen(Window.GetWindow(source)));
        }
    }

    internal void PImageApply(FrameworkElement container, object item, string? name)
    {
        PImage.PImageItemRefine(container, item, name);
        if (QLook.QLookPartFind<Button>(container, "PImageChooser") is Button open)
        {
            open.Click -= _pImageTemplate.PImageOpenHandle;
            open.Click += _pImageTemplate.PImageOpenHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PImageEraser") is Button remove)
        {
            remove.Click -= _pImageTemplate.PImageRemoveHandle;
            remove.Click += _pImageTemplate.PImageRemoveHandle;
        }
    }
}
