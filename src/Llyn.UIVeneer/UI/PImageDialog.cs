using System.Windows;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    internal void PImageAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card })
        {
            PEditorRequestSend(new LRequestImageAddition(
                PEditorDraft, card.PCardId, LStateWritten.LStateWrittenEmpty, card.PCardImage.Count));
        }
    }

    public void PImageRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row } && PCardImageFind(row) is PCard card)
        {
            PEditorRequestSend(new LRequestImageRemoval(PEditorDraft, card.PCardId, row.PImageId));
        }
    }

    public void PImageOpenHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row } && PImage.PImageOpen(_pEditorHost) is string chosen)
        {
            PEditorRequestSend(new LRequestImageLocation(PEditorDraft, row.PImageId, new LStateWritten(chosen)));
        }
    }

    private PCard? PCardImageFind(PImage row)
    {
        foreach (PCard card in _pMeaningList)
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
