using System.Windows;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PImageAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card })
        {
            card.PCardImageAdd();
        }
    }

    internal void PImageRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row })
        {
            PCardImageFind(row)?.PCardImageRemove(row);
        }
    }

    internal void PImageOpenHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PImage row })
        {
            return;
        }

        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Title = "Choose an image",
            Filter = "Image files|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.webp;*.tif;*.tiff|All files|*.*",
            CheckFileExists = true,
        };

        if (dialog.ShowDialog(_pEditorHost) == true)
        {
            row.PImageLocation = dialog.FileName;
        }
    }

    private PCard? PCardImageFind(PImage row)
    {
        foreach (PCard card in _pSenseList)
        {
            if (card.PCardImage.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardImage.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
