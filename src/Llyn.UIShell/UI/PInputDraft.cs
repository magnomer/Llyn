using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// The input form read as a value, and reset back to its opening state. This is the only place the
/// shell walks its own controls for a save: everything on screen is copied into an
/// <see cref="LEntryDraft"/> once, and the engine is handed that value instead of the window.
/// </summary>
public partial class PWindow
{
    private LEntryDraft PInputDraftRead()
    {
        return new LEntryDraft(
            PHeadword.Text ?? string.Empty,
            _pLangcodeChoice,
            PPronunciation.Text ?? string.Empty,
            PInputNoteRead(),
            PInputSenseRead(),
            PInputCollocationRead());
    }

    private void PInputReset()
    {
        PHeadword.Text = string.Empty;
        PPronunciation.Text = string.Empty;

        _pSenseList.Clear();
        _pSenseList.Add(new PInputCard("Sense", 1));
        _pCollocationList.Clear();
        _pCollocationList.Add(new PInputCard("Collocation", 1));

        PNoteContents.Document.Blocks.Clear();
        // Clearing the document does not always route through the TextChanged handler, so the
        // placeholder is put back explicitly rather than left hidden over an empty note.
        PNotePlaceholder.Visibility = Visibility.Visible;
    }

    private IReadOnlyList<LSenseDraft> PInputSenseRead()
    {
        List<LSenseDraft> drafts = new(_pSenseList.Count);
        for (int index = 0; index < _pSenseList.Count; index++)
        {
            PInputCard card = _pSenseList[index];
            drafts.Add(new LSenseDraft(
                index + 1,
                card.PInputCardDefinition,
                card.PInputCardExample,
                card.PInputCardSituation,
                card.PInputCardSynonym,
                card.PInputCardTag));
        }

        return drafts;
    }

    private IReadOnlyList<LCollocationDraft> PInputCollocationRead()
    {
        List<LCollocationDraft> drafts = new(_pCollocationList.Count);
        for (int index = 0; index < _pCollocationList.Count; index++)
        {
            PInputCard card = _pCollocationList[index];
            drafts.Add(new LCollocationDraft(
                index + 1,
                card.PInputCardExpression,
                // The collocation card's Meaning field is bound to PInputCardDefinition; one card
                // class serves both kinds and the label differs, not the property.
                card.PInputCardDefinition,
                card.PInputCardExample,
                card.PInputCardSituation,
                // No collocation control feeds a synonym: the template has no Synonym TextBox.
                card.PInputCardSynonym,
                card.PInputCardTag));
        }

        return drafts;
    }

    private string PInputNoteRead()
    {
        TextRange contents = new(PNoteContents.Document.ContentStart, PNoteContents.Document.ContentEnd);
        return contents.Text.TrimEnd('\r', '\n');
    }
}
