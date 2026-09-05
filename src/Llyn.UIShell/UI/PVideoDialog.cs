using System.Windows;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PVideoAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card })
        {
            card.PCardVideoAdd();
        }
    }

    internal void PVideoRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row })
        {
            PCardVideoFind(row)?.PCardVideoRemove(row);
        }
    }

    internal void PVideoOpenHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PVideo row })
        {
            return;
        }

        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Title = "Choose a video",
            Filter = "Video files|*.mp4;*.m4v;*.mov;*.avi;*.wmv;*.mkv;*.webm|All files|*.*",
            CheckFileExists = true,
        };

        if (dialog.ShowDialog(_pEditorHost) == true)
        {
            row.PVideoLocation = dialog.FileName;
        }
    }

    private PCard? PCardVideoFind(PVideo row)
    {
        foreach (PCard card in _pMeaningList)
        {
            if (card.PCardVideo.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardVideo.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
