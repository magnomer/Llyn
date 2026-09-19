using System.Windows;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    internal void PVideoAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card })
        {
            PEditorRequestSend(new LRequestVideoAddition(
                PEditorDraft, card.PCardId, LStateWritten.LStateWrittenEmpty, card.PCardVideo.Count));
        }
    }

    public void PVideoRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row } && PCardVideoFind(row) is PCard card)
        {
            PEditorRequestSend(new LRequestVideoRemoval(PEditorDraft, card.PCardId, row.PVideoId));
        }
    }

    public void PVideoOpenHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row } && PVideo.PVideoOpen(_pEditorHost) is string chosen)
        {
            PEditorRequestSend(new LRequestVideoLocation(PEditorDraft, row.PVideoId, new LStateWritten(chosen)));
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
