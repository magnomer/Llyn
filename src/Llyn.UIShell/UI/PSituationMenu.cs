using System;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PSituationAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PSituation row })
        {
            PCardSituationFind(row)?.PCardSituationInsert(row);
        }
    }

    internal void PSituationRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PSituation row })
        {
            PCardSituationFind(row)?.PCardSituationRemove(row);
        }
    }

    internal void PSituationReferenceClear(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PSituation row })
        {
            row.PSituationReference = string.Empty;
        }
    }

    internal void PSituationReferenceCreate(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSituation row })
        {
            return;
        }

        string name = row.PSituationReferenceDraft.Trim();
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

        _pExampleReference.Add(PReference.PReferenceCreate(created));
        row.PSituationReferenceDraft = string.Empty;
        row.PSituationReference = created.LReferenceId;
    }

    private void PSituationReferenceShow()
    {
        foreach (PCard card in _pSenseList)
        {
            PSituationReferenceShow(card);
        }

        foreach (PCard card in _pCollocationList)
        {
            PSituationReferenceShow(card);
        }
    }

    private static void PSituationReferenceShow(PCard card)
    {
        foreach (PSituation row in card.PCardSituation)
        {
            row.PSituationReferenceShow();
        }
    }

    private PCard? PCardSituationFind(PSituation row)
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
