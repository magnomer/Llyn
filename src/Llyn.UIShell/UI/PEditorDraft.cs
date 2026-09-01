using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// The editing form read as a value, filled from one, and reset to its opening state. This is the
/// only place the shell walks its own controls: everything on screen is copied into an
/// <see cref="LEntryDraft"/> once, and the engine is handed that value instead of the window. What is
/// then done with that value — stored, or thrown away — is the file beside this one.
/// </summary>
public partial class PEditor
{
    // The form as it stood when it was last filled: what a save would have written the moment the
    // user was given the form. Everything typed since is the unsaved work, so this is the one thing
    // needed to know whether closing the window or changing the workspace would throw anything away.
    private LEntryDraft? _pStateDraft;

    private LEntryDraft PEditorDraftRead()
    {
        return new LEntryDraft(
            PHeadword.Text ?? string.Empty,
            _pLangcodeChoice,
            PPronunciation.Text ?? string.Empty,
            PEditorNoteRead(),
            PCardRead(_pSenseList),
            PCardRead(_pCollocationList),
            // The downloaded recording is form state like any field: it travels in the draft, so the
            // save writes its row inside the same transaction as the rest of the entry.
            _pRecording ?? string.Empty,
            _pRecordingSource);
    }

    // Fills the form from a stored entry: the inverse of PEditorDraftRead, and the other half of the
    // round trip the session restore rides on.
    private void PEditorDraftShow(LEntryDraft draft)
    {
        // Headword first, and the recording last: typing into the headword clears the recording, so
        // filling them the other way round would wipe the audio this entry was saved with.
        PHeadword.Text = draft.LEntryDraftHeadword;
        PPronunciation.Text = draft.LEntryDraftPronunciation;
        PEditorLangcodeShow(draft.LEntryDraftLanguage);

        PCardShow(_pSenseList, "Meaning", draft.LEntryDraftSenses);
        PCardShow(_pCollocationList, "Collocation", draft.LEntryDraftCollocations);

        PEditorNoteShow(draft.LEntryDraftNote);
        PEditorRecordingShow(draft);

        // Read back rather than kept as handed in: a recording whose file is gone is not shown and so
        // is not on the form, and comparing against what is actually on screen is what makes an
        // untouched form count as untouched.
        _pStateDraft = PEditorDraftRead();
    }

    // The inverse of PCardRead, for either list. An entry saved with no cards still shows one empty
    // card: the panel is an editor, and an editor with nothing to type into is not a state the form has.
    private static void PCardShow(
        ObservableCollection<PCard> cards,
        string prefix,
        IReadOnlyList<LCardDraft> drafts)
    {
        cards.Clear();
        foreach (LCardDraft draft in drafts)
        {
            cards.Add(new PCard(prefix, cards.Count + 1)
            {
                PTitle = draft.LCardDraftTitle,
                PCardExpression = draft.LCardDraftExpression,
                PCardDefinition = draft.LCardDraftMeaning,
                PCardExample = PEditorFieldFormat(draft.LCardDraftExample),
                PCardSituation = PEditorFieldFormat(draft.LCardDraftSituation),
                PCardTag = PEditorTagFormat(draft.LCardDraftTag),
                // Which stored row this card is, carried through the form untouched so a save of the
                // same card changes that row instead of adding another one beside it.
                PCardId = draft.LCardDraftId
            });
        }

        if (cards.Count == 0)
        {
            cards.Add(new PCard(prefix, 1));
        }
    }

    // The note goes back exactly as it was read out. The box is a plain TextBox now: a note is stored
    // as text, so offering bold and italic that the store drops on the next save was offering an edit
    // the entry could not keep. Formatting a note is a feature of the store first, and the control
    // will offer it again when the store can hold it.
    private void PEditorNoteShow(string note)
    {
        PNoteContents.Text = note;
        PNotePlaceholder.Visibility = note.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    // Restores the recording the entry was saved with, and only when the file is still there: a
    // workspace whose audio folder was removed shows no play control rather than one that fails.
    private void PEditorRecordingShow(LEntryDraft draft)
    {
        PRecordingClear();
        if (draft.LEntryDraftAudio.Length == 0 || !File.Exists(draft.LEntryDraftAudio))
        {
            return;
        }

        _pRecording = draft.LEntryDraftAudio;
        _pRecordingSource = draft.LEntryDraftSource;
        // The entry's own audio, not a fetch for the spelling currently in the headword box: editing
        // the headword from here on leaves it alone.
        _pRecordingStored = true;
        PPlayback.Visibility = Visibility.Visible;
    }

    // The language selector moved onto the entry's language, flag included. A language whose pack is
    // no longer on disk is still shown: it is what the entry was written in.
    private void PEditorLangcodeShow(string language)
    {
        if (language.Length == 0)
        {
            return;
        }

        // Recorded even when the selector already stands on it: what matters to the language menu
        // being built is that this language is an entry's, not that the selector had to move.
        _pLangcodeEntry = true;

        if (string.Equals(_pLangcodeChoice, language, StringComparison.Ordinal))
        {
            return;
        }

        _pLangcodeChoice = language;
        PLangcodeBaseName.Text = language;
        PLangcodeFlagUpdate();
    }

    internal void PEditorReset()
    {
        // An empty form stands on no entry.
        _pEditorEntry = null;

        PHeadword.Text = string.Empty;
        PPronunciation.Text = string.Empty;
        // The saved recording belongs to the entry that was just written, not to the empty form the
        // next entry is typed into.
        PRecordingClear();
        // An empty form stands on no entry, so its language is nobody's: the language menu may move
        // it onto an installed pack.
        _pLangcodeEntry = false;

        // No cards to show is the empty form, which is one empty card of each kind.
        PCardShow(_pSenseList, "Meaning", []);
        PCardShow(_pCollocationList, "Collocation", []);

        PNoteContents.Text = string.Empty;
        // Clearing the box does not always route through the TextChanged handler, so the placeholder
        // is put back explicitly rather than left hidden over an empty note.
        PNotePlaceholder.Visibility = Visibility.Visible;

        // An empty form is the state it was last filled in, so nothing on it is unsaved work.
        _pStateDraft = PEditorDraftRead();
    }

    // Whether the form now differs from the form the user was given: true once anything has been
    // typed, changed or cleared and not yet written. A form never filled - which cannot happen once
    // the window is up, since both filling paths record their state - counts as unchanged, because a
    // baseline that does not exist is no evidence that work would be lost.
    internal bool PEditorChangeCheck()
    {
        return _pStateDraft is not null && !PEditorDraftMatch(_pStateDraft, PEditorDraftRead());
    }

    // Two forms compared as the user sees them. The generated record equality is no use here: a draft
    // carries lists, and those compare by reference, so two drafts holding the same text are never
    // equal to it.
    private static bool PEditorDraftMatch(LEntryDraft one, LEntryDraft other)
    {
        return string.Equals(one.LEntryDraftHeadword, other.LEntryDraftHeadword, StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftLanguage, other.LEntryDraftLanguage, StringComparison.Ordinal)
            && string.Equals(
                one.LEntryDraftPronunciation, other.LEntryDraftPronunciation, StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftNote, other.LEntryDraftNote, StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftAudio, other.LEntryDraftAudio, StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftSource, other.LEntryDraftSource, StringComparison.Ordinal)
            && PCardMatch(one.LEntryDraftSenses, other.LEntryDraftSenses)
            && PCardMatch(one.LEntryDraftCollocations, other.LEntryDraftCollocations);
    }

    // One list of cards against another, in order: a card moved is a change like any other, because
    // the order of the list is the order the entry is stored in.
    private static bool PCardMatch(IReadOnlyList<LCardDraft> one, IReadOnlyList<LCardDraft> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            LCardDraft first = one[index];
            LCardDraft second = other[index];
            if (!string.Equals(first.LCardDraftTitle, second.LCardDraftTitle, StringComparison.Ordinal)
                || !string.Equals(
                    first.LCardDraftExpression, second.LCardDraftExpression, StringComparison.Ordinal)
                || !string.Equals(first.LCardDraftMeaning, second.LCardDraftMeaning, StringComparison.Ordinal)
                || !string.Equals(first.LCardDraftId, second.LCardDraftId, StringComparison.Ordinal)
                || !PEditorTextMatch(first.LCardDraftExample, second.LCardDraftExample)
                || !PEditorTextMatch(first.LCardDraftSituation, second.LCardDraftSituation)
                || !PEditorTextMatch(first.LCardDraftTag, second.LCardDraftTag))
            {
                return false;
            }
        }

        return true;
    }

    // The ordered sets a card carries, compared element by element.
    private static bool PEditorTextMatch(IReadOnlyList<string> one, IReadOnlyList<string> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (!string.Equals(one[index], other[index], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    // The one read path for both card lists: a Meaning card and a Collocation card are the same card,
    // so they are read into the same value. A Meaning card's Expression stays empty — its template has
    // no Expression control, and no writer looks at the field for a sense.
    private IReadOnlyList<LCardDraft> PCardRead(IReadOnlyList<PCard> cards)
    {
        List<LCardDraft> drafts = new(cards.Count);
        foreach (PCard card in cards)
        {
            drafts.Add(new LCardDraft(
                // The card's own Title field, which is not PCardTitle: that one is the "Meaning 1"
                // header the template shows as a placeholder over this box.
                card.PTitle,
                card.PCardExpression,
                // The Meaning field of a collocation card and the Definition field of a sense card are
                // one property; one card class serves both kinds and the label differs, not the field.
                card.PCardDefinition,
                PEditorFieldRead(card.PCardExample),
                PEditorFieldRead(card.PCardSituation),
                // Neither template has a Synonym control any more: a synonym is a link to a stored
                // Entry or Meaning, and no picker exists to resolve typed text to one, so the field is
                // not offered rather than offered and discarded. Both card kinds read back empty here.
                string.Empty,
                PEditorTagParse(card.PCardTag),
                card.PCardId));
        }

        return drafts;
    }

    // The Example and Situation boxes are single-value controls, so the set a card hands over holds the
    // one thing typed, or nothing at all. Splitting a sentence would be guessing where one example ends.
    private static IReadOnlyList<string> PEditorFieldRead(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? [] : [text];
    }

    // The inverse, for a stored card: one box shows one value, so a card that references several shows
    // the first. The rest stay in the store — a save writes a new entry, so nothing is overwritten.
    private static string PEditorFieldFormat(IReadOnlyList<string> texts)
    {
        return texts.Count == 0 ? string.Empty : texts[0];
    }

    // The Tags box is one control over a set: the label is Tags and the placeholder is Add tags, so
    // commas separate one tag from the next and "verb, formal" is two tags rather than one oddly named
    // one. Splitting is the shell's decision and stays here — the engine takes the list as given.
    private static IReadOnlyList<string> PEditorTagParse(string text)
    {
        List<string> tags = [];
        foreach (string part in text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            tags.Add(part);
        }

        return tags;
    }

    // The inverse of PEditorTagParse: the separator it splits on is the one the box is filled with, so a
    // loaded card can be saved again unchanged.
    private static string PEditorTagFormat(IReadOnlyList<string> tags)
    {
        return string.Join(", ", tags);
    }

    private string PEditorNoteRead()
    {
        return (PNoteContents.Text ?? string.Empty).TrimEnd('\r', '\n');
    }
}
