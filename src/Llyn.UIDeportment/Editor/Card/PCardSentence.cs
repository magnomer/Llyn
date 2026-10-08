using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PCardSentence
{
    private readonly ObservableCollection<QCitationItem> _pCardSentenceCitation;
    private readonly ObservableCollection<string> _pCardSentenceParticle;
    private readonly ObservableCollection<string> _pCardSentenceDependence;
    private readonly ObservableCollection<PLanguageItem> _pCardSentenceLanguage;

    internal PCardSentence(
        ObservableCollection<QCitationItem> catalog,
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        ObservableCollection<PLanguageItem> languages)
    {
        _pCardSentenceCitation = catalog;
        _pCardSentenceParticle = particles;
        _pCardSentenceDependence = dependences;
        _pCardSentenceLanguage = languages;
    }

    public ObservableCollection<PSentence> PCardSentenceRow { get; } = [];

    internal event Action<PSentence, PGloss, string>? PCardSentenceNotice;

    internal void PCardSentenceApply(CSentenceOrder order)
    {
        foreach (PSentence row in PCardSentenceRow)
        {
            row.PSentenceFrame.PSentenceFrameApply(order);
        }
    }

    internal void PCardSentenceShow(IReadOnlyList<CSentenceDraft> drafts, CSentenceOrder? order)
    {
        QLookItem.QLookItemShow(
            PCardSentenceRow,
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
        return PCardSentenceRow.IndexOf(row);
    }

    private PSentence PCardSentenceCreate(CSentenceDraft draft, CSentenceOrder? order)
    {
        PSentence row = new(
            _pCardSentenceCitation, _pCardSentenceParticle, _pCardSentenceDependence, _pCardSentenceLanguage, draft);
        if (order is not null)
        {
            row.PSentenceFrame.PSentenceFrameApply(order);
        }

        row.PSentenceGlossNotice += (gloss, language) => PCardSentenceNotice?.Invoke(row, gloss, language);
        return row;
    }
}
