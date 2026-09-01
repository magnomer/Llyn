# PEditorDraft.cs

## `public partial class PEditor`

The editing form read as a value, filled from one, and reset to its opening state. This is the only place the shell walks its own controls: everything on screen is copied into an `LEntryDraft` once, and the engine is handed that value instead of the window. What is then done with that value — stored, or thrown away — is the file beside this one.

## Inline notes

### `private LEntryDraft? _pStateDraft;`

The form as it stood when it was last filled: what a save would have written the moment the user was given the form. Everything typed since is the unsaved work, so this is the one thing needed to know whether closing the window or changing the workspace would throw anything away.

### `_pRecording ?? string.Empty,`

The downloaded recording is form state like any field: it travels in the draft, so the save writes its row inside the same transaction as the rest of the entry.

### `PSpeechContents.Text ?? string.Empty);`

Exactly what stands in the field, preset or not: which of the two it is, the engine works out when it writes it.

### `private void PEditorDraftShow(LEntryDraft draft)`

Fills the form from a stored entry: the inverse of PEditorDraftRead, and the other half of the round trip the session restore rides on.

### `PHeadword.Text = draft.LEntryDraftHeadword;`

Headword first, and the recording last: typing into the headword clears the recording, so filling them the other way round would wipe the audio this entry was saved with.

### `_pStateDraft = PEditorDraftRead();`

Read back rather than kept as handed in: a recording whose file is gone is not shown and so is not on the form, and comparing against what is actually on screen is what makes an untouched form count as untouched.

### `private static void PCardShow(`

The inverse of PCardRead, for either list. An entry saved with no cards still shows one empty card: the panel is an editor, and an editor with nothing to type into is not a state the form has.

### `PCardId = draft.LCardDraftId`

Which stored row this card is, carried through the form untouched so a save of the same card changes that row instead of adding another one beside it.

### `private void PEditorNoteShow(string note)`

The note goes back exactly as it was read out. The box is a plain TextBox now: a note is stored as text, so offering bold and italic that the store drops on the next save was offering an edit the entry could not keep. Formatting a note is a feature of the store first, and the control will offer it again when the store can hold it.

### `private void PEditorRecordingShow(LEntryDraft draft)`

Restores the recording the entry was saved with, and only when the file is still there: a workspace whose audio folder was removed shows no play control rather than one that fails.

### `_pRecordingStored = true;`

The entry's own audio, not a fetch for the spelling currently in the headword box: editing the headword from here on leaves it alone.

### `private void PEditorLangcodeShow(string language)`

The language selector moved onto the entry's language, flag included. A language whose pack is no longer on disk is still shown: it is what the entry was written in.

### `_pLangcodeEntry = true;`

Recorded even when the selector already stands on it: what matters to the language menu being built is that this language is an entry's, not that the selector had to move.

### `_pEditorEntry = null;`

An empty form stands on no entry.

### `PRecordingClear();`

The saved recording belongs to the entry that was just written, not to the empty form the next entry is typed into.

### `_pLangcodeEntry = false;`

An empty form stands on no entry, so its language is nobody's: the language menu may move it onto an installed pack.

### `PCardShow(_pSenseList, "Meaning", []);`

No cards to show is the empty form, which is one empty card of each kind.

### `PNotePlaceholder.Visibility = Visibility.Visible;`

Clearing the box does not always route through the TextChanged handler, so the placeholder is put back explicitly rather than left hidden over an empty note.

### `_pStateDraft = PEditorDraftRead();`

An empty form is the state it was last filled in, so nothing on it is unsaved work.

### `internal bool PEditorChangeCheck()`

Whether the form now differs from the form the user was given: true once anything has been typed, changed or cleared and not yet written. A form never filled - which cannot happen once the window is up, since both filling paths record their state - counts as unchanged, because a baseline that does not exist is no evidence that work would be lost.

### `private static bool PEditorDraftMatch(LEntryDraft one, LEntryDraft other)`

Two forms compared as the user sees them. The generated record equality is no use here: a draft carries lists, and those compare by reference, so two drafts holding the same text are never equal to it.

### `private static bool PCardMatch(IReadOnlyList<LCardDraft> one, IReadOnlyList<LCardDraft> other)`

One list of cards against another, in order: a card moved is a change like any other, because the order of the list is the order the entry is stored in.

### `private static bool PEditorTextMatch(IReadOnlyList<string> one, IReadOnlyList<string> other)`

The ordered sets a card carries, compared element by element.

### `private IReadOnlyList<LCardDraft> PCardRead(IReadOnlyList<PCard> cards)`

The one read path for both card lists: a Meaning card and a Collocation card are the same card, so they are read into the same value. A Meaning card's Expression stays empty — its template has no Expression control, and no writer looks at the field for a sense.

### `card.PTitle,`

The card's own Title field, which is not PCardTitle: that one is the "Meaning 1" header the template shows as a placeholder over this box.

### `card.PCardDefinition,`

The Meaning field of a collocation card and the Definition field of a sense card are one property; one card class serves both kinds and the label differs, not the field.

### `string.Empty,`

Neither template has a Synonym control any more: a synonym is a link to a stored Entry or Meaning, and no picker exists to resolve typed text to one, so the field is not offered rather than offered and discarded. Both card kinds read back empty here.

### `private static IReadOnlyList<string> PEditorFieldRead(string text)`

The Example and Situation boxes are single-value controls, so the set a card hands over holds the one thing typed, or nothing at all. Splitting a sentence would be guessing where one example ends.

### `private static string PEditorFieldFormat(IReadOnlyList<string> texts)`

The inverse, for a stored card: one box shows one value, so a card that references several shows the first. The rest stay in the store — a save writes a new entry, so nothing is overwritten.

### `private static IReadOnlyList<string> PEditorTagParse(string text)`

The Tags box is one control over a set: the label is Tags and the placeholder is Add tags, so commas separate one tag from the next and "verb, formal" is two tags rather than one oddly named one. Splitting is the shell's decision and stays here — the engine takes the list as given.

### `private static string PEditorTagFormat(IReadOnlyList<string> tags)`

The inverse of PEditorTagParse: the separator it splits on is the one the box is filled with, so a loaded card can be saved again unchanged.
