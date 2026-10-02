using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCitation
{
    private readonly ObservableCollection<PCard> _qCitationMeaning;

    private readonly ObservableCollection<PCard> _qCitationCollocation;

    private readonly QSentence _qCitationSentence;

    private readonly QProffer _qCitationProffer;

    private CEditor _cEditor = null!;

    internal QCitation(
        ObservableCollection<PCard> meaning,
        ObservableCollection<PCard> collocation,
        QSentence sentence,
        QProffer proffer)
    {
        _qCitationMeaning = meaning;
        _qCitationCollocation = collocation;
        _qCitationSentence = sentence;
        _qCitationProffer = proffer;
    }

    internal ObservableCollection<QCitationItem> QCitationCatalog { get; } = [];

    internal void QCitationIntroduce(CEditor editor, CAtelier atelier)
    {
        _cEditor = editor;
        editor.CEditorSentence.CSentenceReferenceChanged += QCitationCatalogRefine;
        atelier.CAtelierWorkspace.CWorkspaceOpened += QCitationCatalogRefine;
    }

    internal void QCitationApply(FrameworkElement container)
    {
        if (QLook.QLookPartFind<TextBox>(container, "PSentenceCitation") is TextBox citation)
        {
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

    internal void QCitationFieldRefine(PCard card, PSentence row, TextBox box)
    {
        _qCitationProffer.QProfferCitationRefine(
            box, _cEditor.CEditorCard.CCardReferenceFind(card.PCardId, row.PSentenceRow, box.Text));
    }

    private void QCitationCatalogRefine()
    {
        QCitationCatalog.Clear();
        foreach (CCatalogReference row in _cEditor.CEditorCard.CCardReferenceRead())
        {
            QCitationCatalog.Add(QCitationItem.QCitationItemCreate(row));
        }

        foreach (PCard card in _qCitationMeaning.Concat(_qCitationCollocation))
        {
            foreach (PSentence row in card.PCardSentence)
            {
                row.PSentenceCitationShow();
            }
        }
    }

    private void QCitationCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not TextBox { DataContext: PSentence row } box
            || _qCitationSentence.QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = true;
        _cEditor.CEditorCard.CCardCitationSet(card.PCardId, row.PSentenceRow, box.Text);
        _qCitationProffer.QProfferShutRefine();
        QCitationTextRefine(box);
    }

    private void QCitationEscapeRefine(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape || sender is not TextBox { DataContext: PSentence } box)
        {
            return;
        }

        e.Handled = true;
        QCitationTextRefine(box);
    }

    private void QCitationLeaveRefine(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox { DataContext: PSentence } box)
        {
            return;
        }

        _qCitationProffer.QProfferShutRefine();
        QCitationTextRefine(box);
    }

    private static void QCitationTextRefine(TextBox box)
    {
        if (box.DataContext is PSentence row)
        {
            box.Text = PSentence.PSentenceCitationFind(row.PSentenceCitationCatalog, row.PSentenceCitation);
        }
    }
}
