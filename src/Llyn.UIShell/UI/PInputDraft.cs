using System;
using System.Collections.Generic;
using System.IO;
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
    private void PStoreHandle(object sender, RoutedEventArgs e)
    {
        LEntry stored;
        try
        {
            // The save is deliberately synchronous: LDatabase keeps its ambient session in a plain
            // instance field and LEngine is built on the UI thread, so the write stays on it.
            stored = _lEngine.LEngineEntrySave(PInputDraftRead());
        }
        catch (Exception exception)
        {
            // A refused save leaves the form exactly as typed, so the missing field can be filled
            // in and the save repeated.
            MessageBox.Show(
                this,
                $"{PLocalizationTextRead("Input.SaveFailed")}\n\n{exception.Message}",
                PLocalizationTextRead("Terms.Product"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        PInputReset();

        // The form goes blank, but the session stands on the entry just written: closing the window
        // now reopens on it rather than on nothing. PInputReset cleared it, so this follows the reset.
        _pStateEntry = stored.LEntryId;
    }

    private void PDiscardHandle(object sender, RoutedEventArgs e)
    {
        PInputReset();
    }

    private LEntryDraft PInputDraftRead()
    {
        return new LEntryDraft(
            PHeadword.Text ?? string.Empty,
            _pLangcodeChoice,
            PPronunciation.Text ?? string.Empty,
            PInputNoteRead(),
            PInputSenseRead(),
            PInputCollocationRead(),
            // The downloaded recording is form state like any field: it travels in the draft, so the
            // save writes its row inside the same transaction as the rest of the entry.
            _pRecording ?? string.Empty,
            _pRecordingSource);
    }

    // Fills the form from a stored entry: the inverse of PInputDraftRead, and the other half of the
    // round trip the session restore rides on.
    private void PInputDraftShow(LEntryDraft draft)
    {
        // Headword first, and the recording last: typing into the headword clears the recording, so
        // filling them the other way round would wipe the audio this entry was saved with.
        PHeadword.Text = draft.LEntryDraftHeadword;
        PPronunciation.Text = draft.LEntryDraftPronunciation;
        PInputLangcodeShow(draft.LEntryDraftLanguage);

        _pSenseList.Clear();
        foreach (LSenseDraft sense in draft.LEntryDraftSenses)
        {
            _pSenseList.Add(new PInputCard("Sense", _pSenseList.Count + 1)
            {
                PInputCardDefinition = sense.LSenseDraftDefinition,
                PInputCardExample = sense.LSenseDraftExample,
                PInputCardSituation = sense.LSenseDraftSituation,
                PInputCardSynonym = sense.LSenseDraftSynonym,
                PInputCardTag = sense.LSenseDraftTag
            });
        }

        // An entry saved with no cards still shows one empty card: the panel is an editor, and an
        // editor with nothing to type into is not a state the form has.
        if (_pSenseList.Count == 0)
        {
            _pSenseList.Add(new PInputCard("Sense", 1));
        }

        _pCollocationList.Clear();
        foreach (LCollocationDraft collocation in draft.LEntryDraftCollocations)
        {
            _pCollocationList.Add(new PInputCard("Collocation", _pCollocationList.Count + 1)
            {
                PInputCardExpression = collocation.LCollocationDraftExpression,
                PInputCardDefinition = collocation.LCollocationDraftMeaning,
                PInputCardExample = collocation.LCollocationDraftExample,
                PInputCardSituation = collocation.LCollocationDraftSituation,
                PInputCardSynonym = collocation.LCollocationDraftSynonym,
                PInputCardTag = collocation.LCollocationDraftTag
            });
        }

        if (_pCollocationList.Count == 0)
        {
            _pCollocationList.Add(new PInputCard("Collocation", 1));
        }

        PInputNoteShow(draft.LEntryDraftNote);
        PInputRecordingShow(draft);
    }

    // Puts the note back as one paragraph of plain text, which is what the note was read out as.
    private void PInputNoteShow(string note)
    {
        PNoteContents.Document.Blocks.Clear();
        if (note.Length > 0)
        {
            PNoteContents.Document.Blocks.Add(new Paragraph(new Run(note)));
        }

        PNotePlaceholder.Visibility = note.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    // Restores the recording the entry was saved with, and only when the file is still there: a
    // workspace whose audio folder was removed shows no play control rather than one that fails.
    private void PInputRecordingShow(LEntryDraft draft)
    {
        PRecordingClear();
        if (draft.LEntryDraftAudio.Length == 0 || !File.Exists(draft.LEntryDraftAudio))
        {
            return;
        }

        _pRecording = draft.LEntryDraftAudio;
        _pRecordingSource = draft.LEntryDraftSource;
        PPlayback.Visibility = Visibility.Visible;
    }

    // The language selector moved onto the entry's language, flag included. A language whose pack is
    // no longer on disk is still shown: it is what the entry was written in.
    private void PInputLangcodeShow(string language)
    {
        if (language.Length == 0 || string.Equals(_pLangcodeChoice, language, StringComparison.Ordinal))
        {
            return;
        }

        _pLangcodeChoice = language;
        PLangcodeBaseName.Text = language;
        PLangcodeFlagUpdate();
    }

    private void PInputReset()
    {
        // An empty form stands on no entry, so the session it records is none.
        _pStateEntry = null;

        PHeadword.Text = string.Empty;
        PPronunciation.Text = string.Empty;
        // The saved recording belongs to the entry that was just written, not to the empty form the
        // next entry is typed into.
        PRecordingClear();

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
