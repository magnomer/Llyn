# PNotation.cs

## `public partial class PEditor : LReceiver`

Pronunciation and transcription lookup as the editor shows it.
Every pronunciation row and every transcription row carries its own lookup button, and the menu opens under the one pressed.
The menu is one popup the editor owns, retargeted at the row that asked for it.
Opening it starts a search for the headword, the same search from whichever row.
A transcription row names its scheme, and the search then runs that scheme's sources instead of the IPA ones.
Candidates stream in from the engine and fill the list, one reading per variety a source returned.
Picking one writes it into the row that opened the menu and stores its variety on that row.
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

### `private async Task PNotationOpen(UIElement anchor, long target)`

Opens the menu under the button of one pronunciation row and remembers which row it serves.
A target of zero is the primary row, which the engine may not have minted yet.
The popup is shut first, so a press on another row's button moves it instead of leaving it put.

### `private async Task PNotationOpen(UIElement anchor, long target, string scheme)`

The same opening, with the scheme the search is for.
A blank scheme is a pronunciation search and a named one a transcription search on that row.

### `private void PNotationApply(PNotationReading reading)`

Writes the reading into the row the menu was opened for, then tags that row with its variety.
A transcription row takes the text alone, because its scheme is already fixed and it carries no variety.
The primary row goes through its own field, whose change defers the IPA request.
A further row goes through its row model, whose change defers the same request on its id.
The pending save is run before the variety is sent, because the primary row exists only once it has run.
A further row that vanished while the menu stood open takes nothing.
A reading without a variety writes the text alone.

### `if (_pNotationFlagged && _pNotationScheme.Length == 0)`

Flags are loaded for a pronunciation search alone.
A transcription search returns untagged readings, so no flag would ever be drawn.

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

### `private PNotationReading? PNotationReadingCreate(LCandidate candidate)`

Builds the button for one candidate, or nothing when the candidate carries no transcription.
Its label and flag are resolved as a pronunciation row resolves its own, under the language the search began for.
It is bracketed for a pronunciation search and bare for a transcription search.
