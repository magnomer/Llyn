using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    public ObservableCollection<PContext> PCardSituation { get; }

    internal void PCardSituationShow(IReadOnlyList<LSituationDraft> drafts)
    {
        foreach (PContext row in PCardSituation)
        {
            row.PropertyChanged -= PCardSituationChange;
        }

        PCardSituation.Clear();
        foreach (LSituationDraft draft in drafts)
        {
            PCardSituationAdd(new PContext(
                _pCardReference,
                draft.LSituationDraftText,
                draft.LSituationDraftId,
                draft.LSituationDraftReference));
        }

        if (PCardSituation.Count == 0)
        {
            PCardSituationAdd(new PContext(_pCardReference));
        }

        PCardSituationUpdate();
    }

    internal IReadOnlyList<LSituationDraft> PCardSituationRead()
    {
        List<LSituationDraft> drafts = [];
        foreach (PContext row in PCardSituation)
        {
            LStateValue text = row.PContextTextRead();
            if (text.LStateValueEmpty)
            {
                continue;
            }

            drafts.Add(new LSituationDraft(text, row.PContextId, row.PContextReferenceRead()));
        }

        return drafts;
    }

    internal void PCardSituationInsert(PContext row)
    {
        int index = PCardSituation.IndexOf(row);
        PContext opened = new(_pCardReference);
        opened.PropertyChanged += PCardSituationChange;
        PCardSituation.Insert(index < 0 ? PCardSituation.Count : index + 1, opened);
        PCardSituationUpdate();
    }

    internal void PCardSituationRemove(PContext row)
    {
        if (PCardSituation.Count <= 1)
        {
            row.PContextClear();
            return;
        }

        row.PropertyChanged -= PCardSituationChange;
        PCardSituation.Remove(row);
        PCardSituationUpdate();
    }

    internal void PCardSituationUpdate()
    {
        bool numbered = PCardSituation.Count > 1;
        for (int index = 0; index < PCardSituation.Count; index++)
        {
            PCardSituation[index].PContextOrderText = numbered
                ? $"({index + 1})"
                : string.Empty;
        }
    }

    private void PCardSituationAdd(PContext row)
    {
        row.PropertyChanged += PCardSituationChange;
        PCardSituation.Add(row);
    }

    private void PCardSituationChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is PContext row &&
            string.Equals(arguments.PropertyName, nameof(PContext.PContextText), StringComparison.Ordinal))
        {
            row.PContextIdentityApply();
        }
    }
}
