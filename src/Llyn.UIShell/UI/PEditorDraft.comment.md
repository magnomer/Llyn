# PEditorDraft.cs

## `public partial class PEditor`

The editing form rendered from the draft the engine holds, and reset to its opening state.
The form keeps no copy of the entry it is editing.
The engine's draft is the truth, and an id on a control is an address into it.
Rendering is a diff: a control whose value already matches the draft is left alone.
So the caret and the selection survive every answer the engine gives.
The form assembles nothing, down to the last chip and row.

## Inline notes

### `private void PEditorDraftShow(LEntryDraft draft)`

Renders the draft over what is shown, changing only what differs.
It runs on every draft bulletin, so it must be cheap and must not move the caret.
The draft is prepared before the render, so the blank rows it asks for are already in what is drawn.
The fill guard is held for the whole pass, because filling raises the same events typing does.
The pronunciation field prints whichever stored form the respelling switch picks, between the brackets that form takes.
The pronunciation rows after the primary are rendered in the same pass, keyed by their draft ids.
The transcription rows are rendered the same way, and shown only while the draft's language declares a scheme.
The glyph row is read off the pack before them, so the transcription pass knows which scheme to skip.
The reflex fetch is started after the render, because it changes nothing in the draft itself.

### `private LEntryDraft PEditorDraftPrepare(LEntryDraft draft)`

Asks for every blank row the form needs and returns the draft as it stands afterwards.
A language with schemes and no transcription row is asked for one in its first scheme.
A language with a glyph section and no glyph row is asked for one the same way.
A list showing no card is asked for one.
A card with no sentence row is asked for a blank one.
So there is always somewhere to type.
An empty row never dirties the draft, so the ask costs nothing when it is left blank.
Each ask answers with a draft bulletin.
The bulletin handler only marks the draft stale while this runs.
Rendering on each answer drew the whole form three or four times per entry.
A wide Chinese form stalled the switch that way.
The stale draft is read back once before the sentence pass, because a fresh card needs its id first.
It is read once more at the end, so the render that follows draws every row that was asked for.
Nothing is asked during a render or with no tenure, since such a request would be dropped anyway.

### `private LEntryDraft PEditorDraftRead(LEntryDraft draft)`

The tenure's current draft when an ask changed it, otherwise the draft handed in.
A read that fails is shown as a hold failure, since the draft itself could not be reached.

### `private static void PEditorTextShow(TextBox box, string text)`

Writes the draft's text into a box only when the box does not already show it.
What was waiting is written before the read, so the draft already holds what the box holds.
Writing the older value first would move the caret and then write it back.

### `private void PEditorSpeechShow(IReadOnlyList<LSpeechDraft> speeches)`

Rebuilds the chips only when they disagree with the draft.
A rebuild clears the typing field, so it is not done for nothing.

### `private void PEditorNoteShow(string note)`

The note goes back exactly as it was read out.
The box is a plain TextBox now.
A note is stored as text.
Offering bold and italic the store drops on the next save was offering an edit it could not keep.
Formatting a note is a feature of the store first.
The control will offer it again when the store can hold it.

### `private void PEditorRecordingShow(LEntryDraft draft)`

Restores the recording the draft carries, and only when the engine says the file is still there.
A recording already shown is left playing, so an unrelated answer does not stop it.
A workspace whose audio folder was removed shows no play control rather than one that fails.
A recording the engine dropped on a headword or language change arrives as an empty draft, and the tray empties.

### `PRecordingClear();`

The saved recording belongs to the entry that was just written.
It does not belong to the empty form the next entry is typed into.

### `_pSpeakerEntry = false;`

An empty form stands on no entry, so its language is nobody's.
The language menu may move it onto an installed pack.

### `PEditorLanguageSend();`

A blank draft has no language until the form says which one it is typed in.
The chosen language is sent at once, so the first keystroke lands in a draft that knows it.

### `PEditorDraftRestore();`

An empty form is one empty card of each kind, each with a blank sentence row.
The language ask is sent with the restore held back, and one restore then prepares and renders the blank form.
The cards are asked for in that pass, so they carry engine ids before anything is typed into them.
