using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Llyn.Application;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    internal void PCitationKeyHandle(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PSentence row } box || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        if (PCandidate.IsOpen && PCandidateHandle(e.Key))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            PSentenceCitationCommit(card, row, box);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape)
        {
            PCandidateHide();
            PSentenceCitationReset(box);
            e.Handled = true;
        }
    }

    internal void PCitationLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox { DataContext: PSentence row } box)
        {
            return;
        }

        if (ReferenceEquals(_pCandidateSentence, row))
        {
            PCandidateHide();
        }

        PSentenceCitationReset(box);
    }

    private static void PSentenceCitationReset(TextBox box)
    {
        BindingOperations.GetMultiBindingExpression(box, TextBox.TextProperty)?.UpdateTarget();
    }

    private void PSentenceCitationCommit(PCard card, PSentence row, TextBox box)
    {
        PCandidateHide();
        try
        {
            _lEditor.LEditorCard.LCardCitationSet(card.PCardId, row.PSentenceRow, box.Text);
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Reference.CreateFailed", exception);
        }

        PSentenceCitationReset(box);
    }

    private void PSentenceCitationSend(PCard card, PSentence row, long reference)
    {
        PEditorRequestSend(new LRequestSentenceReference(PEditorDraft, card.PCardId, row.PSentenceRow, reference));
    }

    private void PSentenceCitationShow()
    {
        foreach (PCard card in _pMeaningList)
        {
            PSentenceCitationShow(card);
        }

        foreach (PCard card in _pCollocationList)
        {
            PSentenceCitationShow(card);
        }
    }

    private static void PSentenceCitationShow(PCard card)
    {
        foreach (PSentence row in card.PCardSentence)
        {
            row.PSentenceCitationShow();
        }
    }
}
