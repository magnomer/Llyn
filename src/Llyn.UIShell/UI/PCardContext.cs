using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    public ObservableCollection<PContext> PCardContext { get; }

    internal void PCardContextShow(IReadOnlyList<LSituationDraft> drafts)
    {
        foreach (PContext row in PCardContext)
        {
            row.PropertyChanged -= PCardContextChange;
        }

        PCardContext.Clear();
        foreach (LSituationDraft draft in drafts)
        {
            PCardContextAdd(new PContext(draft.LSituationDraftText, draft.LSituationDraftId));
        }

        if (PCardContext.Count == 0)
        {
            PCardContextAdd(new PContext());
        }

        PCardContextUpdate();
    }

    internal IReadOnlyList<LSituationDraft> PCardContextRead()
    {
        List<LSituationDraft> drafts = [];
        foreach (PContext row in PCardContext)
        {
            LStateValue text = row.PContextTextRead();
            if (text.LStateValueEmpty)
            {
                continue;
            }

            drafts.Add(new LSituationDraft(text, row.PContextId));
        }

        return drafts;
    }

    internal void PCardContextInsert(PContext row)
    {
        int index = PCardContext.IndexOf(row);
        PContext opened = new();
        opened.PropertyChanged += PCardContextChange;
        PCardContext.Insert(index < 0 ? PCardContext.Count : index + 1, opened);
        PCardContextUpdate();
    }

    internal void PCardContextRemove(PContext row)
    {
        if (PCardContext.Count <= 1)
        {
            row.PContextClear();
            return;
        }

        row.PropertyChanged -= PCardContextChange;
        PCardContext.Remove(row);
        PCardContextUpdate();
    }

    internal void PCardContextUpdate()
    {
        bool numbered = PCardContext.Count > 1;
        for (int index = 0; index < PCardContext.Count; index++)
        {
            PCardContext[index].PContextOrderText = numbered
                ? $"({index + 1})"
                : string.Empty;
        }
    }

    private void PCardContextAdd(PContext row)
    {
        row.PropertyChanged += PCardContextChange;
        PCardContext.Add(row);
    }

    private void PCardContextChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is PContext row &&
            string.Equals(arguments.PropertyName, nameof(PContext.PContextText), StringComparison.Ordinal))
        {
            row.PContextIdentityApply();
        }
    }
}
