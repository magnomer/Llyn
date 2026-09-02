using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PReference> _pExampleReference = [];

    internal void PExampleLoad()
    {
        _pExampleReference.Clear();

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
            _pExampleReference.Add(PReference.PReferenceCreate(reference));
        }

        PExampleReferenceShow();
        PSituationReferenceShow();
    }

    internal void PExampleAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PExample row })
        {
            PCardExampleFind(row)?.PCardExampleInsert(row);
        }
    }

    internal void PExampleRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PExample row })
        {
            PCardExampleFind(row)?.PCardExampleRemove(row);
        }
    }

    internal void PExampleReferenceClear(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PExample row })
        {
            row.PExampleReference = string.Empty;
        }
    }

    internal void PExampleReferenceCreate(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PExample row })
        {
            return;
        }

        string name = row.PExampleReferenceDraft.Trim();
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
        row.PExampleReferenceDraft = string.Empty;
        row.PExampleReference = created.LReferenceId;
    }

    private void PExampleReferenceShow()
    {
        foreach (PCard card in _pSenseList)
        {
            PExampleReferenceShow(card);
        }

        foreach (PCard card in _pCollocationList)
        {
            PExampleReferenceShow(card);
        }
    }

    private static void PExampleReferenceShow(PCard card)
    {
        foreach (PExample row in card.PCardExample)
        {
            row.PExampleReferenceShow();
        }
    }

    private PCard? PCardExampleFind(PExample row)
    {
        foreach (PCard card in _pSenseList)
        {
            if (card.PCardExample.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardExample.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
