using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QGloss
{
    private readonly ObservableCollection<PCard> _qGlossMeaning;

    private readonly ObservableCollection<PCard> _qGlossCollocation;

    private readonly QSentence _qGlossSentence;

    private CEditor _cEditor = null!;

    internal QGloss(
        ObservableCollection<PCard> meaning,
        ObservableCollection<PCard> collocation,
        QSentence sentence)
    {
        _qGlossMeaning = meaning;
        _qGlossCollocation = collocation;
        _qGlossSentence = sentence;
    }

    internal void QGlossIntroduce(CEditor editor)
    {
        _cEditor = editor;
    }

    internal void QGlossAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PSentence row }
            && _qGlossSentence.QSentenceCardFind(row) is PCard card)
        {
            _cEditor.CEditorSentence.CSentenceGlossAdd(card.PCardId, row.PSentenceRow);
        }
    }

    internal void QGlossRemoveObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PGloss gloss
            && e.Source is FrameworkElement { DataContext: PSentence row }
            && _qGlossSentence.QSentenceCardFind(row) is PCard card)
        {
            _cEditor.CEditorSentence.CSentenceGlossRemove(card.PCardId, row.PSentenceRow, gloss.PGlossId);
        }
    }

    internal void QGlossTextObserve(PGloss gloss, string text)
    {
        if (QGlossSentenceFind(gloss) is (PCard card, PSentence row))
        {
            _cEditor.CEditorSentence.CSentenceGlossSet(
                card.PCardId, row.PSentenceRow, gloss.PGlossId, text);
        }
    }

    private (PCard, PSentence)? QGlossSentenceFind(PGloss gloss)
    {
        foreach (PCard card in _qGlossMeaning)
        {
            foreach (PSentence row in card.PCardSentence)
            {
                if (row.PSentenceGloss.Contains(gloss))
                {
                    return (card, row);
                }
            }
        }

        foreach (PCard card in _qGlossCollocation)
        {
            foreach (PSentence row in card.PCardSentence)
            {
                if (row.PSentenceGloss.Contains(gloss))
                {
                    return (card, row);
                }
            }
        }

        return null;
    }
}
