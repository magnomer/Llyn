using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            PWindowFailureShow("Input.SaveFailed", exception);
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
            PInputCardRead(_pSenseList),
            PInputCardRead(_pCollocationList),
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

        PInputCardShow(_pSenseList, "Sense", draft.LEntryDraftSenses);
        PInputCardShow(_pCollocationList, "Collocation", draft.LEntryDraftCollocations);

        PInputNoteShow(draft.LEntryDraftNote);
        PInputRecordingShow(draft);
    }

    // The inverse of PInputCardRead, for either list. An entry saved with no cards still shows one empty
    // card: the panel is an editor, and an editor with nothing to type into is not a state the form has.
    private static void PInputCardShow(
        ObservableCollection<PInputCard> cards,
        string prefix,
        IReadOnlyList<LCardDraft> drafts)
    {
        cards.Clear();
        foreach (LCardDraft draft in drafts)
        {
            cards.Add(new PInputCard(prefix, cards.Count + 1)
            {
                PTitle = draft.LCardDraftTitle,
                PInputCardExpression = draft.LCardDraftExpression,
                PInputCardDefinition = draft.LCardDraftMeaning,
                PInputCardExample = PInputFieldFormat(draft.LCardDraftExample),
                PInputCardSituation = PInputFieldFormat(draft.LCardDraftSituation),
                PInputCardSynonym = draft.LCardDraftSynonym,
                PInputCardTag = PInputTagFormat(draft.LCardDraftTag)
            });
        }

        if (cards.Count == 0)
        {
            cards.Add(new PInputCard(prefix, 1));
        }
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

        // No cards to show is the empty form, which is one empty card of each kind.
        PInputCardShow(_pSenseList, "Sense", []);
        PInputCardShow(_pCollocationList, "Collocation", []);

        PNoteContents.Document.Blocks.Clear();
        // Clearing the document does not always route through the TextChanged handler, so the
        // placeholder is put back explicitly rather than left hidden over an empty note.
        PNotePlaceholder.Visibility = Visibility.Visible;
    }

    // The one read path for both card lists: a Meaning card and a Collocation card are the same card,
    // so they are read into the same value. A Meaning card's Expression stays empty — its template has
    // no Expression control, and no writer looks at the field for a sense.
    private IReadOnlyList<LCardDraft> PInputCardRead(IReadOnlyList<PInputCard> cards)
    {
        List<LCardDraft> drafts = new(cards.Count);
        foreach (PInputCard card in cards)
        {
            drafts.Add(new LCardDraft(
                // The card's own Title field, which is not PInputCardTitle: that one is the "Sense 1"
                // header the template shows as a placeholder over this box.
                card.PTitle,
                card.PInputCardExpression,
                // The Meaning field of a collocation card and the Definition field of a sense card are
                // one property; one card class serves both kinds and the label differs, not the field.
                card.PInputCardDefinition,
                PInputFieldRead(card.PInputCardExample),
                PInputFieldRead(card.PInputCardSituation),
                // No collocation control feeds a synonym: that template has no Synonym TextBox, so a
                // collocation card always reads back empty here.
                card.PInputCardSynonym,
                PInputTagParse(card.PInputCardTag)));
        }

        return drafts;
    }

    // The Example and Situation boxes are single-value controls, so the set a card hands over holds the
    // one thing typed, or nothing at all. Splitting a sentence would be guessing where one example ends.
    private static IReadOnlyList<string> PInputFieldRead(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? [] : [text];
    }

    // The inverse, for a stored card: one box shows one value, so a card that references several shows
    // the first. The rest stay in the store — a save writes a new entry, so nothing is overwritten.
    private static string PInputFieldFormat(IReadOnlyList<string> texts)
    {
        return texts.Count == 0 ? string.Empty : texts[0];
    }

    // The Tags box is one control over a set: the label is Tags and the placeholder is Add tags, so
    // commas separate one tag from the next and "verb, formal" is two tags rather than one oddly named
    // one. Splitting is the shell's decision and stays here — the engine takes the list as given.
    private static IReadOnlyList<string> PInputTagParse(string text)
    {
        List<string> tags = [];
        foreach (string part in text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            tags.Add(part);
        }

        return tags;
    }

    // The inverse of PInputTagParse: the separator it splits on is the one the box is filled with, so a
    // loaded card can be saved again unchanged.
    private static string PInputTagFormat(IReadOnlyList<string> tags)
    {
        return string.Join(", ", tags);
    }

    private string PInputNoteRead()
    {
        TextRange contents = new(PNoteContents.Document.ContentStart, PNoteContents.Document.ContentEnd);
        return contents.Text.TrimEnd('\r', '\n');
    }
}
