# PEditorDraft.cs

## `public partial class PEditor`

The editing form read as a value, filled from one, and reset to its opening state.
This is the only place the shell walks its own controls.
Everything on screen is copied into an `LEntryDraft` once.
The engine is handed that value instead of the window.
The form keeps no copy of the entry it is editing.
It keeps the id of the draft the engine holds for it, and writes into that.
What is then done with that draft — committed, or thrown away — is the file beside this one.

## Inline notes

### `private string _pEditorDraft = string.Empty;`

Which held draft this form is editing.
It is the form's only claim on anything outside itself.
The draft carries the entry it was started from, so the form no longer remembers that.
Empty means the engine refused to start one, and every write here is skipped.

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

### `_pRecording ?? string.Empty,

The downloaded recording is form state like any field.
It travels in the draft.
So the save writes its row inside the same transaction as the rest of the entry.

### `PMarkerField.Text ?? string.Empty);`

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

### `_pEditorFill = false;`

Filling is over, so what the controls raise from here on is the user's.

### `private void PCardShow(`

The inverse of PCardRead, for either list.
Each card takes the number the draft carries, not its place in the loop.
Each card takes the id the draft carries too, which is how a later answer names it back.
An entry saved with no cards still shows one empty card.
That card is minted like any other the form adds, so it is named before anything is typed into it.
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

### `PEditorChangeAttach(card);`

A card that cannot report its own edits would be typed into without ever being written.

### `PCardId = draft.LCardDraftId,`

Which stored row this card is.
It is carried through the form untouched.
So a save of the same card changes that row instead of adding another one beside it.

### `PCardDraft = draft,`

The whole draft the card was drawn from, kept beside the fields drawn out of it.
The read writes the form's fields over it rather than building a card from nothing.
So every column the form has no control for goes back exactly as it came.

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

### `_pSpeakerEntry = true;`

Recorded even when the selector already stands on it.
What matters to the language menu being built is that this language is an entry's.
It does not matter that the selector had to move.

### `PRecordingClear();`

The saved recording belongs to the entry that was just written.
It does not belong to the empty form the next entry is typed into.

### `_pSpeakerEntry = false;`

An empty form stands on no entry, so its language is nobody's.
The language menu may move it onto an installed pack.

### `PCardShow(_pMeaningList, "Meaning", [], PEditorTargetEmpty);`

No cards to show is the empty form, which is one empty card of each kind.
An empty form links to nothing, so there is nothing to look words up for.

### `private LDraft? PEditorDraftStart(string? entry)`

Hands the current draft back and asks the engine for a new one.
Everything the form shows from then on belongs to that draft.
An entry that no longer loads is refused before a file is written, and the form comes up empty.
A refusal is reported and the form is suspended.
A control may buffer keystrokes only while a draft waits for them.
A draft that starts lifts a suspension, since the downstream the form lost is back.

### `private void PEditorHoldSuspend(Exception exception)`

Puts the form into a held-nothing state and says why once.
The whole editor is disabled, so no further keystroke is taken into a buffer that has nowhere to push.
Only the first failure is reported, because a reset after one raises the same failure again.

### `private void PEditorHoldResume()`

Gives the form back to the user once a draft is holding its keystrokes again.
It does nothing to a form that was never suspended, so an ordinary start touches no control.

### `private void PEditorDraftCancel()`

Throws the held draft away, links and all.
The id is dropped first, so a failure to delete cannot leave the form writing into a dead draft.

### `private void PEditorDraftSave()`

Copies what the controls hold into the held draft, and shows back whatever was stored instead.
The draft is read back first, because it carries the entry and the origin this form does not.
A draft that reads back null is gone, which is an ordinary answer and leaves the form alone.
A write that fails is not, so the form is suspended and the failure reported.
The engine may correct what it was sent.
A form that assumed otherwise would drift from the draft with no way to notice.
The content handed back is the content that went in whenever nothing was corrected, so the ordinary keystroke redraws nothing.
Filling from the answer is guarded as any other fill is, so showing it starts no further write.

### `private void PEditorDraftRestore()`

Fills the form back from the draft as stored.
This is what a form does when it can no longer tell whether what it shows is what is held.
A draft that reads back null leaves the form alone, since there is nothing left to agree with.

### `internal bool PEditorDraftFinish(bool store)`

The window's exit answer applied to this form's own draft.
A write still waiting on the typing pause is made before the wait is dropped.
So a form closed between two keystrokes carries the last of them out with it.
The caller is not trusted to have asked the form for changes first.
Storing commits it, which is the same write the save button makes.
A word typed and never saved survives the exit that was meant to keep it.
Discarding cancels it.
A draft matching its entry is cancelled either way, since there is nothing in it to store.
A refused commit keeps the draft and reports the refusal, the same way the save button does.
The answer the user gave was to keep the word.
Deleting it is the one thing that answer never asked for.
What is returned is whether the form is finished.
A false answer holds the window open over work the store would not take.
A settled form leaves the folder no file, so a clean exit is never reported as work a crash cost.

### `private bool PEditorDraftCheck()`

Asks the engine whether the held draft differs from the entry it started from.
A form with no draft has nothing to lose, so it answers no.
A question the engine cannot answer at all suspends the form.
The draft behind it can no longer be trusted.

### `private IReadOnlyList<LCardDraft> PCardRead(IReadOnlyList<PCard> cards)`

The one read path for both card lists.
A Meaning card and a Collocation card are the same card.
So they are read into the same value.
A Meaning card's Expression stays empty.
Its template has no Expression control, and no writer looks at the field for a meaning.
The Translation line reads back the ids the card's link field holds.

### `LCardDraftTitle = card.PCardTitleRead(),`

The card's own Title field, which is not PCardTitle.
That one is the "Meaning 1" header the template shows as a placeholder over this box.

### `LCardDraftMeaning = card.PCardDefinitionRead(),`

The Meaning field of a collocation card and the Definition field of a meaning card are one property.
One card class serves both kinds, so the label differs and not the field.

The Synonym line is not written over at all.
Neither template has a Synonym control any more.
A synonym is a link to a stored Entry or Meaning, and no picker resolves typed text to one.
So the field is not offered, rather than offered and discarded.

### `private static LCardDraft PCardDraftCreate()`

The empty card a form with no stored draft behind it reads back.
Every list is empty and every field unwritten, which is what a blank card means.
It exists so the read has one shape to write the form over, stored or not.

### `private static IReadOnlyList<string> PEditorFieldRead(string text)`

The Example and Situation boxes are single-value controls.
So the set a card hands over holds the one thing typed, or nothing at all.
Splitting a sentence would be guessing where one example ends.

### `private static string PEditorFieldFormat(IReadOnlyList<string> texts)`

The inverse, for a stored card: one box shows one value, so a card that references several shows the first.
The rest stay in the store — a save writes a new entry, so nothing is overwritten.


### `PHeadwordFontApply(language);`

An entry is shown in the typography its own language declares, not the panel's last choice.
This runs before the equality check, because a reopened form may hold another pack's typography.
