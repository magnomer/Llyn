# PEditorDraft.cs

## `public partial class PEditor`

The editing form read as a value, filled from one, and reset to its opening state.
This is the only place the shell walks its own controls.
Everything on screen is copied into an `LEntryDraft` once.
The engine is handed that value instead of the window.
What is then done with that value — stored, or thrown away — is the file beside this one.

## Inline notes

### `private LEntryDraft? _pStateDraft;`

The form as it stood when it was last filled.
It is what a save would have written the moment the user was given the form.
Everything typed since is the unsaved work.
So this is the one thing needed to know whether closing would throw anything away.
Changing the workspace raises the same question.

### `_pRecording ?? string.Empty,`

The downloaded recording is form state like any field.
It travels in the draft.
So the save writes its row inside the same transaction as the rest of the entry.

### `PSpeechContents.Text ?? string.Empty);`

Exactly what stands in the field, preset or not.
Which of the two it is, the engine works out when it writes it.

### `private void PEditorDraftShow(LEntryDraft draft)`

Fills the form from a stored entry.
It is the inverse of PEditorDraftRead.
It is the other half of the round trip the session restore rides on.

### `PHeadword.Text = draft.LEntryDraftHeadword;`

Headword first, and the recording last.
Typing into the headword clears the recording.
So filling them the other way round would wipe the audio this entry was saved with.

### `_pStateDraft = PEditorDraftRead();`

Read back rather than kept as handed in.
A recording whose file is gone is not shown and so is not on the form.
Comparing against what is on screen is what makes an untouched form count as untouched.

### `private static void PCardShow(`

The inverse of PCardRead, for either list.
An entry saved with no cards still shows one empty card.
The panel is an editor.
An editor with nothing to type into is not a state the form has.

### `IReadOnlyDictionary<string, LTranslationTarget> targets = PEditorTargetRead(draft);`

A card stores link ids and the field shows words, so the two are joined before any card is built.
Every card's ids are asked for together, so an entry of many cards still asks once.
The engine is asked, because the panel reaches no database of its own.
A workspace that refuses the read leaves the chips off rather than stopping the load.

### `card.PCardLinkShow(PCardTargetRead(targets, draft.LCardDraftTranslation));`

Each card takes its own words out of the one answer, in the order the card holds them.
An id the answer does not name is passed over, as a chip with no Entry has nothing to say.

### `PLinkAttach(card);`

Every card is given the way back to the editor before it is shown.
A card resolves no typed word on its own.

### `PCardId = draft.LCardDraftId`

Which stored row this card is.
It is carried through the form untouched.
So a save of the same card changes that row instead of adding another one beside it.

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

### `private void PEditorLanguageShow(string language)`

The language selector moved onto the entry's language, flag included.
A language whose pack is no longer on disk is still shown: it is what the entry was written in.

### `_pLanguageEntry = true;`

Recorded even when the selector already stands on it.
What matters to the language menu being built is that this language is an entry's.
It does not matter that the selector had to move.

### `_pEditorEntry = null;`

An empty form stands on no entry.

### `PRecordingClear();`

The saved recording belongs to the entry that was just written.
It does not belong to the empty form the next entry is typed into.

### `_pLanguageEntry = false;`

An empty form stands on no entry, so its language is nobody's.
The language menu may move it onto an installed pack.

### `PCardShow(_pSenseList, "Meaning", [], PEditorTargetEmpty);`

No cards to show is the empty form, which is one empty card of each kind.
An empty form links to nothing, so there is nothing to look words up for.

### `PNotePlaceholder.Visibility = Visibility.Visible;`

Clearing the box does not always route through the TextChanged handler.
So the placeholder is put back explicitly rather than left hidden over an empty note.

### `_pStateDraft = PEditorDraftRead();`

An empty form is the state it was last filled in, so nothing on it is unsaved work.

### `internal bool PEditorChangeCheck()`

Whether the form now differs from the form the user was given.
It is true once anything has been typed, changed or cleared and not yet written.
A form never filled counts as unchanged.
That cannot happen once the window is up, since both filling paths record their state.
A baseline that does not exist is no evidence that work would be lost.

### `private static bool PEditorDraftMatch(LEntryDraft one, LEntryDraft other)`

Two forms compared as the user sees them.
The generated record equality is no use here.
A draft carries lists, and those compare by reference.
So two drafts holding the same text are never equal to it.

### `private static bool PCardMatch(IReadOnlyList<LCardDraft> one, IReadOnlyList<LCardDraft> other)`

One list of cards against another, in order.
A card moved is a change like any other.
The order of the list is the order the entry is stored in.

### `private static bool PEditorTextMatch(IReadOnlyList<string> one, IReadOnlyList<string> other)`

The ordered sets a card carries, compared element by element.

### `private IReadOnlyList<LCardDraft> PCardRead(IReadOnlyList<PCard> cards)`

The one read path for both card lists.
A Meaning card and a Collocation card are the same card.
So they are read into the same value.
A Meaning card's Expression stays empty.
Its template has no Expression control, and no writer looks at the field for a sense.
The Translation line reads back the ids the card's link field holds.

### `card.PTitle,`

The card's own Title field, which is not PCardTitle.
That one is the "Meaning 1" header the template shows as a placeholder over this box.

### `card.PCardDefinition,`

The Meaning field of a collocation card and the Definition field of a sense card are one property.
One card class serves both kinds, so the label differs and not the field.

### `string.Empty,`

Neither template has a Synonym control any more.
A synonym is a link to a stored Entry or Meaning.
No picker exists to resolve typed text to one.
So the field is not offered, rather than offered and discarded.
Both card kinds read back empty here.

### `private static IReadOnlyList<string> PEditorFieldRead(string text)`

The Example and Situation boxes are single-value controls.
So the set a card hands over holds the one thing typed, or nothing at all.
Splitting a sentence would be guessing where one example ends.

### `private static string PEditorFieldFormat(IReadOnlyList<string> texts)`

The inverse, for a stored card: one box shows one value, so a card that references several shows the first.
The rest stay in the store — a save writes a new entry, so nothing is overwritten.

