using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    public ObservableCollection<PExample> PCardExample { get; }

    internal void PCardExampleShow(IReadOnlyList<LExampleDraft> drafts)
    {
        foreach (PExample row in PCardExample)
        {
            row.PropertyChanged -= PCardExampleChange;
        }

        PCardExample.Clear();
        foreach (LExampleDraft draft in drafts)
        {
            PCardExampleAdd(new PExample(
                _pCardReference,
                draft.LExampleDraftText,
                draft.LExampleDraftId,
                draft.LExampleDraftReference));
        }

        if (PCardExample.Count == 0)
        {
            PCardExampleAdd(new PExample(_pCardReference));
        }

        PCardExampleUpdate();
    }

    internal IReadOnlyList<LExampleDraft> PCardExampleRead()
    {
        List<LExampleDraft> drafts = [];
        foreach (PExample row in PCardExample)
        {
            LStateValue text = row.PExampleTextRead();
            if (text.LStateValueEmpty)
            {
                continue;
            }

            drafts.Add(new LExampleDraft(text, row.PExampleId, row.PExampleReferenceRead()));
        }

        return drafts;
    }

    internal void PCardExampleInsert(PExample row)
    {
        int index = PCardExample.IndexOf(row);
        PExample opened = new(_pCardReference);
        opened.PropertyChanged += PCardExampleChange;
        PCardExample.Insert(index < 0 ? PCardExample.Count : index + 1, opened);
        PCardExampleUpdate();
    }

    internal void PCardExampleRemove(PExample row)
    {
        if (PCardExample.Count <= 1)
        {
            row.PExampleClear();
            return;
        }

        row.PropertyChanged -= PCardExampleChange;
        PCardExample.Remove(row);
        PCardExampleUpdate();
    }

    internal void PCardExampleUpdate()
    {
        bool numbered = PCardExample.Count > 1;
        for (int index = 0; index < PCardExample.Count; index++)
        {
            PCardExample[index].PExampleOrderText = numbered
                ? $"({index + 1})"
                : string.Empty;
        }
    }

    private void PCardExampleAdd(PExample row)
    {
        row.PropertyChanged += PCardExampleChange;
        PCardExample.Add(row);
    }

    private void PCardExampleChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is PExample row &&
            string.Equals(arguments.PropertyName, nameof(PExample.PExampleText), StringComparison.Ordinal))
        {
            row.PExampleIdentityApply();
        }
    }
}
