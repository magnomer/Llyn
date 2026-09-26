using System.Windows;
using System.Windows.Controls;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PImageTemplate _pImageTemplate;

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
        if (sender is FrameworkElement { DataContext: PImage row }
            && PImage.PImageOpen(_pEditorHost.PWindowSurface) is string chosen)
        {
            PEditorRequestSend(new LRequestImageLocation(PEditorDraft, row.PImageId, new LStateWritten(chosen)));
        }
    }

    internal void PImageApply(FrameworkElement container, object item, string? name)
    {
        PImage.PImageRowApply(container, item, name);
        if (PLook.PLookPartFind<Button>(container, "PImageChooser") is Button open)
        {
            open.Click -= _pImageTemplate.PImageOpenHandle;
            open.Click += _pImageTemplate.PImageOpenHandle;
        }

        if (PLook.PLookPartFind<Button>(container, "PImageEraser") is Button remove)
        {
            remove.Click -= _pImageTemplate.PImageRemoveHandle;
            remove.Click += _pImageTemplate.PImageRemoveHandle;
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
