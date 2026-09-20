# PNotation.cs

## `public partial class PEditor : LReceiver`

Pronunciation and transcription lookup as the editor shows it.
Every pronunciation row and every transcription row carries its own lookup button, and the menu opens under the one pressed.
The menu is one popup the editor owns, retargeted at the row that asked for it.
Opening it starts a search for the headword, the same search from whichever row.
A transcription row names its scheme, and the search then runs that scheme's sources instead of the IPA ones.
The search is a foray the tenure starts, and the menu keeps only that handle.
The word, language, target row, scheme and flag mode are read off the foray, never copied.
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

### `if (scheme.Length == 0 && language.Length > 0 && _lEngine.LEngineFlaggedCheck(language))`

Flags are loaded for a pronunciation search alone, before it starts.
A transcription search returns untagged readings, so no flag would ever be drawn.
A reading's flag is then ready the moment the reading lands.
A menu closed while the flags loaded starts no search.

### `_lEditor.LEditorNotationStart(word, target, scheme, this);`

The tenure starts the search and the menu listens.
The tenure passes the draft it holds, so the engine can hand back what that draft already found.
Whether a search runs at all is the engine's answer, not the menu's.
The search is over when the receiver is told it is, never when the start returns.
The engine reports the end through LReceiverLookupFinish.

### `catch (Exception)`

A lookup that could not be started reports no end of its own.
So the menu is taken out of its searching state here.
It is not left running under a search that never began.

### `private void PNotationCancel()`

Ends the search in flight and drops the handle, so a stale arrival finds no foray to read.

### `private async Task PNotationOpen(UIElement anchor, long target)`

Opens the menu under the button of one pronunciation row and starts the search for that row.
A target of zero is the primary row, which the engine may not have minted yet.
The popup is shut first, so a press on another row's button moves it instead of leaving it put.

### `private async Task PNotationOpen(UIElement anchor, long target, string scheme)`

The same opening, with the scheme the search is for.
A blank scheme is a pronunciation search and a named one a transcription search on that row.

### `private void PNotationApply(PNotationReading reading)`

Writes the reading into the row the menu was opened for, then tags that row with its variety.
A transcription row takes the text alone, because its scheme is already fixed and it carries no variety.
The primary row and a further row each take a reading request sent at once, carrying the source's phonetic.
The engine derives the respelling from it, so the pick fills both forms whatever the field prints.
The request is sent before the variety, because the primary row exists only once it has run.
A further row that vanished while the menu stood open takes nothing.
A menu with no search behind it takes nothing either.
A reading without a variety writes the text alone.

### `private void PNotationUpdate(string scheme)`

What the menu shows, from the two things it knows.
Those are whether the search is still running, and what has arrived so far.
They are independent, because a source that has already answered does not end the search.
That is why the running line follows the search alone.
Reading it off "nothing found yet" instead is what left it running under a menu that was plainly finished.

### `if (candidates)`

The notice is the one line the menu says while it has no rows to show.
It says what it is doing, or that there was nothing to find.
With rows on screen it says nothing.
The scheme names which empty notice, and comes from the opening before a foray exists and from the foray after.

### `void LReceiver.LReceiverCandidateAdd(LCandidate candidate)`

Each source answers exactly once here, whether it found a reading or not.
The row is resolved in place rather than replaced, so it never jumps under the pointer.
A source that was reached and had nothing reads differently from one that was never reached.
Silence would have said a word is missing from a dictionary that was in fact down.

### `private PNotationReading? PNotationReadingCreate(LCandidate candidate)`

Builds the button for one candidate, or nothing when the candidate carries no transcription.
Its label and flag are resolved as a pronunciation row resolves its own, under the draft's language of the moment.
It prints the candidate's respelling while the switch shows respellings and its phonetic otherwise.
It is bracketed for a pronunciation search, between slashes for a phonemic respelling, and bare for a transcription search.
