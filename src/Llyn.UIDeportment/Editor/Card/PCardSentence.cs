using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    public ObservableCollection<PSentence> PCardSentence { get; } = [];

    internal Action<PSentence, PGloss, string>? PCardSentenceNotice { get; set; }

    internal void PCardSentenceApply(CSentenceOrder order)
    {
        foreach (PSentence row in PCardSentence)
        {
            row.PSentenceOrderApply(order);
        }
    }

    internal void PCardSentenceShow(IReadOnlyList<CSentenceDraft> drafts, CSentenceOrder? order)
    {
        PCardRowShow(
            PCardSentence,
            drafts,
            static row => row.PSentenceRow,
            static draft => draft.CSentenceDraftId,
            draft => PCardSentenceCreate(draft, order),
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

    private PSentence PCardSentenceCreate(CSentenceDraft draft, CSentenceOrder? order)
    {
        PSentence row = new(_pCardCitation, _pCardParticle, _pCardDependence, _pCardLanguage, draft);
        if (order is not null)
        {
            row.PSentenceOrderApply(order);
        }

        row.PSentenceGlossNotice = (gloss, language) => PCardSentenceNotice?.Invoke(row, gloss, language);
        return row;
    }
}
