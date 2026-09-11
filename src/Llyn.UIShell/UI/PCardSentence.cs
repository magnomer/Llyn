using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    private LSentenceOrder _pCardSentenceOrder = LSentenceOrder.LSentenceOrderDefault;

    public ObservableCollection<PSentence> PCardSentence { get; }

    internal void PCardSentenceApply(LSentenceOrder order)
    {
        _pCardSentenceOrder = order ?? LSentenceOrder.LSentenceOrderDefault;
        foreach (PSentence row in PCardSentence)
        {
            row.PSentenceOrderApply(_pCardSentenceOrder);
        }
    }

    internal void PCardSentenceShow(IReadOnlyList<LSentenceDraft> drafts)
    {
        foreach (PSentence row in PCardSentence)
        {
            row.PropertyChanged -= PCardSentenceChange;
        }

        PCardSentence.Clear();
        foreach (LSentenceDraft draft in drafts)
        {
            PCardSentenceAdd(new PSentence(_pCardEngine, _pCardCitation, _pCardParticle, _pCardDependence, draft));
        }

        if (PCardSentence.Count == 0)
        {
            PCardSentenceAdd(new PSentence(_pCardEngine, _pCardCitation, _pCardParticle, _pCardDependence));
        }
    }

    internal IReadOnlyList<LSentenceDraft> PCardSentenceRead()
    {
        List<LSentenceDraft> drafts = [];
        foreach (PSentence row in PCardSentence)
        {
            if (!row.PSentenceCheck())
            {
                continue;
            }

            drafts.Add(row.PSentenceDraftRead());
        }

        return drafts;
    }

    internal void PCardSentenceInsert(PSentence row)
    {
        int index = PCardSentence.IndexOf(row);
        PSentence opened = new(_pCardEngine, _pCardCitation, _pCardParticle, _pCardDependence);
        opened.PSentenceOrderApply(_pCardSentenceOrder);
        opened.PropertyChanged += PCardSentenceChange;
        PCardSentence.Insert(index < 0 ? PCardSentence.Count : index + 1, opened);
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
    }

    private void PCardSentenceAdd(PSentence row)
    {
        row.PSentenceOrderApply(_pCardSentenceOrder);
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
