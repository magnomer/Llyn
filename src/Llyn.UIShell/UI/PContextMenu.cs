using System;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PContextAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PContext row })
        {
            PCardSituationFind(row)?.PCardSituationInsert(row);
        }
    }

    internal void PContextRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PContext row })
        {
            PCardSituationFind(row)?.PCardSituationRemove(row);
        }
    }

    internal void PContextReferenceClear(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PContext row })
        {
            row.PContextReference = string.Empty;
        }
    }

    internal void PContextReferenceCreate(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PContext row })
        {
            return;
        }

        string name = row.PContextReferenceDraft.Trim();
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
        row.PContextReferenceDraft = string.Empty;
        row.PContextReference = created.LReferenceId;
    }

    private void PContextReferenceShow()
    {
        foreach (PCard card in _pSenseList)
        {
            PContextReferenceShow(card);
        }

        foreach (PCard card in _pCollocationList)
        {
            PContextReferenceShow(card);
        }
    }

    private static void PContextReferenceShow(PCard card)
    {
        foreach (PContext row in card.PCardSituation)
        {
            row.PContextReferenceShow();
        }
    }

    private PCard? PCardSituationFind(PContext row)
    {
        foreach (PCard card in _pSenseList)
        {
            if (card.PCardSituation.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardSituation.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
