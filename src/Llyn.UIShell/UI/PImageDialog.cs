using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PImageAttach(PCard card)
    {
        card.PCardImageNotice = row => PEditorRequestDefer(
            PEditorRequestFormat(card, row.PImageId, nameof(PImage.PImageLocation)),
            new LRequestImageLocation(_pEditorDraft, row.PImageId, row.PImageLocationRead()));
    }

    internal void PImageAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCard card })
        {
            PEditorRequestSend(new LRequestImageAddition(
                _pEditorDraft, card.PCardId, LStateWritten.LStateWrittenEmpty, card.PCardImage.Count));
        }
    }

    public void PImageRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row } && PCardImageFind(row) is PCard card)
        {
            PEditorRequestSend(new LRequestImageRemoval(_pEditorDraft, card.PCardId, row.PImageId));
        }
    }

    public void PImageOpenHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row } && PImage.PImageOpen(_pEditorHost) is string chosen)
        {
            row.PImageLocation = chosen;
        }
    }

    private bool PImagePendingCheck(PCard card, PImage row)
    {
        return PEditorRequestCheck(PEditorRequestFormat(card, row.PImageId, nameof(PImage.PImageLocation)));
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
