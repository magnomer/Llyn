using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    public ObservableCollection<PSentence> PCardSentence { get; }

    internal void PCardSentenceShow(IReadOnlyList<LExampleDraft> drafts)
    {
        foreach (PSentence row in PCardSentence)
        {
            row.PropertyChanged -= PCardSentenceChange;
        }

        PCardSentence.Clear();
        foreach (LExampleDraft draft in drafts)
        {
            PCardSentenceAdd(new PSentence(
                _pCardCitation,
                draft.LExampleDraftText,
                draft.LExampleDraftId,
                draft.LExampleDraftReference));
        }

        if (PCardSentence.Count == 0)
        {
            PCardSentenceAdd(new PSentence(_pCardCitation));
        }

        PCardSentenceUpdate();
    }

    internal IReadOnlyList<LExampleDraft> PCardSentenceRead()
    {
        List<LExampleDraft> drafts = [];
        foreach (PSentence row in PCardSentence)
        {
            LStateValue text = row.PSentenceTextRead();
            if (text.LStateValueEmpty)
            {
                continue;
            }

            drafts.Add(new LExampleDraft(text, row.PSentenceId, row.PSentenceCitationRead()));
        }

        return drafts;
    }

    internal void PCardSentenceInsert(PSentence row)
    {
        int index = PCardSentence.IndexOf(row);
        PSentence opened = new(_pCardCitation);
        opened.PropertyChanged += PCardSentenceChange;
        PCardSentence.Insert(index < 0 ? PCardSentence.Count : index + 1, opened);
        PCardSentenceUpdate();
    }

    internal void PCardSentenceRemove(PSentence row)
    {
        if (PCardSentence.Count <= 1)
        {
            row.PSentenceClear();
            return;
        }

        row.PropertyChanged -= PCardSentenceChange;
        PCardSentence.Remove(row);
        PCardSentenceUpdate();
    }

    internal void PCardSentenceUpdate()
    {
        bool numbered = PCardSentence.Count > 1;
        for (int index = 0; index < PCardSentence.Count; index++)
        {
            PCardSentence[index].PSentenceOrderText = numbered
                ? $"({index + 1})"
                : string.Empty;
        }
    }

    private void PCardSentenceAdd(PSentence row)
    {
        row.PropertyChanged += PCardSentenceChange;
        PCardSentence.Add(row);
    }

    private void PCardSentenceChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is PSentence row &&
            string.Equals(arguments.PropertyName, nameof(PSentence.PSentenceText), StringComparison.Ordinal))
        {
            row.PSentenceIdentityApply();
        }
    }
}
