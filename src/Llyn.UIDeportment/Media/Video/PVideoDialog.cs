using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    internal void PVideoAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card })
        {
            _qEditor.QEditorArea.CEditorVideo.CVideoAdd(card.PCardId);
        }
    }

    private void PVideoRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row })
        {
            _qEditor.QEditorArea.CEditorVideo.CVideoRemove(row.PVideoId);
        }
    }

    private void PVideoOpenObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row } source)
        {
            _qEditor.QEditorArea.CEditorVideo.CVideoFileSet(row.PVideoId, PVideo.PVideoOpen(Window.GetWindow(source)));
        }
    }

    internal void PVideoApply(FrameworkElement container, object item, string? name)
    {
        PVideo.PVideoItemRefine(container, item, name);
        if (QLook.QLookPartFind<Button>(container, "PVideoChooser") is Button open)
        {
            open.Click -= PVideoOpenObserve;
            open.Click += PVideoOpenObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PVideoEraser") is Button remove)
        {
            remove.Click -= PVideoRemoveObserve;
            remove.Click += PVideoRemoveObserve;
        }
    }
}
