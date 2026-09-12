using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    private LSentenceOrder _pCardSentenceOrder = LSentenceOrder.LSentenceOrderDefault;

    public ObservableCollection<PSentence> PCardSentence { get; } = [];

    internal Action<PSentence, string>? PCardSentenceNotice { get; set; }

    internal void PCardSentenceApply(LSentenceOrder order)
    {
        _pCardSentenceOrder = order ?? LSentenceOrder.LSentenceOrderDefault;
        foreach (PSentence row in PCardSentence)
        {
            row.PSentenceOrderApply(_pCardSentenceOrder);
        }
    }

    internal void PCardSentenceShow(IReadOnlyList<LSentenceDraft> drafts, Func<PSentence, string, bool> pending)
    {
        ArgumentNullException.ThrowIfNull(pending);

        PCardRowShow(
            PCardSentence,
            drafts,
            static row => row.PSentenceRow,
            static draft => draft.LSentenceDraftId,
            PCardSentenceCreate,
            (row, draft) =>
            {
                row.PSentenceShow(draft, field => pending(row, field));
                return row;
            });
    }

    internal int PCardSentenceFind(PSentence row)
    {
        return PCardSentence.IndexOf(row);
    }

    private PSentence PCardSentenceCreate(LSentenceDraft draft)
    {
        PSentence row = new(_pCardCitation, _pCardParticle, _pCardDependence, _pCardLanguage, draft);
        row.PSentenceOrderApply(_pCardSentenceOrder);
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
