using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PCitationItem> _pEditorCitation = [];

    internal void PSentenceLoad()
    {
        _pEditorCitation.Clear();

        IReadOnlyList<LReference> references;
        try
        {
            references = _lEngine.LEngineReferenceRead();
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Reference.LoadFailed", exception);
            references = [];
        }

        foreach (LReference reference in references)
        {
            _pEditorCitation.Add(PCitationItem.PCitationItemCreate(reference));
        }

        PSentenceCitationShow();
    }

    internal void PSentenceAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PSentence row })
        {
            PCardSentenceFind(row)?.PCardSentenceInsert(row);
        }
    }

    internal void PSentenceRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PSentence row })
        {
            PCardSentenceFind(row)?.PCardSentenceRemove(row);
        }
    }

    internal void PSentenceCitationClear(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PSentence row })
        {
            row.PSentenceCitation = string.Empty;
        }
    }

    private void PSentenceCitationShow()
    {
        foreach (PCard card in _pMeaningList)
        {
            PSentenceCitationShow(card);
        }

        foreach (PCard card in _pCollocationList)
        {
            PSentenceCitationShow(card);
        }
    }

    private static void PSentenceCitationShow(PCard card)
    {
        foreach (PSentence row in card.PCardSentence)
        {
            row.PSentenceCitationShow();
        }
    }

    private PCard? PCardSentenceFind(PSentence row)
    {
        foreach (PCard card in _pMeaningList)
        {
            if (card.PCardSentence.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardSentence.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
