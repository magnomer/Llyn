using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    internal void PImageAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card })
        {
            _qEditor.QEditorArea.CEditorImage.CImageAdd(card.PCardId);
        }
    }

    private void PImageRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row })
        {
            _qEditor.QEditorArea.CEditorImage.CImageRemove(row.PImageId);
        }
    }

    private void PImageOpenObserve(object sender, RoutedEventArgs e)
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
            open.Click -= PImageOpenObserve;
            open.Click += PImageOpenObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PImageEraser") is Button remove)
        {
            remove.Click -= PImageRemoveObserve;
            remove.Click += PImageRemoveObserve;
        }
    }
}
