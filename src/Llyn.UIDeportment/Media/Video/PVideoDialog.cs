using System.Windows;
using System.Windows.Controls;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PVideoTemplate _pVideoTemplate;

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
        if (sender is FrameworkElement { DataContext: PVideo row }
            && PVideo.PVideoOpen(_pEditorHost.PWindowSurface) is string chosen)
        {
            PEditorRequestSend(new LRequestVideoLocation(PEditorDraft, row.PVideoId, new LStateWritten(chosen)));
        }
    }

    internal void PVideoApply(FrameworkElement container, object item, string? name)
    {
        PVideo.PVideoRowApply(container, item, name);
        if (PLook.PLookPartFind<Button>(container, "PVideoChooser") is Button open)
        {
            open.Click -= _pVideoTemplate.PVideoOpenHandle;
            open.Click += _pVideoTemplate.PVideoOpenHandle;
        }

        if (PLook.PLookPartFind<Button>(container, "PVideoEraser") is Button remove)
        {
            remove.Click -= _pVideoTemplate.PVideoRemoveHandle;
            remove.Click += _pVideoTemplate.PVideoRemoveHandle;
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
