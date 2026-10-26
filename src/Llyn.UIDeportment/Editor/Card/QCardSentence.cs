using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCardSentence
{
    private readonly ObservableCollection<string> _qCardSentenceParticle;
    private readonly ObservableCollection<string> _qCardSentenceDependence;
    private readonly ObservableCollection<QLanguageItem> _qCardSentenceLanguage;

    internal QCardSentence(
        ObservableCollection<string> particles,
        ObservableCollection<string> dependences,
        ObservableCollection<QLanguageItem> languages)
    {
        _qCardSentenceParticle = particles;
        _qCardSentenceDependence = dependences;
        _qCardSentenceLanguage = languages;
    }

    public ObservableCollection<QPassage> QCardSentenceRow { get; } = [];

    internal event Action<QPassage, QRendition, string>? QCardSentenceNotice;

    internal void QCardSentenceApply(CSentenceOrder order)
    {
        foreach (QPassage row in QCardSentenceRow)
        {
            row.QSentenceFrame.QSentenceFrameApply(order);
        }
    }

    internal void QCardSentenceShow(IReadOnlyList<CSentenceDraft> drafts, CSentenceOrder? order)
    {
        QLookItem.QLookItemShow(
            QCardSentenceRow,
            drafts,
            static row => row.QPassageRow,
            static draft => draft.CSentenceDraftId,
            draft => QCardSentenceCreate(draft, order),
            (row, draft) =>
            {
                row.QPassageShow(draft);
                return row;
            });
    }

    internal int QCardSentenceFind(QPassage row)
    {
        return QCardSentenceRow.IndexOf(row);
    }

    private QPassage QCardSentenceCreate(CSentenceDraft draft, CSentenceOrder? order)
    {
        QPassage row = new(_qCardSentenceParticle, _qCardSentenceDependence, _qCardSentenceLanguage, draft);
        if (order is not null)
        {
            row.QSentenceFrame.QSentenceFrameApply(order);
        }

        row.QPassageGlossNotice += (gloss, language) => QCardSentenceNotice?.Invoke(row, gloss, language);
        return row;
    }
}
