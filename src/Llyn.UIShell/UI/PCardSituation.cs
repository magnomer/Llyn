using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    public ObservableCollection<PSituation> PCardSituation { get; }

    internal void PCardSituationShow(IReadOnlyList<LSituationDraft> drafts)
    {
        foreach (PSituation row in PCardSituation)
        {
            row.PropertyChanged -= PCardSituationChange;
        }

        PCardSituation.Clear();
        foreach (LSituationDraft draft in drafts)
        {
            PCardSituationAdd(new PSituation(
                _pCardReference,
                draft.LSituationDraftText,
                draft.LSituationDraftId,
                draft.LSituationDraftReference));
        }

        if (PCardSituation.Count == 0)
        {
            PCardSituationAdd(new PSituation(_pCardReference));
        }

        PCardSituationUpdate();
    }

    internal IReadOnlyList<LSituationDraft> PCardSituationRead()
    {
        List<LSituationDraft> drafts = [];
        foreach (PSituation row in PCardSituation)
        {
            LStateValue text = row.PSituationTextRead();
            if (text.LStateValueEmpty)
            {
                continue;
            }

            drafts.Add(new LSituationDraft(text, row.PSituationId, row.PSituationReferenceRead()));
        }

        return drafts;
    }

    internal void PCardSituationInsert(PSituation row)
    {
        int index = PCardSituation.IndexOf(row);
        PSituation opened = new(_pCardReference);
        opened.PropertyChanged += PCardSituationChange;
        PCardSituation.Insert(index < 0 ? PCardSituation.Count : index + 1, opened);
        PCardSituationUpdate();
    }

    internal void PCardSituationRemove(PSituation row)
    {
        if (PCardSituation.Count <= 1)
        {
            row.PSituationClear();
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
            PCardSituation[index].PSituationOrderText = numbered
                ? $"({index + 1})"
                : string.Empty;
        }
    }

    private void PCardSituationAdd(PSituation row)
    {
        row.PropertyChanged += PCardSituationChange;
        PCardSituation.Add(row);
    }

    private void PCardSituationChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is PSituation row &&
            string.Equals(arguments.PropertyName, nameof(PSituation.PSituationText), StringComparison.Ordinal))
        {
            row.PSituationIdentityApply();
        }
    }
}
