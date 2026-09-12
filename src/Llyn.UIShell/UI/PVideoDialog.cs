using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PVideoAttach(PCard card)
    {
        card.PCardVideoNotice = (row, field) => PEditorRequestDefer(
            PEditorRequestFormat(card, row.PVideoId, field),
            field == nameof(PVideo.PVideoLocation)
                ? new LRequestVideoLocation(_pEditorDraft, row.PVideoId, row.PVideoLocationRead())
                : new LRequestVideoSpan(_pEditorDraft, row.PVideoId, row.PVideoSpanRead()));
    }

    internal void PVideoAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card })
        {
            PEditorRequestSend(new LRequestVideoAddition(
                _pEditorDraft, card.PCardId, LStateWritten.LStateWrittenEmpty, card.PCardVideo.Count));
        }
    }

    internal void PVideoRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row } && PCardVideoFind(row) is PCard card)
        {
            PEditorRequestSend(new LRequestVideoRemoval(_pEditorDraft, card.PCardId, row.PVideoId));
        }
    }

    private bool PVideoPendingCheck(PCard card, PVideo row, string field)
    {
        return PEditorRequestCheck(PEditorRequestFormat(card, row.PVideoId, field));
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
