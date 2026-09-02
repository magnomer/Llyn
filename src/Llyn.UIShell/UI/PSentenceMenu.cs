using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PReference> _pSentenceReference = [];

    internal void PSentenceLoad()
    {
        _pSentenceReference.Clear();

        IReadOnlyList<LReference> references;
        try
        {
            references = _lEngine.LEngineReferenceRead();
        }
        catch (Exception)
        {
            references = [];
        }

        foreach (LReference reference in references)
        {
            _pSentenceReference.Add(PReference.PReferenceCreate(reference));
        }

        PSentenceReferenceShow();
        PContextReferenceShow();
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

    internal void PSentenceReferenceClear(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PSentence row })
        {
            row.PSentenceReference = string.Empty;
        }
    }

    internal void PSentenceReferenceCreate(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row })
        {
            return;
        }

        string name = row.PSentenceReferenceDraft.Trim();
        if (name.Length == 0)
        {
            return;
        }

        LReference created;
        try
        {
            created = _lEngine.LEngineReferenceCreate(new LReference(
                string.Empty,
                LStateValue.LStateValueCreate(name),
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified,
                LState.LStateUnspecified));
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Reference.AddFailed", exception);
            return;
        }

        _pSentenceReference.Add(PReference.PReferenceCreate(created));
        row.PSentenceReferenceDraft = string.Empty;
        row.PSentenceReference = created.LReferenceId;
    }

    private void PSentenceReferenceShow()
    {
        foreach (PCard card in _pSenseList)
        {
            PSentenceReferenceShow(card);
        }

        foreach (PCard card in _pCollocationList)
        {
            PSentenceReferenceShow(card);
        }
    }

    private static void PSentenceReferenceShow(PCard card)
    {
        foreach (PSentence row in card.PCardSentence)
        {
            row.PSentenceReferenceShow();
        }
    }

    private PCard? PCardSentenceFind(PSentence row)
    {
        foreach (PCard card in _pSenseList)
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
