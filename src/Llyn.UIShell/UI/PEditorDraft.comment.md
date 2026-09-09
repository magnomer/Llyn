# PEditorDraft.cs

## `public partial class PEditor`

The editing form read as a value, filled from one, and reset to its opening state.
This is the only place the shell walks its own controls.
Everything on screen is copied into an `LEntryDraft` once.
The engine is handed that value instead of the window.
The form keeps no copy of the entry it is editing.
What holds that value between keystrokes, and what is done with it, is the file beside this one.

## Inline notes

### `private LEntryDraft? _pEditorDetail;`

The draft the form was last filled from, kept for the fields the form has no control for.
Forms, inflections, syllables and representations are stored detail this panel never shows.
Reading the form back over the draft it was filled from is how that detail survives a save.

### `private LPronunciationDraft? PEditorSoundRead()`

The pronunciation as the form holds it, written over the pronunciation it was filled from.
The typed reading and the chosen recording are the two parts this panel owns.
The level, the syllables and the representations go back exactly as they came.
A form holding neither reading nor recording carries no pronunciation at all.

### `private IReadOnlyList<LSpeechDraft> PEditorSpeechRead()`

The parts of speech as chips, matched back to the drafts the form was filled from.
A chip whose name was filed under a language-pack value keeps that value.
A chip the user typed is a name, and the engine decides at the write whether a preset names it.

### `_pRecording ?? string.Empty,`

The downloaded recording is form state like any field.
It travels in the draft.
So the save writes its row inside the same transaction as the rest of the entry.

### `private void PEditorDraftShow(LEntryDraft draft)`

Fills the form from a stored entry.
It is the inverse of PEditorDraftRead.
It is the other half of the round trip the session restore rides on.

### `PHeadword.Text = draft.LEntryDraftHeadword;`

Headword first, and the recording last.
Typing into the headword clears the recording.
So filling them the other way round would wipe the audio this entry was saved with.

### `IReadOnlyDictionary<string, LTranslationTarget> targets = PEditorTargetRead(draft);`

A card stores link ids and the field shows words, so the two are joined before any card is built.
Every card's ids are asked for together, so an entry of many cards still asks once.

### `_pEditorFill = false;`

Filling is over, so what the controls raise from here on is the user's.

### `private void PEditorNoteShow(string note)`

The note goes back exactly as it was read out.
The box is a plain TextBox now.
A note is stored as text.
Offering bold and italic the store drops on the next save was offering an edit it could not keep.
Formatting a note is a feature of the store first.
The control will offer it again when the store can hold it.

### `private void PEditorRecordingShow(LEntryDraft draft)`

Restores the recording the entry was saved with, and only when the file is still there.
A workspace whose audio folder was removed shows no play control rather than one that fails.

### `_pRecordingStored = true;`

The entry's own audio, not a fetch for the spelling currently in the headword box.
Editing the headword from here on leaves it alone.

### `PRecordingClear();`

The saved recording belongs to the entry that was just written.
It does not belong to the empty form the next entry is typed into.

### `_pSpeakerEntry = false;`

An empty form stands on no entry, so its language is nobody's.
The language menu may move it onto an installed pack.

### `PCardShow(_pMeaningList, "Meaning", [], PEditorTargetEmpty);`

No cards to show is the empty form, which is one empty card of each kind.
An empty form links to nothing, so there is nothing to look words up for.
