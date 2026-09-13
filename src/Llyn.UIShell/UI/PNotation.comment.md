# PNotation.cs

## `public partial class PEditor : LReceiver`

Pronunciation lookup as the editor shows it.
Opening the menu starts a search for the headword.
Candidates stream in from the engine and fill the list, one reading per variety a source returned.
Picking one writes it into the pronunciation field and stores its variety on the primary pronunciation row.
Taking a whole row writes the first reading there and adds every further one as its own pronunciation row.
A further reading the draft already holds, same variety and same text, is not added again.
This is the shell side of `LReceiver`.
The engine calls back on a worker thread.
So every arrival is marshalled onto the dispatcher here.

## `private PNotationItem PNotationPlace(string source, int order)`

Finds the row one source owns, creating it at its declared position when it has none yet.
Rows stand where the language pack put the source, not where the network put it.
Every source is asked at once, so a fast one would otherwise head a list the user did not order.
The list is short and already sorted, so a walk to the insertion point costs nothing worth avoiding.
The same call serves the start and the answer.
A replayed search that reports no start still lands its rows correctly.
A new row opens saying it is searching.
That makes every declared source visible before any of them answers.

## Inline notes

### `_pNotationFlagged = _lEngine.LEngineFlaggedCheck(_pNotationLanguage);`

Whether the pack shows its varieties as flags is asked once, when the search starts.
In flag mode every declared variety's flag is resolved before the search.
A reading's flag is then ready the moment the reading lands.
The language is kept with the answer, because the speaker choice may change while the search is still running.

### `await _lEngine.LEnginePronunciationFind(`

The panel asks and then listens.
It passes the draft it is editing, so the engine can hand back what that draft already found.
Whether a search runs at all is the engine's answer, not the menu's.
The search is over when the receiver is told it is, never when this call returns.
The two are not the same moment.
The engine reports the end through LReceiverLookupFinish.
Reading completion off the awaited task gave the menu a second opinion.
The search is not one the menu runs.

### `catch (Exception)`

Superseded by a newer lookup, or the window closed.
Ignore it.

### `_pNotationSearching = false;`

A lookup that could not be started reports no end of its own.
So the menu is taken out of its searching state here.
It is not left running under a search that never began.

### `private void PNotationUpdate()`

What the menu shows, from the two things it knows.
Those are whether the search is still running, and what has arrived so far.
They are independent, because a source that has already answered does not end the search.
That is why the running line follows the search alone.
Reading it off "nothing found yet" instead is what left it running under a menu that was plainly finished.

### `if (candidates)`

The notice is the one line the menu says while it has no rows to show.
It says what it is doing, or that there was nothing to find.
With rows on screen it says nothing.

### `void LReceiver.LReceiverCandidateAdd(LCandidate candidate)`

Each source answers exactly once here, whether it found a reading or not.
The row is resolved in place rather than replaced, so it never jumps under the pointer.
A source that was reached and had nothing reads differently from one that was never reached.
Silence would have said a word is missing from a dictionary that was in fact down.

### `private void PNotationPrimaryApply(PNotationReading reading)`

Writes the reading into the pronunciation field, then tags the primary pronunciation row with its variety.
The field change defers the IPA request, so the pending save is run here before the row is read back.
The primary row exists only once that request has run, and its id is what the variety request needs.
A reading without a variety writes the field alone.

### `private void PNotationReadingAdd(PNotationReading reading)`

Adds one more pronunciation row for a further reading, then tags it.
The row is appended at the end, so the last row of the draft read back is the new one.
The id is read from the draft rather than made up here, because the engine alone mints ids.

### `private PNotationReading? PNotationReadingCreate(LCandidate candidate)`

Builds the button for one candidate, or nothing when the candidate carries no transcription.
Its label and flag are resolved as a pronunciation row resolves its own, under the language the search began for.

### `private bool PNotationHeldCheck(PNotationReading reading)`

Whether the draft already holds a pronunciation with this reading's variety and text.
Taking all twice would otherwise double every row after the first.
