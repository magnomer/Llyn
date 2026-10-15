using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QGloss
{
    private readonly ObservableCollection<QPlacard> _qGlossMeaning;

    private readonly ObservableCollection<QPlacard> _qGlossCollocation;

    private readonly QSentence _qGlossSentence;

    private CSentence _cSentence = null!;

    internal QGloss(
        ObservableCollection<QPlacard> meaning,
        ObservableCollection<QPlacard> collocation,
        QSentence sentence)
    {
        _qGlossMeaning = meaning;
        _qGlossCollocation = collocation;
        _qGlossSentence = sentence;
    }

    internal void QGlossIntroduce(CSentence sentence)
    {
        _cSentence = sentence;
    }

    internal void QGlossAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QPassage row }
            && _qGlossSentence.QSentenceCardFind(row) is QPlacard card)
        {
            _cSentence.CSentenceGlossAdd(card.QPlacardId, row.QPassageRow);
        }
    }

    internal void QGlossRemoveObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QRendition gloss
            && e.Source is FrameworkElement { DataContext: QPassage row }
            && _qGlossSentence.QSentenceCardFind(row) is QPlacard card)
        {
            _cSentence.CSentenceGlossRemove(card.QPlacardId, row.QPassageRow, gloss.QRenditionId);
        }
    }

    internal void QGlossTextObserve(QRendition gloss, string text)
    {
        if (QGlossSentenceFind(gloss) is (QPlacard card, QPassage row))
        {
            _cSentence.CSentenceGlossSet(
                card.QPlacardId, row.QPassageRow, gloss.QRenditionId, text);
        }
    }

    private (QPlacard, QPassage)? QGlossSentenceFind(QRendition gloss)
    {
        foreach (QPlacard card in _qGlossMeaning)
        {
            foreach (QPassage row in card.QCardSentence.QCardSentenceRow)
            {
                if (row.QPassageGloss.Contains(gloss))
                {
                    return (card, row);
                }
            }
        }

        foreach (QPlacard card in _qGlossCollocation)
        {
            foreach (QPassage row in card.QCardSentence.QCardSentenceRow)
            {
                if (row.QPassageGloss.Contains(gloss))
                {
                    return (card, row);
                }
            }
        }

        return null;
    }
}
