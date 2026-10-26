using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCitation
{
    private readonly ObservableCollection<QPlacard> _qCitationMeaning;

    private readonly ObservableCollection<QPlacard> _qCitationCollocation;

    private readonly QSentence _qCitationSentence;

    private readonly QProffer _qCitationProffer;

    private CCard _cCard = null!;

    private CCaret _cCaret = null!;

    internal QCitation(
        ObservableCollection<QPlacard> meaning,
        ObservableCollection<QPlacard> collocation,
        QSentence sentence,
        QProffer proffer)
    {
        _qCitationMeaning = meaning;
        _qCitationCollocation = collocation;
        _qCitationSentence = sentence;
        _qCitationProffer = proffer;
    }

    internal void QCitationIntroduce(CCard card, CCaret caret, CSentence sentence, CAtelier atelier)
    {
        _cCard = card;
        _cCaret = caret;
        sentence.CSentenceReferenceChanged += QCitationCatalogRefine;
        atelier.CAtelierWorkspace.CWorkspaceOpened += QCitationCatalogRefine;
    }

    internal void QCitationApply(FrameworkElement container)
    {
        if (QLook.QLookPartFind<TextBox>(container, "PSentenceCitation") is TextBox citation)
        {
            citation.TextChanged -= QCitationFieldRefine;
            citation.TextChanged += QCitationFieldRefine;
            citation.PreviewKeyDown -= _qCitationProffer.QProfferKeyRefine;
            citation.PreviewKeyDown -= _qCitationProffer.QProfferKeyObserve;
            citation.PreviewKeyDown -= QCitationCommitObserve;
            citation.PreviewKeyDown -= QCitationEscapeRefine;
            citation.PreviewKeyDown += _qCitationProffer.QProfferKeyRefine;
            citation.PreviewKeyDown += _qCitationProffer.QProfferKeyObserve;
            citation.PreviewKeyDown += QCitationCommitObserve;
            citation.PreviewKeyDown += QCitationEscapeRefine;
            citation.LostKeyboardFocus -= QCitationLeaveRefine;
            citation.LostKeyboardFocus += QCitationLeaveRefine;
        }
    }

    private void QCitationFieldRefine(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { DataContext: QPassage row } box
            && _qCitationSentence.QSentenceCardFind(row) is QPlacard card)
        {
            _qCitationProffer.QProfferCitationRefine(
                box,
                _cCaret.CCaretReferenceFind(card.QPlacardCollocated, card.QPlacardId, row.QPassageRow, box.Text));
        }
    }

    private void QCitationCatalogRefine()
    {
        foreach (QPlacard card in _qCitationMeaning.Concat(_qCitationCollocation))
        {
            foreach (QPassage row in card.QCardSentence.QCardSentenceRow)
            {
                row.QPassageCitationShow();
            }
        }
    }

    private void QCitationCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not TextBox { DataContext: QPassage row } box
            || _qCitationSentence.QSentenceCardFind(row) is not QPlacard card)
        {
            return;
        }

        e.Handled = true;
        string title = _cCard.CCardCitationSet(card.QPlacardCollocated, card.QPlacardId, row.QPassageRow, box.Text);
        _qCitationProffer.QProfferShutRefine();
        box.Text = title;
    }

    private void QCitationEscapeRefine(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape || sender is not TextBox { DataContext: QPassage } box)
        {
            return;
        }

        e.Handled = true;
        QCitationTextRefine(box);
    }

    private void QCitationLeaveRefine(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox { DataContext: QPassage } box)
        {
            return;
        }

        _qCitationProffer.QProfferShutRefine();
        QCitationTextRefine(box);
    }

    internal void QCitationTextRefine(TextBox box)
    {
        if (box.DataContext is QPassage row)
        {
            box.Text = _qCitationSentence.QSentenceCardFind(row) is QPlacard card
                ? _cCard.CCardCitationRead(card.QPlacardCollocated, card.QPlacardId, row.QPassageRow)
                : string.Empty;
        }
    }
}
