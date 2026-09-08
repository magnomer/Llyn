# PNotation.cs

## `public partial class PEditor : LReceiver`

Pronunciation lookup as the editor shows it.
Opening the menu starts a search for the headword.
Candidates stream in from the engine and fill the list.
Picking one writes it into the pronunciation field.
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

### `if (!candidate.PNotationItemReady)`

A row with no reading carries nothing to take.
Its taking button is hidden, so this only guards a click the template should never have offered.
