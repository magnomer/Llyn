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

    internal void PImageRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row } && PCardImageFind(row) is PCard card)
        {
            PEditorRequestSend(new LRequestImageRemoval(_pEditorDraft, card.PCardId, row.PImageId));
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
