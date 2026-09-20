using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed partial class PCard
{
    public ObservableCollection<PSentence> PCardSentence { get; } = [];

    internal Action<PSentence, string>? PCardSentenceNotice { get; set; }

    internal void PCardSentenceApply(LSentenceOrder order)
    {
        foreach (PSentence row in PCardSentence)
        {
            row.PSentenceOrderApply(order);
        }
    }

    internal void PCardSentenceShow(IReadOnlyList<LSentenceDraft> drafts, string language)
    {
        PCardRowShow(
            PCardSentence,
            drafts,
            static row => row.PSentenceRow,
            static draft => draft.LSentenceDraftId,
            draft => PCardSentenceCreate(draft, language),
            (row, draft) =>
            {
                row.PSentenceShow(draft);
                return row;
            });
    }

    internal int PCardSentenceFind(PSentence row)
    {
        return PCardSentence.IndexOf(row);
    }

    private PSentence PCardSentenceCreate(LSentenceDraft draft, string language)
    {
        PSentence row = new(_pCardCitation, _pCardParticle, _pCardDependence, _pCardLanguage, draft);
        row.PSentenceOrderApply(_pCardWindow.LWindowOrderRead(language));
        row.PropertyChanged += PCardSentenceChange;
        return row;
    }

    private void PCardSentenceChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is PSentence row && arguments.PropertyName is string field)
        {
            PCardSentenceNotice?.Invoke(row, field);
        }
    }
}
