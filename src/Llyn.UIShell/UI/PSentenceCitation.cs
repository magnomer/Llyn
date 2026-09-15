using System;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PCitationKeyHandle(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PSentence row } || PCardSentenceFind(row) is null)
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
            PSentenceCitationCommit(row);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape)
        {
            PCandidateHide();
            row.PSentenceCitationReset();
            e.Handled = true;
        }
    }

    internal void PCitationLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox { DataContext: PSentence row })
        {
            return;
        }

        if (ReferenceEquals(_pCandidateSentence, row))
        {
            PCandidateHide();
        }

        row.PSentenceCitationReset();
    }

    private void PSentenceCitationCommit(PSentence row)
    {
        PCandidateHide();

        string typed = row.PSentenceCitationText.Trim();
        if (typed.Length == 0)
        {
            row.PSentenceCitationId = 0;
            return;
        }

        if (string.Equals(typed, row.PSentenceCitationName, StringComparison.Ordinal))
        {
            row.PSentenceCitationReset();
            return;
        }

        foreach (PCitationItem item in _pEditorCitation)
        {
            if (string.Equals(item.PCitationItemName, typed, StringComparison.CurrentCultureIgnoreCase))
            {
                row.PSentenceCitationId = item.PCitationItemId;
                return;
            }
        }

        LReference stored;
        try
        {
            stored = _lEngine.LEngineCitationCreate(typed);
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Reference.CreateFailed", exception);
            row.PSentenceCitationReset();
            return;
        }

        _pEditorCitation.Add(PCitationItem.PCitationItemCreate(
            LCatalogReference.LCatalogReferenceCreate(stored, null, 0)));
        row.PSentenceCitationId = stored.LReferenceId;
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
